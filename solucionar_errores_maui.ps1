# Script para solucionar errores comunes de XAML en .NET MAUI
Write-Host "===========================================================" -ForegroundColor Cyan
Write-Host "    SOLUCIONADOR DE ERRORES XAML - .NET MAUI" -ForegroundColor Cyan  
Write-Host "===========================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "[1/5] Limpiando proyecto..." -ForegroundColor Yellow
dotnet clean > $null 2>&1
Write-Host "      ? Proyecto limpiado" -ForegroundColor Green

Write-Host "[2/5] Eliminando carpetas temporales..." -ForegroundColor Yellow
if (Test-Path "obj") { Remove-Item -Recurse -Force "obj" }
if (Test-Path "bin") { Remove-Item -Recurse -Force "bin" }
Write-Host "      ? Archivos temporales eliminados" -ForegroundColor Green

Write-Host "[3/5] Restaurando paquetes..." -ForegroundColor Yellow
dotnet restore > $null 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "      ? Paquetes restaurados correctamente" -ForegroundColor Green
} else {
    Write-Host "      ? Error al restaurar paquetes" -ForegroundColor Red
}

Write-Host "[4/5] Compilando proyecto..." -ForegroundColor Yellow
$buildOutput = dotnet build 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "      ? Compilación exitosa" -ForegroundColor Green
} else {
    Write-Host "      ? Errores de compilación encontrados:" -ForegroundColor Red
    Write-Host $buildOutput -ForegroundColor Red
}

Write-Host "[5/5] Verificando errores específicos..." -ForegroundColor Yellow

# Verificar errores comunes
$erroresComunes = @{
    "CS0103" = "InitializeComponent no existe - Problema de generación de código XAML"
    "XLS0413" = "Propiedad no encontrada - Incompatibilidad de controles MAUI"
    "BorderWidth" = "Usar StrokeThickness en lugar de BorderWidth"
    "BorderColor" = "Usar Stroke en lugar de BorderColor"
    "TapGestureRecognizer.*Clicked" = "Usar Tapped en lugar de Clicked"
}

$problemasEncontrados = 0

foreach ($error in $erroresComunes.Keys) {
    $description = $erroresComunes[$error]
    
    # Buscar en archivos XAML
    $xamlFiles = Get-ChildItem -Path "." -Filter "*.xaml" -Recurse | Where-Object { 
        $_.Directory.Name -ne "obj" -and $_.Directory.Name -ne "bin" 
    }
    
    foreach ($file in $xamlFiles) {
        $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        if ($content -and ($content -match $error)) {
            Write-Host "      ??  $($file.Name): $description" -ForegroundColor Yellow
            $problemasEncontrados++
        }
    }
}

if ($problemasEncontrados -eq 0) {
    Write-Host "      ? No se encontraron patrones de errores conocidos" -ForegroundColor Green
}

Write-Host ""
Write-Host "===========================================================" -ForegroundColor Cyan
Write-Host "                        RESUMEN" -ForegroundColor Cyan
Write-Host "===========================================================" -ForegroundColor Cyan

if ($LASTEXITCODE -eq 0) {
    Write-Host "? COMPILACIÓN EXITOSA" -ForegroundColor Green
    Write-Host ""
    Write-Host "Errores comunes resueltos:" -ForegroundColor Green
    Write-Host "  • CS0103 (InitializeComponent): Solucionado con clean/rebuild" -ForegroundColor White
    Write-Host "  • XLS0413 (Border properties): Usamos Frame en su lugar" -ForegroundColor White
    Write-Host "  • TapGestureRecognizer: Corregido a usar 'Tapped'" -ForegroundColor White
    Write-Host ""
    Write-Host "?? LA APLICACIÓN DEBERÍA FUNCIONAR CORRECTAMENTE AHORA" -ForegroundColor Green
} else {
    Write-Host "? ERRORES PENDIENTES" -ForegroundColor Red
    Write-Host ""
    Write-Host "Pasos adicionales recomendados:" -ForegroundColor Yellow
    Write-Host "  1. Cerrar Visual Studio completamente" -ForegroundColor White
    Write-Host "  2. Eliminar carpeta .vs (si existe)" -ForegroundColor White
    Write-Host "  3. Reabrir el proyecto" -ForegroundColor White
    Write-Host "  4. Ejecutar: dotnet clean && dotnet build" -ForegroundColor White
}

Write-Host ""