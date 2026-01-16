using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    [QueryProperty(nameof(ClienteId), "clienteId")]
    public class EditarClienteViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private int _clienteId;
        private string _nombreCompleto = string.Empty;
        private string _telefono = string.Empty;
        private string _cedula = string.Empty;
        private string _direccion = string.Empty;

        public int ClienteId
        {
            get => _clienteId;
            set
            {
                _clienteId = value;
                OnPropertyChanged();
            }
        }

        public string NombreCompleto
        {
            get => _nombreCompleto;
            set
            {
                _nombreCompleto = value;
                OnPropertyChanged();
            }
        }

        public string Telefono
        {
            get => _telefono;
            set
            {
                _telefono = value;
                OnPropertyChanged();
            }
        }

        public string Cedula
        {
            get => _cedula;
            set
            {
                _cedula = value;
                OnPropertyChanged();
            }
        }

        public string Direccion
        {
            get => _direccion;
            set
            {
                _direccion = value;
                OnPropertyChanged();
            }
        }

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }

        public EditarClienteViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            GuardarCommand = new Command(async () => await GuardarClienteAsync());
            CancelarCommand = new Command(async () => await CancelarAsync());
        }

        public async Task LoadClienteAsync()
        {
            try
            {
                var cliente = await _databaseService.GetClienteAsync(ClienteId);
                if (cliente != null)
                {
                    NombreCompleto = cliente.NombreCompleto;
                    Telefono = cliente.Telefono;
                    Cedula = cliente.Cedula;
                    Direccion = cliente.Direccion;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo cargar el cliente: {ex.Message}", "OK");
            }
        }

        private async Task GuardarClienteAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreCompleto))
            {
                await Shell.Current.DisplayAlert("Error", "El nombre completo es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Telefono))
            {
                await Shell.Current.DisplayAlert("Error", "El telefono es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Cedula))
            {
                await Shell.Current.DisplayAlert("Error", "La cedula/DNI es requerida", "OK");
                return;
            }

            try
            {
                var cliente = await _databaseService.GetClienteAsync(ClienteId);
                if (cliente != null)
                {
                    // Actualizar usando las propiedades reales del modelo
                    var nombres = NombreCompleto.Split(' ');
                    cliente.Nombres = nombres.Length > 0 ? nombres[0] : NombreCompleto;
                    cliente.Apellidos = nombres.Length > 1 ? string.Join(" ", nombres.Skip(1)) : "";
                    cliente.NumeroCelular = Telefono;
                    cliente.NumeroDocumento = Cedula;
                    cliente.AvalDireccion = Direccion;

                    await _databaseService.SaveClienteAsync(cliente);
                    await Shell.Current.DisplayAlert("Exito", "Cliente actualizado correctamente", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo actualizar el cliente: {ex.Message}", "OK");
            }
        }

        private async Task CancelarAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
