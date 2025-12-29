$files = Get-ChildItem "C:\Proyectos\App_CrediVnzl\Pages\*.xaml"

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $original = $content
    
    # Reemplazar tildes y caracteres especiales
    $content = $content -replace 'é', 'e'
    $content = $content -replace 'ó', 'o'
    $content = $content -replace 'á', 'a'
    $content = $content -replace 'í', 'i'
    $content = $content -replace 'ú', 'u'
    $content = $content -replace 'ñ', 'n'
    
    # Eliminar emojis comunes
    $content = $content -replace '??', ''
    $content = $content -replace '??', ''
    $content = $content -replace '?', ''
    $content = $content -replace '??', ''
    $content = $content -replace '??', ''
    $content = $content -replace '??', ''
    $content = $content -replace '??', ''
    $content = $content -replace '?', ''
    $content = $content -replace '??', ''
    $content = $content -replace '??', ''
    $content = $content -replace '??', ''
    $content = $content -replace '??', ''
    $content = $content -replace '?', ''
    $content = $content -replace '??', ''
    $content = $content -replace '??', ''
    $content = $content -replace '??', ''
    $content = $content -replace '??', ''
    $content = $content -replace '?', ''
    $content = $content -replace '??', ''
    $content = $content -replace '?', ''
    
    if ($content -ne $original) {
        [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
        Write-Host "Corregido: $($file.Name)"
    }
}

Write-Host "`nLimpieza completada!"
