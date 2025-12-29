namespace App_CrediVnzl.Models
{
    /// <summary>
    /// Configuración para WhatsApp Business API (Meta Cloud API)
    /// </summary>
    public class WhatsAppConfig
    {
        /// <summary>
        /// ID de la cuenta de WhatsApp Business
        /// Obtener en: https://business.facebook.com/latest/whatsapp_manager
        /// </summary>
        public string BusinessAccountId { get; set; } = string.Empty;

        /// <summary>
        /// ID del número de teléfono de WhatsApp
        /// Obtener en: Panel de Cloud API ? Phone Numbers
        /// </summary>
        public string PhoneNumberId { get; set; } = string.Empty;

        /// <summary>
        /// Token de acceso de la API
        /// Generar en: Panel de Cloud API ? Access Token
        /// Tipos:
        /// - Temporal (24h): Para pruebas
        /// - Permanente: Para producción (recomendado)
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Versión de la API de WhatsApp
        /// Actual: v18.0 (enero 2024)
        /// Ver: https://developers.facebook.com/docs/graph-api/changelog
        /// </summary>
        public string ApiVersion { get; set; } = "v18.0";

        /// <summary>
        /// URL base de la API
        /// </summary>
        public string BaseUrl => $"https://graph.facebook.com/{ApiVersion}";

        /// <summary>
        /// Token para verificar webhook (opcional)
        /// Debe ser una cadena aleatoria y segura
        /// </summary>
        public string? WebhookVerifyToken { get; set; }

        /// <summary>
        /// Validar si la configuración está completa
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(PhoneNumberId) &&
                   !string.IsNullOrWhiteSpace(AccessToken);
        }

        /// <summary>
        /// URL completa para enviar mensajes
        /// </summary>
        public string GetMessagesUrl()
        {
            return $"{BaseUrl}/{PhoneNumberId}/messages";
        }
    }
}
