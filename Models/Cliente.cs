using SQLite;

namespace App_CrediVnzl.Models
{
    [Table("clientes")]
    public class Cliente
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(200)]
        public string NombreCompleto { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Cedula { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Direccion { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public int PrestamosActivos { get; set; } = 0;

        public decimal DeudaPendiente { get; set; } = 0;
    }
}
