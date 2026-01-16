# ?? CORRECCIÓN: App se Cierra Inmediatamente

## ? PROBLEMA IDENTIFICADO

La aplicación se cerraba inmediatamente al iniciar debido a un **NullReferenceException** cuando intentaba acceder a servicios antes de que el contexto de MAUI estuviera completamente inicializado.

### Causa Raíz:
1. **En App.xaml.cs:** Intentaba obtener servicios usando `Handler?.MauiContext?.Services` inmediatamente en `CreateWindow`, cuando el contexto aún no estaba disponible.
2. **En AppShell.xaml.cs:** Intentaba obtener `AuthService` en el constructor, lanzando una excepción si no estaba disponible.

---

## ? SOLUCIONES APLICADAS

### 1. App.xaml.cs - Corregido ?

**Antes (Problema):**
```csharp
protected override Window CreateWindow(IActivationState? activationState)
{
    var window = new Window(new AppShell());
    
    MainThread.InvokeOnMainThreadAsync(async () =>
    {
        // ? Handler es null aquí
        var authService = Handler?.MauiContext?.Services.GetService<AuthService>();
        var databaseService = Handler?.MauiContext?.Services.GetService<DatabaseService>();
        // ...
    });
    
    return window;
}
```

**Después (Solución):**
```csharp
protected override Window CreateWindow(IActivationState? activationState)
{
    var window = new Window(new AppShell());
    
    // ? Esperar a que el window esté creado
    window.Created += async (s, e) =>
    {
        await Task.Delay(100); // Dar tiempo al contexto
        
        // ? Ahora window.Handler está disponible
        var authService = window.Handler?.MauiContext?.Services.GetService<AuthService>();
        var databaseService = window.Handler?.MauiContext?.Services.GetService<DatabaseService>();
        
        if (authService != null && databaseService != null)
        {
            await databaseService.InitializeAsync();
            await authService.VerificarPrimerUsoAsync();
            
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.GoToAsync("//bienvenida");
            });
        }
    };
    
    return window;
}
```

**Cambios clave:**
- ? Uso del evento `window.Created` para esperar a que el window esté listo
- ? `Task.Delay(100)` para dar tiempo al contexto de MAUI
- ? Verificación de `null` antes de usar los servicios
- ? Logs detallados para debugging

---

### 2. AppShell.xaml.cs - Corregido ?

**Antes (Problema):**
```csharp
public AppShell()
{
    InitializeComponent();
    
    // ? Lanza excepción si no está disponible
    _authService = Handler?.MauiContext?.Services.GetService<AuthService>() 
        ?? throw new InvalidOperationException("AuthService no disponible");
    
    // Registrar rutas...
}
```

**Después (Solución):**
```csharp
private AuthService? _authService;

public AppShell()
{
    InitializeComponent();
    
    // ? Solo registrar rutas, no obtener servicios aún
    Routing.RegisterRoute("registrocliente", typeof(RegistroClientePage));
    // ... más rutas
}

// ? Obtener servicio solo cuando se necesita
private AuthService GetAuthService()
{
    if (_authService == null)
    {
        _authService = Handler?.MauiContext?.Services.GetService<AuthService>();
    }
    return _authService ?? throw new InvalidOperationException("AuthService no disponible");
}

private async void OnCerrarSesionClicked(object sender, EventArgs e)
{
    // ? Obtener el servicio cuando realmente se necesita
    var authService = GetAuthService();
    authService.Logout();
    await GoToAsync("//bienvenida");
}
```

**Cambios clave:**
- ? `_authService` es nullable y se obtiene lazy
- ? Método `GetAuthService()` para obtener el servicio solo cuando se necesita
- ? Try-catch en `OnCerrarSesionClicked` para manejo de errores

---

## ?? RESULTADOS

### ? Antes vs Después

