using SQLite;

namespace App_CrediVnzl.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public TipoUsuario Tipo { get; set; }
        
        // Propiedades para clientes
        public int? ClienteId { get; set; }
        public string? Dni { get; set; }
        
        // Propiedades adicionales
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool Activo { get; set; } = true;
        public EstadosUsuario Estado { get; set; } = EstadosUsuario.Activo;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }

    public enum TipoUsuario
    {
        Administrador,
        Cliente
    }
    
    public enum EstadosUsuario
    {
        Pendiente,
        Activo,
        Rechazado,
        Inactivo
    }
}
