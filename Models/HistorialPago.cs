using SQLite;

namespace App_CrediVnzl.Models
{
    [Table("historial_pagos")]
    public class HistorialPago
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int PrestamoId { get; set; }

        [Indexed]
        public int ClienteId { get; set; }

        public decimal MontoTotal { get; set; }

        public decimal MontoInteres { get; set; }

        public decimal MontoCapital { get; set; }

        public decimal CapitalPendienteAntes { get; set; }

        public decimal CapitalPendienteDespues { get; set; }

        public decimal InteresAcumuladoAntes { get; set; }

        public decimal InteresAcumuladoDespues { get; set; }

        public DateTime FechaPago { get; set; }

        [MaxLength(500)]
        public string? Nota { get; set; }

        // Propiedades de navegacion
        [Ignore]
        public string? ClienteNombre { get; set; }

        [Ignore]
        public string? MetodoPago { get; set; }
    }
}
