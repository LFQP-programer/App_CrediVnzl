using SQLite;

namespace App_CrediVnzl.Models
{
    [Table("pagos")]
    public class Pago
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int ClienteId { get; set; }

        [Indexed]
        public int PrestamoId { get; set; }

        public decimal MontoPago { get; set; }

        public DateTime FechaProgramada { get; set; }

        public DateTime? FechaPagado { get; set; }

        [MaxLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Pagado, Vencido

        [MaxLength(500)]
        public string? Notas { get; set; }

        // Propiedades de navegacion (no mapeadas en DB)
        [Ignore]
        public string? ClienteNombre { get; set; }

        [Ignore]
        public string? ClienteTelefono { get; set; }
    }

    public enum EstadoPago
    {
        Todos,
        Pendiente,
        Pagado,
        Vencido
    }
}
