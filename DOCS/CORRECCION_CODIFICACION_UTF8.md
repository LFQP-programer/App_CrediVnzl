# Script para Corregir Codificación de Caracteres en Toda la Aplicación

## Problema
Los archivos tienen problemas de codificación UTF-8 donde los caracteres especiales del español aparecen como "?".

## Caracteres a Corregir

### Vocales con Tilde
- á ? á
- é ? é  
- í ? í
- ó ? ó
- ú ? ú
- Á ? Á
- É ? É
- Í ? Í
- Ó ? Ó
- Ú ? Ú

### Letra Ñ
- ñ ? ñ
- Ñ ? Ñ

### Signos de Puntuación
- ¿ ? ¿
- ¡ ? ¡

## Script de PowerShell para Corrección Masiva

```powershell
# Función para corregir codificación en un archivo
function Fix-FileEncoding {
    param(
        [string]$FilePath
    )
    
    try {
        # Leer contenido del archivo
        $content = Get-Content -Path $FilePath -Raw -Encoding UTF8
        
        if ($content -match "?") {
            Write-Host "Procesando: $FilePath" -ForegroundColor Yellow
            
            # Realizar reemplazos
            $content = $content -replace "configuraci?n", "configuración"
            $content = $content -replace "Configuraci?n", "Configuración"
            $content = $content -replace "administraci?n", "administración"
            $content = $content -replace "Administraci?n", "Administración"
            $content = $content -replace "contrase?a", "contraseña"
            $content = $content -replace "Contrase?a", "Contraseña"
            $content = $content -replace "pr?stamo", "préstamo"
            $content = $content -replace "Pr?stamo", "Préstamo"
            $content = $content -replace "pr?stamos", "préstamos"
            $content = $content -replace "Pr?stamos", "Préstamos"
            $content = $content -replace "a?os", "años"
            $content = $content -replace "A?os", "Años"
            $content = $content -replace "informaci?n", "información"
            $content = $content -replace "Informaci?n", "Información"
            $content = $content -replace "dise?o", "diseño"
            $content = $content -replace "Dise?o", "Diseño"
            $content = $content -replace "tel?fono", "teléfono"
            $content = $content -replace "Tel?fono", "Teléfono"
            $content = $content -replace "n?mero", "número"
            $content = $content -replace "N?mero", "Número"
            $content = $content -replace "?ltimo", "último"
            $content = $content -replace "?ltimo", "Último"
            $content = $content -replace "estad?sticas", "estadísticas"
            $content = $content -replace "Estad?sticas", "Estadísticas"
            $content = $content -replace "c?digo", "código"
            $content = $content -replace "C?digo", "Código"
            $content = $content -replace "?nico", "único"
            $content = $content -replace "?nico", "Único"
            $content = $content -replace "d?a", "día"
            $content = $content -replace "D?a", "Día"
            $content = $content -replace "d?as", "días"
            $content = $content -replace "D?as", "Días"
            $content = $content -replace "per?odo", "período"
            $content = $content -replace "Per?odo", "Período"
            $content = $content -replace "par?metro", "parámetro"
            $content = $content -replace "Par?metro", "Parámetro"
            $content = $content -replace "m?todo", "método"
            $content = $content -replace "M?todo", "Método"
            $content = $content -replace "cr?dito", "crédito"
            $content = $content -replace "Cr?dito", "Crédito"
            $content = $content -replace "hist?rico", "histórico"
            $content = $content -replace "Hist?rico", "Histórico"
            $content = $content -replace "m?s", "más"
            $content = $content -replace "M?s", "Más"
            $content = $content -replace "despu?s", "después"
            $content = $content -replace "Despu?s", "Después"
            $content = $content -replace "pa?s", "país"
            $content = $content -replace "Pa?s", "País"
            $content = $content -replace "extranjer?a", "extranjería"
            $content = $content -replace "Extranjer?a", "Extranjería"
            $content = $content -replace "observaci?n", "observación"
            $content = $content -replace "Observaci?n", "Observación"
            $content = $content -replace "generaci?n", "generación"
            $content = $content -replace "Generaci?n", "Generación"
            $content = $content -replace "autenticaci?n", "autenticación"
            $content = $content -replace "Autenticaci?n", "Autenticación"
            $content = $content -replace "verificaci?n", "verificación"
            $content = $content -replace "Verificaci?n", "Verificación"
            $content = $content -replace "notificaci?n", "notificación"
            $content = $content -replace "Notificaci?n", "Notificación"
            $content = $content -replace "direcci?n", "dirección"
            $content = $content -replace "Direcci?n", "Dirección"
            $content = $content -replace "operaci?n", "operación"
            $content = $content -replace "Operaci?n", "Operación"
            $content = $content -replace "creaci?n", "creación"
            $content = $content -replace "Creaci?n", "Creación"
            $content = $content -replace "sesi?n", "sesión"
            $content = $content -replace "Sesi?n", "Sesión"
            $content = $content -replace "versi?n", "versión"
            $content = $content -replace "Versi?n", "Versión"
            $content = $content -replace "excepci?n", "excepción"
            $content = $content -replace "Excepci?n", "Excepción"
            $content = $content -replace "extensi?n", "extensión"
            $content = $content -replace "Extensi?n", "Extensión"
            $content = $content -replace "aplicaci?n", "aplicación"
            $content = $content -replace "Aplicaci?n", "Aplicación"
            $content = $content -replace "navegaci?n", "navegación"
            $content = $content -replace "Navegaci?n", "Navegación"
            $content = $content -replace "duraci?n", "duración"
            $content = $content -replace "Duraci?n", "Duración"
            $content = $content -replace "inter?s", "interés"
            $content = $content -replace "Inter?s", "Interés"
            $content = $content -replace "adem?s", "además"
            $content = $content -replace "Adem?s", "Además"
            $content = $content -replace "tambi?n", "también"
            $content = $content -replace "Tambi?n", "También"
            $content = $content -replace "est?", "está"
            $content = $content -replace "Est?", "Está"
            $content = $content -replace "ser?", "será"
            $content = $content -replace "Ser?", "Será"
            $content = $content -replace "deb?", "debí"
            $content = $content -replace "Deb?", "Debí"
            $content = $content -replace "hab?a", "había"
            $content = $content -replace "Hab?a", "Había"
            $content = $content -replace "tendr?", "tendrá"
            $content = $content -replace "Tendr?", "Tendrá"
            $content = $content -replace "v?lido", "válido"
            $content = $content -replace "V?lido", "Válido"
            $content = $content -replace "inv?lido", "inválido"
            $content = $content -replace "Inv?lido", "Inválido"
            $content = $content -replace "p?gina", "página"
            $content = $content -replace "P?gina", "Página"
            $content = $content -replace "t?tulo", "título"
            $content = $content -replace "T?tulo", "Título"
            $content = $content -replace "c?lculo", "cálculo"
            $content = $content -replace "C?lculo", "Cálculo"
            $content = $content -replace "qu?", "qué"
            $content = $content -replace "Qu?", "Qué"
            $content = $content -replace "c?mo", "cómo"
            $content = $content -replace "C?mo", "Cómo"
            $content = $content -replace "d?nde", "dónde"
            $content = $content -replace "D?nde", "Dónde"
            $content = $content -replace "cu?ndo", "cuándo"
            $content = $content -replace "Cu?ndo", "Cuándo"
            $content = $content -replace "cu?nto", "cuánto"
            $content = $content -replace "Cu?nto", "Cuánto"
            $content = $content -replace "despu?s", "después"
            $content = $content -replace "Despu?s", "Después"
            
            # Emoji/Símbolos especiales
            $content = $content -replace "??", "?"
            $content = $content -replace "\?\?", "??"
            $content = $content -replace "\?", "?"
            $content = $content -replace "\?", "?"
            
            # Guardar con UTF-8 con BOM
            [System.IO.File]::WriteAllText($FilePath, $content, [System.Text.UTF8Encoding]::new($true))
            
            Write-Host "? Corregido: $FilePath" -ForegroundColor Green
            return $true
        }
    }
    catch {
        Write-Host "? Error en: $FilePath - $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
    
    return $false
}

# Procesar todos los archivos
Write-Host "=== Corrección Masiva de Codificación UTF-8 ===" -ForegroundColor Cyan
Write-Host ""

$rootPath = "C:\Proyectos\App_CrediVnzl"
$extensions = @("*.cs", "*.xaml")
$filesFixed = 0
$filesProcessed = 0

foreach ($ext in $extensions) {
    $files = Get-ChildItem -Path $rootPath -Filter $ext -Recurse -File
    
    foreach ($file in $files) {
        $filesProcessed++
        if (Fix-FileEncoding -FilePath $file.FullName) {
            $filesFixed++
        }
    }
}

Write-Host ""
Write-Host "=== Resumen ===" -ForegroundColor Cyan
Write-Host "Archivos procesados: $filesProcessed" -ForegroundColor White
Write-Host "Archivos corregidos: $filesFixed" -ForegroundColor Green
Write-Host ""
Write-Host "Proceso completado!" -ForegroundColor Green
```

