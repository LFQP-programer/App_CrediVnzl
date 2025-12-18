# Script simple para ver logs de Android
# Ejecutar ANTES de iniciar la app

Write-Host "Capturando logs de Android..." -ForegroundColor Cyan
Write-Host "Presiona Ctrl+C para detener" -ForegroundColor Yellow
Write-Host ""

# Limpiar logs
adb logcat -c

# Mostrar todos los logs
adb logcat *:E *:F | Select-String -Pattern "app_credivnzl|mono|Exception|FATAL|AndroidRuntime" -Context 2,2
