# INSTRUCCIONES PARA RESOLVER EL CRASH

## ? Cambios Realizados

He realizado los siguientes cambios para diagnosticar y solucionar el problema:

### 1. Colores faltantes agregados (Resources\Styles\Colors.xaml)
- `Gray950`
- `OffBlack`
- `Magenta`
- `MidnightBlue`
- `PrimaryDarkText`
- `SecondaryDarkText`

### 2. Logging extensivo agregado
- `MauiProgram.cs`: Logs en CreateMauiApp y manejo de excepciones global
- `App.xaml.cs`: Logs en constructor y CreateWindow
- `AppShell.xaml.cs`: Logs en constructor y registro de rutas
- `DashboardPage.xaml.cs`: Logs detallados en cada paso

### 3. Correcciones en ViewModels
- `CalendarioPagosViewModel.cs`: Removida llamada a LoadDataAsync() del constructor
- `ClientesViewModel.cs`: Removida llamada a LoadClientesAsync() del constructor
- `DashboardViewModel.cs`: Agregado try-catch en LoadDashboardDataAsync()

### 4. Try-catch agregado en todas las páginas
- `DashboardPage.xaml.cs`
- `ClientesPage.xaml.cs`
- `CalendarioPagosPage.xaml.cs`
- `EnviarMensajesPage.xaml.cs`

## ?? PASOS A SEGUIR AHORA

### Paso 1: Limpiar y Reconstruir
```
1. En Visual Studio, click derecho en el proyecto App_CrediVnzl
2. Seleccionar "Clean"
3. Esperar a que termine
4. Build > Rebuild Solution
```

### Paso 2: Desinstalar la app del emulador
En Android Studio o desde el emulador:
- Mantén presionada la app "App_CrediVnzl"
- Arrastra a "Desinstalar"
- O desde PowerShell: `adb uninstall com.companyname.app_credivnzl`

### Paso 3: Ejecutar con diagnóstico
```
1. En Visual Studio, ir a: View > Output
2. En el dropdown de "Show output from:", seleccionar "Debug"
3. Presionar F5 o click en el botón de Run
4. Observar los mensajes en la ventana Output
```

### Paso 4: Identificar el problema

Busca en la ventana Output la secuencia de mensajes que comienzan con `***`:

**Secuencia esperada (todo OK):**
```
*** App Constructor - Iniciando ***
*** App Constructor - InitializeComponent OK ***
*** CreateWindow - Iniciando ***
*** AppShell Constructor - Iniciando ***
*** AppShell Constructor - InitializeComponent OK ***
*** AppShell Constructor - Rutas registradas OK ***
*** CreateWindow - Window creado OK ***
*** DashboardPage Constructor - Iniciando ***
*** DashboardPage Constructor - InitializeComponent OK ***
*** DashboardPage Constructor - Completo ***
*** DashboardPage OnAppearing - Iniciando ***
*** DashboardPage OnAppearing - Inicializando DB ***
*** DashboardPage OnAppearing - DB Inicializada OK ***
*** DashboardPage OnAppearing - Creando ViewModel ***
*** DashboardPage OnAppearing - ViewModel creado OK ***
*** DashboardPage OnAppearing - Cargando datos ***
*** DashboardPage OnAppearing - Datos cargados OK ***
```

**Si encuentras un mensaje de ERROR:**
```
*** ERROR EN [NombreDelMetodo] ***: [Mensaje de error]
StackTrace: [...]
InnerException: [...]
```

Esto te dirá exactamente dónde y por qué falla.

## ?? DIAGNÓSTICO POR TIPO DE ERROR

### Si falla en "App Constructor - InitializeComponent"
**Problema:** Recursos de App.xaml no cargan
**Solución:**
1. Verificar que existan los archivos:
   - `Resources\Styles\Colors.xaml`
   - `Resources\Styles\Styles.xaml`
2. Verificar que todos los Converters existan en la carpeta `Converters\`

### Si falla en "AppShell Constructor - InitializeComponent"
**Problema:** AppShell.xaml tiene errores
**Solución:**
1. Revisar `AppShell.xaml`
2. Verificar que `DashboardPage` esté correctamente referenciada

### Si falla en "DashboardPage Constructor - InitializeComponent"
**Problema:** DashboardPage.xaml tiene errores XAML
**Solución temporal:**
1. Simplificar `DashboardPage.xaml` (ver archivo DIAGNOSTICO_CRASH.md)
2. Una vez que funcione, ir agregando elementos gradualmente

### Si falla en "DashboardPage OnAppearing - Inicializando DB"
**Problema:** SQLite no puede crear la base de datos
**Posibles causas:**
- Permisos de escritura en Android
- Paquetes SQLite no instalados correctamente
**Solución:**
1. Verificar paquetes NuGet:
   - `sqlite-net-pcl` Version 1.9.172
   - `SQLitePCLRaw.bundle_green` Version 2.1.10
2. Agregar permiso en AndroidManifest.xml:
   ```xml
   <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
   ```

### Si falla en "DashboardPage OnAppearing - Creando ViewModel"
**Problema:** Error en el constructor de DashboardViewModel
**Solución:**
1. Revisar que DatabaseService esté correctamente inyectado
2. Verificar que todos los modelos (DashboardCard, MenuCard, etc.) existan

## ?? SI AÚN NO FUNCIONA

### Opción A: Modo Super Simplificado
Reemplaza temporalmente `DashboardPage.xaml` con:

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="App_CrediVnzl.Pages.DashboardPage"
             Title="Dashboard"
             Shell.NavBarIsVisible="False">
    
    <VerticalStackLayout Padding="20" Spacing="20" VerticalOptions="Center">
        <Label Text="? La App Funciona!"
               FontSize="32"
               FontAttributes="Bold"
               HorizontalOptions="Center" />
        
        <Label Text="Dashboard Simplificado"
               FontSize="18"
               HorizontalOptions="Center" />
        
        <Button Text="Ver Clientes"
                Clicked="OnClientesTapped" />
    </VerticalStackLayout>
    
</ContentPage>
```

Si con esto funciona, el problema está en el XAML complejo.

### Opción B: Ver logs de Android directamente
En una ventana de PowerShell separada:
```powershell
# Ver todos los logs filtrados
adb logcat | Select-String -Pattern "mono|FATAL|Exception|app_credivnzl"

# O ver solo crashes
adb logcat -b crash

# O guardar logs en archivo
adb logcat > C:\temp\android_logs.txt
```

## ?? INFORMACIÓN PARA REPORTAR

Si después de seguir estos pasos sigue fallando, necesito que me proporciones:

1. **El último mensaje `***` que aparece en Output antes del crash**
2. **El mensaje de ERROR completo (si aparece)**
3. **El contenido del StackTrace**
4. **En qué paso de la secuencia esperada se detiene**

Ejemplo:
```
"La app se cierra después de mostrar:
*** AppShell Constructor - InitializeComponent OK ***

Y aparece el error:
*** ERROR EN AppShell Constructor ***: System.NullReferenceException: Object reference not set to an instance of an object
StackTrace: at App_CrediVnzl.AppShell..ctor() in C:\...\AppShell.xaml.cs:line 15"
```

Con esa información podré identificar exactamente el problema.
