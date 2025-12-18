# Script para capturar logs de Android en tiempo real
# Ejecuta este script ANTES de iniciar la aplicación en Visual Studio

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Capturando logs de Android..." -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que ADB esté disponible
$adbPath = "adb"
try {
    $null = & $adbPath devices 2>&1
    Write-Host "? ADB encontrado" -ForegroundColor Green
} catch {
    Write-Host "? ADB no encontrado. Agregando ruta de Android SDK..." -ForegroundColor Yellow
    $env:Path += ";C:\Program Files (x86)\Android\android-sdk\platform-tools"
    $env:Path += ";$env:LOCALAPPDATA\Android\Sdk\platform-tools"
}

Write-Host ""
Write-Host "Limpiando logs anteriores..." -ForegroundColor Yellow
& $adbPath logcat -c

Write-Host "Esperando logs de la aplicación..." -ForegroundColor Yellow
Write-Host "Presiona Ctrl+C para detener" -ForegroundColor Gray
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Capturar logs filtrados
& $adbPath logcat -v time `
    -s "mono:V" `
    "mono-rt:V" `
    "DEBUG:V" `
    "AndroidRuntime:E" `
    "System.err:W" `
    "*:E" `
    "*:F" | ForEach-Object {
    
    $line = $_
    
    # Colorear según tipo de mensaje
    if ($line -match "FATAL|AndroidRuntime") {
        Write-Host $line -ForegroundColor Red
    }
    elseif ($line -match "ERROR|Exception") {
        Write-Host $line -ForegroundColor Yellow
    }
    elseif ($line -match "\*\*\*") {
        Write-Host $line -ForegroundColor Cyan
    }
    elseif ($line -match "app_credivnzl|CrediVnzl") {
        Write-Host $line -ForegroundColor Green
    }
    else {
        Write-Host $line
    }
}
