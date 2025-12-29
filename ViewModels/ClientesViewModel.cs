using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class ClientesViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private string _searchText = string.Empty;
        private bool _isRefreshing;

        public ObservableCollection<Cliente> Clientes { get; set; } = new();

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                _ = SearchClientesAsync();
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand AddClienteCommand { get; }
        public ICommand EditClienteCommand { get; }
        public ICommand ModificarClienteCommand { get; }
        public ICommand EliminarClienteCommand { get; }

        public ClientesViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            RefreshCommand = new Command(async () => await LoadClientesAsync());
            AddClienteCommand = new Command(async () => await NavigateToNewCliente());
            EditClienteCommand = new Command<Cliente>(async (cliente) => await NavigateToDetalleCliente(cliente));
            ModificarClienteCommand = new Command<Cliente>(async (cliente) => await ModificarCliente(cliente));
            EliminarClienteCommand = new Command<Cliente>(async (cliente) => await EliminarCliente(cliente));
        }

        public async Task LoadClientesAsync()
        {
            IsRefreshing = true;
            
            try
            {
                var clientes = await _databaseService.GetClientesAsync();
                Clientes.Clear();
                
                foreach (var cliente in clientes)
                {
                    Clientes.Add(cliente);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"No se pudieron cargar los clientes: {ex.Message}", "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task SearchClientesAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadClientesAsync();
                return;
            }

            try
            {
                var clientes = await _databaseService.SearchClientesAsync(SearchText);
                Clientes.Clear();
                
                foreach (var cliente in clientes)
                {
                    Clientes.Add(cliente);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Error en la busqueda: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToNewCliente()
        {
            await Shell.Current.GoToAsync("nuevocliente");
        }

        private async Task NavigateToDetalleCliente(Cliente cliente)
        {
            if (cliente != null)
            {
                await Shell.Current.GoToAsync($"detallecliente?clienteId={cliente.Id}");
            }
        }

        private async Task ModificarCliente(Cliente cliente)
        {
            if (cliente != null)
            {
                await Shell.Current.GoToAsync($"editarcliente?clienteId={cliente.Id}");
            }
        }

        private async Task EliminarCliente(Cliente cliente)
        {
            if (cliente == null) return;

            try
            {
                // Verificar si tiene préstamos activos
                var prestamos = await _databaseService.GetPrestamosByClienteAsync(cliente.Id);
                var prestamosActivos = prestamos.Where(p => p.Estado == "Activo").ToList();

                if (prestamosActivos.Any())
                {
                    var confirmar = await Shell.Current.DisplayAlert(
                        "Advertencia", 
                        $"Este cliente tiene {prestamosActivos.Count} préstamo(s) activo(s) con una deuda total de S/{prestamosActivos.Sum(p => p.TotalAdeudado):N2}.\n\n¿Está seguro de eliminar al cliente y todos sus préstamos, pagos e historial?", 
                        "Sí, eliminar", 
                        "Cancelar");

                    if (!confirmar)
                        return;
                }
                else
                {
                    var confirmar = await Shell.Current.DisplayAlert(
                        "Confirmar eliminación", 
                        $"¿Está seguro de eliminar a {cliente.NombreCompleto}? Esta acción no se puede deshacer.", 
                        "Sí, eliminar", 
                        "Cancelar");

                    if (!confirmar)
                        return;
                }

                // Eliminar todos los datos relacionados en cascada
                await _databaseService.EliminarClienteConDatosRelacionadosAsync(cliente.Id);

                await Shell.Current.DisplayAlert("Éxito", "Cliente eliminado correctamente", "OK");
                
                // Recargar la lista de clientes
                await LoadClientesAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo eliminar el cliente: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
