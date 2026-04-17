using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using App_CrediVnzl.Models;

namespace App_CrediVnzl.Services
{
    public class NotificationService
    {
        private readonly DatabaseService _databaseService;
        
        public NotificationService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task InicializarNotificacionesAsync()
        {
            try
            {
                // Solicitar permiso de notificaciones en Android 13+
                if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
                {
                    await LocalNotificationCenter.Current.RequestNotificationPermission();
                }

                // Cancelar notificaciones antiguas
                LocalNotificationCenter.Current.CancelAll();

                // Programar todas las notificaciones
                await ProgramarNotificacionesPagosProximosAsync();
                await ProgramarNotificacionesPagosHoyAsync();
                await ProgramarNotificacionesAtrasosAsync();
                await ProgramarResumenDiarioAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al inicializar notificaciones: {ex.Message}");
            }
        }

        // 1. Notificaci�n: "Pago pr�ximo - Cliente [Nombre]" - 1 d�a antes
        public async Task ProgramarNotificacionesPagosProximosAsync()
        {
            var prestamos = await _databaseService.GetPrestamosActivosAsync();
            
            foreach (var prestamo in prestamos)
            {
                if (prestamo.FechaProximoPago.HasValue)
                {
                    var cliente = await _databaseService.GetClienteAsync(prestamo.ClienteId);
                    if (cliente == null) continue;

                    var fechaNotificacion = prestamo.FechaProximoPago.Value.AddDays(-1);
                    
                    // Solo programar si la fecha es futura
                    if (fechaNotificacion > DateTime.Now)
                    {
                        var nombreCompleto = $"{cliente.Nombres} {cliente.Apellidos}";
                        var montoEsperado = prestamo.InteresSemanalActual;

                        var request = new NotificationRequest
                        {
                            NotificationId = 1000 + prestamo.Id,
                            Title = "Pago pr�ximo ma�ana",
                            Description = $"{nombreCompleto} debe pagar ${montoEsperado:N2}",
                            Schedule = new NotificationRequestSchedule
                            {
                                NotifyTime = fechaNotificacion.Date.AddHours(9) // 9:00 AM
                            },
                            Android = new AndroidOptions
                            {
                                ChannelId = "pagos_proximos",
                                Priority = AndroidPriority.High
                            }
                        };

                        await LocalNotificationCenter.Current.Show(request);
                    }
                }
            }
        }

        // 2. Notificaci�n: "Hoy vence pago de [Nombre]" - El d�a del pago
        public async Task ProgramarNotificacionesPagosHoyAsync()
        {
            var prestamos = await _databaseService.GetPrestamosActivosAsync();
            
            foreach (var prestamo in prestamos)
            {
                if (prestamo.FechaProximoPago.HasValue)
                {
                    var cliente = await _databaseService.GetClienteAsync(prestamo.ClienteId);
                    if (cliente == null) continue;

                    var fechaNotificacion = prestamo.FechaProximoPago.Value.Date;
                    
                    if (fechaNotificacion.Date == DateTime.Now.Date || fechaNotificacion > DateTime.Now)
                    {
                        var nombreCompleto = $"{cliente.Nombres} {cliente.Apellidos}";
                        var montoEsperado = prestamo.InteresSemanalActual;

                        var request = new NotificationRequest
                        {
                            NotificationId = 2000 + prestamo.Id,
                            Title = "�Hoy vence un pago!",
                            Description = $"{nombreCompleto} - ${montoEsperado:N2}",
                            Schedule = new NotificationRequestSchedule
                            {
                                NotifyTime = fechaNotificacion.Date.AddHours(8) // 8:00 AM
                            },
                            Android = new AndroidOptions
                            {
                                ChannelId = "pagos_hoy",
                                Priority = AndroidPriority.Max
                            }
                        };

                        await LocalNotificationCenter.Current.Show(request);
                    }
                }
            }
        }

        // 3. Notificaci�n: "Tienes X cobros pendientes hoy" - Resumen diario
        public async Task ProgramarResumenDiarioAsync()
        {
            var prestamos = await _databaseService.GetPrestamosActivosAsync();
            var pagosHoy = prestamos.Where(p => p.FechaProximoPago.HasValue && 
                                                  p.FechaProximoPago.Value.Date == DateTime.Now.Date).ToList();

            if (pagosHoy.Count > 0)
            {
                var totalEsperado = pagosHoy.Sum(p => p.InteresSemanalActual);

                var request = new NotificationRequest
                {
                    NotificationId = 9000,
                    Title = $"Resumen del d�a",
                    Description = $"Tienes {pagosHoy.Count} cobros pendientes hoy - Total: ${totalEsperado:N2}",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.Date.AddHours(7) // 7:00 AM
                    },
                    Android = new AndroidOptions
                    {
                        ChannelId = "resumen_diario",
                        Priority = AndroidPriority.High
                    }
                };

                await LocalNotificationCenter.Current.Show(request);
            }
        }

        // 4. Notificaci�n: "Pago vencido - [Nombre] lleva X d�as de atraso"
        public async Task ProgramarNotificacionesAtrasosAsync()
        {
            var prestamos = await _databaseService.GetPrestamosActivosAsync();
            
            foreach (var prestamo in prestamos)
            {
                if (prestamo.FechaProximoPago.HasValue && prestamo.FechaProximoPago.Value.Date < DateTime.Now.Date)
                {
                    var cliente = await _databaseService.GetClienteAsync(prestamo.ClienteId);
                    if (cliente == null) continue;

                    var diasAtraso = (DateTime.Now.Date - prestamo.FechaProximoPago.Value.Date).Days;
                    var nombreCompleto = $"{cliente.Nombres} {cliente.Apellidos}";

                    var request = new NotificationRequest
                    {
                        NotificationId = 3000 + prestamo.Id,
                        Title = "Pago vencido",
                        Description = $"{nombreCompleto} lleva {diasAtraso} d�a{(diasAtraso != 1 ? "s" : "")} de atraso",
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = DateTime.Now.AddMinutes(5) // Notificar en 5 minutos
                        },
                        Android = new AndroidOptions
                        {
                            ChannelId = "atrasos",
                            Priority = AndroidPriority.Max
                        }
                    };

                    await LocalNotificationCenter.Current.Show(request);
                }
            }
        }

        // M�todo para notificar inmediatamente cuando se registra un pago
        public async Task NotificarPagoRegistradoAsync(Prestamo prestamo, decimal montoPagado)
        {
            var cliente = await _databaseService.GetClienteAsync(prestamo.ClienteId);
            if (cliente == null) return;

            var nombreCompleto = $"{cliente.Nombres} {cliente.Apellidos}";

            var request = new NotificationRequest
            {
                NotificationId = 5000 + prestamo.Id,
                Title = "Pago registrado",
                Description = $"{nombreCompleto} pag� ${montoPagado:N2}",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(2)
                },
                Android = new AndroidOptions
                {
                    ChannelId = "confirmaciones",
                    Priority = AndroidPriority.High
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }

        // Actualizar todas las notificaciones (llamar despu�s de cambios en pr�stamos)
        public async Task ActualizarTodasLasNotificacionesAsync()
        {
            await InicializarNotificacionesAsync();
        }

        // Cancelar notificaci�n espec�fica de un pr�stamo
        public void CancelarNotificacionesPrestamoAsync(int prestamoId)
        {
            LocalNotificationCenter.Current.Cancel(1000 + prestamoId); // Pago pr�ximo
            LocalNotificationCenter.Current.Cancel(2000 + prestamoId); // Pago hoy
            LocalNotificationCenter.Current.Cancel(3000 + prestamoId); // Atraso
        }
    }
}
