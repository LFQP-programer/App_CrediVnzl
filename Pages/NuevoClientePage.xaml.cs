using App_CrediVnzl.ViewModels;
using App_CrediVnzl.Services;
using App_CrediVnzl.Models;

namespace App_CrediVnzl.Pages
{
    public partial class NuevoClientePage : ContentPage
    {
        private readonly NuevoClienteViewModel _viewModel;

        public NuevoClientePage(DatabaseService databaseService, WhatsAppService whatsAppService)
        {
            InitializeComponent();
            _viewModel = new NuevoClienteViewModel(databaseService, whatsAppService);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.Initialize();
        }

        private void OnTipoAvalChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is RadioButton radioButton && e.Value)
            {
                _viewModel.TipoAval = radioButton.Content?.ToString() ?? "Nueva persona";
            }
        }
    }
}
