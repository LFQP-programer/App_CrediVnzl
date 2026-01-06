using SQLite;

namespace App_CrediVnzl.Models
{
    [Table("clientes")]
    public class Cliente
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        // Tipo de documento: "DNI" o "Carnet"
        [MaxLength(10)]
        public string TipoDocumento { get; set; } = "DNI";

        [MaxLength(20), Indexed]
        public string NumeroDocumento { get; set; } = string.Empty;

        // Ruta de la imagen del recibo de luz o agua
        [MaxLength(500)]
        public string? RutaImagenRecibo { get; set; }

        // ID del cliente aval (puede ser null)
        public int? AvalClienteId { get; set; }

        // Datos del aval si no es un cliente existente
        [MaxLength(200)]
        public string? AvalNombres { get; set; }

        [MaxLength(200)]
        public string? AvalApellidos { get; set; }

        [MaxLength(20)]
        public string? AvalTelefono { get; set; }

        [MaxLength(20)]
        public string? AvalNumeroDocumento { get; set; }

        [MaxLength(1000)]
        public string? Observaciones { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public int PrestamosActivos { get; set; } = 0;

        public decimal DeudaPendiente { get; set; } = 0;

        // Propiedades calculadas (no mapeadas en DB)
        [Ignore]
        public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();

        [Ignore]
        public string DocumentoCompleto => $"{TipoDocumento}: {NumeroDocumento}";

        [Ignore]
        public string AvalNombreCompleto => 
            !string.IsNullOrEmpty(AvalNombres) && !string.IsNullOrEmpty(AvalApellidos) 
                ? $"{AvalNombres} {AvalApellidos}".Trim() 
                : "";

        [Ignore]
        public bool TieneAval => AvalClienteId.HasValue || !string.IsNullOrEmpty(AvalNombres);

        [Ignore]
        public bool TieneImagenRecibo => !string.IsNullOrEmpty(RutaImagenRecibo);
    }

    public enum TipoDocumentoEnum
    {
        DNI,
        Carnet
    }
}
