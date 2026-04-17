using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using App_CrediVnzl.Platforms.Android;

namespace App_CrediVnzl
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, 
              LaunchMode = LaunchMode.SingleTop,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
              WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Forzar tema claro en Android - Deshabilita el modo oscuro del sistema
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
            
            // Configuración para el teclado - CRÍTICO para que funcione el scroll
            if (Window != null)
            {
                // AdjustResize redimensiona la ventana cuando aparece el teclado
                Window.SetSoftInputMode(SoftInput.AdjustResize);
            }
            
            // Inicializar el helper de teclado para scroll automático
            KeyboardHelper.Initialize(this);
            
            System.Diagnostics.Debug.WriteLine("*** MainActivity: Configuración de teclado completada ***");

            // Crear canales de notificación para Android 8.0+
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                CreateNotificationChannels();
            }
        }

        private void CreateNotificationChannels()
        {
            var notificationManager = (NotificationManager?)GetSystemService(NotificationService);
            if (notificationManager == null) return;

            // Canal: Pagos Próximos
            var channel1 = new NotificationChannel("pagos_proximos", "Pagos Próximos", NotificationImportance.High)
            {
                Description = "Notificaciones de pagos que vencen pronto"
            };
            notificationManager.CreateNotificationChannel(channel1);

            // Canal: Pagos de Hoy
            var channel2 = new NotificationChannel("pagos_hoy", "Pagos de Hoy", NotificationImportance.Max)
            {
                Description = "Notificaciones de pagos que vencen hoy"
            };
            channel2.EnableVibration(true);
            notificationManager.CreateNotificationChannel(channel2);

            // Canal: Resumen Diario
            var channel3 = new NotificationChannel("resumen_diario", "Resumen Diario", NotificationImportance.High)
            {
                Description = "Resumen de cobros pendientes del día"
            };
            notificationManager.CreateNotificationChannel(channel3);

            // Canal: Atrasos
            var channel4 = new NotificationChannel("atrasos", "Pagos Vencidos", NotificationImportance.Max)
            {
                Description = "Alertas de pagos vencidos y atrasos"
            };
            channel4.EnableVibration(true);
            notificationManager.CreateNotificationChannel(channel4);

            // Canal: Confirmaciones
            var channel5 = new NotificationChannel("confirmaciones", "Confirmaciones", NotificationImportance.High)
            {
                Description = "Confirmaciones de pagos registrados"
            };
            notificationManager.CreateNotificationChannel(channel5);

            System.Diagnostics.Debug.WriteLine("*** CANALES DE NOTIFICACIÓN CREADOS ***");
        }
    }
}



