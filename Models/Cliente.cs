using SQLite;

namespace App_CrediVnzl.Models
{
    [Table("clientes")]
    public class Cliente
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(50)]
        public string TipoDocumento { get; set; } = "DNI"; // DNI o Carnet de extranjer�a

        [MaxLength(20), Unique]
        public string NumeroDocumento { get; set; } = string.Empty; // Funciona como ID �nico

        [MaxLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [MaxLength(20)]
        public string NumeroCelular { get; set; } = string.Empty;

        public string RutaImagenRecibo { get; set; } = string.Empty; // Ruta de la imagen del recibo

        // Datos del Aval
        public int? AvalClienteId { get; set; } // Si el aval es un cliente existente
        
        [MaxLength(100)]
        public string AvalNombres { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string AvalApellidos { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string AvalCelular { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string AvalDireccion { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Observaciones { get; set; } = string.Empty; // Comentarios del admin

        [MaxLength(50)]
        public string EstadoCliente { get; set; } = "Activo"; // Activo, En observaci�n, Moroso

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public int PrestamosActivos { get; set; } = 0;

        public decimal DeudaPendiente { get; set; } = 0;

        // Campos para acceso del cliente
        public bool TieneAccesoApp { get; set; } = false;
        
        [MaxLength(10)]
        public string? PasswordTemporal { get; set; }
        
        public bool RequiereCambioPassword { get; set; } = true;
        
        public DateTime? FechaGeneracionPassword { get; set; }

        // Propiedades calculadas para compatibilidad
        [Ignore]
        public string NombreCompleto => $"{Nombres} {Apellidos}";

        [Ignore]
        public string Cedula => NumeroDocumento; // Para compatibilidad con c�digo existente

        [Ignore]
        public string Telefono => NumeroCelular; // Para compatibilidad

        [Ignore]
        public string Direccion => AvalDireccion; // Para compatibilidad temporal
    }
}
