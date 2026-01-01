# ?? DOCUMENTACIÓN: FUNCIONALIDAD "RECORDAR SESIÓN"

## ?? DESCRIPCIÓN GENERAL

La funcionalidad "Recordar sesión" permite que los usuarios administradores puedan tener sus datos prellenados en el login, facilitando el acceso sin necesidad de escribir el nombre de usuario cada vez.

**?? IMPORTANTE:** La aplicación **SIEMPRE** inicia en la página de Bienvenida, mostrando las opciones "Administrador" y "Cliente". NO hay login automático.

---

## ?? COMPORTAMIENTO ESPERADO

### ? **CUANDO "RECORDAR SESIÓN" ESTÁ ACTIVADO:**

1. **En el Login:**
   - Usuario marca el CheckBox "Recordar mis datos"
   - Ingresa usuario y contraseña
   - Hace click en "INGRESAR"
   - Sistema guarda el nombre de usuario en Preferences
   - Usuario accede al sistema normalmente

2. **Al Cerrar y Abrir la App:**
   - Sistema va a la página de Bienvenida
   - Usuario selecciona "Administrador"
   - Campo "Usuario" aparece prellenado
   - CheckBox "Recordar mis datos" aparece marcado
   - Usuario solo necesita ingresar contraseña y hacer login

3. **Al Volver al Login:**
   - Campo "Usuario" aparece prellenado
   - CheckBox "Recordar mis datos" aparece marcado
   - Usuario solo necesita ingresar contraseña

### ? **CUANDO "RECORDAR SESIÓN" NO ESTÁ ACTIVADO:**

1. **En el Login:**
   - Usuario NO marca el CheckBox
   - Ingresa usuario y contraseña
   - Hace click en "INGRESAR"
   - Sistema NO guarda nada en Preferences
   - Usuario accede al sistema normalmente

2. **Al Cerrar y Abrir la App:**
   - Sistema va a la página de Bienvenida
   - Usuario selecciona "Administrador"
   - Campos vacíos
   - CheckBox desmarcado
   - Usuario debe ingresar todo manualmente

3. **Al Volver al Login:**
   - Campos vacíos
   - CheckBox desmarcado
   - Usuario debe ingresar todo manualmente

---

## ?? **FLUJO DE CIERRE DE SESIÓN MANUAL**

Cuando el usuario cierra sesión desde el menú hamburguesa:

1. Usuario hace click en "Cerrar Sesión"
2. Sistema muestra confirmación
3. Si confirma:
   - **Se limpian las Preferences** (`recordar_usuario` y `ultimo_usuario`)
   - Se cierra la sesión en AuthService
   - Se navega a página de Bienvenida
4. Próxima vez que abra la app:
   - Va a página de Bienvenida
   - Debe hacer login manualmente

**?? IMPORTANTE:** Cerrar sesión manualmente SIEMPRE limpia las preferences, incluso si el usuario tenía "Recordar sesión" activado. Esto es por seguridad.

---

## ?? **FLUJO DE INICIO DE LA APLICACIÓN**

### **SIEMPRE:**
```
1. App inicia
2. Inicializa base de datos
3. Verifica primer uso
4. Navega a BienvenidaPage
5. Usuario ve 2 opciones:
   - ?? Administrador
   - ?? Cliente
```

**NO HAY LOGIN AUTOMÁTICO** - El usuario siempre debe seleccionar su tipo de acceso e ingresar sus credenciales.

La funcionalidad "Recordar sesión" solo **prellena el campo usuario** en el LoginPage, pero el usuario debe:
- Seleccionar "Administrador" en BienvenidaPage
- Ingresar su contraseña
- Hacer click en "INGRESAR"

---

## ?? **DATOS ALMACENADOS EN PREFERENCES**

La aplicación guarda dos claves en Preferences:

| Clave | Tipo | Descripción |
|-------|------|-------------|
| `recordar_usuario` | `bool` | Indica si el usuario quiere recordar sesión |
| `ultimo_usuario` | `string` | Nombre de usuario guardado |

**?? Seguridad:**
- NO se guarda la contraseña
- Solo se guarda el nombre de usuario
- NO hay login automático
- Usuario siempre debe ingresar contraseña manualmente

---

## ?? **VALIDACIONES DE SEGURIDAD**

### 1. **NO hay login automático:**
La app **SIEMPRE** va a BienvenidaPage al iniciar, sin importar si hay preferences guardadas.

### 2. **Solo prellena datos:**
Si hay preferences guardadas:
- Campo "Usuario" se prellena en LoginAdminPage
- CheckBox "Recordar mis datos" aparece marcado
- Usuario debe ingresar contraseña manualmente

### 3. **Limpieza al cerrar sesión:**
Al cerrar sesión manualmente, se limpian las preferences automáticamente.

