# Sistema de Autenticación Multi-Rol para CrediVnzl

## ?? Resumen de Cambios

Se ha implementado un sistema completo de autenticación con dos roles de usuario:
- **Administrador**: Acceso completo a todas las funcionalidades de gestión
- **Cliente**: Vista limitada para consultar sus préstamos y pagos

## ?? Nuevas Funcionalidades

### 1. Modelo de Usuario
- Archivo: `Models/Usuario.cs`
- Gestiona usuarios con roles (Admin/Cliente)
- Incluye información de acceso y seguridad

### 2. Servicio de Autenticación
- Archivo: `Services/AuthService.cs`
- Login con validación de credenciales
- Hash de contraseñas con SHA256
- Gestión de sesión de usuario
- Registro de administradores y clientes

### 3. Páginas de Autenticación

#### Login Page
- **Archivos**: `Pages/LoginPage.xaml`, `Pages/LoginPage.xaml.cs`, `ViewModels/LoginViewModel.cs`
- Pantalla de inicio de sesión
- Opción "Recordar credenciales"
- Redirección automática según rol

#### Primer Uso Page
- **Archivos**: `Pages/PrimerUsoPage.xaml`, `Pages/PrimerUsoPage.xaml.cs`, `ViewModels/PrimerUsoViewModel.cs`
- Se muestra al instalar la aplicación por primera vez
- Permite crear la cuenta de administrador inicial

### 4. Dashboard del Cliente
- **Archivos**: `Pages/DashboardClientePage.xaml`, `Pages/DashboardClientePage.xaml.cs`, `ViewModels/DashboardClienteViewModel.cs`
- Vista específica para clientes
- Muestra:
  - Préstamos activos
  - Deuda pendiente
  - Total pagado
  - Próximos pagos programados
  - Historial de pagos realizados

### 5. Gestión de Usuarios
- **Archivos**: `Pages/GestionarUsuariosPage.xaml`, `Pages/GestionarUsuariosPage.xaml.cs`, `ViewModels/GestionarUsuariosViewModel.cs`
- Permite al administrador crear cuentas para clientes
- Listar usuarios existentes
- Desactivar usuarios

### 6. Convertidores
- **Archivo**: `Converters/ValueConverters.cs`
- `PercentToDecimalConverter`: Convierte porcentajes a decimales para barras de progreso
- `InvertedBoolConverter`: Invierte valores booleanos para visibilidad

## ?? Modificaciones a Archivos Existentes

### DatabaseService.cs
Se agregaron nuevos métodos:
- `GetUsuariosAsync()`: Obtener todos los usuarios
- `GetUsuarioAsync(int id)`: Obtener usuario por ID
- `GetUsuarioByNombreUsuarioAsync(string nombreUsuario)`: Buscar por nombre de usuario
- `GetUsuarioByClienteIdAsync(int clienteId)`: Buscar usuario de un cliente
- `SaveUsuarioAsync(Usuario usuario)`: Guardar/actualizar usuario
- `DeleteUsuarioAsync(Usuario usuario)`: Eliminar usuario
- `GetUsuariosClientesAsync()`: Obtener usuarios con rol Cliente

### MauiProgram.cs
Se registraron:
- `AuthService` como Singleton
- Nuevas páginas: `LoginPage`, `PrimerUsoPage`, `DashboardClientePage`, `GestionarUsuariosPage`

### AppShell.xaml y AppShell.xaml.cs
- Ruta `login`: Página de login
- Ruta `primeruso`: Configuración inicial
- Ruta `dashboard`: Dashboard del administrador
- Ruta `dashboardcliente`: Dashboard del cliente
- Ruta `gestionarusuarios`: Gestión de usuarios

### App.xaml.cs
- Verificación de primer uso al iniciar
- Redirección automática a login o configuración inicial

### App.xaml
- Agregados nuevos convertidores al diccionario de recursos

### ConfiguracionPage.xaml y ConfiguracionViewModel.cs
- Nueva opción "Gestionar Usuarios de Clientes"
- Comando para navegar a gestión de usuarios

