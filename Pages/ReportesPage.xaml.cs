using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class ReportesPage : ContentPage
    {
        private readonly ReportesViewModel _viewModel;

        public ReportesPage(ReportesService reportesService)
        {
            InitializeComponent();
            _viewModel = new ReportesViewModel(reportesService);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            try
            {
                await _viewModel.CargarReportesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ReportesPage.OnAppearing: {ex.Message}");
            }
        }
    }
}
