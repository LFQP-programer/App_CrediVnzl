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
                    System.Diagnostics.Debug.WriteLine("Número de teléfono vacío o inválido");
                    return false;
                }

                // Validar que el número tenga al menos 9 dígitos (formato Perú)
                if (numeroLimpio.Length < 9)
                {
                    System.Diagnostics.Debug.WriteLine($"Número de teléfono muy corto: {numeroLimpio}");
                    return false;
                }

                // Codificar el mensaje para URL
                var mensajeCodificado = Uri.EscapeDataString(mensaje);

                // Construir la URL de WhatsApp
                // Formato: https://wa.me/51XXXXXXXXX?text=mensaje
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
                System.Diagnostics.Debug.WriteLine($"Error en envío de mensajes seguidos: {ex.Message}");
                return false;
            }
        }

        private string LimpiarNumeroTelefono(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return string.Empty;

            // Remover todos los caracteres que no sean dígitos o el símbolo +
            var numeroLimpio = new string(numero.Where(c => char.IsDigit(c) || c == '+').ToArray());

            // Si el número empieza con +, removerlo (la API de WhatsApp no lo necesita)
            if (numeroLimpio.StartsWith("+"))
            {
                numeroLimpio = numeroLimpio.TrimStart('+');
            }

            // Detección automática de formato peruano
            // Perú: 9 dígitos, código de país 51
            
            if (numeroLimpio.Length == 9)
            {
                // Formato: 987654321 -> 51987654321
                // Verificar que empiece con 9 (móviles en Perú)
                if (numeroLimpio.StartsWith("9"))
                {
                    numeroLimpio = "51" + numeroLimpio;
                }
            }
            else if (numeroLimpio.Length == 11 && numeroLimpio.StartsWith("51"))
            {
                // Ya tiene el formato correcto: 51987654321
                // No hacer nada
            }
            else if (numeroLimpio.Length == 10 && numeroLimpio.StartsWith("51"))
            {
                // Posible error, faltan dígitos
                // Intentar agregar un 9 después del 51
                System.Diagnostics.Debug.WriteLine($"Número posiblemente incorrecto: {numeroLimpio}");
            }

            return numeroLimpio;
        }

        public bool ValidarNumeroTelefono(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return false;

            var numeroLimpio = LimpiarNumeroTelefono(numero);
            
            // Número peruano válido: 11 dígitos (51 + 9 dígitos)
            // O 9 dígitos sin código de país
            return (numeroLimpio.Length == 11 && numeroLimpio.StartsWith("51")) || 
                   (numeroLimpio.Length == 9 && numeroLimpio.StartsWith("9"));
        }

        public string FormatearNumeroTelefono(string numero)
        {
            var numeroLimpio = LimpiarNumeroTelefono(numero);
            
            if (string.IsNullOrWhiteSpace(numeroLimpio))
                return string.Empty;

            // Si es número peruano completo (51 + 9 dígitos)
            if (numeroLimpio.Length == 11 && numeroLimpio.StartsWith("51"))
            {
                // Formato: +51 987 654 321
                return $"+{numeroLimpio.Substring(0, 2)} {numeroLimpio.Substring(2, 3)} {numeroLimpio.Substring(5, 3)} {numeroLimpio.Substring(8, 3)}";
            }

            // Si es formato peruano sin código de país (9 dígitos)
            if (numeroLimpio.Length == 9 && numeroLimpio.StartsWith("9"))
            {
                // Formato: 987 654 321
                return $"{numeroLimpio.Substring(0, 3)} {numeroLimpio.Substring(3, 3)} {numeroLimpio.Substring(6, 3)}";
            }

            // Formato genérico con espacios cada 3 dígitos
            if (numeroLimpio.Length > 6)
            {
                var formatted = "";
                for (int i = 0; i < numeroLimpio.Length; i += 3)
                {
                    int length = Math.Min(3, numeroLimpio.Length - i);
                    formatted += numeroLimpio.Substring(i, length);
                    if (i + length < numeroLimpio.Length)
                        formatted += " ";
                }
                return formatted;
            }

            return numeroLimpio;
        }
    }
}
