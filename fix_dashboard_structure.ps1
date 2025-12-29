# Script para corregir DashboardPage.xaml
$filePath = "C:\Proyectos\App_CrediVnzl\Pages\DashboardPage.xaml"
$content = Get-Content $filePath -Raw

# 1. Reemplazar la apertura después de Shell.NavBarIsVisible
$content = $content -replace '(Shell\.NavBarIsVisible="False">)\s*<ScrollView>', '$1`n`n    <Grid>`n        <ScrollView ZIndex="1">'

# 2. Encontrar y eliminar los overlays y popups duplicados dentro del ScrollView
# Eliminar OverlayCapital
$content = $content -replace '(?s)<!-- Ventana Flotante de Capital -->.*?<!-- Overlay oscuro para Capital -->.*?<BoxView x:Name="OverlayCapital".*?</BoxView>\s*<Grid x:Name="PopupCapital".*?</Grid>\s*<!-- Ventana Flotante de Ganancias -->', '<!-- Ventana Flotante de Ganancias -->'

# Eliminar OverlayGanancias  
$content = $content -replace '(?s)<!-- Ventana Flotante de Ganancias -->.*?<!-- Overlay oscuro para Ganancias -->.*?<BoxView x:Name="OverlayGanancias".*?</BoxView>\s*<Grid x:Name="PopupGanancias".*?</Grid>\s*<!-- Menu Cards Grid -->', '<!-- Menu Cards Grid -->'

# 3. Antes del cierre de ScrollView y ContentPage, agregar cierre de Grid
$content = $content -replace '(</VerticalStackLayout>\s*</ScrollView>)\s*(</ContentPage>)', '$1`n    </Grid>$2'

# Guardar el archivo
$utf8NoBom = New-Object System.Text.UTF8Encoding $false
[System.IO.File]::WriteAllText($filePath, $content, $utf8NoBom)

Write-Host "Archivo corregido exitosamente!"
