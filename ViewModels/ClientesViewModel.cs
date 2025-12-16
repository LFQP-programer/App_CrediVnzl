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

        public ClientesViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            RefreshCommand = new Command(async () => await LoadClientesAsync());
            AddClienteCommand = new Command(async () => await NavigateToNewCliente());
            
            _ = LoadClientesAsync();
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
