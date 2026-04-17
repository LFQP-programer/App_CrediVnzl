using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class BienvenidaPage : ContentPage
    {
        public BienvenidaPage()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("*** BienvenidaPage - Constructor iniciado ***");
                InitializeComponent();
                System.Diagnostics.Debug.WriteLine("*** BienvenidaPage - InitializeComponent OK ***");
                
                var viewModel = new BienvenidaViewModel();
                BindingContext = viewModel;
                System.Diagnostics.Debug.WriteLine($"*** BienvenidaPage - BindingContext establecido: {BindingContext != null} ***");
                System.Diagnostics.Debug.WriteLine($"*** BienvenidaPage - ViewModel tipo: {viewModel.GetType().Name} ***");
                System.Diagnostics.Debug.WriteLine($"*** BienvenidaPage - AdminLoginCommand: {viewModel.AdminLoginCommand != null} ***");
                System.Diagnostics.Debug.WriteLine($"*** BienvenidaPage - ClienteLoginCommand: {viewModel.ClienteLoginCommand != null} ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR en BienvenidaPage Constructor: {ex.Message} ***");
                System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace} ***");
                throw;
            }
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            System.Diagnostics.Debug.WriteLine("*** BienvenidaPage - OnAppearing llamado ***");
            System.Diagnostics.Debug.WriteLine($"*** BienvenidaPage - BindingContext v�lido: {BindingContext != null} ***");
        }
    }
}
