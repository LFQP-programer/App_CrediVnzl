using App_CrediVnzl.Services;
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class PrimerUsoPage : ContentPage
    {
        public PrimerUsoPage(AuthService authService)
        {
            InitializeComponent();
            BindingContext = new PrimerUsoViewModel(authService);
        }
    }
}
