# Script para buscar y reportar errores de XAML en .NET MAUI
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "    BUSCANDO ERRORES XAML - .NET MAUI" -ForegroundColor Cyan  
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

$errorsFound = 0

# Buscar archivos XAML
$xamlFiles = Get-ChildItem -Path "." -Filter "*.xaml" -Recurse | Where-Object { $_.Directory.Name -ne "obj" -and $_.Directory.Name -ne "bin" }

Write-Host "Archivos XAML encontrados: $($xamlFiles.Count)" -ForegroundColor Green
Write-Host ""

foreach ($file in $xamlFiles) {
    Write-Host "Revisando: $($file.FullName)" -ForegroundColor Yellow
    
    $content = Get-Content $file.FullName -Raw
    
    # Buscar TapGestureRecognizer con Clicked (debería ser Tapped)
    if ($content -match 'TapGestureRecognizer.*Clicked=') {
        Write-Host "  ? ERROR: TapGestureRecognizer con 'Clicked=' encontrado (debería ser 'Tapped=')" -ForegroundColor Red
        $errorsFound++
    }
    
    # Buscar Frame con BorderWidth (debería ser Stroke + StrokeThickness)
    if ($content -match 'BorderWidth=') {
        Write-Host "  ? ERROR: Frame con 'BorderWidth=' encontrado (debería ser 'Stroke=' + 'StrokeThickness=')" -ForegroundColor Red
        $errorsFound++
    }
    
    # Buscar Frame con BorderColor (debería ser Stroke)
    if ($content -match 'BorderColor=') {
        Write-Host "  ??  ADVERTENCIA: Frame con 'BorderColor=' encontrado (debería ser 'Stroke=')" -ForegroundColor Yellow
        $errorsFound++
    }
    
    # Verificar si no tiene errores
    if (-not ($content -match 'TapGestureRecognizer.*Clicked=' -or $content -match 'BorderWidth=' -or $content -match 'BorderColor=')) {
        Write-Host "  ? Sin errores XAML" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "              RESUMEN" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan

if ($errorsFound -eq 0) {
    Write-Host "? NO SE ENCONTRARON ERRORES XAML" -ForegroundColor Green
    Write-Host "   Todos los archivos XAML son compatibles con .NET MAUI"
} else {
    Write-Host "? SE ENCONTRARON $errorsFound PROBLEMAS" -ForegroundColor Red
    Write-Host ""
    Write-Host "Correcciones necesarias:" -ForegroundColor Yellow
    Write-Host "  • TapGestureRecognizer: Cambiar 'Clicked=' por 'Tapped='" -ForegroundColor White
    Write-Host "  • Frame: Cambiar 'BorderWidth=' por 'StrokeThickness='" -ForegroundColor White  
    Write-Host "  • Frame: Cambiar 'BorderColor=' por 'Stroke='" -ForegroundColor White
}

Write-Host ""
Write-Host "Compilación actual:" -ForegroundColor Cyan
dotnet build --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "? COMPILACIÓN EXITOSA" -ForegroundColor Green
} else {
    Write-Host "? ERRORES DE COMPILACIÓN" -ForegroundColor Red
}

Write-Host ""