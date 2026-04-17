using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;

namespace App_CrediVnzl.Pages
{
    public partial class AyudaPage : ContentPage
    {
        public AyudaPage()
        {
            InitializeComponent();
        }

        private async void OnContactarSoporteClicked(object sender, EventArgs e)
        {
            try
            {
                // Un número de soporte de ejemplo. Reemplazar con el número real de soporte si es necesario.
                string soporteWhatsAppNumber = "+11234567890";
                string message = "Hola, necesito soporte con la app CrediVnzl.";

                string url = $"https://wa.me/{soporteWhatsAppNumber.Replace("+", "")}?text={Uri.EscapeDataString(message)}";
                await Launcher.Default.OpenAsync(url);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se pudo abrir WhatsApp.", "Aceptar");
                System.Diagnostics.Debug.WriteLine($"Error al abrir WhatsApp de soporte: {ex.Message}");
            }
        }
    }
}