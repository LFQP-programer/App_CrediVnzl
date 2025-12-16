# Sistema de Gestion de Clientes - Prestafacil

## Funcionalidades Implementadas

### 1. Base de Datos SQLite
- **DatabaseService.cs**: Servicio completo para gestionar la base de datos SQLite
- Tabla `clientes` con los siguientes campos:
  - Id (AutoIncrement)
  - NombreCompleto
  - Telefono (incluye codigo de pais para WhatsApp)
  - Cedula/DNI
  - Direccion
  - FechaRegistro
  - PrestamosActivos (contador)
  - DeudaPendiente (monto decimal)

### 2. Pagina de Clientes (ClientesPage)
**Ruta**: `clientes`

**Caracteristicas**:
- Header azul con titulo "Clientes" y contador de registrados
- Buscador funcional por:
  - Nombre completo
  - Telefono
  - Cedula/DNI
- Boton "+ Agregar Nuevo Cliente" prominente
- Lista de clientes con RefreshView (pull to refresh)
- EmptyView cuando no hay clientes registrados

**Cada tarjeta de cliente muestra**:
- Nombre completo (bold)
- Badge verde con numero de prestamos activos (si tiene)
- Icono y telefono
- Icono y cedula/DNI
- Deuda pendiente en rojo (si tiene)

### 3. Pagina Nuevo Cliente (NuevoClientePage)
**Ruta**: `nuevocliente`

**Caracteristicas**:
- Header informativo azul
- Card "Informacion del Cliente" con explicacion
- Formulario completo con iconos:
  - Nombre Completo (obligatorio)
  - Telefono / WhatsApp (obligatorio) con nota sobre codigo de pais
  - Cedula / DNI (obligatorio)
  - Direccion (Editor multilinea)
- Alerta de confidencialidad en amarillo
- Botones "Cancelar" y "Guardar Cliente"
- Validaciones de campos obligatorios

### 4. Dashboard Integrado
- Carga automatica de estadisticas desde la base de datos:
  - Total de clientes
  - Clientes con prestamos activos
- Tarjeta "Clientes" clickeable para navegar a la lista
- Actualizacion automatica al regresar de otras paginas

### 5. ViewModels

**ClientesViewModel**:
- Carga lista de clientes desde la base de datos
- Busqueda en tiempo real
- Comando RefreshCommand para actualizar
- Comando AddClienteCommand para navegacion

**NuevoClienteViewModel**:
- Manejo de formulario de cliente
- Validaciones de campos obligatorios
- Guardado en base de datos
- Navegacion de retorno

**DashboardViewModel** (actualizado):
- Carga datos desde DatabaseService
- Metodo LoadDashboardDataAsync() async
- Actualizacion dinamica de estadisticas

### 6. Convertidores
- **PercentageToProgressConverter**: Convierte porcentaje a progreso (0.0-1.0)
- **IsGreaterThanZeroConverter**: Valida si un numero es mayor que cero para mostrar/ocultar elementos

### 7. Navegacion
- Rutas registradas en AppShell.xaml.cs:
  - `clientes` -> ClientesPage
  - `nuevocliente` -> NuevoClientePage
- Navegacion desde Dashboard al hacer clic en tarjeta de Clientes

## Estructura de Archivos

```
App_CrediVnzl/
??? Models/
?   ??? Cliente.cs
?   ??? DashboardCard.cs
?   ??? MenuCard.cs
?   ??? PrestamoActivo.cs
??? Services/
?   ??? DatabaseService.cs
??? ViewModels/
?   ??? DashboardViewModel.cs
?   ??? ClientesViewModel.cs
?   ??? NuevoClienteViewModel.cs
??? Pages/
?   ??? DashboardPage.xaml / .cs
?   ??? ClientesPage.xaml / .cs
?   ??? NuevoClientePage.xaml / .cs
??? Converters/
?   ??? PercentageToProgressConverter.cs
?   ??? IsGreaterThanZeroConverter.cs
??? App_CrediVnzl.csproj (con paquetes SQLite)
```

## Paquetes NuGet Instalados

```xml
<PackageReference Include="sqlite-net-pcl" Version="1.9.172" />
<PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.1.10" />
```

## Como Usar

### 1. Agregar un Cliente
1. Desde el Dashboard, haz clic en la tarjeta "Clientes"
2. Haz clic en el boton "+ Agregar Nuevo Cliente"
3. Completa el formulario:
   - Nombre completo
   - Telefono (incluye codigo de pais, ej: +51987654321)
   - Cedula/DNI
   - Direccion
4. Haz clic en "Guardar Cliente"

### 2. Buscar Clientes
1. En la pagina de Clientes, usa el buscador
2. Escribe el nombre, telefono o cedula
3. Los resultados se filtran en tiempo real

### 3. Ver Dashboard Actualizado
1. El Dashboard muestra automaticamente:
   - Numero total de clientes
   - Clientes con prestamos activos
2. Las estadisticas se actualizan al regresar de otras paginas

## Ubicacion de la Base de Datos

La base de datos se guarda en:
```
FileSystem.AppDataDirectory/prestafacil.db3
```

En Android:
```
/data/data/com.companyname.app_credivnzl/files/prestafacil.db3
```

## Proximos Pasos

1. Implementar edicion de clientes (tap en tarjeta de cliente)
2. Implementar eliminacion de clientes (swipe o menu contextual)
3. Agregar funcionalidad de prestamos
4. Implementar calculadora de intereses
5. Agregar exportacion de datos
6. Implementar recordatorios por WhatsApp

## Notas Tecnicas

- La base de datos se inicializa automaticamente al abrir la app
- Todos los metodos de base de datos son asincronos
- Se usa inyeccion de dependencias para DatabaseService
- Las paginas se registran como Transient en MauiProgram.cs
- El DatabaseService se registra como Singleton

## Estado del Proyecto

? Compilacion exitosa
? Base de datos SQLite configurada
? CRUD de clientes (Create, Read)
? Busqueda funcional
? Navegacion implementada
? Dashboard integrado
?? Pendiente: Update y Delete de clientes
?? Pendiente: Modulo de prestamos
