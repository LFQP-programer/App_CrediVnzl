namespace App_CrediVnzl.Models
{
    public class PagoCalendario
    {
        public int Id { get; set; }
        public int PrestamoId { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
        public decimal MontoPago { get; set; }
        public bool EsPagado { get; set; }
        public string TipoPago { get; set; } = "Interes"; // "Interes" o "Capital"
        public int NumeroCuota { get; set; }
        public DateTime? FechaPagoReal { get; set; }

        // Propiedades calculadas para UI
        public string EstadoPago => EsPagado ? "Pagado" : 
                                    (FechaPago.Date < DateTime.Now.Date ? "Vencido" : 
                                    (FechaPago.Date == DateTime.Now.Date ? "Hoy" : "Pendiente"));
        
        public string ColorEstado => EsPagado ? "#4CAF50" : 
                                    (FechaPago.Date < DateTime.Now.Date ? "#F44336" : 
                                    (FechaPago.Date == DateTime.Now.Date ? "#FF9800" : "#9E9E9E"));

        public string IconoEstado => EsPagado ? "&#x2713;" : 
                                    (FechaPago.Date < DateTime.Now.Date ? "&#x26A0;" : 
                                    (FechaPago.Date == DateTime.Now.Date ? "&#x23F0;" : "&#x23F3;"));
    }
}
