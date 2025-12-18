# ?? INSTRUCCIONES URGENTES - MODO DIAGNÓSTICO SIMPLIFICADO

## ? CAMBIOS REALIZADOS

He simplificado la aplicación al mínimo para que pueda iniciar:

1. **DashboardPage.xaml** - Ahora es una página simple con solo botones
2. **DashboardPage.xaml.cs** - Sin ViewModel, solo navegación básica
3. **App.xaml** - Sin convertidores (eliminados temporalmente)

## ?? PASOS A SEGUIR (MUY IMPORTANTE)

### Paso 1: Limpiar y Reconstruir
```
1. En Visual Studio: Build > Clean Solution
2. Esperar a que termine
3. Build > Rebuild Solution
4. Esperar a que compile (debe decir "Compilación correcta")
```

### Paso 2: Capturar Logs (CRÍTICO)

**Opción A: Usar el script PowerShell**
1. Abrir PowerShell como Administrador
2. Navegar a: `cd C:\Proyectos\App_CrediVnzl`
3. Ejecutar: `.\logs_simple.ps1`
4. Dejar esa ventana abierta
5. En Visual Studio, presionar F5 para ejecutar la app
6. Observar la ventana de PowerShell para ver los logs

**Opción B: Manualmente en PowerShell**
```powershell
# En PowerShell:
adb logcat -c
adb logcat *:E *:F
```

**Opción C: Ver en la ventana Output de Visual Studio**
1. View > Output
2. En el dropdown seleccionar "Debug"
3. Buscar mensajes que empiecen con `***`

### Paso 3: Ejecutar la App
```
1. En Visual Studio, presionar F5
2. Esperar a que el emulador se abra
3. Observar:
   - ¿La app se abre con una pantalla blanca con botones?
   - ¿Se cierra inmediatamente?
   - ¿Aparece algún mensaje de error?
```

## ?? RESULTADOS ESPERADOS

### ? CASO 1: La app se abre correctamente
**Síntoma:** Ves una pantalla con:
- Título "CrediVnzl" en azul
- Mensaje "La aplicación se inició correctamente"
- 3 botones (Ver Clientes, Calendario, Mensajes)

**Acción:** 
¡Excelente! Significa que el problema estaba en el XAML complejo o en los Convertidores.
Reporta: "? La app se abrió con la versión simplificada"

### ? CASO 2: La app se cierra inmediatamente
**Síntoma:** Ves el splash screen pero luego se cierra

**Acción:**
1. Revisar la ventana Output (View > Output > Debug)
2. Buscar el ÚLTIMO mensaje que empiece con `***`
3. Copiar TODO el texto desde ese mensaje hasta el error

**EJEMPLO de lo que necesito ver:**
```
*** App Constructor - Iniciando ***
*** App Constructor - InitializeComponent OK ***
*** CreateWindow - Iniciando ***
*** ERROR EN CreateWindow ***: System.TypeInitializationException: The type initializer for 'App_CrediVnzl.AppShell' threw an exception.
StackTrace: at System.RuntimeType.CreateInstanceDefaultCtor...
InnerException: Could not load file or assembly 'sqlite-net-pcl'...
```

## ?? QUÉ BUSCAR EN LOS LOGS

### Buscar estos mensajes en orden:
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
```

**El último mensaje que veas te dirá EXACTAMENTE dónde falla.**

### Errores Comunes:

#### Error 1: Falla en "App Constructor"
```
*** App Constructor - Iniciando ***
*** ERROR EN App Constructor ***: Could not load file or assembly 'Microsoft.Maui.Controls'
```
**Significa:** Problema con paquetes NuGet
**Solución:** Tools > NuGet Package Manager > Update All

#### Error 2: Falla en "AppShell Constructor"
```
*** AppShell Constructor - Iniciando ***
*** ERROR EN AppShell Constructor ***: System.NullReferenceException
```
**Significa:** Problema con AppShell.xaml o rutas
**Solución:** Revisar que todas las páginas existan

#### Error 3: Falla en "DashboardPage Constructor"
```
*** DashboardPage Constructor - Iniciando ***
*** ERROR EN DashboardPage Constructor ***: XamlParseException
```
**Significa:** Error en el XAML de DashboardPage
**Solución:** El XAML está mal formado (pero lo simplifiqué, así que no debería pasar)

#### Error 4: Falla en "Inicializando DB"
```
*** DashboardPage OnAppearing - Inicializando DB ***
*** ERROR EN DashboardPage.OnAppearing ***: SQLite.SQLiteException
```
**Significa:** Problema con SQLite
**Solución:** 
1. Verificar paquetes NuGet de SQLite
2. Agregar permisos en AndroidManifest.xml

## ?? SI LA APP SE ABRE CORRECTAMENTE

Si la app se abre con la versión simplificada, prueba lo siguiente:

### Prueba 1: Probar los botones
1. Click en "Ver Clientes" - ¿Funciona la navegación?
2. Click en "Calendario" - ¿Funciona?
3. Click en "Mensajes" - ¿Funciona?

### Prueba 2: Verificar la base de datos
Si la app se abre, el mensaje en Output debería decir:
```
*** DashboardPage OnAppearing - DB Inicializada OK ***
```

Esto significa que SQLite funciona correctamente.

## ?? INFORMACIÓN QUE NECESITO

Si la app SIGUE cerrándose, necesito EXACTAMENTE esto:

1. **El ÚLTIMO mensaje `***` que aparece antes del crash**
2. **El mensaje completo de ERROR (con el StackTrace)**
3. **Cualquier mensaje en ROJO en la ventana Output**

### Formato del reporte:
```
ÚLTIMO MENSAJE VISTO:
*** AppShell Constructor - Iniciando ***

ERROR COMPLETO:
*** ERROR EN AppShell Constructor ***: System.TypeInitializationException: The type initializer for 'Microsoft.Maui.Controls.Shell' threw an exception.
StackTrace: 
   at App_CrediVnzl.AppShell..ctor() in C:\Proyectos\App_CrediVnzl\AppShell.xaml.cs:line 12
   at App_CrediVnzl.App.CreateWindow(IActivationState activationState) in C:\Proyectos\App_CrediVnzl\App.xaml.cs:line 25
InnerException: Could not load type 'Microsoft.Maui.Controls.ContentPage'
```

## ? SOLUCIÓN RÁPIDA SI FALLA TODO

Si nada de esto funciona, intenta esto:

1. Cerrar Visual Studio
2. Eliminar las carpetas:
   - `bin`
   - `obj`
3. Abrir Visual Studio
4. Rebuild Solution
5. Desinstalar la app del emulador manualmente
6. Run (F5)

## ?? DESPUÉS DE PROBAR

Repórtame UNO de estos dos resultados:

**Resultado A: ? Éxito**
"La app se abrió correctamente con la versión simplificada. Veo los 3 botones."

**Resultado B: ? Sigue fallando**
"La app se sigue cerrando. El último mensaje que veo es: [copiar aquí]"
"El error completo es: [copiar aquí]"

Con cualquiera de estas dos respuestas podré ayudarte a continuar.
