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
            
            // Crear o actualizar tablas (SQLite automáticamente agrega nuevas columnas)
            await _database.CreateTableAsync<Cliente>();
            await _database.CreateTableAsync<Prestamo>();
            await _database.CreateTableAsync<Pago>();
            await _database.CreateTableAsync<HistorialPago>();
            await _database.CreateTableAsync<CapitalConfig>();
            
            // Verificar y agregar columnas faltantes en Cliente (para migración)
            await MigrarTablaClienteAsync();
        }
        
        private async Task MigrarTablaClienteAsync()
        {
            try
            {
                var db = await GetDatabaseAsync();
                
                // Intentar agregar columnas nuevas si no existen
                // SQLite-net automáticamente maneja esto con CreateTableAsync, pero por seguridad:
                try
                {
                    await db.ExecuteAsync("ALTER TABLE clientes ADD COLUMN TieneAccesoApp INTEGER DEFAULT 0");
                }
                catch
                {
                    // La columna ya existe, ignorar
                }
                
                try
                {
                    await db.ExecuteAsync("ALTER TABLE clientes ADD COLUMN PasswordTemporal TEXT");
                }
                catch
                {
                    // La columna ya existe, ignorar
                }
                
                try
                {
                    await db.ExecuteAsync("ALTER TABLE clientes ADD COLUMN RequiereCambioPassword INTEGER DEFAULT 1");
                }
                catch
                {
                    // La columna ya existe, ignorar
                }
                
                try
                {
                    await db.ExecuteAsync("ALTER TABLE clientes ADD COLUMN FechaGeneracionPassword TEXT");
                }
                catch
                {
                    // La columna ya existe, ignorar
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en migración de tabla Cliente: {ex.Message}");
            }
        }

        private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
            if (_database == null)
                await InitializeAsync();
            return _database!;
        }

        // Métodos para Clientes
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

        // Métodos para Pagos
        public async Task<List<Pago>> GetPagosAsync()
        {
            var db = await GetDatabaseAsync();
            var pagos = await db.Table<Pago>().OrderBy(p => p.FechaProgramada).ToListAsync();
            
            // Cargar información del cliente
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

        // Métodos para Prestamos
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
                // Calcular semanas desde el último pago o desde el inicio
                var fechaReferencia = prestamo.FechaUltimoPago ?? prestamo.FechaInicio;
                var dias = (DateTime.Now - fechaReferencia).Days;
                var semanasTranscurridas = Math.Max(0, dias / 7);
                
                // Solo actualizar si han pasado semanas completas
                if (semanasTranscurridas > 0)
                {
                    // Si no hay FechaUltimoPago y solo ha pasado 1 semana, no agregar porque ya tiene el interés inicial
                    if (prestamo.FechaUltimoPago == null && semanasTranscurridas == 1)
                    {
                        // Marcar FechaUltimoPago para evitar recálculos pero no agregar interés
                        prestamo.FechaUltimoPago = DateTime.Now;
                        await db.UpdateAsync(prestamo);
                        continue;
                    }
                    
                    // Calcular nuevo interés
                    var interesPorSemana = prestamo.CapitalPendiente * (prestamo.TasaInteresSemanal / 100);
                    var nuevoInteres = interesPorSemana * semanasTranscurridas;
                    
                    // Agregar al interés acumulado
                    prestamo.InteresAcumulado += nuevoInteres;
                    prestamo.TotalAdeudado = prestamo.CapitalPendiente + prestamo.InteresAcumulado;
                    
                    // Actualizar fecha de último cálculo
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

        // Métodos para Historial de Pagos
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

        // Métodos para CapitalConfig
        public async Task<CapitalConfig?> GetCapitalConfigAsync()
        {
            var db = await GetDatabaseAsync();
            var config = await db.Table<CapitalConfig>().FirstOrDefaultAsync();
            
            // Si no existe, crear una configuración por defecto
            if (config == null)
            {
                config = new CapitalConfig
                {
                    CapitalInicial = 0,
                    CapitalDisponible = 0,
                    GananciaTotal = 0,
                    FechaActualizacion = DateTime.Now
                };
                await db.InsertAsync(config);
            }
            
            return config;
        }

        public async Task<int> SaveCapitalConfigAsync(CapitalConfig config)
        {
            var db = await GetDatabaseAsync();
            config.FechaActualizacion = DateTime.Now;
            
            var existente = await db.Table<CapitalConfig>().FirstOrDefaultAsync();
            if (existente != null)
            {
                config.Id = existente.Id;
                return await db.UpdateAsync(config);
            }
            else
            {
                return await db.InsertAsync(config);
            }
        }

        public async Task ActualizarCapitalDisponibleAsync()
        {
            var config = await GetCapitalConfigAsync();
            if (config != null)
            {
                var capitalEnPrestamos = await GetTotalCapitalActivoAsync();
                config.CapitalDisponible = config.CapitalInicial - capitalEnPrestamos;
                await SaveCapitalConfigAsync(config);
            }
        }

        public async Task AgregarGananciaAsync(decimal ganancia)
        {
            var config = await GetCapitalConfigAsync();
            if (config != null)
            {
                config.GananciaTotal += ganancia;
                await SaveCapitalConfigAsync(config);
            }
        }

        // Métodos para estadísticas globales
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

        public async Task<int> GetClientesMorososAsync()
        {
            var db = await GetDatabaseAsync();
            var hoy = DateTime.Today;
            
            // Obtener todos los clientes con préstamos activos
            var clientesConPrestamos = await db.Table<Cliente>()
                .Where(c => c.PrestamosActivos > 0)
                .ToListAsync();
            
            int clientesMorosos = 0;
            
            foreach (var cliente in clientesConPrestamos)
            {
                // Verificar si tiene préstamos con pagos vencidos
                var prestamosCliente = await GetPrestamosByClienteAsync(cliente.Id);
                var prestamosActivos = prestamosCliente.Where(p => p.Estado == "Activo");
                
                foreach (var prestamo in prestamosActivos)
                {
                    var fechaReferencia = prestamo.FechaUltimoPago ?? prestamo.FechaInicio;
                    var diasTranscurridos = (hoy - fechaReferencia.Date).Days;
                    
                    // Si han pasado más de 14 días (2 semanas) sin pago, se considera moroso
                    if (diasTranscurridos > 14)
                    {
                        clientesMorosos++;
                        break; // Contar el cliente solo una vez
                    }
                }
            }
            
            return clientesMorosos;
        }

        public async Task<int> GetPrestamosVencidosAsync()
        {
            var db = await GetDatabaseAsync();
            var hoy = DateTime.Today;
            
            var prestamosActivos = await GetPrestamosActivosAsync();
            int prestamosVencidos = 0;
            
            foreach (var prestamo in prestamosActivos)
            {
                var fechaReferencia = prestamo.FechaUltimoPago ?? prestamo.FechaInicio;
                var diasTranscurridos = (hoy - fechaReferencia.Date).Days;
                
                // Si han pasado más de 14 días (2 semanas) sin pago, se considera vencido
                if (diasTranscurridos > 14)
                {
                    prestamosVencidos++;
                }
            }
            
            return prestamosVencidos;
        }

        public async Task<decimal> GetGananciaCobradaAsync()
        {
            var db = await GetDatabaseAsync();
            var historial = await db.Table<HistorialPago>().ToListAsync();
            return historial.Sum(h => h.MontoInteres);
        }

        public async Task<decimal> GetGananciaPendienteAsync()
        {
            var db = await GetDatabaseAsync();
            var prestamosActivos = await GetPrestamosActivosAsync();
            return prestamosActivos.Sum(p => p.InteresAcumulado);
        }

        // Método para eliminar cliente y todos sus datos relacionados
        public async Task EliminarClienteConDatosRelacionadosAsync(int clienteId)
        {
            var db = await GetDatabaseAsync();
            
            // Obtener todos los préstamos del cliente
            var prestamos = await GetPrestamosByClienteAsync(clienteId);
            
            foreach (var prestamo in prestamos)
            {
                // Eliminar historial de pagos del préstamo
                await db.ExecuteAsync("DELETE FROM HistorialPago WHERE PrestamoId = ?", prestamo.Id);
                
                // Eliminar pagos programados del préstamo
                await db.ExecuteAsync("DELETE FROM Pago WHERE PrestamoId = ?", prestamo.Id);
                
                // Eliminar el préstamo
                await db.DeleteAsync(prestamo);
            }
            
            // Finalmente, eliminar el cliente
            var cliente = await GetClienteAsync(clienteId);
            if (cliente != null)
            {
                await db.DeleteAsync(cliente);
            }
        }

        // Método para reiniciar la base de datos (eliminar todos los datos)
        public async Task ReiniciarBaseDeDatosAsync()
        {
            var db = await GetDatabaseAsync();
            
            // Eliminar todos los registros en el orden correcto para mantener integridad
            await db.DeleteAllAsync<HistorialPago>();
            await db.DeleteAllAsync<Pago>();
            await db.DeleteAllAsync<Prestamo>();
            await db.DeleteAllAsync<Cliente>();
        }

        // Método para eliminar completamente el archivo de base de datos
        public async Task EliminarBaseDeDatosCompletaAsync()
        {
            try
            {
                // Cerrar la conexión actual
                if (_database != null)
                {
                    await _database.CloseAsync();
                    _database = null;
                }

                // Obtener la ruta del archivo
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "prestafacil.db3");

                // Eliminar el archivo si existe
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }

                // Reinicializar la base de datos
                await InitializeAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al eliminar base de datos: {ex.Message}");
                throw;
            }
        }

        // Método para obtener información de la base de datos
        public async Task<DatabaseInfo> ObtenerInformacionBaseDeDatosAsync()
        {
            var db = await GetDatabaseAsync();
            
            var info = new DatabaseInfo
            {
                RutaArchivo = Path.Combine(FileSystem.AppDataDirectory, "prestafacil.db3"),
                TotalClientes = await db.Table<Cliente>().CountAsync(),
                TotalPrestamos = await db.Table<Prestamo>().CountAsync(),
                TotalPagos = await db.Table<Pago>().CountAsync(),
                TotalHistorialPagos = await db.Table<HistorialPago>().CountAsync()
            };

            // Obtener tamaño del archivo
            if (File.Exists(info.RutaArchivo))
            {
                var fileInfo = new FileInfo(info.RutaArchivo);
                info.TamañoArchivo = fileInfo.Length;
            }

            return info;
        }

        // Métodos para gestión de contraseñas de clientes
        public async Task<string> GenerarPasswordTemporalAsync(int clienteId)
        {
            var cliente = await GetClienteAsync(clienteId);
            if (cliente == null)
                throw new Exception("Cliente no encontrado");

            // Generar contraseña de 6 dígitos
            var random = new Random();
            var password = random.Next(100000, 999999).ToString();

            // Actualizar cliente
            cliente.TieneAccesoApp = true;
            cliente.PasswordTemporal = password;
            cliente.RequiereCambioPassword = true;
            cliente.FechaGeneracionPassword = DateTime.Now;

            await SaveClienteAsync(cliente);
            return password;
        }

        public async Task<bool> CambiarPasswordClienteAsync(int clienteId, string passwordActual, string passwordNueva)
        {
            var cliente = await GetClienteAsync(clienteId);
            if (cliente == null)
                return false;

            // Verificar contraseña actual
            if (cliente.PasswordTemporal != passwordActual)
                return false;

            // Validar que la nueva contraseña tenga al menos 6 dígitos
            if (passwordNueva.Length < 6 || !passwordNueva.All(char.IsDigit))
                return false;

            // Actualizar contraseña
            cliente.PasswordTemporal = passwordNueva;
            cliente.RequiereCambioPassword = false;

            await SaveClienteAsync(cliente);
            return true;
        }

        public async Task<bool> RestablecerPasswordClienteAsync(int clienteId)
        {
            var cliente = await GetClienteAsync(clienteId);
            if (cliente == null)
                return false;

            // Generar nueva contraseña temporal
            var random = new Random();
            var password = random.Next(100000, 999999).ToString();

            cliente.PasswordTemporal = password;
            cliente.RequiereCambioPassword = true;
            cliente.FechaGeneracionPassword = DateTime.Now;

            await SaveClienteAsync(cliente);
            return true;
        }

        public async Task<bool> DeshabilitarAccesoClienteAsync(int clienteId)
        {
            var cliente = await GetClienteAsync(clienteId);
            if (cliente == null)
                return false;

            cliente.TieneAccesoApp = false;
            cliente.PasswordTemporal = null;

            await SaveClienteAsync(cliente);
            return true;
        }

        public async Task<List<Cliente>> GetClientesConAccesoAppAsync()
        {
            var db = await GetDatabaseAsync();
            return await db.Table<Cliente>()
                .Where(c => c.TieneAccesoApp)
                .OrderByDescending(c => c.FechaRegistro)
                .ToListAsync();
        }

    }

    // Clase para información de la base de datos
    public class DatabaseInfo
    {
        public string RutaArchivo { get; set; } = string.Empty;
        public int TotalClientes { get; set; }
        public int TotalPrestamos { get; set; }
        public int TotalPagos { get; set; }
        public int TotalHistorialPagos { get; set; }
        public long TamañoArchivo { get; set; }
        
        public string TamañoArchivoFormateado
        {
            get
            {
                if (TamañoArchivo < 1024)
                    return $"{TamañoArchivo} bytes";
                else if (TamañoArchivo < 1024 * 1024)
                    return $"{TamañoArchivo / 1024.0:F2} KB";
                else
                    return $"{TamañoArchivo / (1024.0 * 1024.0):F2} MB";
            }
        }
    }
}