---

## ?? **ARCHIVOS INVOLUCRADOS**

### 1. **App.xaml.cs**

**Responsabilidades:**
- Inicializar base de datos
- Verificar primer uso
- Navegar a BienvenidaPage **SIEMPRE**

**Código simplificado (líneas 50-70):**
```csharp
// FLUJO DE INICIO SIMPLIFICADO
// SIEMPRE va a BienvenidaPage, sin login automático
await MainThread.InvokeOnMainThreadAsync(async () =>
{
    System.Diagnostics.Debug.WriteLine("? Navegando a: //bienvenida");
    System.Diagnostics.Debug.WriteLine("? Usuario debe seleccionar Administrador o Cliente");
    
    await Shell.Current.GoToAsync("//bienvenida");
    
    System.Diagnostics.Debug.WriteLine("? Navegación a bienvenida completada");
});
```

---

### 2. **ViewModels/LoginViewModel.cs**

**Responsabilidades:**
- Manejar la propiedad `RecordarMe`
- Guardar/limpiar preferences en el login
- Cargar credenciales guardadas al aparecer la página

**Métodos clave:**
```csharp
// Guardar preferences después del login exitoso
if (RecordarMe)
{
    Preferences.Set("recordar_usuario", true);
    Preferences.Set("ultimo_usuario", NombreUsuario);
}
else
{
    Preferences.Remove("recordar_usuario");
    Preferences.Remove("ultimo_usuario");
}

// Cargar credenciales guardadas
public void CargarCredencialesGuardadas()
{
    bool recordarUsuario = Preferences.Get("recordar_usuario", false);
    
    if (recordarUsuario)
    {
        string ultimoUsuario = Preferences.Get("ultimo_usuario", string.Empty);
        
        if (!string.IsNullOrWhiteSpace(ultimoUsuario))
        {
            NombreUsuario = ultimoUsuario;
            RecordarMe = true;
        }
    }
}
```

---

### 3. **Pages/LoginAdminPage.xaml.cs**

**Responsabilidades:**
- Llamar a `CargarCredencialesGuardadas()` cuando aparece la página

**Código:**
```csharp
protected override void OnAppearing()
{
    base.OnAppearing();
    _viewModel.CargarCredencialesGuardadas();
}
```

---

### 4. **Pages/LoginAdminPage.xaml**

**Responsabilidades:**
- Binding del CheckBox a la propiedad `RecordarMe`

**Código:**
```xaml
<HorizontalStackLayout Spacing="10" Margin="0,10,0,0">
    <CheckBox IsChecked="{Binding RecordarMe}" Color="#7B1FA2"/>
    <Label Text="Recordar mis datos" 
           VerticalOptions="Center"
           TextColor="#757575"
           FontSize="13"/>
</HorizontalStackLayout>
```

---

### 5. **AppShell.xaml.cs**

**Responsabilidades:**
- Limpiar preferences al cerrar sesión manualmente

**Código (líneas 54-56):**
```csharp
// IMPORTANTE: Limpiar preferences de login automático
System.Diagnostics.Debug.WriteLine("*** Limpiando preferences de login automático ***");
Preferences.Remove("recordar_usuario");
Preferences.Remove("ultimo_usuario");
System.Diagnostics.Debug.WriteLine("*** Preferences limpiadas correctamente ***");
```

---

## ?? **ESCENARIOS DE PRUEBA**

### **Escenario 1: Login con "Recordar sesión" ACTIVADO**

**Pasos:**
1. Abrir la app
2. Se ve página de Bienvenida
3. Click en "Administrador"
4. Ingresar: admin / admin123
5. ? Marcar "Recordar mis datos"
6. Click "INGRESAR"
7. Cerrar la app completamente
8. Abrir la app nuevamente

**Resultado Esperado:**
- ? Va a página de Bienvenida (NO login automático)
- ? Usuario selecciona "Administrador"
- ? Campo "Usuario" aparece prellenado con "admin"
- ? CheckBox "Recordar mis datos" está marcado
- ? Usuario solo ingresa contraseña

---

### **Escenario 2: Login con "Recordar sesión" DESACTIVADO**

**Pasos:**
1. Abrir la app
2. Click en "Administrador"
3. Ingresar: admin / admin123
4. ? NO marcar "Recordar mis datos"
5. Click "INGRESAR"
6. Cerrar la app completamente
7. Abrir la app nuevamente

**Resultado Esperado:**
- ? Va a página de Bienvenida
- ? Usuario selecciona "Administrador"
- ? Campos vacíos
- ? CheckBox desmarcado
- ? Debe ingresar todo manualmente

---

### **Escenario 3: Cerrar sesión manualmente (con "Recordar" activado)**

