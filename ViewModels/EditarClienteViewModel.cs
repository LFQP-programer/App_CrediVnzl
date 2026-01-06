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
        private string _nombres = string.Empty;
        private string _apellidos = string.Empty;
        private string _telefono = string.Empty;
        private string _tipoDocumento = "DNI";
        private string _numeroDocumento = string.Empty;
        private string _observaciones = string.Empty;

        public int ClienteId
        {
            get => _clienteId;
            set
            {
                _clienteId = value;
                OnPropertyChanged();
            }
        }

        public string Nombres
        {
            get => _nombres;
            set
            {
                _nombres = value;
                OnPropertyChanged();
            }
        }

        public string Apellidos
        {
            get => _apellidos;
            set
            {
                _apellidos = value;
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

        public string TipoDocumento
        {
            get => _tipoDocumento;
            set
            {
                _tipoDocumento = value;
                OnPropertyChanged();
            }
        }

        public string NumeroDocumento
        {
            get => _numeroDocumento;
            set
            {
                _numeroDocumento = value;
                OnPropertyChanged();
            }
        }

        public string Observaciones
        {
            get => _observaciones;
            set
            {
                _observaciones = value;
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
                var cliente = await _databaseService.GetClienteByIdAsync(ClienteId);
                if (cliente != null)
                {
                    Nombres = cliente.Nombres;
                    Apellidos = cliente.Apellidos;
                    Telefono = cliente.Telefono;
                    TipoDocumento = cliente.TipoDocumento;
                    NumeroDocumento = cliente.NumeroDocumento;
                    Observaciones = cliente.Observaciones ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo cargar el cliente: {ex.Message}", "OK");
            }
        }

        private async Task GuardarClienteAsync()
        {
            if (string.IsNullOrWhiteSpace(Nombres))
            {
                await Shell.Current.DisplayAlert("Error", "Los nombres son requeridos", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Apellidos))
            {
                await Shell.Current.DisplayAlert("Error", "Los apellidos son requeridos", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Telefono))
            {
                await Shell.Current.DisplayAlert("Error", "El teléfono es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NumeroDocumento))
            {
                await Shell.Current.DisplayAlert("Error", $"El {TipoDocumento} es requerido", "OK");
                return;
            }

            try
            {
                var cliente = await _databaseService.GetClienteByIdAsync(ClienteId);
                if (cliente != null)
                {
                    cliente.Nombres = Nombres;
                    cliente.Apellidos = Apellidos;
                    cliente.Telefono = Telefono;
                    cliente.TipoDocumento = TipoDocumento;
                    cliente.NumeroDocumento = NumeroDocumento;
                    cliente.Observaciones = Observaciones;

                    await _databaseService.SaveClienteAsync(cliente);
                    await Shell.Current.DisplayAlert("Éxito", "Cliente actualizado correctamente", "OK");
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
