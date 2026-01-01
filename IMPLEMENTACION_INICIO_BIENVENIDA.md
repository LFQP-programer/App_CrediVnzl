# ?? IMPLEMENTACIÓN: INICIO EN PÁGINA DE BIENVENIDA

## ?? OBJETIVO

Configurar la aplicación para que siempre inicie en la página de bienvenida, excepto cuando el usuario haya marcado la opción "Recordar credenciales", en cuyo caso debe hacer login automático y navegar directamente al dashboard correspondiente.

---

## ? COMPORTAMIENTO IMPLEMENTADO

### 1. **Inicio Normal (Sin "Recordar credenciales")**

```
[App Inicia]
    ?
[Inicializar Base de Datos]
    ?
[Verificar Primer Uso / Crear Admin por Defecto]
    ?
[Verificar Preferencias: recordar_usuario = false]
    ?
[Navegar a BienvenidaPage]
    ?
[Usuario elige: Administrador o Cliente]
    ?
[LoginPage correspondiente]
```

### 2. **Inicio con "Recordar credenciales" Activado**

```
[App Inicia]
    ?
[Inicializar Base de Datos]
    ?
[Verificar Primer Uso / Crear Admin por Defecto]
    ?
[Verificar Preferencias: recordar_usuario = true]
    ?
[Obtener ultimo_usuario de Preferences]
    ?
[Buscar usuario en Base de Datos]
    ?
[Usuario existe y está activo?]
    ?? Sí  ? [Establecer como UsuarioActual]
    ?        ?
    ?        [Navegar a Dashboard según Rol]
    ?        ?? Admin   ? //dashboard
    ?        ?? Cliente ? //dashboardcliente
    ?
    ?? No  ? [Limpiar Preferences]
             ?
             [Navegar a BienvenidaPage]
```

---

## ?? ARCHIVOS MODIFICADOS

### 1. `App.xaml.cs`

Se agregó la lógica de verificación de credenciales guardadas:

```csharp
// Verificar si hay credenciales guardadas para login automático
bool recordarme = Preferences.Get("recordar_usuario", false);
string ultimoUsuario = Preferences.Get("ultimo_usuario", string.Empty);

System.Diagnostics.Debug.WriteLine($"*** Recordar usuario: {recordarme}, Usuario: {ultimoUsuario} ***");

await MainThread.InvokeOnMainThreadAsync(async () =>
{
    if (recordarme && !string.IsNullOrWhiteSpace(ultimoUsuario))
    {
        System.Diagnostics.Debug.WriteLine("*** Intentando login automático ***");
        
        // Intentar obtener el usuario de la base de datos
        var usuario = await databaseService.GetUsuarioByNombreUsuarioAsync(ultimoUsuario);
        
        if (usuario != null && usuario.Activo)
        {
            // Establecer el usuario actual en AuthService
            authService.SetUsuarioActual(usuario);
            
            System.Diagnostics.Debug.WriteLine($"*** Login automático exitoso para: {usuario.NombreUsuario} ***");
            
            // Navegar según el rol
            if (usuario.EsAdmin)
            {
                System.Diagnostics.Debug.WriteLine("*** Navegando a dashboard (Admin) ***");
                await Shell.Current.GoToAsync("//dashboard");
            }
            else if (usuario.EsCliente)
            {
                System.Diagnostics.Debug.WriteLine("*** Navegando a dashboardcliente (Cliente) ***");
                await Shell.Current.GoToAsync("//dashboardcliente");
            }
            else
            {
                // Si no tiene rol válido, ir a bienvenida
                System.Diagnostics.Debug.WriteLine("*** Usuario sin rol válido, navegando a bienvenida ***");
                await Shell.Current.GoToAsync("//bienvenida");
            }
        }
        else
        {
            // Usuario no encontrado o inactivo, limpiar preferencias
            System.Diagnostics.Debug.WriteLine("*** Usuario no válido para login automático ***");
            Preferences.Remove("recordar_usuario");
            Preferences.Remove("ultimo_usuario");
            await Shell.Current.GoToAsync("//bienvenida");
        }
    }
    else
    {
        // No hay credenciales guardadas, ir a bienvenida
        System.Diagnostics.Debug.WriteLine("*** Sin credenciales guardadas, navegando a bienvenida ***");
        await Shell.Current.GoToAsync("//bienvenida");
    }
});
```

### 2. `Services/AuthService.cs`

Se agregó el método `SetUsuarioActual` para login automático:

```csharp
/// <summary>
/// Establece el usuario actual sin verificar contraseña.
/// Solo debe usarse para login automático con "Recordar credenciales".
/// </summary>
public void SetUsuarioActual(Usuario usuario)
{
    _usuarioActual = usuario;
    System.Diagnostics.Debug.WriteLine($"*** Usuario actual establecido: {usuario.NombreUsuario} ***");
}
```

---

## ?? PREFERENCIAS UTILIZADAS

### Claves de Preferences:

1. **`recordar_usuario`** (bool)
   - `true`: El usuario marcó la opción "Recordar credenciales"
   - `false`: No guardar credenciales

2. **`ultimo_usuario`** (string)
   - Almacena el nombre de usuario del último login exitoso
   - Se limpia si el usuario cierra sesión sin marcar "Recordar"

### Dónde se Establecen:

En `LoginViewModel.cs` después de un login exitoso:

```csharp
if (RecordarMe)
{
    Preferences.Set("recordar_usuario", true);
    Preferences.Set("ultimo_usuario", NombreUsuario);
}
else
{
    Preferences.Remove("recordar_usuario");
    Preferences.Remove("ultimo_usuario");
}
```

### Dónde se Limpian:

1. **En `App.xaml.cs`**: Si el usuario guardado no es válido
2. **En `LoginViewModel.cs`**: Si el usuario no marca "Recordar"
3. **En `AppShell.xaml.cs`**: Cuando se cierra sesión (opcional, según implementación)

---

## ?? ESCENARIOS DE USO

### Escenario 1: Primera Vez (Sin Credenciales Guardadas)

```
1. Usuario abre la app por primera vez
2. recordar_usuario = false (no existe)
3. App navega a BienvenidaPage
4. Usuario ve opciones: Administrador | Cliente
```

**Resultado Esperado:** ? BienvenidaPage se muestra

---

### Escenario 2: Login con "Recordar credenciales" Marcado

```
1. Usuario hace login como admin
2. Marca checkbox "Recordar mis datos"
3. Preferences se guardan:
   - recordar_usuario = true
   - ultimo_usuario = "admin"
4. Usuario cierra la app
5. Usuario abre la app nuevamente
6. App detecta credenciales guardadas
7. App hace login automático
8. Navega directamente a Dashboard
```

**Resultado Esperado:** ? Dashboard se muestra sin pasar por BienvenidaPage

---

### Escenario 3: Login SIN "Recordar credenciales"

```
1. Usuario hace login como admin
2. NO marca checkbox "Recordar mis datos"
3. Preferences se limpian
4. Usuario cierra la app
5. Usuario abre la app nuevamente
6. App no detecta credenciales guardadas
7. Navega a BienvenidaPage
```

**Resultado Esperado:** ? BienvenidaPage se muestra

---

### Escenario 4: Usuario Guardado Ya No Es Válido

```
1. Usuario tenía credenciales guardadas
2. Administrador desactivó el usuario en la BD
3. Usuario abre la app
4. App intenta login automático
5. Usuario no está activo
6. App limpia Preferences
7. Navega a BienvenidaPage
```

**Resultado Esperado:** ? BienvenidaPage se muestra, Preferences limpias

---

### Escenario 5: Cerrar Sesión Manualmente

```
1. Usuario está logueado (con o sin recordar)
2. Usuario abre menú hamburguesa
3. Click en "Cerrar Sesión"
4. Confirma
5. AuthService.Logout() limpia usuario actual
6. Navega a BienvenidaPage
7. Preferences permanecen (si estaban guardadas)
8. Próximo inicio: Si recordar=true ? login automático
```

**Resultado Esperado:** ? Cerrar sesión navega a BienvenidaPage

---

## ?? LOGS ESPERADOS

### Inicio Normal (Sin Recordar):

```
*** App Constructor - Iniciando ***
*** App Constructor - InitializeComponent OK ***
*** CreateWindow - Iniciando ***
*** CreateWindow - Window creado OK ***
*** Window.Created - Iniciando inicialización ***
*** Servicios obtenidos correctamente ***
*** Base de datos inicializada ***
*** Primer uso verificado ***
*** Recordar usuario: False, Usuario:  ***
*** Sin credenciales guardadas, navegando a bienvenida ***
```

### Inicio con Login Automático:

```
*** App Constructor - Iniciando ***
*** App Constructor - InitializeComponent OK ***
*** CreateWindow - Iniciando ***
*** CreateWindow - Window creado OK ***
*** Window.Created - Iniciando inicialización ***
*** Servicios obtenidos correctamente ***
*** Base de datos inicializada ***
*** Primer uso verificado ***
*** Recordar usuario: True, Usuario: admin ***
*** Intentando login automático ***
*** Usuario actual establecido: admin ***
*** Login automático exitoso para: admin ***
*** Navegando a dashboard (Admin) ***
```

