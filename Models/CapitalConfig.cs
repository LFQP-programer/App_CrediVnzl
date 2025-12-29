using SQLite;

namespace App_CrediVnzl.Models
{
    [Table("CapitalConfig")]
    public class CapitalConfig
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public decimal CapitalInicial { get; set; }

        public decimal CapitalDisponible { get; set; }

        public decimal GananciaTotal { get; set; }

        public DateTime FechaActualizacion { get; set; }
    }
}