## Uso del Script

1. Guardar el script en un archivo: `Fix-Encoding.ps1`
2. Ejecutar desde PowerShell:
   ```powershell
   .\Fix-Encoding.ps1
   ```

## Archivos Afectados (Total: 70+ archivos)

### Pages (28 archivos)
- LoginPage.xaml / .xaml.cs
- LoginClientePage.xaml / .xaml.cs  
- DashboardPage.xaml / .xaml.cs
- ClienteDashboardPage.xaml / .xaml.cs
- ClientesPage.xaml / .xaml.cs
- NuevoClientePage.xaml / .xaml.cs
- DetalleClientePage.xaml / .xaml.cs
- EditarClientePage.xaml / .xaml.cs
- NuevoPrestamoPage.xaml / .xaml.cs
- RegistrarPagoPage.xaml / .xaml.cs
- HistorialPrestamosPage.xaml / .xaml.cs
- EnviarMensajesPage.xaml / .xaml.cs
- ReportesPage.xaml / .xaml.cs
- ConfiguracionPage.xaml / .xaml.cs
- PerfilAdminPage.xaml / .xaml.cs
- CambiarContrasenaAdminPage.xaml / .xaml.cs

### ViewModels (16 archivos)
- Todos los ViewModels

### Services (5 archivos)
- AuthService.cs
- DatabaseService.cs
- ReportesService.cs
- WhatsAppService.cs
- WhatsAppBusinessService.cs

