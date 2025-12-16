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
    }
}