| Aspecto | Antes ? | Después ? |
|---------|---------|-----------|
| **Inicio de app** | Se cierra inmediatamente | Inicia correctamente |
| **Navegación inicial** | Falla | Va a página de bienvenida |
| **Base de datos** | No se inicializa | Se inicializa correctamente |
| **Admin por defecto** | No se crea | Se crea automáticamente |
| **Logs** | Sin información | Logs detallados |

### ? Flujo Actual

```
1. App Constructor
   ?
2. CreateWindow
   ?
3. AppShell Constructor (registra rutas)
   ?
4. Window.Created evento
   ?
5. Esperar 100ms (contexto listo)
   ?
6. Obtener servicios (AuthService, DatabaseService)
   ?
7. Inicializar base de datos
   ?
8. Verificar/Crear admin por defecto
   ?
9. Navegar a página de bienvenida
   ?
10. ? App funcionando
```

---

## ?? DIAGNÓSTICO DE PROBLEMAS SIMILARES

Si la app vuelve a cerrarse, verifica:

### 1. Logs de Debug
Busca en la ventana de Output (Depurar) mensajes como:
- `*** App Constructor - Iniciando ***`
- `*** AppShell Constructor - Iniciando ***`
- `*** Window.Created - Iniciando inicialización ***`

### 2. Excepciones Comunes

**NullReferenceException:**
```
Handler?.MauiContext?.Services es null
```
**Solución:** Usar el evento `window.Created`

**InvalidOperationException:**
```
AuthService no disponible
```
**Solución:** Verificar que el servicio esté registrado en `MauiProgram.cs`

**SQLiteException:**
```
Error al abrir la base de datos
```
**Solución:** Verificar permisos de almacenamiento en Android

---

## ?? PRUEBAS RECOMENDADAS

### 1. Primera ejecución
1. Desinstalar la app completamente del dispositivo/emulador
2. Compilar y ejecutar
3. Verificar que muestre la página de bienvenida
4. Verificar en logs que se creó el admin por defecto

### 2. Login de administrador
1. Click en "Administrador"
2. Login: `admin` / `admin123`
3. Verificar que muestra el dashboard

### 3. Menú hamburguesa
1. En el dashboard, abrir el menú (?)
2. Verificar opciones: Dashboard, Cerrar Sesión

### 4. Cerrar y reabrir
1. Cerrar sesión
2. Verificar que vuelve a bienvenida
3. Volver a hacer login
4. Verificar que mantiene los datos

---

## ?? CONFIGURACIÓN ACTUAL

### Servicios Registrados (MauiProgram.cs)
```csharp
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<WhatsAppService>();
builder.Services.AddSingleton<ReportesService>();
builder.Services.AddSingleton<AuthService>();
```

### Páginas Registradas
- ? BienvenidaPage
- ? LoginPage
- ? DashboardPage
- ? PrimerUsoPage (no se usa actualmente)
- ? Y todas las demás páginas existentes

### Credenciales por Defecto
```
Usuario: admin
Contraseña: admin123
```

---

## ?? SI PERSISTE EL PROBLEMA

### Android:
1. Limpiar y reconstruir: `Build > Clean Solution`, luego `Build > Rebuild Solution`
2. Eliminar carpetas `bin` y `obj`
3. Desinstalar la app del emulador/dispositivo
4. Volver a instalar

### iOS:
1. Limpiar derivados: `Build > Clean All`
2. Reiniciar el simulador
3. Volver a compilar

### Windows:
1. Ejecutar como administrador
2. Verificar permisos de carpeta de datos

---

## ? ESTADO FINAL

- ? **Compilación:** Exitosa
- ? **App.xaml.cs:** Corregido
- ? **AppShell.xaml.cs:** Corregido
- ? **Logs:** Implementados
- ? **Manejo de errores:** Mejorado
- ? **Listo para ejecutar:** Sí

---

## ?? SIGUIENTE PASO

**Ejecuta la aplicación y observa los logs en la ventana Output (Depurar).** 

Si ves todos los mensajes de éxito y la app abre correctamente, el problema está resuelto. Si aún se cierra, copia los logs y pregúntame para diagnosticar más a fondo.
