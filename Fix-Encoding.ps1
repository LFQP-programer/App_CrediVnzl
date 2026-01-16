# Script para corregir codificación UTF-8 en toda la aplicación

function Fix-FileEncoding {
    param(
        [string]$FilePath
    )
    
    try {
        $content = Get-Content -Path $FilePath -Raw -Encoding UTF8
        
        if ($content.Contains("?")) {
            Write-Host "Procesando: $FilePath" -ForegroundColor Yellow
            
            # Realizar todos los reemplazos usando -replace con [regex]::Escape cuando sea necesario
            $content = $content.Replace("configuraci?n", "configuración")
            $content = $content.Replace("Configuraci?n", "Configuración")
            $content = $content.Replace("administraci?n", "administración")
            $content = $content.Replace("Administraci?n", "Administración")
            $content = $content.Replace("contrase?a", "contraseña")
            $content = $content.Replace("Contrase?a", "Contraseña")
            $content = $content.Replace("pr?stamo", "préstamo")
            $content = $content.Replace("Pr?stamo", "Préstamo")
            $content = $content.Replace("pr?stamos", "préstamos")
            $content = $content.Replace("Pr?stamos", "Préstamos")
            $content = $content.Replace("a?os", "años")
            $content = $content.Replace("A?os", "Años")
            $content = $content.Replace("informaci?n", "información")
            $content = $content.Replace("Informaci?n", "Información")
            $content = $content.Replace("dise?o", "diseño")
            $content = $content.Replace("Dise?o", "Diseño")
            $content = $content.Replace("tel?fono", "teléfono")
            $content = $content.Replace("Tel?fono", "Teléfono")
            $content = $content.Replace("n?mero", "número")
            $content = $content.Replace("N?mero", "Número")
            $content = $content.Replace("?ltimo", "último")
            $content = $content.Replace("?ltima", "última")
            $content = $content.Replace("?ltimos", "últimos")
            $content = $content.Replace("?ltimas", "últimas")
            $content = $content.Replace("estad?sticas", "estadísticas")
            $content = $content.Replace("Estad?sticas", "Estadísticas")
            $content = $content.Replace("c?digo", "código")
            $content = $content.Replace("C?digo", "Código")
            $content = $content.Replace("?nico", "único")
            $content = $content.Replace("?nica", "única")
            $content = $content.Replace("d?a", "día")
            $content = $content.Replace("D?a", "Día")
            $content = $content.Replace("d?as", "días")
            $content = $content.Replace("D?as", "Días")
            $content = $content.Replace("per?odo", "período")
            $content = $content.Replace("Per?odo", "Período")
            $content = $content.Replace("par?metro", "parámetro")
            $content = $content.Replace("Par?metro", "Parámetro")
            $content = $content.Replace("m?todo", "método")
            $content = $content.Replace("M?todo", "Método")
            $content = $content.Replace("cr?dito", "crédito")
            $content = $content.Replace("Cr?dito", "Crédito")
            $content = $content.Replace("hist?rico", "histórico")
            $content = $content.Replace("Hist?rico", "Histórico")
            $content = $content.Replace("m?s", "más")
            $content = $content.Replace("M?s", "Más")
            $content = $content.Replace("despu?s", "después")
            $content = $content.Replace("Despu?s", "Después")
            $content = $content.Replace("pa?s", "país")
            $content = $content.Replace("Pa?s", "País")
            $content = $content.Replace("extranjer?a", "extranjería")
            $content = $content.Replace("Extranjer?a", "Extranjería")
            $content = $content.Replace("observaci?n", "observación")
            $content = $content.Replace("Observaci?n", "Observación")
            $content = $content.Replace("generaci?n", "generación")
            $content = $content.Replace("Generaci?n", "Generación")
            $content = $content.Replace("autenticaci?n", "autenticación")
            $content = $content.Replace("Autenticaci?n", "Autenticación")
            $content = $content.Replace("verificaci?n", "verificación")
            $content = $content.Replace("Verificaci?n", "Verificación")
            $content = $content.Replace("notificaci?n", "notificación")
            $content = $content.Replace("Notificaci?n", "Notificación")
            $content = $content.Replace("direcci?n", "dirección")
            $content = $content.Replace("Direcci?n", "Dirección")
            $content = $content.Replace("operaci?n", "operación")
            $content = $content.Replace("Operaci?n", "Operación")
            $content = $content.Replace("creaci?n", "creación")
            $content = $content.Replace("Creaci?n", "Creación")
            $content = $content.Replace("sesi?n", "sesión")
            $content = $content.Replace("Sesi?n", "Sesión")
            $content = $content.Replace("versi?n", "versión")
            $content = $content.Replace("Versi?n", "Versión")
            $content = $content.Replace("excepci?n", "excepción")
            $content = $content.Replace("Excepci?n", "Excepción")
            $content = $content.Replace("extensi?n", "extensión")
            $content = $content.Replace("Extensi?n", "Extensión")
            $content = $content.Replace("aplicaci?n", "aplicación")
            $content = $content.Replace("Aplicaci?n", "Aplicación")
            $content = $content.Replace("navegaci?n", "navegación")
            $content = $content.Replace("Navegaci?n", "Navegación")
            $content = $content.Replace("duraci?n", "duración")
            $content = $content.Replace("Duraci?n", "Duración")
            $content = $content.Replace("inter?s", "interés")
            $content = $content.Replace("Inter?s", "Interés")
            $content = $content.Replace("adem?s", "además")
            $content = $content.Replace("Adem?s", "Además")
            $content = $content.Replace("tambi?n", "también")
            $content = $content.Replace("Tambi?n", "También")
            $content = $content.Replace("est?", "está")
            $content = $content.Replace("Est?", "Está")
            $content = $content.Replace("ser?", "será")
            $content = $content.Replace("Ser?", "Será")
            $content = $content.Replace("hab?a", "había")
            $content = $content.Replace("Hab?a", "Había")
            $content = $content.Replace("tendr?", "tendrá")
            $content = $content.Replace("Tendr?", "Tendrá")
            $content = $content.Replace("v?lido", "válido")
            $content = $content.Replace("V?lido", "Válido")
            $content = $content.Replace("inv?lido", "inválido")
            $content = $content.Replace("Inv?lido", "Inválido")
            $content = $content.Replace("p?gina", "página")
            $content = $content.Replace("P?gina", "Página")
            $content = $content.Replace("p?ginas", "páginas")
            $content = $content.Replace("P?ginas", "Páginas")
            $content = $content.Replace("t?tulo", "título")
            $content = $content.Replace("T?tulo", "Título")
            $content = $content.Replace("c?lculo", "cálculo")
            $content = $content.Replace("C?lculo", "Cálculo")
            $content = $content.Replace("qu?", "qué")
            $content = $content.Replace("Qu?", "Qué")
            $content = $content.Replace("c?mo", "cómo")
            $content = $content.Replace("C?mo", "Cómo")
            $content = $content.Replace("d?nde", "dónde")
            $content = $content.Replace("D?nde", "Dónde")
            $content = $content.Replace("cu?ndo", "cuándo")
            $content = $content.Replace("Cu?ndo", "Cuándo")
            $content = $content.Replace("cu?nto", "cuánto")
            $content = $content.Replace("Cu?nto", "Cuánto")
            $content = $content.Replace("c?dula", "cédula")
            $content = $content.Replace("C?dula", "Cédula")
            $content = $content.Replace("?xito", "éxito")
            $content = $content.Replace("?xito", "Éxito")
            $content = $content.Replace("cr?ditos", "créditos")
            $content = $content.Replace("Cr?ditos", "Créditos")
            
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

Write-Host "=== Corrección Masiva de Codificación UTF-8 ===" -ForegroundColor Cyan
Write-Host ""

$rootPath = "C:\Proyectos\App_CrediVnzl"
$extensions = @("*.cs", "*.xaml")
$filesFixed = 0
$filesProcessed = 0

foreach ($ext in $extensions) {
    $files = Get-ChildItem -Path $rootPath -Filter $ext -Recurse -File | Where-Object { $_.FullName -notlike "*\obj\*" -and $_.FullName -notlike "*\bin\*" }
    
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
