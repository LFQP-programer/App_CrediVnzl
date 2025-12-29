$files = Get-ChildItem "C:\Proyectos\App_CrediVnzl\Pages\*.xaml"

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $original = $content
    
    # Reemplazar caracteres con tildes mal codificados
    $content = $content -replace 'á', 'a'
    $content = $content -replace 'é', 'e'
    $content = $content -replace 'í', 'i'
    $content = $content -replace 'ó', 'o'
    $content = $content -replace 'ú', 'u'
    $content = $content -replace 'Á', 'A'
    $content = $content -replace 'É', 'E'
    $content = $content -replace 'Í', 'I'
    $content = $content -replace 'Ó', 'O'
    $content = $content -replace 'Ú', 'U'
    $content = $content -replace 'ñ', 'n'
    $content = $content -replace 'Ñ', 'N'
    
    # Reemplazar caracteres especiales HTML mal codificados
    $content = $content -replace '&#x1F4CB;', '??'
    $content = $content -replace '&#x26A0;&#xFE0F;', '??'
    
    # Eliminar emojis que se muestran como ??
    $content = $content -replace '\?\?', ''
    
    # Asegurarse de que el encoding sea correcto
    if ($content -ne $original) {
        $utf8NoBom = New-Object System.Text.UTF8Encoding $false
        [System.IO.File]::WriteAllText($file.FullName, $content, $utf8NoBom)
        Write-Host "Corregido: $($file.Name)"
    }
}

Write-Host "`nLimpieza completada!"
Write-Host "Archivos procesados: $($files.Count)"
