using System.Text.Json.Serialization;

namespace App_CrediVnzl.Models
{
    /// <summary>
    /// Request para enviar mensaje de texto simple
    /// </summary>
    public class WhatsAppTextMessageRequest
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; } = "whatsapp";

        [JsonPropertyName("recipient_type")]
        public string RecipientType { get; set; } = "individual";

        [JsonPropertyName("to")]
        public string To { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";

        [JsonPropertyName("text")]
        public WhatsAppTextContent? Text { get; set; }
    }

    public class WhatsAppTextContent
    {
        [JsonPropertyName("preview_url")]
        public bool PreviewUrl { get; set; } = false;

        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request para enviar mensaje con plantilla
    /// </summary>
    public class WhatsAppTemplateMessageRequest
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; } = "whatsapp";

        [JsonPropertyName("to")]
        public string To { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = "template";

        [JsonPropertyName("template")]
        public WhatsAppTemplate? Template { get; set; }
    }

    public class WhatsAppTemplate
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("language")]
        public WhatsAppLanguage? Language { get; set; }

        [JsonPropertyName("components")]
        public List<WhatsAppComponent>? Components { get; set; }
    }

    public class WhatsAppLanguage
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = "es"; // español
    }

    public class WhatsAppComponent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "body";

        [JsonPropertyName("parameters")]
        public List<WhatsAppParameter>? Parameters { get; set; }
    }

    public class WhatsAppParameter
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// Respuesta de la API
    /// </summary>
    public class WhatsAppMessageResponse
    {
        [JsonPropertyName("messaging_product")]
        public string? MessagingProduct { get; set; }

        [JsonPropertyName("contacts")]
        public List<WhatsAppContact>? Contacts { get; set; }

        [JsonPropertyName("messages")]
        public List<WhatsAppMessageInfo>? Messages { get; set; }

        [JsonPropertyName("error")]
        public WhatsAppError? Error { get; set; }

        public bool IsSuccess => Error == null && Messages != null && Messages.Any();
    }

    public class WhatsAppContact
    {
        [JsonPropertyName("input")]
        public string? Input { get; set; }

        [JsonPropertyName("wa_id")]
        public string? WhatsAppId { get; set; }
    }

    public class WhatsAppMessageInfo
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("message_status")]
        public string? MessageStatus { get; set; }
    }

    public class WhatsAppError
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("error_data")]
        public object? ErrorData { get; set; }

        [JsonPropertyName("fbtrace_id")]
        public string? FbTraceId { get; set; }
    }

    /// <summary>
    /// Resultado del envío de mensaje
    /// </summary>
    public class WhatsAppSendResult
    {
        public bool Success { get; set; }
        public string? MessageId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
