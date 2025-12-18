# Diagnóstico de Crash en Android

## Pasos para diagnosticar el problema

### 1. Limpiar y Reconstruir
```bash
# En Visual Studio:
- Click derecho en el proyecto > Clean
- Build > Rebuild Solution
```

### 2. Desinstalar la app del emulador
```bash
adb uninstall com.companyname.app_credivnzl
```

### 3. Capturar logs en tiempo real

#### Opción A: Desde Visual Studio
1. Ir a View > Output
2. En el dropdown seleccionar "Debug"
3. Ejecutar la aplicación
4. Buscar líneas que comiencen con `***`

#### Opción B: Desde línea de comandos
```powershell
# Limpiar logs anteriores
adb logcat -c

# En una ventana de PowerShell separada, ejecutar:
adb logcat -s "mono-rt:V" "DEBUG:V" "*:E"

# O para ver todos los logs:
adb logcat | Select-String -Pattern "app_credivnzl|mono|FATAL|Exception"
```

### 4. Buscar mensajes específicos

Los logs que agregamos tienen el formato:
```
*** AppShell Constructor - Iniciando ***
*** App Constructor - Iniciando ***
*** DashboardPage Constructor - Iniciando ***
*** ERROR EN [Método] ***: [Mensaje de error]
```

### 5. Problemas comunes y soluciones

#### Problema: Recursos faltantes
**Síntoma**: Error al cargar `InitializeComponent()`
**Solución**: 
- Verificar que existan todos los archivos .xaml
- Verificar que todos los convertidores estén en `App.xaml`
- Verificar que `Colors.xaml` y `Styles.xaml` existan

#### Problema: Base de datos no inicializa
**Síntoma**: Error en `InitializeAsync()`
**Solución**:
- Verificar permisos de escritura
- Verificar que SQLite esté correctamente instalado

#### Problema: ViewModels con errores
**Síntoma**: Error al crear ViewModel
**Solución**:
- No llamar métodos async en constructores
- Inicializar colecciones en el constructor
- Cargar datos solo en `OnAppearing()`

### 6. Verificar en la ventana Output de Visual Studio

Buscar estos mensajes en orden:
1. `*** App Constructor - Iniciando ***`
2. `*** App Constructor - InitializeComponent OK ***`
3. `*** CreateWindow - Iniciando ***`
4. `*** CreateWindow - Window creado OK ***`
5. `*** AppShell Constructor - Iniciando ***`
6. `*** AppShell Constructor - InitializeComponent OK ***`
7. `*** AppShell Constructor - Rutas registradas OK ***`
8. `*** DashboardPage Constructor - Iniciando ***`
9. `*** DashboardPage Constructor - InitializeComponent OK ***`
10. `*** DashboardPage OnAppearing - Iniciando ***`

**Si la aplicación se cierra antes de completar esta secuencia, el último mensaje indica dónde está el problema.**

### 7. Solución temporal: Simplificar DashboardPage.xaml

Si el problema es en `InitializeComponent()` del Dashboard, editar `DashboardPage.xaml` para tener solo:

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="App_CrediVnzl.Pages.DashboardPage"
             Title="Dashboard"
             Shell.NavBarIsVisible="False">
    
    <VerticalStackLayout Padding="20" Spacing="10">
        <Label Text="Dashboard Simplificado"
               FontSize="24"
               FontAttributes="Bold" />
        <Label Text="La app se inició correctamente"
               FontSize="16" />
    </VerticalStackLayout>
    
</ContentPage>
```

Si con esto la app se abre, el problema está en el XAML complejo del Dashboard.

### 8. Verificar logs específicos de SQLite

Si hay un error con la base de datos:
```powershell
adb logcat | Select-String -Pattern "SQLite|sqlite|database"
```

### 9. Comando para ver crash completo

```powershell
# Ver solo errores y crashes
adb logcat -b crash
```

## Cambios realizados para diagnóstico

1. ? Agregado logging extensivo en:
   - `MauiProgram.cs`
   - `App.xaml.cs`
   - `AppShell.xaml.cs`
   - `DashboardPage.xaml.cs`

2. ? Agregado manejo de excepciones global

3. ? Agregados colores faltantes en `Colors.xaml`

4. ? Removidas llamadas async de constructores de ViewModels

5. ? Agregado try-catch en todos los métodos `OnAppearing()`

## Próximos pasos

1. Ejecutar la aplicación
2. Revisar la ventana Output en Visual Studio
3. Buscar el último mensaje `***` antes del crash
4. Reportar ese mensaje para diagnóstico específico