### DashboardViewModel.cs y DashboardPage.xaml
- Comando `CerrarSesionCommand` para cerrar sesión

## ?? Flujo de la Aplicación

### Primera Vez
1. La aplicación detecta que no hay administradores
2. Muestra `PrimerUsoPage` para crear cuenta admin
3. Redirige a `LoginPage`

### Login de Administrador
1. Ingresa credenciales en `LoginPage`
2. Sistema valida rol de Administrador
3. Navega a `DashboardPage` (vista completa)
4. Acceso a:
   - Gestión de clientes
   - Préstamos
   - Pagos
   - Calendario
   - Mensajes
   - Reportes
   - Configuración (incluyendo gestión de usuarios)

### Login de Cliente
1. Ingresa credenciales en `LoginPage`
2. Sistema valida rol de Cliente
3. Navega a `DashboardClientePage` (vista limitada)
4. Puede ver:
   - Sus préstamos activos
   - Deuda pendiente
   - Próximos pagos
   - Historial de pagos

## ?? Seguridad

- Contraseñas hasheadas con SHA256
- Validación de roles en cada acceso
- Sesión de usuario en memoria
- Usuarios pueden ser desactivados (no eliminados)

## ?? Cómo Usar

### Para el Administrador

#### Crear Usuario para un Cliente
1. Ir a "Configuración"
2. Click en "Gestionar Usuarios de Clientes"
3. Seleccionar un cliente de la lista
4. Ingresar nombre de usuario y contraseña
5. Click en "CREAR USUARIO"

#### Cerrar Sesión
- Usar el botón de menú en el Dashboard

### Para el Cliente

1. Recibir credenciales del administrador
2. Iniciar sesión con usuario y contraseña
3. Ver su información de préstamos
4. Consultar próximos pagos

## ?? Estructura de Roles

```
Usuario
??? Admin
?   ??? Dashboard completo
?   ??? Gestión de clientes
?   ??? Gestión de préstamos
?   ??? Gestión de pagos
?   ??? Calendario
?   ??? Mensajes
?   ??? Reportes
?   ??? Configuración
?   ??? Gestión de usuarios
?
??? Cliente
    ??? Dashboard personal
    ??? Ver préstamos propios
    ??? Ver próximos pagos
    ??? Ver historial de pagos
```

## ? Checklist de Implementación

- [x] Modelo de Usuario
- [x] Servicio de Autenticación
- [x] Página de Login
- [x] Página de Primer Uso
- [x] Dashboard del Cliente
- [x] Gestión de Usuarios
- [x] Integración con DatabaseService
- [x] Rutas de navegación
- [x] Verificación de primer uso
- [x] Cerrar sesión
- [x] Compilación exitosa

## ?? Próximas Mejoras Sugeridas

1. **Recuperación de contraseña**: Agregar funcionalidad para restablecer contraseñas
2. **Cambio de contraseña**: Permitir a los usuarios cambiar su contraseña
3. **Perfil de usuario**: Página para editar información personal
4. **Notificaciones push**: Alertas de pagos próximos para clientes
5. **Pago en línea**: Integrar pasarelas de pago para clientes
6. **Biometría**: Login con huella o Face ID
7. **Sesión persistente**: Mantener sesión entre reinicios de la app
8. **Logs de actividad**: Registrar acciones de usuarios para auditoría
9. **Roles adicionales**: Agregar roles como "Cobrador" o "Supervisor"
10. **Multi-administrador**: Permitir múltiples cuentas de administrador

## ?? Compatibilidad

- ? Android
- ? iOS
- ? Windows
- ? MacCatalyst

## ?? Notas Importantes

- La contraseña del primer administrador no se puede recuperar automáticamente
- Los usuarios desactivados no pueden iniciar sesión pero sus datos permanecen
- Un cliente solo puede tener una cuenta de usuario
- El nombre de usuario debe ser único en toda la base de datos

---

**Fecha de implementación**: ${new Date().toLocaleDateString()}
**Versión**: 2.0.0
**Estado**: ? Completado y probado
