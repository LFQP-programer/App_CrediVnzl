namespace App_CrediVnzl.Models
{
    /// <summary>
    /// Modelo para reporte financiero general
    /// </summary>
    public class ReporteFinanciero
    {
        public decimal CapitalTotal { get; set; }
        public decimal CapitalEnCalle { get; set; }
        public decimal CapitalDisponible { get; set; }
        public decimal GananciaTotal { get; set; }
        public decimal InteresesPendientes { get; set; }
        public decimal InteresesCobrados { get; set; }
        public decimal ROI { get; set; } // Return on Investment
        public decimal TasaRecuperacion { get; set; }
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
        
        // Comparativas con periodo anterior
        public decimal CambioGanancias { get; set; }
        public decimal CambioCapitalEnCalle { get; set; }
        public decimal PorcentajeCambioGanancias { get; set; }
    }

    /// <summary>
    /// Modelo para reporte de prestamos
    /// </summary>
    public class ReportePrestamos
    {
        public int TotalPrestamos { get; set; }
        public int PrestamosActivos { get; set; }
        public int PrestamosCompletados { get; set; }
        public int PrestamosVencidos { get; set; }
        public decimal MontoPromedio { get; set; }
        public int DuracionPromedioDias { get; set; }
        public decimal TasaMorosidad { get; set; }
        public decimal MontoTotalPrestado { get; set; }
        public decimal MontoTotalRecuperado { get; set; }
        
        // Distribucion por estado
        public Dictionary<string, int> PrestamosPorEstado { get; set; } = new();
    }

    /// <summary>
    /// Modelo para reporte de clientes
    /// </summary>
    public class ReporteClientes
    {
        public int TotalClientes { get; set; }
        public int ClientesActivos { get; set; }
        public int ClientesInactivos { get; set; }
        public int ClientesNuevosEsteMes { get; set; }
        public decimal PromedioPrestamosPorCliente { get; set; }
        public decimal DeudaPromedioCliente { get; set; }
        
        public List<ClienteTop> TopClientes { get; set; } = new();
        public List<ClienteRiesgo> ClientesEnRiesgo { get; set; } = new();
    }

    /// <summary>
    /// Cliente destacado
    /// </summary>
    public class ClienteTop
    {
        public int ClienteId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public int CantidadPrestamos { get; set; }
        public decimal MontoTotalPrestado { get; set; }
        public decimal TasaCumplimiento { get; set; }
        public string Clasificacion { get; set; } = "Regular"; // VIP, Regular, Nuevo
        public string ColorBadge { get; set; } = "#4CAF50";
    }

    /// <summary>
    /// Cliente con riesgo de morosidad
    /// </summary>
    public class ClienteRiesgo
    {
        public int ClienteId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public int PagosVencidos { get; set; }
        public decimal MontoVencido { get; set; }
        public int DiasAtraso { get; set; }
        public string NivelRiesgo { get; set; } = "Medio"; // Alto, Medio, Bajo
        public string ColorAlerta { get; set; } = "#FF9800";
    }

    /// <summary>
    /// Modelo para flujo de caja
    /// </summary>
    public class ReporteFlujoCaja
    {
        public decimal TotalPrestado { get; set; } // Salidas
        public decimal TotalCobrado { get; set; } // Entradas
        public decimal InteresesGenerados { get; set; }
        public decimal FlujoNeto { get; set; }
        public decimal ProyeccionProximos30Dias { get; set; }
        
        public List<FlujoDiario> FlujoPorDia { get; set; } = new();
    }

    public class FlujoDiario
    {
        public DateTime Fecha { get; set; }
        public decimal Entradas { get; set; }
        public decimal Salidas { get; set; }
        public decimal Neto { get; set; }
    }

    /// <summary>
    /// Modelo para reporte de pagos
    /// </summary>
    public class ReportePagos
    {
        public int TotalPagos { get; set; }
        public int PagosPuntuales { get; set; }
        public int PagosAtrasados { get; set; }
        public decimal TasaCumplimiento { get; set; }
        public int PromedioDiasAtraso { get; set; }
        public decimal MontoTotalPagos { get; set; }
        
        // Analisis por dia de la semana
        public Dictionary<DayOfWeek, int> PagosPorDiaSemana { get; set; } = new();
        public DayOfWeek MejorDiaCobro { get; set; }
    }

    /// <summary>
    /// Dato para graficos
    /// </summary>
    public class DatoGrafico
    {
        public string Etiqueta { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime Fecha { get; set; }
        public string Color { get; set; } = "#2196F3";
        public string Categoria { get; set; } = string.Empty;
    }

    /// <summary>
    /// Enumeracion de periodos de reporte
    /// </summary>
    public enum PeriodoReporte
    {
        Hoy,
        Semana,
        Mes,
        Trimestre,
        Anio,
        Personalizado
    }

    /// <summary>
    /// Resumen completo de reportes
    /// </summary>
    public class ResumenReportes
    {
        public ReporteFinanciero Financiero { get; set; } = new();
        public ReportePrestamos Prestamos { get; set; } = new();
        public ReporteClientes Clientes { get; set; } = new();
        public ReporteFlujoCaja FlujoCaja { get; set; } = new();
        public ReportePagos Pagos { get; set; } = new();
        
        public PeriodoReporte Periodo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
