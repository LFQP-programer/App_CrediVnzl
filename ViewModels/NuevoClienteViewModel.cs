using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class NuevoClienteViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private Cliente _cliente = new();
        private bool _isEditing;

        public Cliente Cliente
        {
            get => _cliente;
            set
            {
                _cliente = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PageTitle));
            }
        }

        public string PageTitle => IsEditing ? "Editar Cliente" : "Nuevo Cliente";

        public ICommand GuardarCommand { get; }
        public ICommand CancelarCommand { get; }

        public NuevoClienteViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            GuardarCommand = new Command(async () => await GuardarClienteAsync());
            CancelarCommand = new Command(async () => await CancelarAsync());
        }

        public void Initialize(Cliente? cliente = null)
        {
            if (cliente != null)
            {
                Cliente = cliente;
                IsEditing = true;
            }
            else
            {
                Cliente = new Cliente();
                IsEditing = false;
            }
        }

        private async Task GuardarClienteAsync()
        {
            if (string.IsNullOrWhiteSpace(Cliente.NombreCompleto))
            {
                await Shell.Current.DisplayAlertAsync("Error", "El nombre completo es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Cliente.Telefono))
            {
                await Shell.Current.DisplayAlertAsync("Error", "El telefono es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Cliente.Cedula))
            {
                await Shell.Current.DisplayAlertAsync("Error", "La cedula/DNI es requerida", "OK");
                return;
            }

            try
            {
                await _databaseService.SaveClienteAsync(Cliente);
                await Shell.Current.DisplayAlertAsync("Exito", "Cliente guardado correctamente", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"No se pudo guardar el cliente: {ex.Message}", "OK");
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
