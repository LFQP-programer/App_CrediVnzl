@echo off
echo ================================================
echo       SCRIPT DE DIAGNOSTICO - CrediVzla
echo ================================================
echo.

echo [1/4] Verificando compilacion...
dotnet build
if %errorlevel% neq 0 (
    echo ERROR: La aplicacion no compila correctamente
    pause
    exit /b 1
)
echo ? Compilacion exitosa
echo.

echo [2/4] Verificando archivos de recursos...
if not exist "Resources\Images\logo_credivzla.png" (
    echo ADVERTENCIA: Logo no encontrado, pero la app deberia funcionar
) else (
    echo ? Archivo de logo encontrado
)

if not exist "Resources\Fonts\OpenSans-Regular.ttf" (
    echo ADVERTENCIA: Fuente OpenSans-Regular no encontrada
) else (
    echo ? Fuente OpenSans-Regular encontrada
)

if not exist "Resources\Fonts\OpenSans-Semibold.ttf" (
    echo ADVERTENCIA: Fuente OpenSans-Semibold no encontrada
) else (
    echo ? Fuente OpenSans-Semibold encontrada
)
echo.

echo [3/4] Verificando estructura del proyecto...
if not exist "App.xaml" (
    echo ERROR: App.xaml no encontrado
    pause
    exit /b 1
)
echo ? App.xaml OK

if not exist "AppShell.xaml" (
    echo ERROR: AppShell.xaml no encontrado
    pause
    exit /b 1
)
echo ? AppShell.xaml OK

if not exist "MauiProgram.cs" (
    echo ERROR: MauiProgram.cs no encontrado
    pause
    exit /b 1
)
echo ? MauiProgram.cs OK

if not exist "Pages\BienvenidaPage.xaml" (
    echo ERROR: BienvenidaPage.xaml no encontrado
    pause
    exit /b 1
)
echo ? BienvenidaPage.xaml OK
echo.

echo [4/4] DIAGNOSTICO COMPLETADO
echo ================================================
echo   RESULTADO: La aplicacion deberia funcionar
echo   
echo   Para probar la app:
echo   - Windows: dotnet run -f net10.0-windows10.0.19041.0
echo   - Android: dotnet run -f net10.0-android
echo ================================================
echo.
pause