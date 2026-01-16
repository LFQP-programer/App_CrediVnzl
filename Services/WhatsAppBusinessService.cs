using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using App_CrediVnzl.Models;

namespace App_CrediVnzl.Services
{
    /// <summary>
    /// Servicio para env�o autom�tico de mensajes mediante WhatsApp Business API
    /// Requiere: WhatsApp Business Account y Access Token de Meta
    /// Documentaci�n: https://developers.facebook.com/docs/whatsapp/cloud-api
    /// </summary>
    public class WhatsAppBusinessService
    {
        private readonly HttpClient _httpClient;
        private readonly WhatsAppConfig _config;
        private const int MAX_RETRIES = 3;
        private const int DELAY_BETWEEN_MESSAGES_MS = 1000; // 1 segundo entre mensajes

        public WhatsAppBusinessService(HttpClient httpClient, WhatsAppConfig config)
        {
            _httpClient = httpClient;
            _config = config;
            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _config.AccessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Enviar mensaje de texto simple (100% autom�tico)
        /// </summary>
        public async Task<WhatsAppSendResult> EnviarMensajeTextoAsync(string numeroTelefono, string mensaje)
        {
            try
            {
                if (!_config.IsValid())
                {
                    return new WhatsAppSendResult
                    {
                        Success = false,
                        ErrorMessage = "Configuraci�n de WhatsApp Business API incompleta",
                        PhoneNumber = numeroTelefono
                    };
                }

                var numeroLimpio = LimpiarNumeroTelefono(numeroTelefono);
                
                if (string.IsNullOrEmpty(numeroLimpio))
                {
                    return new WhatsAppSendResult
                    {
                        Success = false,
                        ErrorMessage = "N�mero de tel�fono inv�lido",
                        PhoneNumber = numeroTelefono
                    };
                }

                var request = new WhatsAppTextMessageRequest
                {
                    To = numeroLimpio,
                    Text = new WhatsAppTextContent
                    {
                        Body = mensaje,
                        PreviewUrl = false
                    }
                };

                var result = await EnviarConReintentos(request, numeroTelefono);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al enviar mensaje WhatsApp: {ex.Message}");
                return new WhatsAppSendResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    PhoneNumber = numeroTelefono
                };
            }
        }

        /// <summary>
        /// Enviar mensaje usando una plantilla aprobada (recomendado para notificaciones)
        /// </summary>
        public async Task<WhatsAppSendResult> EnviarMensajePlantillaAsync(
            string numeroTelefono, 
            string nombrePlantilla, 
            List<string> parametros)
        {
            try
            {
                if (!_config.IsValid())
                {
                    return new WhatsAppSendResult
                    {
                        Success = false,
                        ErrorMessage = "Configuraci�n de WhatsApp Business API incompleta",
                        PhoneNumber = numeroTelefono
                    };
                }

                var numeroLimpio = LimpiarNumeroTelefono(numeroTelefono);

                var request = new WhatsAppTemplateMessageRequest
                {
                    To = numeroLimpio,
                    Template = new WhatsAppTemplate
                    {
                        Name = nombrePlantilla,
                        Language = new WhatsAppLanguage { Code = "es" },
                        Components = new List<WhatsAppComponent>
                        {
                            new WhatsAppComponent
                            {
                                Type = "body",
                                Parameters = parametros.Select(p => new WhatsAppParameter
                                {
                                    Type = "text",
                                    Text = p
                                }).ToList()
                            }
                        }
                    }
                };

                var result = await EnviarConReintentos(request, numeroTelefono);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al enviar plantilla WhatsApp: {ex.Message}");
                return new WhatsAppSendResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    PhoneNumber = numeroTelefono
                };
            }
        }

        /// <summary>
        /// Enviar mensajes masivos con delay autom�tico
        /// </summary>
        public async Task<List<WhatsAppSendResult>> EnviarMensajesMasivosAsync(
            List<(string telefono, string mensaje)> mensajes,
            IProgress<int>? progreso = null)
        {
            var resultados = new List<WhatsAppSendResult>();
            var contador = 0;

            foreach (var (telefono, mensaje) in mensajes)
            {
                var resultado = await EnviarMensajeTextoAsync(telefono, mensaje);
                resultados.Add(resultado);

                contador++;
                progreso?.Report(contador);

                // Delay entre mensajes para evitar rate limiting
                if (contador < mensajes.Count)
                {
                    await Task.Delay(DELAY_BETWEEN_MESSAGES_MS);
                }
            }

            return resultados;
        }

