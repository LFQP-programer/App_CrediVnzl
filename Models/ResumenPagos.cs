namespace App_CrediVnzl.Models
{
    public class ResumenPagos
    {
        public int TotalMes { get; set; }
        public decimal MontoEsperado { get; set; }
        public int Pendientes { get; set; }
        public int Vencidos { get; set; }
        public int Pagados { get; set; }
    }
}
