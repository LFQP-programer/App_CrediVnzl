using SQLite;
using App_CrediVnzl.Models;

namespace App_CrediVnzl.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;

        public async Task InitializeAsync()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "prestafacil.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            
            await _database.CreateTableAsync<Cliente>();
            await _database.CreateTableAsync<Prestamo>();
            await _database.CreateTableAsync<Pago>();
            await _database.CreateTableAsync<HistorialPago>();
        }

        private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
            if (_database == null)
                await InitializeAsync();
            return _database!;
        }

        // Metodos para Clientes
        public async Task<List<Cliente>> GetClientesAsync()
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Cliente>().OrderByDescending(c => c.FechaRegistro).ToListAsync();
        }

        public async Task<Cliente?> GetClienteAsync(int id)
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Cliente>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveClienteAsync(Cliente cliente)
        {
            var db = await GetDatabaseAsync();
            
            if (cliente.Id != 0)
            {
                return await db.UpdateAsync(cliente);
            }
            else
            {
                return await db.InsertAsync(cliente);
            }
        }

        public async Task<int> DeleteClienteAsync(Cliente cliente)
        {
            var db = await GetDatabaseAsync();
            return await db.DeleteAsync(cliente);
        }

        public async Task<List<Cliente>> SearchClientesAsync(string searchText)
        {
            var db = await GetDatabaseAsync();
            
            return await db.Table<Cliente>()
                .Where(c => c.NombreCompleto.Contains(searchText) || 
                           c.Telefono.Contains(searchText) || 
                           c.Cedula.Contains(searchText))
                .ToListAsync();
        }

        public async Task<int> GetTotalClientesAsync()
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Cliente>().CountAsync();
        }

        public async Task<int> GetClientesConPrestamosActivosAsync()
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Cliente>().Where(c => c.PrestamosActivos > 0).CountAsync();
        }

        public async Task<decimal> GetTotalDeudaPendienteAsync()
        {
            var db = await GetDatabaseAsync();
            var clientes = await db.Table<Cliente>().ToListAsync();
            return clientes.Sum(c => c.DeudaPendiente);
        }

        // Metodos para Pagos
        public async Task<List<Pago>> GetPagosAsync()
        {
            var db = await GetDatabaseAsync();
            var pagos = await db.Table<Pago>().OrderBy(p => p.FechaProgramada).ToListAsync();
            
            // Cargar informacion del cliente
            foreach (var pago in pagos)
            {
                var cliente = await GetClienteAsync(pago.ClienteId);
                if (cliente != null)
                {
                    pago.ClienteNombre = cliente.NombreCompleto;
                    pago.ClienteTelefono = cliente.Telefono;
                }
            }
            
            return pagos;
        }

        public async Task<List<Pago>> GetPagosByEstadoAsync(string estado)
        {
            var db = await GetDatabaseAsync();
            var pagos = await db.Table<Pago>()
                .Where(p => p.Estado == estado)
                .OrderBy(p => p.FechaProgramada)
                .ToListAsync();
            
            foreach (var pago in pagos)
            {
                var cliente = await GetClienteAsync(pago.ClienteId);
                if (cliente != null)
                {
                    pago.ClienteNombre = cliente.NombreCompleto;
                    pago.ClienteTelefono = cliente.Telefono;
                }
            }
            
            return pagos;
        }

        public async Task<List<Pago>> GetPagosByFechaAsync(DateTime fecha)
        {
            var db = await GetDatabaseAsync();
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1);
            
            var pagos = await db.Table<Pago>()
                .Where(p => p.FechaProgramada >= fechaInicio && p.FechaProgramada < fechaFin)
                .ToListAsync();
            
            foreach (var pago in pagos)
            {
                var cliente = await GetClienteAsync(pago.ClienteId);
                if (cliente != null)
                {
                    pago.ClienteNombre = cliente.NombreCompleto;
                    pago.ClienteTelefono = cliente.Telefono;
                }
            }
            
            return pagos;
        }

        public async Task<List<Pago>> GetPagosByMesAsync(int year, int month)
        {
            var db = await GetDatabaseAsync();
            var fechaInicio = new DateTime(year, month, 1);
            var fechaFin = fechaInicio.AddMonths(1);
            
            var pagos = await db.Table<Pago>()
                .Where(p => p.FechaProgramada >= fechaInicio && p.FechaProgramada < fechaFin)
                .OrderBy(p => p.FechaProgramada)
                .ToListAsync();
            
            foreach (var pago in pagos)
            {
                var cliente = await GetClienteAsync(pago.ClienteId);
                if (cliente != null)
                {
                    pago.ClienteNombre = cliente.NombreCompleto;
                    pago.ClienteTelefono = cliente.Telefono;
                }
            }
            
            return pagos;
        }

        public async Task<ResumenPagos> GetResumenPagosMesAsync(int year, int month)
        {
            var pagos = await GetPagosByMesAsync(year, month);
            var hoy = DateTime.Today;
            
            return new ResumenPagos
            {
                TotalMes = pagos.Count,
                MontoEsperado = pagos.Sum(p => p.MontoPago),
                Pendientes = pagos.Count(p => p.Estado == "Pendiente" && p.FechaProgramada >= hoy),
                Vencidos = pagos.Count(p => p.Estado == "Pendiente" && p.FechaProgramada < hoy),
                Pagados = pagos.Count(p => p.Estado == "Pagado")
            };
        }

        public async Task<int> SavePagoAsync(Pago pago)
        {
            var db = await GetDatabaseAsync();
            
            if (pago.Id != 0)
            {
                return await db.UpdateAsync(pago);
            }
            else
            {
                return await db.InsertAsync(pago);
            }
        }

        public async Task<int> MarcarPagoComoPagadoAsync(int pagoId)
        {
            var db = await GetDatabaseAsync();
            var pago = await db.Table<Pago>().Where(p => p.Id == pagoId).FirstOrDefaultAsync();
            
            if (pago != null)
            {
                pago.Estado = "Pagado";
                pago.FechaPagado = DateTime.Now;
                return await db.UpdateAsync(pago);
            }
            
            return 0;
        }

        public async Task ActualizarEstadosPagosVencidosAsync()
        {
            var db = await GetDatabaseAsync();
            var hoy = DateTime.Today;
            
            var pagosVencidos = await db.Table<Pago>()
                .Where(p => p.Estado == "Pendiente" && p.FechaProgramada < hoy)
                .ToListAsync();
            
            foreach (var pago in pagosVencidos)
            {
                pago.Estado = "Vencido";
                await db.UpdateAsync(pago);
            }
        }

        // Metodos para Prestamos
        public async Task<List<Prestamo>> GetPrestamosAsync()
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Prestamo>().OrderByDescending(p => p.FechaInicio).ToListAsync();
        }

        public async Task<List<Prestamo>> GetPrestamosByClienteAsync(int clienteId)
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Prestamo>()
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.FechaInicio)
                .ToListAsync();
        }

        public async Task<Prestamo?> GetPrestamoAsync(int id)
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Prestamo>().Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SavePrestamoAsync(Prestamo prestamo)
        {
            var db = await GetDatabaseAsync();
            
            if (prestamo.Id != 0)
            {
                return await db.UpdateAsync(prestamo);
            }
            else
            {
                return await db.InsertAsync(prestamo);
            }
        }

        public async Task<int> DeletePrestamoAsync(Prestamo prestamo)
        {
            var db = await GetDatabaseAsync();
            return await db.DeleteAsync(prestamo);
        }

        public async Task<List<Prestamo>> GetPrestamosActivosAsync()
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Prestamo>()
                .Where(p => p.Estado == "Activo")
                .OrderByDescending(p => p.FechaInicio)
                .ToListAsync();
        }

        public async Task ActualizarInteresesPrestamosActivosAsync()
        {
            var db = await GetDatabaseAsync();
            var prestamosActivos = await GetPrestamosActivosAsync();
            
            foreach (var prestamo in prestamosActivos)
            {
                // Calcular semanas desde el ultimo pago o desde el inicio
                var fechaReferencia = prestamo.FechaUltimoPago ?? prestamo.FechaInicio;
                var dias = (DateTime.Now - fechaReferencia).Days;
                var semanasTranscurridas = Math.Max(0, dias / 7);
                
                // Solo actualizar si han pasado semanas completas
                if (semanasTranscurridas > 0)
                {
                    // Si no hay FechaUltimoPago y solo ha pasado 1 semana, no agregar porque ya tiene el interes inicial
                    if (prestamo.FechaUltimoPago == null && semanasTranscurridas == 1)
                    {
                        // Marcar FechaUltimoPago para evitar recalculos pero no agregar interes
                        prestamo.FechaUltimoPago = DateTime.Now;
                        await db.UpdateAsync(prestamo);
                        continue;
                    }
                    
                    // Calcular nuevo interes
                    var interesPorSemana = prestamo.CapitalPendiente * (prestamo.TasaInteresSemanal / 100);
                    var nuevoInteres = interesPorSemana * semanasTranscurridas;
                    
                    // Agregar al interes acumulado
                    prestamo.InteresAcumulado += nuevoInteres;
                    prestamo.TotalAdeudado = prestamo.CapitalPendiente + prestamo.InteresAcumulado;
                    
                    // Actualizar fecha de ultimo calculo
                    prestamo.FechaUltimoPago = DateTime.Now;
                    
                    await db.UpdateAsync(prestamo);
                }
            }
            
            // Actualizar deuda pendiente de todos los clientes afectados
            var clientesIds = prestamosActivos.Select(p => p.ClienteId).Distinct();
            foreach (var clienteId in clientesIds)
            {
                await ActualizarDeudaClienteAsync(clienteId);
            }
        }

        public async Task ActualizarDeudaClienteAsync(int clienteId)
        {
            var cliente = await GetClienteAsync(clienteId);
            if (cliente != null)
            {
                var prestamosCliente = await GetPrestamosByClienteAsync(clienteId);
                cliente.DeudaPendiente = prestamosCliente
                    .Where(p => p.Estado == "Activo")
                    .Sum(p => p.TotalAdeudado);
                cliente.PrestamosActivos = prestamosCliente.Count(p => p.Estado == "Activo");
                await SaveClienteAsync(cliente);
            }
        }

        // Metodos para Historial de Pagos
        public async Task<int> SaveHistorialPagoAsync(HistorialPago historial)
        {
            var db = await GetDatabaseAsync();
            return await db.InsertAsync(historial);
        }

        public async Task<List<HistorialPago>> GetHistorialPagosByPrestamoAsync(int prestamoId)
        {
            var db = await GetDatabaseAsync();
            var historial = await db.Table<HistorialPago>()
                .Where(h => h.PrestamoId == prestamoId)
                .OrderByDescending(h => h.FechaPago)
                .ToListAsync();
            
            foreach (var item in historial)
            {
                var cliente = await GetClienteAsync(item.ClienteId);
                if (cliente != null)
                {
                    item.ClienteNombre = cliente.NombreCompleto;
                }
            }
            
            return historial;
        }

        public async Task<List<HistorialPago>> GetHistorialPagosByClienteAsync(int clienteId)
        {
            var db = await GetDatabaseAsync();
            return await db.Table<HistorialPago>()
                .Where(h => h.ClienteId == clienteId)
                .OrderByDescending(h => h.FechaPago)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalCobradoHoyAsync()
        {
            var db = await GetDatabaseAsync();
            var hoy = DateTime.Today;
            var historial = await db.Table<HistorialPago>()
                .Where(h => h.FechaPago >= hoy && h.FechaPago < hoy.AddDays(1))
                .ToListAsync();
            return historial.Sum(h => h.MontoTotal);
        }

        public async Task<decimal> GetTotalCobradoMesAsync(int year, int month)
        {
            var db = await GetDatabaseAsync();
            var fechaInicio = new DateTime(year, month, 1);
            var fechaFin = fechaInicio.AddMonths(1);
            var historial = await db.Table<HistorialPago>()
                .Where(h => h.FechaPago >= fechaInicio && h.FechaPago < fechaFin)
                .ToListAsync();
            return historial.Sum(h => h.MontoTotal);
        }

        // Metodos para estadisticas globales
        public async Task<decimal> GetTotalCapitalPrestadoAsync()
        {
            var db = await GetDatabaseAsync();
            var prestamos = await db.Table<Prestamo>().ToListAsync();
            return prestamos.Sum(p => p.MontoInicial);
        }

        public async Task<decimal> GetTotalCapitalActivoAsync()
        {
            var db = await GetDatabaseAsync();
            var prestamosActivos = await GetPrestamosActivosAsync();
            return prestamosActivos.Sum(p => p.CapitalPendiente);
        }

        public async Task<decimal> GetTotalInteresGeneradoAsync()
        {
            var db = await GetDatabaseAsync();
            var historial = await db.Table<HistorialPago>().ToListAsync();
            return historial.Sum(h => h.MontoInteres);
        }

        public async Task<int> GetTotalPrestamosAsync()
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Prestamo>().CountAsync();
        }

        public async Task<int> GetTotalPrestamosCompletadosAsync()
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Prestamo>().Where(p => p.Estado == "Completado").CountAsync();
        }
    }
}