**Pasos:**
1. Login con "Recordar mis datos" ? marcado
2. Acceder al Dashboard
3. Abrir menú hamburguesa (?)
4. Click "Cerrar Sesión"
5. Confirmar
6. Cerrar la app
7. Abrir la app nuevamente

**Resultado Esperado:**
- ? Preferences limpiadas
- ? Va a página de Bienvenida
- ? Usuario selecciona "Administrador"
- ? Campos vacíos
- ? CheckBox desmarcado

---

## ?? **CONSIDERACIONES DE SEGURIDAD**

### ? **Buenas Prácticas Implementadas:**

1. **NO se guarda la contraseña**: Solo el nombre de usuario
2. **NO hay login automático**: Usuario siempre debe seleccionar tipo e ingresar contraseña
3. **Limpieza manual**: Cerrar sesión limpia preferences obligatoriamente
4. **Página de Bienvenida obligatoria**: Usuario siempre ve las opciones al iniciar
5. **Logs detallados**: Para debugging y auditoría

### ?? **Ventajas:**

1. **Mayor seguridad**: No hay acceso automático sin intervención del usuario
2. **Claridad**: Usuario siempre sabe qué tipo de acceso está usando
3. **Control**: Usuario siempre puede elegir entre Administrador y Cliente
4. **Comodidad**: "Recordar sesión" prellena datos pero no compromete seguridad

---

## ?? **LOGS DE DEBUGGING**

La funcionalidad genera logs detallados para facilitar el debugging:

### **Al iniciar la app:**
```
??????????????????????????????????????????????????
?        FLUJO DE INICIO DE APLICACIÓN          ?
??????????????????????????????????????????????????

? Navegando a: //bienvenida
? Usuario debe seleccionar Administrador o Cliente

? Navegación a bienvenida completada

??????????????????????????????????????????????????
?     FLUJO DE INICIO COMPLETADO                 ?
??????????????????????????????????????????????????
```

### **Al cargar credenciales en LoginPage:**
```
?????????????????????????????????????????????????
?  CARGANDO CREDENCIALES GUARDADAS              ?
?????????????????????????????????????????????????
? Recordar usuario: True
? Último usuario guardado: 'admin'
? Usuario y CheckBox cargados correctamente
?????????????????????????????????????????????????
```

### **Al hacer login manual:**
```
*** Login exitoso para: admin (Rol: Admin) ***
*** RecordarMe está: True ***
*** Guardando credenciales en Preferences ***
*** Navegación post-login completada ***
```

### **Al cerrar sesión:**
```
*** Usuario confirmó cerrar sesión ***
*** Sesión cerrada en AuthService ***
*** Limpiando preferences de login automático ***
*** Preferences limpiadas correctamente ***
*** MainPage cambiada exitosamente ***
```

---

## ?? **DIAGRAMA DE FLUJO**

```
???????????????????????????????????????????????????????????????
?                    INICIO DE LA APP                         ?
???????????????????????????????????????????????????????????????
                             ?
                             ?
???????????????????????????????????????????????????????????????
?              SIEMPRE va a BienvenidaPage                    ?
???????????????????????????????????????????????????????????????
                             ?
                             ?
        ??????????????????????????????????????????
        ?  Usuario ve opciones:                  ?
        ?  - ?? Administrador                    ?
        ?  - ?? Cliente                          ?
        ??????????????????????????????????????????
                    ?                ?
     Usuario elige  ?                ?  Usuario elige
     Administrador  ?                ?  Cliente
                    ?                ?
        ????????????????????    ????????????????????
        ? LoginAdminPage   ?    ? LoginClientePage ?
        ????????????????????    ????????????????????
                    ?
                    ?
        ???????????????????????????????????????
        ? ¿Hay preferences guardadas?         ?
        ???????????????????????????????????????
               NO ?              ? SÍ
                  ?              ?
                  ?              ?
   ????????????????????    ????????????????????
   ? Campos vacíos    ?    ? Usuario prellenado?
   ? CheckBox ?       ?    ? CheckBox ?       ?
   ????????????????????    ????????????????????
                    ?
                    ?
        ????????????????????????????????
        ? Usuario ingresa contraseña   ?
        ? y hace login                 ?
        ????????????????????????????????
                    ?
                    ?
        ????????????????????????????????
        ? Dashboard correspondiente    ?
        ????????????????????????????????
```

---

## ?? **CONCLUSIÓN**

La funcionalidad "Recordar sesión" está implementada con un enfoque de seguridad:

? Página de Bienvenida obligatoria al iniciar  
? NO hay login automático  
? Solo prellena datos para comodidad  
? Usuario siempre debe ingresar contraseña  
? Limpieza al cerrar sesión manual  
? Logs detallados para debugging  

**Estado:** ? **FUNCIONAL Y SEGURO**
