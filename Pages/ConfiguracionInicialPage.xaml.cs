using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class ConfiguracionInicialPage : ContentPage
    {
        public ConfiguracionInicialPage(AuthService authService)
        {
            InitializeComponent();
            BindingContext = new PrimerUsoViewModel(authService);
        }
    }
}