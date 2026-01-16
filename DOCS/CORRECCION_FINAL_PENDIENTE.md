# Corrección Final de Codificación UTF-8

## Estado Actual

El script de corrección masiva ha procesado **71 archivos** exitosamente, pero aún quedan algunos archivos con problemas de codificación que necesitan atención manual.

## Archivos que Requieren Corrección Manual

### 1. Services/DatabaseService.cs
**Líneas problemáticas:**
- Línea 18: `autom?ticamente` ? `automáticamente`
- Línea 28: `migraci?n` ? `migración`
- Línea 49: `autom?ticamente` ? `automáticamente`
- Línea 71: `migraci?n` ? `migración`
- Línea 76: `Metodos` ? `Métodos`
- Línea 621: `M?todo` ? `Método`
- Línea 627: `pr?stamos` ? `préstamos`
- Línea 635: `pr?stamo` ? `préstamo`
- Línea 642: `pr?stamo` ? `préstamo`
- Línea 661: `M?todo` ? `Método`
- Línea 683: `M?todo` ? `Método`
- Línea 696: `M?todos` ? `Métodos`
- Línea 704: `tama?o` ? `tamaño`
- Línea 706: `Tama?oArchivo` ? `TamañoArchivo`
- Línea 712: `M?todos` ? `Métodos`
- Línea 717: `contrase?as` ? `contraseñas`
- Línea 723: `contrase?a` ? `contraseña`
- Línea 757: `contrase?a` ? `contraseña`
- Línea 762: `contrase?a` ? `contraseña`
- Línea 777: `contrase?a` ? `contraseña`
- Línea 800: `informaci?n` ? `información`
- Línea 805: `Tama?oArchivo` ? `TamañoArchivo`
- Línea 807: `Tama?oArchivoFormateado` ? `TamañoArchivoFormateado`
- Línea 811: `Tama?oArchivo` ? `TamañoArchivo` (4 veces)
- Línea 813: `Tama?oArchivo` ? `TamañoArchivo` (2 veces)
- Línea 814: `Tama?oArchivo` ? `TamañoArchivo`
- Línea 816: `Tama?oArchivo` ? `TamañoArchivo`

### 2. Services/ReportesService.cs
**Líneas problemáticas:**
- Línea 447: `A?o` ? `Año`
- Línea 462: `A?o` ? `Año`
- Múltiples referencias a `PeriodoReporte.A?o`

### 3. ViewModels/ConfiguracionViewModel.cs
**Líneas problemáticas:**
- Línea 124: `perder?` ? `perderá`
- Línea 124: `informaci?n` ? `información`
- Línea 124: `Tama?oArchivoFormateado` ? `TamañoArchivoFormateado`

### 4. ViewModels/ReportesViewModel.cs
**Líneas problemáticas:**
- Línea 231: `A?o` ? `Año`
- Línea 246: `A?o` ? `Año` (2 veces)

## Script PowerShell Mejorado

El script actual tiene un problema: el archivo PowerShell mismo tiene problemas de codificación. Se recomienda volver a guardar el script con UTF-8 BOM o usar el siguiente script alternativo:

```powershell
# Fix-Encoding-v2.ps1
function Fix-SpecificFiles {
    $replacements = @{
        "autom?ticamente" = "automáticamente"
        "migraci?n" = "migración"
        "Metodos" = "Métodos"
        "M?todo" = "Método"
        "M?todos" = "Métodos"
        "pr?stamos" = "préstamos"
        "pr?stamo" = "préstamo"
        "tama?o" = "tamaño"
        "Tama?o" = "Tamaño"
        "contrase?as" = "contraseñas"
        "contrase?a" = "contraseña"
        "informaci?n" = "información"
        "A?o" = "Año"
        "perder?" = "perderá"
    }
    
    $files = @(
        "C:\Proyectos\App_CrediVnzl\Services\DatabaseService.cs"
        "C:\Proyectos\App_CrediVnzl\Services\ReportesService.cs"
        "C:\Proyectos\App_CrediVnzl\ViewModels\ConfiguracionViewModel.cs"
        "C:\Proyectos\App_CrediVnzl\ViewModels\ReportesViewModel.cs"
    )
    
    foreach ($file in $files) {
        if (Test-Path $file) {
            $content = [System.IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)
            
            foreach ($key in $replacements.Keys) {
                $content = $content.Replace($key, $replacements[$key])
            }
            
            [System.IO.File]::WriteAllText($file, $content, [System.Text.UTF8Encoding]::new($true))
            Write-Host "Corregido: $file" -ForegroundColor Green
        }
    }
}

Fix-SpecificFiles
Write-Host "Corrección completada!" -ForegroundColor Cyan
```