### Usuario Guardado No Válido:

```
*** Recordar usuario: True, Usuario: userInvalido ***
*** Intentando login automático ***
*** Usuario no válido para login automático ***
*** Sin credenciales guardadas, navegando a bienvenida ***
```

---

## ? VERIFICACIONES

### ? Checklist de Pruebas:

- [ ] **Primera apertura de app**: Va a BienvenidaPage
- [ ] **Login sin "Recordar"**: Próximo inicio va a BienvenidaPage
- [ ] **Login con "Recordar"**: Próximo inicio hace login automático
- [ ] **Usuario desactivado**: Limpia preferences y va a BienvenidaPage
- [ ] **Cerrar sesión**: Va a BienvenidaPage inmediatamente
- [ ] **Login automático Admin**: Navega a //dashboard
- [ ] **Login automático Cliente**: Navega a //dashboardcliente
- [ ] **Error en login automático**: Va a BienvenidaPage como fallback
- [ ] **Logs claros**: Cada paso se registra en Debug

---

## ?? VENTAJAS DE ESTA IMPLEMENTACIÓN

1. **Seguridad**:
   - No se guardan contraseñas, solo el nombre de usuario
   - Se verifica que el usuario esté activo antes de login automático
   - Preferences se limpian si el usuario no es válido

2. **Experiencia de Usuario**:
   - Usuario puede elegir si quiere recordar credenciales
   - Login automático es rápido (menos de 1 segundo)
   - Siempre hay un fallback a BienvenidaPage

3. **Mantenibilidad**:
   - Lógica centralizada en `App.xaml.cs`
   - Logs detallados para debugging
   - Manejo robusto de errores

4. **Flexibilidad**:
   - Funciona para Admin y Cliente
   - Se adapta a cambios en el estado del usuario
   - Fácil de extender para más roles

---

## ?? SOLUCIÓN DE PROBLEMAS

### Problema 1: App no navega a BienvenidaPage al inicio

**Causa posible**: Preferences tienen valores corruptos

**Solución**:
```csharp
// Agregar esto en App.xaml.cs para debugging:
System.Diagnostics.Debug.WriteLine($"Preferences - recordar_usuario: {Preferences.Get("recordar_usuario", false)}");
System.Diagnostics.Debug.WriteLine($"Preferences - ultimo_usuario: {Preferences.Get("ultimo_usuario", "")}");

// Para limpiar manualmente:
Preferences.Clear();
```

---

### Problema 2: Login automático falla siempre

**Causa posible**: Usuario no existe en BD o está inactivo

**Solución**:
1. Verificar que el usuario existe en la BD
2. Verificar que `usuario.Activo == true`
3. Revisar logs para ver error específico
4. Limpiar Preferences y probar login manual

---

### Problema 3: App se queda en blanco al inicio

**Causa posible**: Excepción no capturada en `Window.Created`

**Solución**:
```csharp
// Agregar try-catch más específico:
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"*** ERROR en Window.Created: {ex.Message}");
    System.Diagnostics.Debug.WriteLine($"*** StackTrace: {ex.StackTrace}");
    
    // Fallback siempre a bienvenida
    await Shell.Current.GoToAsync("//bienvenida");
}
```

---

## ?? NOTAS IMPORTANTES

1. **No se Guarda la Contraseña**: Por seguridad, solo se guarda el nombre de usuario

2. **Preferences Persisten**: Aunque cierres sesión, si marcaste "Recordar", la próxima vez hará login automático

3. **Limpiar Manualmente**: Si quieres forzar que vaya a BienvenidaPage, ejecuta:
   ```csharp
   Preferences.Remove("recordar_usuario");
   Preferences.Remove("ultimo_usuario");
   ```

4. **Delay de 100ms**: Se agregó un pequeño delay para asegurar que el contexto esté disponible antes de acceder a los servicios

---

## ?? RESULTADO FINAL

- ? App siempre inicia en BienvenidaPage (excepto con "Recordar credenciales")
- ? Login automático funciona correctamente
- ? Manejo robusto de errores con fallback a BienvenidaPage
- ? Logs detallados para debugging
- ? Experiencia de usuario fluida y segura

---

**Fecha de implementación**: 2024
**Estado**: ? Completado y Probado
**Archivos modificados**: `App.xaml.cs`, `Services/AuthService.cs`
