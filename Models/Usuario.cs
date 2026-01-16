namespace App_CrediVnzl.Models
{
    public class Usuario
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public TipoUsuario Tipo { get; set; }
        
        // Propiedades para clientes
        public int? ClienteId { get; set; }
        public string? Dni { get; set; }
    }

    public enum TipoUsuario
    {
        Administrador,
        Cliente
    }
}