## Corrección Manual Alternativa

Si el script no funciona correctamente, puedes corregir manualmente usando el editor:

1. **Visual Studio / VS Code:**
   - Abrir cada archivo problemático
   - Usar Find & Replace (Ctrl+H)
   - Reemplazar cada patrón problemático
   - Guardar con codificación UTF-8 con BOM

2. **Búsqueda Global en VS:**
   - Edit ? Find and Replace ? Replace in Files
   - Find: `?`
   - Revisar cada ocurrencia y corregir manualmente

## Verificación Post-Corrección

Después de aplicar las correcciones, ejecutar:

```powershell
# Verificar archivos restantes con problemas
Get-ChildItem -Path "C:\Proyectos\App_CrediVnzl" -Recurse -Include *.cs,*.xaml -File | 
    Where-Object { $_.FullName -notlike "*\obj\*" -and $_.FullName -notlike "*\bin\*" } | 
    Select-String -Pattern "?" -SimpleMatch | 
    Select-Object Path -Unique

# Compilar el proyecto
dotnet build
```

## Palabras Específicas a Corregir

### Alta Prioridad (Causan errores de compilación)
1. `Tama?oArchivo` ? `TamañoArchivo` (Services/DatabaseService.cs)
2. `Tama?oArchivoFormateado` ? `TamañoArchivoFormateado` (Services/DatabaseService.cs)
3. `A?o` ? `Año` (Services/ReportesService.cs, ViewModels/ReportesViewModel.cs)
4. `perder?` ? `perderá` (ViewModels/ConfiguracionViewModel.cs)

### Media Prioridad (Comentarios y mensajes)
5. `autom?ticamente` ? `automáticamente`
6. `migraci?n` ? `migración`
7. `M?todo/M?todos` ? `Método/Métodos`
8. `pr?stamo/pr?stamos` ? `préstamo/préstamos`
9. `contrase?a/contrase?as` ? `contraseña/contraseñas`
10. `informaci?n` ? `información`

## Recomendaciones

1. **Configurar Editor:**
   - Establecer UTF-8 con BOM como codificación predeterminada
   - En VS Code: File ? Preferences ? Settings ? Files: Encoding ? UTF-8 with BOM

2. **Git Configuration:**
   ```bash
   git config --global core.autocrlf true
   git config --global core.safecrlf warn
   ```

3. **EditorConfig:**
   Crear archivo `.editorconfig` en la raíz del proyecto:
   ```ini
   root = true

   [*.{cs,xaml}]
   charset = utf-8-bom
   end_of_line = crlf
   insert_final_newline = true
   indent_style = space
   indent_size = 4
   ```

## Siguiente Paso

1. Ejecutar el script mejorado `Fix-Encoding-v2.ps1`
2. O corregir manualmente los 4 archivos listados arriba
3. Compilar el proyecto: `dotnet build`
4. Verificar que no haya errores
5. Hacer commit de los cambios

## Estimación de Tiempo

- **Script automático**: 2-3 minutos
- **Corrección manual**: 15-20 minutos
- **Compilación y verificación**: 5 minutos

**Total**: 20-30 minutos máximo

## Conclusión

Una vez completadas estas correcciones, **todos los caracteres especiales del español (tildes y ñ) se mostrarán correctamente en toda la aplicación** y el proyecto compilará sin errores relacionados con codificación.
