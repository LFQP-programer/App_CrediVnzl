# ??? Cómo Eliminar Datos Existentes y Crear Nueva Cuenta de Admin

## ?? Problema: "Ya existe un administrador configurado"

Si ves este mensaje, significa que ya hay una cuenta de administrador en la base de datos y necesitas eliminarla para crear una nueva.

---

## ?? SOLUCIÓN RÁPIDA (3 Opciones)

### **Opción 1: Desde el Código (LA MÁS RÁPIDA)**

Ya he agregado un método en el `AuthService` para eliminar datos. Aquí está cómo usarlo:

#### **Desde App.xaml.cs:**

Agrega esto temporalmente en el método `CreateWindow`:

```csharp
protected override Window CreateWindow(IActivationState? activationState)
{
    try
    {
        System.Diagnostics.Debug.WriteLine("*** CreateWindow - Iniciando ***");
        var window = new Window(new AppShell());
        
        // ?? TEMPORAL: Eliminar datos existentes
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                var authService = Handler?.MauiContext?.Services.GetService<AuthService>();
                var databaseService = Handler?.MauiContext?.Services.GetService<DatabaseService>();
                
                if (authService != null && databaseService != null)
                {
                    // Preguntar si quiere eliminar datos
                    var eliminar = await Application.Current!.MainPage!.DisplayAlert(
                        "Eliminar Datos",
                        "¿Quieres eliminar todos los datos existentes?",
                        "Sí",
                        "No");
                    
                    if (eliminar)
                    {
                        await authService.EliminarDatosExistentesAsync();
                        await Application.Current!.MainPage!.DisplayAlert(
                            "Éxito",
                            "Datos eliminados. Ahora puedes crear tu cuenta.",
                            "OK");
                    }
                    
                    await databaseService.InitializeAsync();
                    var esPrimerUso = await authService.VerificarPrimerUsoAsync();
                    
                    if (esPrimerUso)
                    {
                        await Shell.Current.GoToAsync("//primeruso");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("//bienvenida");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR: {ex.Message}");
            }
        });
        
        return window;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"*** ERROR EN CreateWindow ***: {ex.Message}");
        throw;
    }
}
```

**Después de eliminar los datos, QUITA este código temporal.**

---

### **Opción 2: Desde la PrimerUsoPage (Recomendada para Usuario Final)**

He actualizado el `PrimerUsoViewModel` con un comando `EliminarDatosCommand`. 

Para usarlo, agrega este botón en `PrimerUsoPage.xaml`:

```xml
<!-- Después del botón CREAR CUENTA -->
<Button Text="?? ELIMINAR DATOS EXISTENTES"
        Command="{Binding EliminarDatosCommand}"
        BackgroundColor="#F44336"
        TextColor="White"
        CornerRadius="10"
        HeightRequest="50"
        FontAttributes="Bold"
        Margin="0,20,0,0"
        IsEnabled="{Binding EstaOcupado, Converter={StaticResource InvertedBoolConverter}}"/>

<Label Text="?? Usa este botón si ya existe una cuenta y quieres empezar de nuevo" 
       FontSize="12" 
       HorizontalOptions="Center"
       HorizontalTextAlignment="Center"
       TextColor="#F44336"
       Margin="10,5,10,0"
       LineBreakMode="WordWrap"/>
```

---

### **Opción 3: Manualmente (Eliminar archivo físico)**

#### **En Windows:**
1. Cierra completamente la aplicación
2. Ve a: `C:\Users\TuUsuario\AppData\Local\Packages\[NombreDeTuApp]\LocalState\`
3. Busca el archivo: `prestafacil.db3`
4. Elimínalo
5. Ejecuta la app nuevamente

#### **En Android (Emulador o Device):**
1. Cierra la app
2. Ve a Configuración ? Apps ? CrediVnzl
3. Toca "Almacenamiento"
4. Toca "Borrar datos" o "Clear data"
5. Ejecuta la app nuevamente

#### **En Android (Archivo Explorer):**
1. Abre Android Device Monitor o File Explorer
2. Navega a: `/data/data/com.companyname.app_credivnzl/files/`
3. Elimina: `prestafacil.db3`
4. Ejecuta la app nuevamente

---

## ?? Flujo Recomendado

### **Para Desarrollo (Ahora):**

1. **Usa la Opción 1** (código temporal en App.xaml.cs)
2. Ejecuta la app
3. Clickea "Sí" para eliminar datos
4. Crea tu nueva cuenta de admin
5. **QUITA el código temporal** de App.xaml.cs

### **Para Producción (Futuro):**

1. **Usa la Opción 2** (botón en PrimerUsoPage)
2. Los usuarios verán el botón si tienen problemas
3. No necesitas código temporal

---

## ?? Código que ya agregué:

### **AuthService.cs:**
```csharp
public async Task<bool> EliminarDatosExistentesAsync()
{
    try
    {
        await _databaseService.EliminarBaseDeDatosCompletaAsync();
        _usuarioActual = null;
        return true;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error eliminando datos: {ex.Message}");
        return false;
    }
}
```

### **PrimerUsoViewModel.cs:**
```csharp
public ICommand EliminarDatosCommand { get; }

// Constructor:
EliminarDatosCommand = new Command(async () => await OnEliminarDatosAsync());

private async Task OnEliminarDatosAsync()
{
    var confirmar = await Application.Current!.MainPage!.DisplayAlert(
        "?? Eliminar Datos",
        "¿Estás seguro de eliminar TODOS los datos existentes?...",
        "Sí, eliminar todo",
        "Cancelar");

    if (!confirmar) return;

    // ... código de eliminación
}
```

---

## ? SOLUCIÓN MÁS RÁPIDA AHORA:

**Copia y pega este código en `App.xaml.cs` TEMPORALMENTE:**

```csharp
protected override Window CreateWindow(IActivationState? activationState)
{
    try
    {
        System.Diagnostics.Debug.WriteLine("*** CreateWindow - Iniciando ***");
        var window = new Window(new AppShell());
        
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                var authService = Handler?.MauiContext?.Services.GetService<AuthService>();
                
                if (authService != null)
                {
                    // Eliminar datos sin preguntar (SOLO PARA DESARROLLO)
                    await authService.EliminarDatosExistentesAsync();
                    System.Diagnostics.Debug.WriteLine("*** Datos eliminados automáticamente ***");
                    
                    await Shell.Current.GoToAsync("//primeruso");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR: {ex.Message}");
            }
        });
        
        return window;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"*** ERROR EN CreateWindow ***: {ex.Message}");
        throw;
    }
}
```

**Después de crear tu cuenta, reemplaza con el código original.**

---

## ?? Checklist:

- [ ] Elegir una de las 3 opciones
- [ ] Eliminar datos existentes
- [ ] Verificar que se eliminaron (la app debe ir a PrimerUsoPage)
- [ ] Crear nueva cuenta de administrador
- [ ] Quitar código temporal (si usaste Opción 1)
- [ ] ¡Listo para usar!

---

**¿Cuál opción prefieres usar?** ??
