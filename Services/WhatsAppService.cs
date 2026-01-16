namespace App_CrediVnzl.Services
{
    public class WhatsAppService
    {
        public async Task<bool> EnviarMensajeAsync(string numeroTelefono, string mensaje)
        {
            try
            {
                // Limpiar el n�mero de tel�fono (quitar espacios, guiones, etc.)
                var numeroLimpio = LimpiarNumeroTelefono(numeroTelefono);

                if (string.IsNullOrWhiteSpace(numeroLimpio))
                {
                    System.Diagnostics.Debug.WriteLine("N�mero de tel�fono vac�o o inv�lido");
                    return false;
                }

                // Validar que el n�mero tenga al menos 10 d�gitos
                if (numeroLimpio.Length < 10)
                {
                    System.Diagnostics.Debug.WriteLine($"N�mero de tel�fono muy corto: {numeroLimpio}");
                    return false;
                }

                // Codificar el mensaje para URL
                var mensajeCodificado = Uri.EscapeDataString(mensaje);

                // Construir la URL de WhatsApp
                // Formato: https://wa.me/58XXXXXXXXXX?text=mensaje
                var url = $"https://wa.me/{numeroLimpio}?text={mensajeCodificado}";

                System.Diagnostics.Debug.WriteLine($"Abriendo WhatsApp con URL: {url}");

                // Abrir WhatsApp
                await Launcher.OpenAsync(new Uri(url));

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al enviar mensaje de WhatsApp: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EnviarMensajesSeguidos(List<(string telefono, string mensaje)> mensajes, int delayMilisegundos = 1000)
        {
            try
            {
                var exitosos = 0;
                
                foreach (var (telefono, mensaje) in mensajes)
                {
                    var enviado = await EnviarMensajeAsync(telefono, mensaje);
                    if (enviado) exitosos++;
                    await Task.Delay(delayMilisegundos);
                }
                
                return exitosos > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en env�o de mensajes seguidos: {ex.Message}");
                return false;
            }
        }

        private string LimpiarNumeroTelefono(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return string.Empty;

            // Remover todos los caracteres que no sean d�gitos o el s�mbolo +
            var numeroLimpio = new string(numero.Where(c => char.IsDigit(c) || c == '+').ToArray());

            // Si el n�mero empieza con +, removerlo (la API de WhatsApp no lo necesita)
            if (numeroLimpio.StartsWith("+"))
            {
                numeroLimpio = numeroLimpio.TrimStart('+');
            }

            // Si el n�mero no comienza con c�digo de pa�s, intentar detectarlo
            // Para Venezuela: si tiene 10 d�gitos y empieza con 0, cambiar 0 por 58
            // Si tiene 10 d�gitos y empieza con 4, agregar 58
            if (numeroLimpio.Length == 10)
            {
                if (numeroLimpio.StartsWith("0"))
                {
                    // Formato: 0424XXXXXXX -> 58424XXXXXXX
                    numeroLimpio = "58" + numeroLimpio.Substring(1);
                }
                else if (numeroLimpio.StartsWith("4"))
                {
                    // Formato: 424XXXXXXX -> 58424XXXXXXX
                    numeroLimpio = "58" + numeroLimpio;
                }
            }
            else if (numeroLimpio.Length == 11 && numeroLimpio.StartsWith("58"))
            {
                // Ya tiene el formato correcto: 58424XXXXXXX
                // No hacer nada
            }

            return numeroLimpio;
        }

        public bool ValidarNumeroTelefono(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return false;

            var numeroLimpio = LimpiarNumeroTelefono(numero);
            
            // Un n�mero de tel�fono v�lido debe tener al menos 10 d�gitos
            return numeroLimpio.Length >= 10 && numeroLimpio.Length <= 15;
        }

        public string FormatearNumeroTelefono(string numero)
        {
            var numeroLimpio = LimpiarNumeroTelefono(numero);
            
            if (string.IsNullOrWhiteSpace(numeroLimpio))
                return string.Empty;

            // Si es n�mero venezolano (58 + 10 d�gitos)
            if (numeroLimpio.Length == 12 && numeroLimpio.StartsWith("58"))
            {
                // Formato: +58 (424) 123-4567
                return $"+{numeroLimpio.Substring(0, 2)} ({numeroLimpio.Substring(2, 3)}) {numeroLimpio.Substring(5, 3)}-{numeroLimpio.Substring(8, 4)}";
            }

            // Si es formato venezolano sin c�digo de pa�s
            if (numeroLimpio.Length == 10 && numeroLimpio.StartsWith("4"))
            {
                // Formato: (424) 123-4567
                return $"({numeroLimpio.Substring(0, 3)}) {numeroLimpio.Substring(3, 3)}-{numeroLimpio.Substring(6, 4)}";
            }

            // Formato gen�rico con espacios cada 3-4 d�gitos
            if (numeroLimpio.Length > 6)
            {
                return $"{numeroLimpio.Substring(0, numeroLimpio.Length - 6)} {numeroLimpio.Substring(numeroLimpio.Length - 6, 3)} {numeroLimpio.Substring(numeroLimpio.Length - 3)}";
            }

            return numeroLimpio;
        }
    }
}
