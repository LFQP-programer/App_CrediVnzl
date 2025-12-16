namespace App_CrediVnzl.Services
{
    public class WhatsAppService
    {
        public async Task<bool> EnviarMensajeAsync(string numeroTelefono, string mensaje)
        {
            try
            {
                // Limpiar el número de teléfono (quitar espacios, guiones, etc.)
                var numeroLimpio = LimpiarNumeroTelefono(numeroTelefono);

                if (string.IsNullOrWhiteSpace(numeroLimpio))
                {
                    return false;
                }

                // Codificar el mensaje para URL
                var mensajeCodificado = Uri.EscapeDataString(mensaje);

                // Construir la URL de WhatsApp
                var url = $"https://wa.me/{numeroLimpio}?text={mensajeCodificado}";

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
                foreach (var (telefono, mensaje) in mensajes)
                {
                    await EnviarMensajeAsync(telefono, mensaje);
                    await Task.Delay(delayMilisegundos);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string LimpiarNumeroTelefono(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return string.Empty;

            // Remover todos los caracteres que no sean dígitos o el símbolo +
            var numeroLimpio = new string(numero.Where(c => char.IsDigit(c) || c == '+').ToArray());

            // Si comienza con +, dejarlo; si no, podría necesitar código de país
            if (!numeroLimpio.StartsWith("+"))
            {
                // Si el número tiene 10 dígitos, probablemente sea un número local
                // Aquí podrías agregar el código de país por defecto si es necesario
                // Por ejemplo para Venezuela: numeroLimpio = "58" + numeroLimpio;
            }
            else
            {
                // Remover el + inicial ya que la API de WhatsApp no lo necesita
                numeroLimpio = numeroLimpio.TrimStart('+');
            }

            return numeroLimpio;
        }

        public bool ValidarNumeroTelefono(string numero)
        {
            var numeroLimpio = LimpiarNumeroTelefono(numero);
            
            // Un número de teléfono válido debe tener al menos 10 dígitos
            return numeroLimpio.Length >= 10;
        }
    }
}
