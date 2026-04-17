# Script para generar APK de CrediVnzl
# Autor: Sistema de CrediVnzl
# Fecha: 2024

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   GENERADOR DE APK - CrediVnzl" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que estamos en el directorio correcto
if (-not (Test-Path "App_CrediVnzl.csproj")) {
    Write-Host "? Error: No se encuentra el archivo App_CrediVnzl.csproj" -ForegroundColor Red
    Write-Host "   Asegúrate de ejecutar este script en la carpeta del proyecto." -ForegroundColor Yellow
    exit 1
}

# Paso 1: Limpiar
Write-Host "?? Limpiando proyecto..." -ForegroundColor Yellow
dotnet clean -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Error al limpiar el proyecto" -ForegroundColor Red
    exit 1
}
Write-Host "? Proyecto limpio" -ForegroundColor Green
Write-Host ""

# Paso 2: Restaurar paquetes
Write-Host "?? Restaurando paquetes NuGet..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Error al restaurar paquetes" -ForegroundColor Red
    exit 1
}
Write-Host "? Paquetes restaurados" -ForegroundColor Green
Write-Host ""

# Paso 3: Compilar
Write-Host "?? Compilando en modo Release..." -ForegroundColor Yellow
Write-Host "   (Esto puede tardar unos minutos...)" -ForegroundColor Gray
dotnet build -f net10.0-android -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Error al compilar el proyecto" -ForegroundColor Red
    exit 1
}
Write-Host "? Compilación exitosa" -ForegroundColor Green
Write-Host ""

# Paso 4: Publicar APK
Write-Host "?? Generando APK..." -ForegroundColor Yellow
dotnet publish -f net10.0-android -c Release /p:AndroidPackageFormat=apk
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Error al generar el APK" -ForegroundColor Red
    exit 1
}
Write-Host "? APK generado exitosamente" -ForegroundColor Green
Write-Host ""

# Verificar archivos generados
Write-Host "?? Verificando archivos generados..." -ForegroundColor Yellow
$publishPath = "bin\Release\net10.0-android\publish"
$apkSigned = "$publishPath\com.companyname.app_credivnzl-Signed.apk"
$apkUnsigned = "$publishPath\com.companyname.app_credivnzl.apk"

$apkFound = $false

if (Test-Path $apkSigned) {
    Write-Host "? APK Firmado encontrado" -ForegroundColor Green
    Write-Host "   ?? Ubicación: $apkSigned" -ForegroundColor Cyan
    $apkPath = $apkSigned
    $apkFound = $true
    
    # Obtener tamaño del archivo
    $fileSize = (Get-Item $apkSigned).Length
    $fileSizeMB = [math]::Round($fileSize / 1MB, 2)
    Write-Host "   ?? Tamaño: $fileSizeMB MB" -ForegroundColor Cyan
}

if (Test-Path $apkUnsigned) {
    if (-not $apkFound) {
        Write-Host "? APK (sin firmar) encontrado" -ForegroundColor Yellow
        Write-Host "   ?? Ubicación: $apkUnsigned" -ForegroundColor Cyan
        $apkPath = $apkUnsigned
        $apkFound = $true
        
        # Obtener tamaño del archivo
        $fileSize = (Get-Item $apkUnsigned).Length
        $fileSizeMB = [math]::Round($fileSize / 1MB, 2)
        Write-Host "   ?? Tamaño: $fileSizeMB MB" -ForegroundColor Cyan
    }
}

if (-not $apkFound) {
    Write-Host "? No se encontró el APK generado" -ForegroundColor Red
    Write-Host "   Verifica la carpeta: $publishPath" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "   ? GENERACIÓN COMPLETADA ?" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Opciones post-generación
Write-Host "¿Qué deseas hacer ahora?" -ForegroundColor Cyan
Write-Host "1. Abrir carpeta del APK" -ForegroundColor White
Write-Host "2. Copiar APK al escritorio" -ForegroundColor White
Write-Host "3. Instalar en dispositivo conectado (ADB)" -ForegroundColor White
Write-Host "4. Salir" -ForegroundColor White
Write-Host ""

$opcion = Read-Host "Selecciona una opción (1-4)"

switch ($opcion) {
    "1" {
        Write-Host "?? Abriendo carpeta..." -ForegroundColor Yellow
        explorer $publishPath
    }
    "2" {
        $desktopPath = [Environment]::GetFolderPath("Desktop")
        $destino = "$desktopPath\CrediVnzl.apk"
        Copy-Item $apkPath $destino -Force
        Write-Host "? APK copiado al escritorio: $destino" -ForegroundColor Green
        explorer $desktopPath
    }
    "3" {
        Write-Host "?? Verificando dispositivos conectados..." -ForegroundColor Yellow
        adb devices
        Write-Host ""
        $confirm = Read-Host "¿Instalar en dispositivo? (S/N)"
        if ($confirm -eq "S" -or $confirm -eq "s") {
            Write-Host "?? Instalando APK..." -ForegroundColor Yellow
            adb install -r $apkPath
            if ($LASTEXITCODE -eq 0) {
                Write-Host "? APK instalado exitosamente" -ForegroundColor Green
            } else {
                Write-Host "? Error al instalar APK" -ForegroundColor Red
            }
        }
    }
    "4" {
        Write-Host "?? ¡Hasta luego!" -ForegroundColor Cyan
    }
    default {
        Write-Host "Opción no válida" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "?? ¡Proceso finalizado!" -ForegroundColor Green
Write-Host ""
