using SQLite;

namespace App_CrediVnzl.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(200), Indexed]
        public string NombreUsuario { get; set; } = string.Empty;

        [MaxLength(200)]
        public string NombreCompleto { get; set; } = string.Empty;

        [MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Rol { get; set; } = "Cliente"; // Admin o Cliente

        [Indexed]
        public int? ClienteId { get; set; } // Null para Admin, ID del cliente para rol Cliente

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Estado { get; set; } = "Activo"; // Pendiente, Activo, Rechazado, Inactivo

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? UltimoAcceso { get; set; }

        public DateTime? FechaSolicitud { get; set; }

        public DateTime? FechaAprobacion { get; set; }

        public bool Activo { get; set; } = true;

        // Propiedades de navegación
        [Ignore]
        public bool EsAdmin => Rol == "Admin";

        [Ignore]
        public bool EsCliente => Rol == "Cliente";

        [Ignore]
        public bool EstaPendiente => Estado == "Pendiente";

        [Ignore]
        public bool EstaActivo => Estado == "Activo";

        [Ignore]
        public bool EstaRechazado => Estado == "Rechazado";
    }

    public static class RolesUsuario
    {
        public const string Admin = "Admin";
        public const string Cliente = "Cliente";
    }

    public static class EstadosUsuario
    {
        public const string Pendiente = "Pendiente";
        public const string Activo = "Activo";
        public const string Rechazado = "Rechazado";
        public const string Inactivo = "Inactivo";
    }
}
