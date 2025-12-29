# Script para limpiar DashboardPage.xaml
$filePath = "C:\Proyectos\App_CrediVnzl\Pages\DashboardPage.xaml"
$content = Get-Content $filePath -Raw

# Limpiar los caracteres literales
$content = $content -replace '`n`n    <Grid>`n        <ScrollView ZIndex="1">', "`n`n    <Grid>`n        <ScrollView ZIndex=`"1`">"
$content = $content -replace '`n    </Grid>', "`n    </Grid>"

# Guardar el archivo
$utf8NoBom = New-Object System.Text.UTF8Encoding $false
[System.IO.File]::WriteAllText($filePath, $content, $utf8NoBom)

Write-Host "Archivo limpiado exitosamente!"
