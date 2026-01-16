using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class BienvenidaPage : ContentPage
    {
        public BienvenidaPage()
        {
            InitializeComponent();
            BindingContext = new BienvenidaViewModel();
        }
    }
}
