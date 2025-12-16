# Implementación: Pantalla Detalle del Cliente

## Archivos Creados

### 1. **Models/Prestamo.cs**
Modelo de datos para préstamos con las siguientes características:
- Campos principales: MontoInicial, TasaInteresSemanal, DuracionSemanas, FechaInicio, Estado
- Campos calculados: CapitalPendiente, InteresAcumulado, TotalAdeudado, MontoPagado
- Propiedades computadas: SemanasTranscurridas, InteresSemanalActual, PorcentajePagado
- Estados: Activo, Completado, Cancelado

### 2. **Pages/DetalleClientePage.xaml**
Interfaz completa de detalle del cliente con:
- **Información del Cliente**: Nombre, teléfono, cédula, dirección, fecha de registro
- **Estadísticas**: Préstamos activos y completados en tarjetas separadas
- **Capital Pendiente Total**: Suma de todo el capital pendiente de todos los préstamos activos
- **Total Adeudado Hoy**: Capital + intereses acumulados de todos los préstamos
- **Botón "Nuevo Préstamo"**: Visible pero sin funcionalidad (placeholder)
- **Lista de Préstamos Activos**: Cada tarjeta muestra:
  - Monto inicial del préstamo y estado (badge verde "activo")
  - Fecha de inicio y semanas transcurridas
  - Cuenta corriente con:
    - Capital pendiente
    - Interés semanal actual
    - Interés acumulado
    - Total adeudado (resaltado en amarillo)
  - Barra de progreso del pago con porcentaje
  - Botón "Registrar Pago" (funcionalidad pendiente)

### 3. **Pages/DetalleClientePage.xaml.cs**
Code-behind que:
- Inyecta el DatabaseService
- Inicializa el ViewModel
- Carga los datos cuando la página aparece

### 4. **ViewModels/DetalleClienteViewModel.cs**
ViewModel con:
- Propiedad `ClienteId` con atributo `[QueryProperty]` para recibir el ID del cliente desde la navegación
- Propiedades observables:
  - `Cliente`: Información completa del cliente
  - `PrestamosActivos`: Colección de préstamos activos
  - `PrestamosCompletados`: Contador de préstamos completados
  - `CapitalPendienteTotal`: Suma del capital pendiente
  - `TotalAdeudadoHoy`: Total de deuda (capital + intereses)
- Comandos:
  - `NuevoPrestamoCommand`: Muestra alerta "Próximamente"
  - `RegistrarPagoCommand`: Muestra alerta "Próximamente"
- Método `LoadDataAsync()`: Carga todos los datos desde la base de datos

## Archivos Modificados

### 1. **Services/DatabaseService.cs**
Se agregaron métodos para gestión de préstamos:
- `GetPrestamosAsync()`: Obtener todos los préstamos
- `GetPrestamosByClienteAsync(clienteId)`: Obtener préstamos de un cliente específico
- `GetPrestamoAsync(id)`: Obtener un préstamo por ID
- `SavePrestamoAsync(prestamo)`: Guardar o actualizar un préstamo
- `DeletePrestamoAsync(prestamo)`: Eliminar un préstamo
- `GetPrestamosActivosAsync()`: Obtener solo préstamos activos
- Se agregó `CreateTableAsync<Prestamo>()` en InitializeAsync()

### 2. **ViewModels/ClientesViewModel.cs**
Se agregó:
- `EditClienteCommand`: Comando para navegar al detalle del cliente
- Método `NavigateToDetalleCliente(cliente)`: Navega usando el parámetro clienteId

### 3. **AppShell.xaml.cs**
Se registró la nueva ruta:
```csharp
Routing.RegisterRoute("detallecliente", typeof(DetalleClientePage));
```

### 4. **MauiProgram.cs**
Se registró la página en el contenedor de inyección de dependencias:
```csharp
builder.Services.AddTransient<DetalleClientePage>();
```

## Navegación Implementada

Desde **ClientesPage**, al hacer clic en una tarjeta de cliente:
```csharp
await Shell.Current.GoToAsync($"detallecliente?clienteId={cliente.Id}");
```

El parámetro `clienteId` se recibe automáticamente en el ViewModel gracias al atributo `[QueryProperty]`.

## Características Implementadas

? Visualización completa de información del cliente
? Estadísticas de préstamos activos y completados
? Cálculo automático de capital pendiente total
? Cálculo automático de total adeudado (capital + intereses)
? Lista de préstamos activos con detalle completo
? Cuenta corriente por préstamo
? Barra de progreso de pago
? Diseño responsive y profesional con Material Design
? Navegación desde lista de clientes
? Base de datos SQLite con tabla de préstamos

## Funcionalidades Pendientes (Placeholders)

? **Botón "Nuevo Préstamo"**: Muestra alerta "Próximamente"
? **Botón "Registrar Pago"**: Muestra alerta "Próximamente"

Estos botones ya están visibles y preparados para futuras implementaciones.

## Cómo Usar

1. Ir a la página de **Clientes** desde el Dashboard
2. Hacer clic en cualquier tarjeta de cliente
3. Se abrirá la pantalla de **Detalle del Cliente**
4. Se mostrará:
   - Toda la información del cliente
   - Estadísticas de préstamos
   - Lista de préstamos activos con detalles completos

## Base de Datos

Se creó la tabla `prestamos` con los siguientes campos:
- Id, ClienteId, MontoInicial, TasaInteresSemanal, DuracionSemanas
- FechaInicio, Estado, CapitalPendiente, InteresAcumulado
- TotalAdeudado, MontoPagado, Notas

## Diseño

El diseño sigue los mockups proporcionados:
- Header azul con título "Detalle del Cliente"
- Cards blancos con sombras
- Colores distintivos:
  - Verde (#4CAF50) para estado activo y progreso
  - Rojo (#FF5722) para deudas
  - Amarillo (#FFF9C4) para total adeudado
  - Azul (#2196F3) para botones principales
- Iconos emoji para mejor UX
- Espaciado y padding consistentes

## Estado del Proyecto

? Compilación exitosa
? Modelo de Prestamo creado
? Página de Detalle del Cliente funcional
? Navegación implementada
? Base de datos actualizada
? ViewModel completo con carga de datos
? Pendiente: Implementar "Nuevo Préstamo"
? Pendiente: Implementar "Registrar Pago"
