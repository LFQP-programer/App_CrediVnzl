using App_CrediVnzl.Models;

namespace App_CrediVnzl.Services
{
    /// <summary>
    /// Servicio para generacion de reportes y estadisticas
    /// </summary>
    public class ReportesService
    {
        private readonly DatabaseService _databaseService;

        public ReportesService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Obtener resumen completo de reportes
        /// </summary>
        public async Task<ResumenReportes> ObtenerResumenCompletoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var resumen = new ResumenReportes
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Financiero = await ObtenerReporteFinancieroAsync(fechaInicio, fechaFin),
                Prestamos = await ObtenerReportePrestamosAsync(fechaInicio, fechaFin),
                Clientes = await ObtenerReporteClientesAsync(),
                FlujoCaja = await ObtenerReporteFlujoCajaAsync(fechaInicio, fechaFin),
                Pagos = await ObtenerReportePagosAsync(fechaInicio, fechaFin)
            };

            return resumen;
        }

        /// <summary>
        /// Obtener reporte financiero
        /// </summary>
        public async Task<ReporteFinanciero> ObtenerReporteFinancieroAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var config = await _databaseService.GetCapitalConfigAsync();
                var prestamosActivos = await _databaseService.GetPrestamosActivosAsync();
                var historialPagos = await _databaseService.GetHistorialPagosByClienteAsync(0); // Todos

                var capitalEnCalle = prestamosActivos.Sum(p => p.CapitalPendiente);
                var interesesPendientes = prestamosActivos.Sum(p => p.InteresAcumulado);
                
                // Filtrar historial por rango de fechas
                var pagosEnPeriodo = historialPagos
                    .Where(h => h.FechaPago >= fechaInicio && h.FechaPago <= fechaFin)
                    .ToList();

                var interesesCobrados = pagosEnPeriodo.Sum(h => h.MontoInteres);
                var capitalRecuperado = pagosEnPeriodo.Sum(h => h.MontoCapital);

                // Calcular ROI
                var capitalInicial = config?.CapitalInicial ?? 0;
                var gananciaTotal = config?.GananciaTotal ?? 0;
                var roi = capitalInicial > 0 ? (gananciaTotal / capitalInicial) * 100 : 0;

                // Tasa de recuperacion
                var totalPrestado = prestamosActivos.Sum(p => p.MontoInicial);
                var tasaRecuperacion = totalPrestado > 0 
                    ? (capitalRecuperado / totalPrestado) * 100 
                    : 0;

                // Comparativa con periodo anterior
                var duracionPeriodo = (fechaFin - fechaInicio).Days;
                var fechaInicioAnterior = fechaInicio.AddDays(-duracionPeriodo);
                var fechaFinAnterior = fechaInicio;

                var pagosAnterior = historialPagos
                    .Where(h => h.FechaPago >= fechaInicioAnterior && h.FechaPago < fechaFinAnterior)
                    .Sum(h => h.MontoInteres);

                var cambioGanancias = interesesCobrados - pagosAnterior;
                var porcentajeCambio = pagosAnterior > 0 
                    ? (cambioGanancias / pagosAnterior) * 100 
                    : 0;

                return new ReporteFinanciero
                {
                    CapitalTotal = capitalInicial,
                    CapitalEnCalle = capitalEnCalle,
                    CapitalDisponible = config?.CapitalDisponible ?? 0,
                    GananciaTotal = gananciaTotal,
                    InteresesPendientes = interesesPendientes,
                    InteresesCobrados = interesesCobrados,
                    ROI = roi,
                    TasaRecuperacion = tasaRecuperacion,
                    CambioGanancias = cambioGanancias,
                    PorcentajeCambioGanancias = porcentajeCambio
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerReporteFinancieroAsync: {ex.Message}");
                return new ReporteFinanciero();
            }
        }

        /// <summary>
        /// Obtener reporte de prestamos
        /// </summary>
        public async Task<ReportePrestamos> ObtenerReportePrestamosAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var todosPrestamos = await _databaseService.GetPrestamosAsync();
                
                // Filtrar por fecha
                var prestamosEnPeriodo = todosPrestamos
                    .Where(p => p.FechaInicio >= fechaInicio && p.FechaInicio <= fechaFin)
                    .ToList();

                var prestamosActivos = prestamosEnPeriodo.Count(p => p.Estado == "Activo");
                var prestamosCompletados = prestamosEnPeriodo.Count(p => p.Estado == "Completado");
                var prestamosVencidos = prestamosEnPeriodo.Count(p => p.Estado == "Vencido");

                var montoPromedio = prestamosEnPeriodo.Any() 
                    ? prestamosEnPeriodo.Average(p => p.MontoInicial) 
                    : 0;

                var duracionPromedio = prestamosCompletados > 0
                    ? prestamosEnPeriodo
                        .Where(p => p.Estado == "Completado")
                        .Average(p => (DateTime.Now - p.FechaInicio).Days)
                    : 0;

                var tasaMorosidad = prestamosEnPeriodo.Any()
                    ? ((decimal)prestamosVencidos / prestamosEnPeriodo.Count) * 100
                    : 0;

                var montoTotalPrestado = prestamosEnPeriodo.Sum(p => p.MontoInicial);
                var montoTotalRecuperado = prestamosEnPeriodo
                    .Where(p => p.Estado == "Completado")
                    .Sum(p => p.MontoInicial);

                // Distribucion por estado
                var distribucion = new Dictionary<string, int>
                {
                    ["Activo"] = prestamosActivos,
                    ["Completado"] = prestamosCompletados,
                    ["Vencido"] = prestamosVencidos
                };

                return new ReportePrestamos
                {
                    TotalPrestamos = prestamosEnPeriodo.Count,
                    PrestamosActivos = prestamosActivos,
                    PrestamosCompletados = prestamosCompletados,
                    PrestamosVencidos = prestamosVencidos,
                    MontoPromedio = montoPromedio,
                    DuracionPromedioDias = (int)duracionPromedio,
                    TasaMorosidad = tasaMorosidad,
                    MontoTotalPrestado = montoTotalPrestado,
                    MontoTotalRecuperado = montoTotalRecuperado,
                    PrestamosPorEstado = distribucion
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerReportePrestamosAsync: {ex.Message}");
                return new ReportePrestamos();
            }
        }

        /// <summary>
        /// Obtener reporte de clientes
        /// </summary>
        public async Task<ReporteClientes> ObtenerReporteClientesAsync()
        {
            try
            {
                var clientes = await _databaseService.GetClientesAsync();
                var totalClientes = clientes.Count;
                var clientesActivos = clientes.Count(c => c.PrestamosActivos > 0);
                var clientesInactivos = totalClientes - clientesActivos;

                // Clientes nuevos este mes
                var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var clientesNuevos = clientes.Count(c => c.FechaRegistro >= inicioMes);

                // Promedio de prestamos por cliente
                var prestamos = await _databaseService.GetPrestamosAsync();
                var promedioPrestamosPorCliente = totalClientes > 0 
                    ? (decimal)prestamos.Count / totalClientes 
                    : 0;

                var deudaPromedio = clientesActivos > 0
                    ? clientes.Where(c => c.PrestamosActivos > 0).Average(c => c.DeudaPendiente)
                    : 0;

                // Top 10 clientes
                var topClientes = new List<ClienteTop>();
                foreach (var cliente in clientes.OrderByDescending(c => c.PrestamosActivos).Take(10))
                {
                    var prestamosCliente = await _databaseService.GetPrestamosByClienteAsync(cliente.Id);
                    var cantidadPrestamos = prestamosCliente.Count;
                    var montoTotal = prestamosCliente.Sum(p => p.MontoInicial);
                    
                    // Calcular tasa de cumplimiento
                    var pagosCliente = await _databaseService.GetHistorialPagosByClienteAsync(cliente.Id);
                    var totalPagos = pagosCliente.Count;
                    var pagosPuntuales = pagosCliente.Count; // Simplificado por ahora
                    var tasaCumplimiento = totalPagos > 0 ? ((decimal)pagosPuntuales / totalPagos) * 100 : 100;

                    // Clasificar cliente
                    string clasificacion;
                    string colorBadge;
                    if (cantidadPrestamos >= 10 && tasaCumplimiento >= 90)
                    {
                        clasificacion = "VIP";
                        colorBadge = "#FFD700"; // Gold
                    }
                    else if (cantidadPrestamos < 3)
                    {
                        clasificacion = "Nuevo";
                        colorBadge = "#2196F3"; // Blue
                    }
                    else
                    {
                        clasificacion = "Regular";
                        colorBadge = "#4CAF50"; // Green
                    }

                    topClientes.Add(new ClienteTop
                    {
                        ClienteId = cliente.Id,
                        NombreCompleto = cliente.NombreCompleto,
                        CantidadPrestamos = cantidadPrestamos,
                        MontoTotalPrestado = montoTotal,
                        TasaCumplimiento = tasaCumplimiento,
                        Clasificacion = clasificacion,
                        ColorBadge = colorBadge
                    });
                }

                // Clientes en riesgo
                var clientesRiesgo = new List<ClienteRiesgo>();
                foreach (var cliente in clientes.Where(c => c.DeudaPendiente > 0))
                {
                    var pagosCliente = await _databaseService.GetPagosByEstadoAsync("Vencido");
                    var pagosVencidosCliente = pagosCliente.Where(p => p.ClienteId == cliente.Id).ToList();
                    
                    if (pagosVencidosCliente.Any())
                    {
                        var montoVencido = pagosVencidosCliente.Sum(p => p.MontoPago);
                        var diasAtraso = pagosVencidosCliente
                            .Max(p => (DateTime.Now - p.FechaProgramada).Days);

                        string nivelRiesgo;
                        string colorAlerta;
                        if (pagosVencidosCliente.Count >= 3 || diasAtraso > 30)
                        {
                            nivelRiesgo = "Alto";
                            colorAlerta = "#F44336"; // Red
                        }
                        else if (pagosVencidosCliente.Count >= 2 || diasAtraso > 15)
                        {
                            nivelRiesgo = "Medio";
                            colorAlerta = "#FF9800"; // Orange
                        }
                        else
                        {
                            nivelRiesgo = "Bajo";
                            colorAlerta = "#FFC107"; // Amber
                        }

                        clientesRiesgo.Add(new ClienteRiesgo
                        {
                            ClienteId = cliente.Id,
                            NombreCompleto = cliente.NombreCompleto,
                            PagosVencidos = pagosVencidosCliente.Count,
                            MontoVencido = montoVencido,
                            DiasAtraso = diasAtraso,
                            NivelRiesgo = nivelRiesgo,
                            ColorAlerta = colorAlerta
                        });
                    }
                }

                return new ReporteClientes
                {
                    TotalClientes = totalClientes,
                    ClientesActivos = clientesActivos,
                    ClientesInactivos = clientesInactivos,
                    ClientesNuevosEsteMes = clientesNuevos,
                    PromedioPrestamosPorCliente = promedioPrestamosPorCliente,
                    DeudaPromedioCliente = deudaPromedio,
                    TopClientes = topClientes,
                    ClientesEnRiesgo = clientesRiesgo.OrderByDescending(c => c.PagosVencidos).ToList()
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerReporteClientesAsync: {ex.Message}");
                return new ReporteClientes();
            }
        }

        /// <summary>
        /// Obtener reporte de flujo de caja
        /// </summary>
        public async Task<ReporteFlujoCaja> ObtenerReporteFlujoCajaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var prestamos = await _databaseService.GetPrestamosAsync();
                var historialPagos = await _databaseService.GetHistorialPagosByClienteAsync(0);

                // Filtrar por periodo
                var prestamosEnPeriodo = prestamos
                    .Where(p => p.FechaInicio >= fechaInicio && p.FechaInicio <= fechaFin)
                    .ToList();

                var pagosEnPeriodo = historialPagos
                    .Where(h => h.FechaPago >= fechaInicio && h.FechaPago <= fechaFin)
                    .ToList();

                var totalPrestado = prestamosEnPeriodo.Sum(p => p.MontoInicial);
                var totalCobrado = pagosEnPeriodo.Sum(h => h.MontoTotal);
                var interesesGenerados = pagosEnPeriodo.Sum(h => h.MontoInteres);
                var flujoNeto = totalCobrado - totalPrestado;

                // Proyeccion proximos 30 dias
                var fechaProyeccion = DateTime.Now.AddDays(30);
                var pagosProyectados = await _databaseService.GetPagosByFechaAsync(DateTime.Now);
                var proyeccion = pagosProyectados
                    .Where(p => p.FechaProgramada <= fechaProyeccion && p.Estado == "Pendiente")
                    .Sum(p => p.MontoPago);

                // Flujo por dia
                var flujoPorDia = new List<FlujoDiario>();
                var fechaActual = fechaInicio;
                while (fechaActual <= fechaFin)
                {
                    var entradas = pagosEnPeriodo
                        .Where(h => h.FechaPago.Date == fechaActual.Date)
                        .Sum(h => h.MontoTotal);

                    var salidas = prestamosEnPeriodo
                        .Where(p => p.FechaInicio.Date == fechaActual.Date)
                        .Sum(p => p.MontoInicial);

                    if (entradas > 0 || salidas > 0)
                    {
                        flujoPorDia.Add(new FlujoDiario
                        {
                            Fecha = fechaActual,
                            Entradas = entradas,
                            Salidas = salidas,
                            Neto = entradas - salidas
                        });
                    }

                    fechaActual = fechaActual.AddDays(1);
                }

                return new ReporteFlujoCaja
                {
                    TotalPrestado = totalPrestado,
                    TotalCobrado = totalCobrado,
                    InteresesGenerados = interesesGenerados,
                    FlujoNeto = flujoNeto,
                    ProyeccionProximos30Dias = proyeccion,
                    FlujoPorDia = flujoPorDia
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerReporteFlujoCajaAsync: {ex.Message}");
                return new ReporteFlujoCaja();
            }
        }

        /// <summary>
        /// Obtener reporte de pagos
        /// </summary>
        public async Task<ReportePagos> ObtenerReportePagosAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var historialPagos = await _databaseService.GetHistorialPagosByClienteAsync(0);
                var pagosEnPeriodo = historialPagos
                    .Where(h => h.FechaPago >= fechaInicio && h.FechaPago <= fechaFin)
                    .ToList();

                var totalPagos = pagosEnPeriodo.Count;
                var pagosPuntuales = totalPagos; // Simplificado
                var pagosAtrasados = 0; // Simplificado
                var tasaCumplimiento = totalPagos > 0 ? ((decimal)pagosPuntuales / totalPagos) * 100 : 0;
                var montoTotal = pagosEnPeriodo.Sum(h => h.MontoTotal);

                // Analisis por dia de la semana
                var pagosPorDia = pagosEnPeriodo
                    .GroupBy(h => h.FechaPago.DayOfWeek)
                    .ToDictionary(g => g.Key, g => g.Count());

                var mejorDia = pagosPorDia.Any() 
                    ? pagosPorDia.OrderByDescending(kvp => kvp.Value).First().Key 
                    : DayOfWeek.Monday;

                return new ReportePagos
                {
                    TotalPagos = totalPagos,
                    PagosPuntuales = pagosPuntuales,
                    PagosAtrasados = pagosAtrasados,
                    TasaCumplimiento = tasaCumplimiento,
                    PromedioDiasAtraso = 0,
                    MontoTotalPagos = montoTotal,
                    PagosPorDiaSemana = pagosPorDia,
                    MejorDiaCobro = mejorDia
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerReportePagosAsync: {ex.Message}");
                return new ReportePagos();
            }
        }

        /// <summary>
        /// Obtener datos para grafico de evolucion
        /// </summary>
        public async Task<List<DatoGrafico>> ObtenerDatosGraficoEvolucionAsync(PeriodoReporte periodo)
        {
            try
            {
                var datos = new List<DatoGrafico>();
                var fechaFin = DateTime.Now;
                DateTime fechaInicio;

                switch (periodo)
                {
                    case PeriodoReporte.Semana:
                        fechaInicio = fechaFin.AddDays(-7);
                        break;
                    case PeriodoReporte.Mes:
                        fechaInicio = fechaFin.AddMonths(-1);
                        break;
                    case PeriodoReporte.Trimestre:
                        fechaInicio = fechaFin.AddMonths(-3);
                        break;
                    case PeriodoReporte.Anio:
                        fechaInicio = fechaFin.AddYears(-1);
                        break;
                    default:
                        fechaInicio = fechaFin.AddDays(-30);
                        break;
                }

                var historialPagos = await _databaseService.GetHistorialPagosByClienteAsync(0);
                var pagosEnPeriodo = historialPagos
                    .Where(h => h.FechaPago >= fechaInicio && h.FechaPago <= fechaFin)
                    .OrderBy(h => h.FechaPago)
                    .ToList();

                // Agrupar por dia/semana/mes segun el periodo
                if (periodo == PeriodoReporte.Anio)
                {
                    // Agrupar por mes
                    var pagosPorMes = pagosEnPeriodo
                        .GroupBy(h => new { h.FechaPago.Year, h.FechaPago.Month })
                        .Select(g => new
                        {
                            Fecha = new DateTime(g.Key.Year, g.Key.Month, 1),
                            Monto = g.Sum(h => h.MontoInteres)
                        })
                        .OrderBy(x => x.Fecha);

                    foreach (var item in pagosPorMes)
                    {
                        datos.Add(new DatoGrafico
                        {
                            Etiqueta = item.Fecha.ToString("MMM"),
                            Valor = item.Monto,
                            Fecha = item.Fecha,
                            Color = "#4CAF50"
                        });
                    }
                }
                else
                {
                    // Agrupar por dia
                    var pagosPorDia = pagosEnPeriodo
                        .GroupBy(h => h.FechaPago.Date)
                        .Select(g => new
                        {
                            Fecha = g.Key,
                            Monto = g.Sum(h => h.MontoInteres)
                        })
                        .OrderBy(x => x.Fecha);

                    foreach (var item in pagosPorDia)
                    {
                        datos.Add(new DatoGrafico
                        {
                            Etiqueta = item.Fecha.ToString("dd/MM"),
                            Valor = item.Monto,
                            Fecha = item.Fecha,
                            Color = "#2196F3"
                        });
                    }
                }

                return datos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerDatosGraficoEvolucionAsync: {ex.Message}");
                return new List<DatoGrafico>();
            }
        }
    }
}