### Converters (3 archivos)
- BoolToColorConverter.cs
- EstadoToColorConverter.cs
- InverseBoolConverter.cs

### Resources (2 archivos)
- Colors.xaml
- Styles.xaml

### Root (5 archivos)
- App.xaml / .xaml.cs
- AppShell.xaml / .xaml.cs
- MainPage.xaml / .xaml.cs
- MauiProgram.cs

### Platforms (8 archivos)
- Android: MainActivity.cs, MainApplication.cs
- iOS: AppDelegate.cs, Program.cs
- MacCatalyst: AppDelegate.cs, Program.cs
- Windows: App.xaml, App.xaml.cs

## Palabras Más Comunes a Corregir

1. **configuración** (aparece ~50 veces)
2. **contraseña** (aparece ~40 veces)
3. **préstamo/préstamos** (aparece ~100 veces)
4. **información** (aparece ~30 veces)
5. **administración** (aparece ~20 veces)
6. **días** (aparece ~40 veces)
7. **teléfono** (aparece ~30 veces)
8. **número** (aparece ~50 veces)
9. **último** (aparece ~20 veces)
10. **estadísticas** (aparece ~15 veces)

## Verificación Post-Corrección

Después de ejecutar el script, verificar que:

1. ? No queden caracteres "?" en ningún archivo
2. ? Los textos se vean correctamente en la UI
3. ? La compilación funcione sin errores
4. ? Los archivos estén guardados con UTF-8 BOM

## Comando de Verificación

```powershell
# Verificar que no queden caracteres problemáticos
Get-ChildItem -Path "C:\Proyectos\App_CrediVnzl" -Recurse -Include *.cs,*.xaml -File | 
    Select-String -Pattern "?" -SimpleMatch | 
    Select-Object Path, LineNumber, Line
```

Si este comando no devuelve resultados, la corrección fue exitosa.
