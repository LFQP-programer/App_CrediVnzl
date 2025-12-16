namespace App_CrediVnzl.Models
{
    public class PrestamoActivo
    {
        public string ClienteNombre { get; set; } = string.Empty;
        public decimal MontoInicial { get; set; }
        public decimal InteresSemanal { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal MontoPendiente { get; set; }
        public int PorcentajePagado { get; set; }
        public string ColorProgreso { get; set; } = "#4CAF50";
    }
}
