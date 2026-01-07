namespace App_CrediVnzl.Helpers
{
    /// <summary>
    /// Clase helper con emojis y símbolos seguros para usar en toda la aplicación
    /// </summary>
    public static class SafeEmojis
    {
        // ?? Usuarios y personas
        public static readonly string User = "\U0001F464";        // ??
        public static readonly string Users = "\U0001F465";       // ??
        public static readonly string Admin = "\U0001F451";       // ??
        
        // ?? Seguridad
        public static readonly string Lock = "\U0001F512";        // ??
        public static readonly string Unlock = "\U0001F513";      // ??
        public static readonly string Key = "\U0001F511";         // ??
        
        // ?? Comunicación
        public static readonly string Phone = "\U0001F4F1";       // ??
        public static readonly string Email = "\u2709";           // ?
        public static readonly string Message = "\U0001F4AC";     // ??
        public static readonly string WhatsApp = "\U0001F4F2";    // ??
        
        // ?? Finanzas
        public static readonly string Money = "\U0001F4B0";       // ??
        public static readonly string Dollar = "\U0001F4B5";      // ??
        public static readonly string Chart = "\U0001F4CA";       // ??
        public static readonly string TrendUp = "\U0001F4C8";     // ??
        public static readonly string TrendDown = "\U0001F4C9";   // ??
        
        // ?? Tiempo y calendario
        public static readonly string Calendar = "\U0001F4C5";    // ??
        public static readonly string Clock = "\u23F0";           // ?
        public static readonly string Timer = "\u23F1";           // ?
        
        // ? Estados
        public static readonly string Success = "\u2705";         // ?
        public static readonly string Error = "\u274C";           // ?
        public static readonly string Warning = "\u26A0";         // ?
        public static readonly string Info = "\u2139";            // ?
        public static readonly string Question = "\u2753";        // ?
        
        // ?? Documentos
        public static readonly string Document = "\U0001F4C4";    // ??
        public static readonly string File = "\U0001F4C1";        // ??
        public static readonly string Note = "\U0001F4DD";        // ??
        
        // ?? Acciones
        public static readonly string Search = "\U0001F50D";      // ??
        public static readonly string Settings = "\u2699";        // ?
        public static readonly string Menu = "\U0001F4CB";        // ??
        public static readonly string Home = "\U0001F3E0";        // ??
        public static readonly string Back = "\U0001F519";        // ??
        
        // ?? Navegación (símbolos geométricos más seguros)
        public static readonly string ArrowRight = "\u25B6";      // ?
        public static readonly string ArrowLeft = "\u25C0";       // ?
        public static readonly string ArrowUp = "\u25B2";         // ?
        public static readonly string ArrowDown = "\u25BC";       // ?
        
        // ? Formas geométricas
        public static readonly string Circle = "\u25CF";          // ?
        public static readonly string Square = "\u25A0";          // ?
        public static readonly string Diamond = "\u25C6";         // ?
        public static readonly string Star = "\u2605";            // ?
        
        // ?? Utilidades
        public static readonly string Refresh = "\U0001F504";     // ??
        public static readonly string Plus = "\u2795";            // ?
        public static readonly string Minus = "\u2796";           // ?
        public static readonly string Check = "\u2713";           // ?
        public static readonly string Cross = "\u2717";           // ?
        
        /// <summary>
        /// Obtiene un emoji aleatorio para hacer la interfaz más dinámica
        /// </summary>
        public static string GetRandomHappyEmoji()
        {
            var happyEmojis = new[]
            {
                "\U0001F60A", // ??
                "\U0001F600", // ??
                "\U0001F603", // ??
                "\U0001F604", // ??
                "\U0001F601"  // ??
            };
            
            var random = new Random();
            return happyEmojis[random.Next(happyEmojis.Length)];
        }
        
        /// <summary>
        /// Combina texto con emoji de forma segura
        /// </summary>
        public static string WithEmoji(string emoji, string text)
        {
            return $"{emoji} {text}";
        }
    }
}