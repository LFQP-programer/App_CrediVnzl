namespace App_CrediVnzl.Models
{
    /// <summary>
    /// Configuraci�n para WhatsApp Business API (Meta Cloud API)
    /// </summary>
    public class WhatsAppConfig
    {
        /// <summary>
        /// ID de la cuenta de WhatsApp Business
        /// Obtener en: https://business.facebook.com/latest/whatsapp_manager
        /// </summary>
        public string BusinessAccountId { get; set; } = string.Empty;

        /// <summary>
        /// ID del n�mero de tel�fono de WhatsApp
        /// Obtener en: Panel de Cloud API ? Phone Numbers
        /// </summary>
        public string PhoneNumberId { get; set; } = string.Empty;

        /// <summary>
        /// Token de acceso de la API
        /// Generar en: Panel de Cloud API ? Access Token
        /// Tipos:
        /// - Temporal (24h): Para pruebas
        /// - Permanente: Para producci�n (recomendado)
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Versi�n de la API de WhatsApp
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
        /// Validar si la configuraci�n est� completa
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
