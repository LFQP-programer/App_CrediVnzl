# ?? SOLUCIÓN - ERROR FormattedText en DashboardPage

## ? ERROR IDENTIFICADO

**Mensaje:** "Cannot assign property 'FormattedText': Property does not exist"

**Ubicación:** `DashboardPage.xaml` línea 723

**Causa:** En .NET MAUI, `Label.FormattedText` y `Button.FormattedText` no funcionan de la misma manera que en Xamarin.Forms.

---

## ? SOLUCIÓN RÁPIDA

### **Opción 1: Eliminar FormattedText (Recomendado)**

Reemplazar todos los usos de `FormattedText` con `HorizontalStackLayout` + iconos SVG.

**ANTES (Con error):**
```xaml
<Label>
    <Label.FormattedText>
        <FormattedString>
            <Span Text="{x:Static helpers:IconHelper.MoneyBag}" />
            <Span Text=" Configurar Capital" />
        </FormattedString>
    </Label.FormattedText>
</Label>
```

**DESPUÉS (Correcto):**
```xaml
<HorizontalStackLayout Spacing="8">
    <Image Source="{x:Static helpers:IconPaths.Money}"
           WidthRequest="20"
           HeightRequest="20"
           VerticalOptions="Center" />
    <Label Text="Configurar Capital"
           FontSize="22"
           FontAttributes="Bold"
           TextColor="White"
           VerticalOptions="Center" />
</HorizontalStackLayout>
```

---

## ??? SCRIPT DE CORRECCIÓN AUTOMÁTICA

Ejecuta este script en PowerShell para corregir automáticamente:

```powershell
$filePath = "C:\Proyectos\App_CrediVnzl\Pages\DashboardPage.xaml"
$content = Get-Content $filePath -Raw

# Eliminar bloques FormattedText
$pattern = '(<Label[^>]*>)\s*<Label\.FormattedText>\s*<FormattedString>.*?</FormattedString>\s*</Label\.FormattedText>'
$content = $content -replace $pattern, '$1'

# Eliminar FormattedText de botones
$pattern2 = '(<Button[^>]*>)\s*<Button\.FormattedText>\s*<FormattedString>.*?</FormattedString>\s*</Button\.FormattedText>'
$content = $content -replace $pattern2, '$1'

# Guardar
[System.IO.File]::WriteAllText($filePath, $content, [System.Text.UTF8Encoding]::new($false))
Write-Host "Corrección completada" -ForegroundColor Green
```

---

## ?? OCURRENCIAS A CORREGIR EN DashboardPage.xaml

### **1. Modal de Capital - Header (Línea ~723)**
```xaml
<!-- ELIMINAR esto -->
<Label.FormattedText>
    <FormattedString>
        <Span Text="{x:Static helpers:IconHelper.MoneyBag}" />
        <Span Text=" Configurar Capital" />
    </FormattedString>
</Label.FormattedText>

<!-- REEMPLAZAR con -->
<HorizontalStackLayout Spacing="8">
    <Image Source="{x:Static helpers:IconPaths.Money}" WidthRequest="20" HeightRequest="20" />
    <Label Text="Configurar Capital" FontSize="22" FontAttributes="Bold" TextColor="White" />
</HorizontalStackLayout>
```

### **2. Modal de Capital - Info (Línea ~780)**
```xaml
<!-- ELIMINAR FormattedText -->
<!-- USAR Label simple -->
<Label Text="?? Ingresa el monto total..." FontSize="12" />
```

### **3. Modal de Capital - Botones (Líneas ~840-860)**
```xaml
<!-- ELIMINAR FormattedText de ambos botones -->
<!-- USAR ImageSource + Text -->
<Button ImageSource="{x:Static helpers:IconPaths.Exit}"
        Text=" Cancelar" />
        
<Button ImageSource="{x:Static helpers:IconPaths.Save}"
        Text=" Guardar" />
```

### **4. Modal de Ganancias - Header (Línea ~900)**
```xaml
<!-- ELIMINAR FormattedText -->
<HorizontalStackLayout Spacing="8">
    <Image Source="{x:Static helpers:IconPaths.Chart}" WidthRequest="20" HeightRequest="20" />
    <Label Text="Resumen de Ganancias" FontSize="22" FontAttributes="Bold" TextColor="White" />
</HorizontalStackLayout>
```

### **5. Modal de Ganancias - Botón (Línea ~950)**
```xaml
<!-- ELIMINAR FormattedText -->
<Button ImageSource="{x:Static helpers:IconPaths.Check}"
        Text=" Cerrar" />
```

---

## ?? SOLUCIÓN MANUAL PASO A PASO

### **Paso 1: Abrir DashboardPage.xaml**

### **Paso 2: Buscar "FormattedText"**
Usar Ctrl+F para buscar todas las ocurrencias

### **Paso 3: Por cada ocurrencia:**

**Si es un Label:**
- Eliminar `<Label.FormattedText>...` hasta `</Label.FormattedText>`
- Reemplazar con `<HorizontalStackLayout>` con Image + Label

**Si es un Button:**
- Eliminar `<Button.FormattedText>...` hasta `</Button.FormattedText>`
- Usar `ImageSource` + `Text` en el Button directamente

### **Paso 4: Compilar**
```
Build ? Rebuild Solution
```

---

## ? ALTERNATIVA SIMPLE

Si quieres una solución rápida, simplemente **elimina** todos los bloques `FormattedText` y deja los Labels/Buttons con texto simple:

```xaml
<!-- En lugar de texto con icono, solo texto -->
<Label Text="Configurar Capital" />
<Button Text="Cancelar" />
<Button Text="Guardar" />
```

---

## ?? RESULTADO ESPERADO

Después de la corrección:
- ? No más errores de `FormattedText`
- ? DashboardPage compila correctamente
- ? La app no se cierra al intentar abrir el dashboard

---

## ?? SI EL ERROR PERSISTE

1. Limpiar solución
2. Rebuild
3. Verificar que NO haya ningún `FormattedText` en el archivo
4. Si sigue fallando, compartir el mensaje de error completo

---

**Estado:** ?? Corrección Necesaria  
**Prioridad:** Alta  
**Tiempo:** 10-15 minutos

¡Este es el último error antes de que funcione!** ??