        /// <summary>
        /// Enviar con reintentos autom�ticos
        /// </summary>
        private async Task<WhatsAppSendResult> EnviarConReintentos(
            object request, 
            string numeroTelefono, 
            int intentoActual = 0)
        {
            try
            {
                var url = _config.GetMessagesUrl();
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"Enviando mensaje WhatsApp a: {numeroTelefono}");
                System.Diagnostics.Debug.WriteLine($"URL: {url}");
                System.Diagnostics.Debug.WriteLine($"Payload: {json}");

                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<WhatsAppMessageResponse>(
                        responseContent, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (result?.IsSuccess == true)
                    {
                        return new WhatsAppSendResult
                        {
                            Success = true,
                            MessageId = result.Messages?.FirstOrDefault()?.Id,
                            PhoneNumber = numeroTelefono
                        };
                    }
                    else
                    {
                        return new WhatsAppSendResult
                        {
                            Success = false,
                            ErrorMessage = result?.Error?.Message ?? "Error desconocido",
                            PhoneNumber = numeroTelefono
                        };
                    }
                }
                else
                {
                    // Reintentar en caso de error temporal
                    if (intentoActual < MAX_RETRIES && 
                        (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                         response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable))
                    {
                        var delay = (int)Math.Pow(2, intentoActual) * 1000; // Exponential backoff
                        await Task.Delay(delay);
                        return await EnviarConReintentos(request, numeroTelefono, intentoActual + 1);
                    }

                    return new WhatsAppSendResult
                    {
                        Success = false,
                        ErrorMessage = $"HTTP {(int)response.StatusCode}: {responseContent}",
                        PhoneNumber = numeroTelefono
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepci�n en EnviarConReintentos: {ex.Message}");
                
                // Reintentar en caso de excepci�n de red
                if (intentoActual < MAX_RETRIES)
                {
                    await Task.Delay(2000);
                    return await EnviarConReintentos(request, numeroTelefono, intentoActual + 1);
                }

                return new WhatsAppSendResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    PhoneNumber = numeroTelefono
                };
            }
        }

        /// <summary>
        /// Limpiar y formatear n�mero de tel�fono para WhatsApp
        /// </summary>
        private string LimpiarNumeroTelefono(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return string.Empty;

            // Remover todos los caracteres que no sean d�gitos o +
            var numeroLimpio = new string(numero.Where(c => char.IsDigit(c) || c == '+').ToArray());

            // Remover el + inicial
            if (numeroLimpio.StartsWith("+"))
            {
                numeroLimpio = numeroLimpio.TrimStart('+');
            }

            // Si el n�mero no comienza con c�digo de pa�s, agregar el de Venezuela (58)
            if (numeroLimpio.Length == 10 && numeroLimpio.StartsWith("0"))
            {
                // 0424XXXXXXX -> 58424XXXXXXX
                numeroLimpio = "58" + numeroLimpio.Substring(1);
            }
            else if (numeroLimpio.Length == 10 && numeroLimpio.StartsWith("4"))
            {
                // 424XXXXXXX -> 58424XXXXXXX
                numeroLimpio = "58" + numeroLimpio;
            }

            return numeroLimpio;
        }

        /// <summary>
        /// Verificar estado de la configuraci�n
        /// </summary>
        public async Task<bool> VerificarConexionAsync()
        {
            try
            {
                // Intenta obtener informaci�n del n�mero de tel�fono
                var url = $"{_config.BaseUrl}/{_config.PhoneNumberId}";
                var response = await _httpClient.GetAsync(url);
                
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtener l�mite actual de mensajes
        /// </summary>
        public async Task<(bool success, string tier, int limit)> ObtenerLimitesMensajesAsync()
        {
            try
            {
                var url = $"{_config.BaseUrl}/{_config.PhoneNumberId}?fields=quality_rating,messaging_limit_tier";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // Parsear respuesta para obtener tier y l�mite
                    // Por ahora retornar valores por defecto
                    return (true, "TIER_1", 1000);
                }

                return (false, "UNKNOWN", 0);
            }
            catch
            {
                return (false, "ERROR", 0);
            }
        }
    }
}
