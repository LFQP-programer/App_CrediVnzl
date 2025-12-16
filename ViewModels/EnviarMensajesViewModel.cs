using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using App_CrediVnzl.Models;
using App_CrediVnzl.Services;

namespace App_CrediVnzl.ViewModels
{
    public class EnviarMensajesViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private readonly WhatsAppService _whatsAppService;

        private string _tipoMensajeSeleccionado = "Recordatorios";
        private Cliente? _clienteSeleccionado;
        private string _mensajePersonalizado = string.Empty;
        private string _mensajeMasivo = string.Empty;
        private int _totalMensajesRecordatorios;
        private int _totalMensajesMasivos;
        private string _vistaPrevia = string.Empty;
        private bool _tienePagosProximos;

        public ObservableCollection<Cliente> Clientes { get; set; } = new();
        public ObservableCollection<Pago> PagosProximos { get; set; } = new();

        public ICommand SeleccionarTipoMensajeCommand { get; }
        public ICommand EnviarMensajesCommand { get; }

        public EnviarMensajesViewModel(DatabaseService databaseService, WhatsAppService whatsAppService)
        {
            _databaseService = databaseService;
            _whatsAppService = whatsAppService;

            SeleccionarTipoMensajeCommand = new Command<string>(SeleccionarTipoMensaje);
            EnviarMensajesCommand = new Command(async () => await EnviarMensajes(), () => PuedeEnviar);
        }

