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
        public string Cedula => NumeroDocumento; // Para compatibilidad con código existente

        [Ignore]
        public string Telefono => NumeroCelular; // Para compatibilidad

        [Ignore]
        public string Direccion => AvalDireccion; // Para compatibilidad temporal

        // Propiedades para el diseño visual
        [Ignore]
        public string Iniciales
        {
            get
            {
                var inicialNombre = !string.IsNullOrEmpty(Nombres) ? Nombres[0].ToString().ToUpper() : "";
                var inicialApellido = !string.IsNullOrEmpty(Apellidos) ? Apellidos[0].ToString().ToUpper() : "";
                return $"{inicialNombre}{inicialApellido}";
            }
        }

        [Ignore]
        public string ColorAvatar
        {
            get
            {
                // Generar color basado en el nombre para consistencia
                var hash = NombreCompleto.GetHashCode();
                var colores = new[] { "#2196F3", "#4CAF50", "#FF9800", "#9C27B0", "#00BCD4", "#FF5722" };
                var index = Math.Abs(hash) % colores.Length;
                return colores[index];
            }
        }

        [Ignore]
        public string EstadoTexto
        {
            get
            {
                if (PrestamosActivos == 0)
                    return "Sin préstamos";
                    
                if (EstadoCliente == "Moroso")
                    return "Moroso";
                    
                if (EstadoCliente == "En observación" || EstadoCliente == "En observacion")
                    return "Atención";
                    
                return "Al día";
            }
        }

        [Ignore]
        public string EstadoColor
        {
            get
            {
                if (PrestamosActivos == 0)
                    return "#9E9E9E"; // Gris
                    
                if (EstadoCliente == "Moroso")
                    return "#F44336"; // Rojo
                    
                if (EstadoCliente == "En observación" || EstadoCliente == "En observacion")
                    return "#FF9800"; // Naranja
                    
                return "#4CAF50"; // Verde
            }
        }

        [Ignore]
        public string EstadoColorClaro
        {
            get
            {
                if (PrestamosActivos == 0)
                    return "#F5F5F5"; // Gris claro
                    
                if (EstadoCliente == "Moroso")
                    return "#FFEBEE"; // Rojo claro
                    
                if (EstadoCliente == "En observación" || EstadoCliente == "En observacion")
                    return "#FFF3E0"; // Naranja claro
                    
                return "#E8F5E9"; // Verde claro
            }
        }
    }
}
