namespace App_CrediVnzl.Models
{
    public class DiaCalendario
    {
        public int Dia { get; set; }
        public DateTime Fecha { get; set; }
        public bool EsMesActual { get; set; }
        public bool TienePagos { get; set; }
        public int CantidadPagos { get; set; }
        public int PagosPendientes { get; set; }
        public int PagosVencidos { get; set; }
        public int PagosPagados { get; set; }
        public bool EsHoy { get; set; }
        public bool EstaSeleccionado { get; set; }

        public string ColorFondo => EsHoy ? "#E3F2FD" : 
                                   (EstaSeleccionado ? "#BBDEFB" : 
                                   (EsMesActual ? "White" : "#F5F5F5"));

        public string ColorTexto => EsMesActual ? "#212121" : "#9E9E9E";

        public string ColorBadge => PagosVencidos > 0 ? "#F44336" : 
                                   (PagosPendientes > 0 ? "#FF9800" : 
                                   (PagosPagados > 0 ? "#4CAF50" : "Transparent"));

        public string TextoBadge => TienePagos ? (PagosVencidos > 0 ? "!" : 
                                   (PagosPendientes > 0 ? PagosPendientes.ToString() : "?")) : "";
    }
}