        public string TipoMensajeSeleccionado
        {
            get => _tipoMensajeSeleccionado;
            set
            {
                _tipoMensajeSeleccionado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EsRecordatorios));
                OnPropertyChanged(nameof(EsIndividual));
                OnPropertyChanged(nameof(EsMasivo));
                OnPropertyChanged(nameof(TextoBotonEnviar));
                OnPropertyChanged(nameof(ColorBotonEnviar));
                OnPropertyChanged(nameof(PuedeEnviar));
            }
        }

        public Cliente? ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set
            {
                _clienteSeleccionado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PuedeEnviar));
            }
        }

        public string MensajePersonalizado
        {
            get => _mensajePersonalizado;
            set
            {
                _mensajePersonalizado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PuedeEnviar));
            }
        }

        public string MensajeMasivo
        {
            get => _mensajeMasivo;
            set
            {
                _mensajeMasivo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PuedeEnviar));
            }
        }

        public int TotalMensajesRecordatorios
        {
            get => _totalMensajesRecordatorios;
            set
            {
                _totalMensajesRecordatorios = value;
                OnPropertyChanged();
            }
        }

        public int TotalMensajesMasivos
        {
            get => _totalMensajesMasivos;
            set
            {
                _totalMensajesMasivos = value;
                OnPropertyChanged();
            }
        }

        public string VistaPrevia
        {
            get => _vistaPrevia;
            set
            {
                _vistaPrevia = value;
                OnPropertyChanged();
            }
        }

        public bool TienePagosProximos
        {
            get => _tienePagosProximos;
            set
            {
                _tienePagosProximos = value;
                OnPropertyChanged();
            }
        }

        public bool EsRecordatorios => TipoMensajeSeleccionado == "Recordatorios";
        public bool EsIndividual => TipoMensajeSeleccionado == "Individual";
        public bool EsMasivo => TipoMensajeSeleccionado == "Masivo";

        public string TextoBotonEnviar
        {
            get
            {
                return TipoMensajeSeleccionado switch
                {
                    "Recordatorios" => $"?? Enviar a Todos ({TotalMensajesRecordatorios})",
                    "Individual" => "?? Enviar por WhatsApp",
                    "Masivo" => $"?? Enviar a Todos ({TotalMensajesMasivos})",
                    _ => "Enviar"
                };
            }
        }

        public string ColorBotonEnviar
        {
            get
            {
                return TipoMensajeSeleccionado switch
                {
                    "Individual" => "#25D366",
                    _ => "#757575"
                };
            }
        }

        public bool PuedeEnviar
        {
            get
            {
                return TipoMensajeSeleccionado switch
                {
                    "Recordatorios" => TotalMensajesRecordatorios > 0,
                    "Individual" => ClienteSeleccionado != null && !string.IsNullOrWhiteSpace(MensajePersonalizado),
                    "Masivo" => TotalMensajesMasivos > 0 && !string.IsNullOrWhiteSpace(MensajeMasivo),
                    _ => false
                };
            }
        }

        private void SeleccionarTipoMensaje(string tipo)
        {
            TipoMensajeSeleccionado = tipo;
            
            if (tipo == "Recordatorios" || tipo == "Masivo")
            {
                ActualizarVistaPrevia();
            }
        }

        public async Task LoadDataAsync()
        {
            try
            {
                // Cargar clientes
                var clientes = await _databaseService.GetClientesAsync();
                Clientes.Clear();
                foreach (var cliente in clientes)
                {
                    Clientes.Add(cliente);
                }

                // Cargar pagos próximos (hoy y mañana)
                var hoy = DateTime.Today;
                var manana = hoy.AddDays(1);
                
                var pagosHoy = await _databaseService.GetPagosByFechaAsync(hoy);
                var pagosManana = await _databaseService.GetPagosByFechaAsync(manana);

                PagosProximos.Clear();
                foreach (var pago in pagosHoy.Concat(pagosManana))
                {
                    if (pago.Estado == "Pendiente")
                    {
                        PagosProximos.Add(pago);
                    }
                }

                TotalMensajesRecordatorios = PagosProximos.Count;
                TotalMensajesMasivos = PagosProximos.Count;
                TienePagosProximos = PagosProximos.Count > 0;

                ActualizarVistaPrevia();

                OnPropertyChanged(nameof(PuedeEnviar));
                OnPropertyChanged(nameof(TextoBotonEnviar));
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "OK");
            }
        }

        private void ActualizarVistaPrevia()
        {
            if (!TienePagosProximos)
            {
                VistaPrevia = "No hay pagos pendientes próximos";
                return;
            }

            var primerPago = PagosProximos.FirstOrDefault();
            if (primerPago != null)
            {
                var mensaje = GenerarMensajeRecordatorio(primerPago);
                VistaPrevia = mensaje;
            }
        }

        private string GenerarMensajeRecordatorio(Pago pago)
        {
            var fecha = pago.FechaProgramada.Date == DateTime.Today ? "hoy" : "mañana";
            return $"Hola {pago.ClienteNombre},\n\n" +
                   $"Te recordamos que tienes un pago programado para {fecha}.\n\n" +
                   $"?? Monto: ${pago.MontoPago:N2}\n" +
                   $"?? Fecha: {pago.FechaProgramada:dd/MM/yyyy}\n\n" +
                   $"Gracias por tu puntualidad.";
        }

        private async Task EnviarMensajes()
        {
            try
            {
                switch (TipoMensajeSeleccionado)
                {
                    case "Recordatorios":
                        await EnviarRecordatorios();
                        break;
                    case "Individual":
                        await EnviarMensajeIndividual();
                        break;
                    case "Masivo":
                        await EnviarMensajesMasivos();
                        break;
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Error al enviar mensajes: {ex.Message}", "OK");
            }
        }

        private async Task EnviarRecordatorios()
        {
            var confirmacion = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar envío",
                $"¿Deseas enviar {TotalMensajesRecordatorios} recordatorios por WhatsApp?",
                "Sí, enviar",
                "Cancelar");

            if (!confirmacion) return;

            var exitosos = 0;
            foreach (var pago in PagosProximos)
            {
                var mensaje = GenerarMensajeRecordatorio(pago);
                var enviado = await _whatsAppService.EnviarMensajeAsync(pago.ClienteTelefono ?? "", mensaje);
                if (enviado) exitosos++;
                
                // Pequeña pausa entre mensajes
                await Task.Delay(500);
            }

            await Application.Current.MainPage.DisplayAlert(
                "Envío completado",
                $"Se abrieron {exitosos} de {TotalMensajesRecordatorios} conversaciones de WhatsApp.",
                "OK");
        }

        private async Task EnviarMensajeIndividual()
        {
            if (ClienteSeleccionado == null || string.IsNullOrWhiteSpace(ClienteSeleccionado.Telefono))
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", "El cliente no tiene número de teléfono registrado", "OK");
                return;
            }

            var enviado = await _whatsAppService.EnviarMensajeAsync(ClienteSeleccionado.Telefono, MensajePersonalizado);

            if (enviado)
            {
                await Application.Current.MainPage.DisplayAlert("Éxito", "WhatsApp se ha abierto con el mensaje listo para enviar", "OK");
                MensajePersonalizado = string.Empty;
                ClienteSeleccionado = null;
            }
        }

        private async Task EnviarMensajesMasivos()
        {
            var confirmacion = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar envío",
                $"¿Deseas enviar este mensaje a {TotalMensajesMasivos} clientes por WhatsApp?",
                "Sí, enviar",
                "Cancelar");

            if (!confirmacion) return;

            var exitosos = 0;
            foreach (var pago in PagosProximos)
            {
                var mensaje = $"Hola {pago.ClienteNombre},\n\n{MensajeMasivo}";
                var enviado = await _whatsAppService.EnviarMensajeAsync(pago.ClienteTelefono ?? "", mensaje);
                if (enviado) exitosos++;
                
                await Task.Delay(500);
            }

            await Application.Current.MainPage.DisplayAlert(
                "Envío completado",
                $"Se abrieron {exitosos} de {TotalMensajesMasivos} conversaciones de WhatsApp.",
                "OK");

            MensajeMasivo = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
