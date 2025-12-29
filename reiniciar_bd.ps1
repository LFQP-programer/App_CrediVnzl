# Script PowerShell para Reiniciar Base de Datos - CrediVnzl
# Uso: .\reiniciar_bd.ps1 -Platform [Android|Windows|All]

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('Android','Windows','All')]
    [string]$Platform = 'All'
)

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  REINICIAR BASE DE DATOS - CrediVnzl" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

function Reset-AndroidDatabase {
    Write-Host "Reiniciando base de datos en Android..." -ForegroundColor Yellow
    
    # Verificar si ADB está disponible
    $adb = Get-Command adb -ErrorAction SilentlyContinue
    if (-not $adb) {
        Write-Host "[ERROR] ADB no encontrado. Instale Android SDK Platform Tools." -ForegroundColor Red
        return $false
    }
    
    # Verificar dispositivos conectados
    $devices = adb devices
    if ($devices -like "*device*") {
        Write-Host "[OK] Dispositivo Android conectado" -ForegroundColor Green
        
        # Package name de la app
        $packageName = "com.companyname.app_credivnzl"
        $dbPath = "/data/data/$packageName/files/prestafacil.db3"
        
        Write-Host "Eliminando base de datos..." -ForegroundColor Yellow
        
        # Eliminar el archivo
        $result = adb shell "run-as $packageName rm $dbPath 2>&1"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[EXITO] Base de datos eliminada en Android" -ForegroundColor Green
            Write-Host "La app recreara la BD automaticamente al iniciar" -ForegroundColor Cyan
            return $true
        } else {
            Write-Host "[ERROR] No se pudo eliminar la base de datos" -ForegroundColor Red
            Write-Host "Resultado: $result" -ForegroundColor Red
            return $false
        }
    } else {
        Write-Host "[ERROR] No hay dispositivos Android conectados" -ForegroundColor Red
        Write-Host "Conecte un dispositivo o inicie un emulador" -ForegroundColor Yellow
        return $false
    }
}

function Reset-WindowsDatabase {
    Write-Host "Reiniciando base de datos en Windows..." -ForegroundColor Yellow
    
    # Buscar el archivo de base de datos
    $packagesPath = "$env:LOCALAPPDATA\Packages"
    
    if (Test-Path $packagesPath) {
        Write-Host "Buscando base de datos..." -ForegroundColor Yellow
        
        # Buscar carpeta de la app
        $appFolders = Get-ChildItem -Path $packagesPath -Directory | Where-Object { $_.Name -like "*CrediVnzl*" -or $_.Name -like "*App_CrediVnzl*" }
        
        if ($appFolders) {
            foreach ($folder in $appFolders) {
                $dbPath = Join-Path $folder.FullName "LocalState\prestafacil.db3"
                
                if (Test-Path $dbPath) {
                    Write-Host "Encontrada: $dbPath" -ForegroundColor Green
                    Write-Host "Eliminando..." -ForegroundColor Yellow
                    
                    try {
                        Remove-Item -Path $dbPath -Force
                        Write-Host "[EXITO] Base de datos eliminada en Windows" -ForegroundColor Green
                        Write-Host "La app recreara la BD automaticamente al iniciar" -ForegroundColor Cyan
                        return $true
                    }
                    catch {
                        Write-Host "[ERROR] No se pudo eliminar la base de datos" -ForegroundColor Red
                        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
                        Write-Host "Asegurese de que la app este cerrada" -ForegroundColor Yellow
                        return $false
                    }
                }
            }
            
            Write-Host "[ADVERTENCIA] No se encontro el archivo de base de datos" -ForegroundColor Yellow
            Write-Host "Es posible que la app no se haya ejecutado aun" -ForegroundColor Yellow
            return $false
        }
        else {
            Write-Host "[ADVERTENCIA] No se encontro la carpeta de la app" -ForegroundColor Yellow
            Write-Host "Es posible que la app no este instalada" -ForegroundColor Yellow
            return $false
        }
    }
    else {
        Write-Host "[ERROR] No se encontro la carpeta Packages" -ForegroundColor Red
        return $false
    }
}

function Show-DatabaseInfo {
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host "  INFORMACION DE LA BASE DE DATOS" -ForegroundColor Cyan
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Nombre:           prestafacil.db3" -ForegroundColor White
    Write-Host "Tablas:           4 (Cliente, Prestamo, Pago, HistorialPago)" -ForegroundColor White
    Write-Host "Tamano inicial:   ~4-12 KB" -ForegroundColor White
    Write-Host ""
    Write-Host "Android:          /data/data/com.companyname.app_credivnzl/files/" -ForegroundColor Gray
    Write-Host "Windows:          %LOCALAPPDATA%\Packages\[App]\LocalState\" -ForegroundColor Gray
    Write-Host ""
}

function Confirm-Action {
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Yellow
    Write-Host "  ADVERTENCIA: Esta accion es IRREVERSIBLE" -ForegroundColor Yellow
    Write-Host "================================================" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Se eliminara PERMANENTEMENTE:" -ForegroundColor Red
    Write-Host "  - Todos los clientes" -ForegroundColor Red
    Write-Host "  - Todos los prestamos" -ForegroundColor Red
    Write-Host "  - Todos los pagos" -ForegroundColor Red
    Write-Host "  - Todo el historial" -ForegroundColor Red
    Write-Host ""
    
    $confirmation = Read-Host "Escriba 'SI' para continuar o 'NO' para cancelar"
    
    if ($confirmation -eq 'SI') {
        return $true
    }
    else {
        Write-Host ""
        Write-Host "Operacion cancelada" -ForegroundColor Yellow
        return $false
    }
}

# ====================================
# EJECUCION PRINCIPAL
# ====================================

Show-DatabaseInfo

if (-not (Confirm-Action)) {
    exit
}

Write-Host ""
Write-Host "Iniciando proceso de reinicio..." -ForegroundColor Cyan
Write-Host ""

$success = $false

switch ($Platform) {
    'Android' {
        $success = Reset-AndroidDatabase
    }
    'Windows' {
        $success = Reset-WindowsDatabase
    }
    'All' {
        Write-Host "Reiniciando en todas las plataformas disponibles..." -ForegroundColor Cyan
        Write-Host ""
        
        $androidSuccess = Reset-AndroidDatabase
        Write-Host ""
        $windowsSuccess = Reset-WindowsDatabase
        
        $success = $androidSuccess -or $windowsSuccess
    }
}

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan

if ($success) {
    Write-Host "  PROCESO COMPLETADO EXITOSAMENTE" -ForegroundColor Green
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "La base de datos ha sido reiniciada." -ForegroundColor Green
    Write-Host "Al abrir la app, se creara una BD nueva y vacia." -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Siguiente paso:" -ForegroundColor Yellow
    Write-Host "  1. Abrir la aplicacion" -ForegroundColor White
    Write-Host "  2. Verificar que no hay clientes" -ForegroundColor White
    Write-Host "  3. Comenzar a usar con datos limpios" -ForegroundColor White
}
else {
    Write-Host "  PROCESO FINALIZADO CON ERRORES" -ForegroundColor Red
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "No se pudo completar el reinicio." -ForegroundColor Red
    Write-Host "Revise los mensajes de error anteriores." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Alternativas:" -ForegroundColor Yellow
    Write-Host "  1. Usar la opcion de reinicio desde la app" -ForegroundColor White
    Write-Host "  2. Desinstalar y reinstalar la app" -ForegroundColor White
    Write-Host "  3. Eliminar el archivo manualmente" -ForegroundColor White
}

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Pausar para ver los resultados
Read-Host "Presione Enter para salir"
