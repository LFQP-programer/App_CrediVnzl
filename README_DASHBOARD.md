# Dashboard de Prestafácil

## Estructura del Proyecto

### Modelos Creados

1. **DashboardCard.cs** - Modelo para las tarjetas del dashboard
   - Propiedades: Title, Value, Icon, BackgroundColor, IconColor

2. **PrestamoActivo.cs** - Modelo para los préstamos activos
   - Propiedades: ClienteNombre, MontoInicial, InteresSemanal, MontoPagado, MontoPendiente, PorcentajePagado, ColorProgreso

3. **MenuCard.cs** - Modelo para las tarjetas del menú
   - Propiedades: Title, Subtitle, Icon, BackgroundColor, Route

### ViewModels

**DashboardViewModel.cs** - ViewModel principal del dashboard
- Propiedades observables:
  - TotalClientes
  - PrestamosActivos
  - CapitalEnCalle
  - InteresesAcumulados
  - DashboardCards (ObservableCollection)
  - MenuCards (ObservableCollection)
  - PrestamosActivosList (ObservableCollection)

### Páginas

**DashboardPage.xaml** - Página principal del dashboard con:
- Header con título y botón de menú
- 4 tarjetas de estadísticas (Clientes, Activos, Capital en la Calle, Intereses Acumulados)
- 4 tarjetas de menú (Clientes, Calendario, Mensajes, Reportes)
- Lista de préstamos activos con:
  - Nombre del cliente
  - Porcentaje pagado con badge
  - Monto inicial
  - Interés semanal
  - Barra de progreso
  - Montos pagado y pendiente

### Convertidores

**PercentageToProgressConverter.cs** - Convierte porcentaje entero a valor de progreso (0.0 - 1.0)

## Conexión con Base de Datos

Para conectar con tu base de datos, debes modificar los siguientes métodos en `DashboardViewModel.cs`:

### LoadDashboardData()
```csharp
private async void LoadDashboardData()
{
    // Reemplazar con llamadas a tu base de datos
    TotalClientes = await _database.GetTotalClientesAsync();
    PrestamosActivos = await _database.GetPrestamosActivosCountAsync();
    CapitalEnCalle = await _database.GetCapitalEnCalleAsync();
    InteresesAcumulados = await _database.GetInteresesAcumuladosAsync();
}
```

### LoadPrestamosActivos()
```csharp
private async void LoadPrestamosActivos()
{
    // Reemplazar con llamada a tu base de datos
    var prestamos = await _database.GetPrestamosActivosAsync();
    PrestamosActivosList = new ObservableCollection<PrestamoActivo>(prestamos);
}
```

## Colores Utilizados

- **Azul** (#2196F3) - Clientes
- **Verde** (#4CAF50) - Activos/Estadísticas positivas
- **Morado** (#9C27B0) - Capital en la Calle/Mensajes
- **Naranja** (#FF5722) - Intereses/Reportes/Pendientes

## Próximos Pasos

1. Implementar la capa de datos (Database Service)
2. Agregar navegación a las páginas de Clientes, Calendario, Mensajes y Reportes
3. Implementar funcionalidad de actualización en tiempo real
4. Agregar filtros y búsqueda en la lista de préstamos activos
5. Implementar notificaciones push para recordatorios de pagos
