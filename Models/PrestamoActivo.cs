namespace App_CrediVnzl.Models
{
    public class PrestamoActivo
    {
        public string ClienteNombre { get; set; } = string.Empty;
        public decimal MontoInicial { get; set; }
        public decimal InteresSemanal { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal MontoPendiente { get; set; }
        public int PorcentajePagado { get; set; }
        public string ColorProgreso { get; set; } = "#4CAF50";
        
        // Nuevas propiedades para el diseno mejorado
        public decimal CuotaSemanal { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string EstadoTexto { get; set; } = "Al dia";
        public string EstadoColor { get; set; } = "#4CAF50"; // Verde por defecto
        
        // Metodo helper para determinar el estado
        public void DeterminarEstado()
        {
            var diasVencido = (DateTime.Now - FechaVencimiento).Days;
            
            if (diasVencido > 14) // Mas de 2 semanas
            {
                EstadoTexto = "Vencido";
                EstadoColor = "#E4002B"; // Rojo
            }
            else if (diasVencido > 7) // Mas de 1 semana
            {
                EstadoTexto = "Atrasado";
                EstadoColor = "#FF9800"; // Naranja
            }
            else if (diasVencido > 0) // Paso la fecha pero menos de 1 semana
            {
                EstadoTexto = "Por vencer";
                EstadoColor = "#FFC107"; // Amarillo
            }
            else
            {
                EstadoTexto = "Al dia";
                EstadoColor = "#4CAF50"; // Verde
            }
        }
    }
}
