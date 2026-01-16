using SQLite;

namespace App_CrediVnzl.Models
{
    [Table("prestamos")]
    public class Prestamo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int ClienteId { get; set; }

        public decimal MontoInicial { get; set; }

        public decimal TasaInteresSemanal { get; set; }

        public int DuracionSemanas { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaUltimoPago { get; set; }

        [MaxLength(20)]
        public string Estado { get; set; } = "Activo"; // Activo, Completado, Cancelado

        public decimal CapitalPendiente { get; set; }

        public decimal InteresAcumulado { get; set; }

        public decimal TotalAdeudado { get; set; }

        public decimal MontoPagado { get; set; }

        [MaxLength(500)]
        public string? Notas { get; set; }

        // Propiedades calculadas (no mapeadas en DB)
        [Ignore]
        public int SemanasTranscurridas 
        { 
            get 
            {
                var fechaReferencia = FechaUltimoPago ?? FechaInicio;
                var dias = (DateTime.Now - fechaReferencia).Days;
                return Math.Max(0, dias / 7);
            } 
        }

        [Ignore]
        public decimal InteresSemanalActual
        {
            get
            {
                return CapitalPendiente * (TasaInteresSemanal / 100);
            }
        }

        [Ignore]
        public int PorcentajePagado
        {
            get
            {
                var totalOriginal = MontoInicial + (MontoInicial * (TasaInteresSemanal / 100) * SemanasTranscurridas);
                if (totalOriginal == 0) return 0;
                return (int)((MontoPagado / totalOriginal) * 100);
            }
        }

        [Ignore]
        public bool Expandido { get; set; } = false;
    }
}
