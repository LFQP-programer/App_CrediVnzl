# ?? SISTEMA DE LOGIN CON DNI Y CONTRASEÑA AUTOMÁTICA

## ? IMPLEMENTACIÓN COMPLETADA

Se ha implementado el sistema de acceso de clientes usando:
- **Usuario:** DNI del cliente
- **Contraseña:** 6 dígitos generados automáticamente

---

## ?? ARCHIVOS MODIFICADOS

### 1. Services/AuthService.cs ?
**Cambios implementados:**
- ? `GenerarPasswordTemporal()` - Genera contraseñas de 6 dígitos
- ? `AprobarClienteAsync()` - Genera contraseña al aprobar solicitud
- ? `RegenerarPasswordClienteAsync()` - Regenera contraseña para clientes
- ? `RegistrarClientePendienteAsync()` - Usa DNI como nombre de usuario
- ? `RegistrarClienteUsuarioAsync()` - Crea usuario con DNI y genera contraseña

### 2. ViewModels/GestionarUsuariosViewModel.cs ?
**Cambios implementados:**
- ? Eliminados campos `NombreUsuario` y `Password`
- ? Agregada colección `SolicitudesPendientes`
- ? Comando `AprobarSolicitudCommand` - Aprueba y muestra contraseña
- ? Comando `RechazarSolicitudCommand` - Rechaza solicitudes
- ? Comando `RegenerarPasswordCommand` - Regenera contraseñas
- ? `OnCrearUsuarioAsync()` - Muestra credenciales generadas

### 3. Pages/GestionarUsuariosPage.xaml ?? REQUIERE ACTUALIZACIÓN MANUAL

**El archivo está abierto en el editor. Necesitas cerrar y actualizar manualmente.**

---

## ?? CONTENIDO PARA GestionarUsuariosPage.xaml

Cierra el archivo `Pages/GestionarUsuariosPage.xaml` y reemplaza su contenido con:

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="App_CrediVnzl.Pages.GestionarUsuariosPage"
             Title="Gestionar Usuarios"
             BackgroundColor="#F5F5F5">

    <ScrollView>
        <VerticalStackLayout Spacing="20" Padding="20">

            <!-- Solicitudes Pendientes -->
            <Label Text="?? Solicitudes Pendientes"
                   FontSize="20"
                   FontAttributes="Bold"
                   TextColor="{StaticResource Primary}"/>

            <Frame CornerRadius="15" Padding="15" BackgroundColor="White" HasShadow="True">
                <CollectionView ItemsSource="{Binding SolicitudesPendientes}">
                    <CollectionView.EmptyView>
                        <VerticalStackLayout Padding="20" Spacing="10">
                            <Label Text="?"
                                   FontSize="48"
                                   HorizontalOptions="Center"/>
                            <Label Text="No hay solicitudes pendientes"
                                   TextColor="#9E9E9E"
                                   HorizontalOptions="Center"
                                   FontSize="14"/>
                        </VerticalStackLayout>
                    </CollectionView.EmptyView>
                    <CollectionView.ItemTemplate>
                        <DataTemplate>
                            <Frame CornerRadius="10" Padding="15" Margin="0,5" BackgroundColor="#FFF3E0" HasShadow="False" BorderColor="#FF9800">
                                <Grid RowDefinitions="Auto,Auto,Auto" ColumnDefinitions="*,Auto" RowSpacing="5">
                                    
                                    <Label Grid.Row="0" Grid.Column="0"
                                           Text="{Binding NombreCompleto}"
                                           FontSize="16"
                                           FontAttributes="Bold"
                                           TextColor="{StaticResource Gray900}"/>
                                    
                                    <Label Grid.Row="1" Grid.Column="0"
                                           Text="{Binding NombreUsuario, StringFormat='DNI: {0}'}"
                                           FontSize="14"
                                           TextColor="{StaticResource Gray700}"/>
                                    
                                    <Label Grid.Row="2" Grid.Column="0"
                                           Text="{Binding FechaSolicitud, StringFormat='Solicitud: {0:dd/MM/yyyy HH:mm}'}"
                                           FontSize="12"
                                           TextColor="{StaticResource Gray600}"/>
                                    
                                    <VerticalStackLayout Grid.Row="0" Grid.RowSpan="3" Grid.Column="1" 
                                                        Spacing="5" VerticalOptions="Center">
                                        <Button Text="? Aprobar"
                                                Command="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, Path=BindingContext.AprobarSolicitudCommand}"
                                                CommandParameter="{Binding .}"
                                                BackgroundColor="{StaticResource Success}"
                                                TextColor="White"
                                                FontSize="12"
                                                CornerRadius="8"
                                                Padding="15,8"/>
                                        <Button Text="? Rechazar"
                                                Command="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, Path=BindingContext.RechazarSolicitudCommand}"
                                                CommandParameter="{Binding .}"
                                                BackgroundColor="{StaticResource Danger}"
                                                TextColor="White"
                                                FontSize="12"
                                                CornerRadius="8"
                                                Padding="15,8"/>
                                    </VerticalStackLayout>
                                </Grid>
                            </Frame>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
            </Frame>

            <!-- Crear Usuario para Cliente Existente -->
            <Label Text="?? Crear Usuario para Cliente"
                   FontSize="20"
                   FontAttributes="Bold"
                   TextColor="{StaticResource Primary}"
                   Margin="0,20,0,0"/>

            <Frame CornerRadius="15" Padding="20" BackgroundColor="White" HasShadow="True">
                <VerticalStackLayout Spacing="15">

                    <Frame BackgroundColor="#E3F2FD" CornerRadius="10" Padding="15" HasShadow="False">
                        <HorizontalStackLayout Spacing="10">
                            <Label Text="??" FontSize="20" VerticalOptions="Center"/>
                            <Label FontSize="13" TextColor="#1565C0" VerticalOptions="Center">
                                <Label.FormattedText>
                                    <FormattedString>
                                        <Span Text="El usuario será el DNI del cliente y se generará una contraseña de 6 dígitos automáticamente"/>
                                    </FormattedString>
                                </Label.FormattedText>
                            </Label>
                        </HorizontalStackLayout>
                    </Frame>

                    <Label Text="Seleccionar Cliente" FontSize="14" FontAttributes="Bold" TextColor="{StaticResource Gray900}"/>
                    <Frame CornerRadius="10" Padding="0" HasShadow="False" BorderColor="{StaticResource Gray300}" BackgroundColor="#F9FAFB">
                        <Picker ItemsSource="{Binding ClientesSinUsuario}"
                                ItemDisplayBinding="{Binding NombreCompleto}"
                                SelectedItem="{Binding ClienteSeleccionado}"
                                Title="Seleccione un cliente"
                                Margin="10,5"
                                TextColor="{StaticResource Gray900}"/>
                    </Frame>

                    <Button Text="?? GENERAR CREDENCIALES"
                            Command="{Binding CrearUsuarioCommand}"
                            BackgroundColor="{StaticResource Primary}"
                            TextColor="White"
                            CornerRadius="12"
                            HeightRequest="55"
                            FontSize="16"
                            FontAttributes="Bold"
                            IsEnabled="{Binding EstaOcupado, Converter={StaticResource InvertedBoolConverter}}"/>

                </VerticalStackLayout>
            </Frame>

            <!-- Usuarios Activos -->
            <Label Text="?? Usuarios Activos"
                   FontSize="20"
                   FontAttributes="Bold"
                   TextColor="{StaticResource Primary}"
                   Margin="0,20,0,0"/>

            <Frame CornerRadius="15" Padding="15" BackgroundColor="White" HasShadow="True">
                <CollectionView ItemsSource="{Binding UsuariosClientes}">
                    <CollectionView.EmptyView>
                        <VerticalStackLayout Padding="20" Spacing="10">
                            <Label Text="??"
                                   FontSize="48"
                                   HorizontalOptions="Center"/>
                            <Label Text="No hay usuarios activos"
                                   TextColor="#9E9E9E"
                                   HorizontalOptions="Center"
                                   FontSize="14"/>
                        </VerticalStackLayout>
                    </CollectionView.EmptyView>
                    <CollectionView.ItemTemplate>
                        <DataTemplate>
                            <Frame CornerRadius="10" Padding="15" Margin="0,5" BackgroundColor="#F9FAFB" HasShadow="False" BorderColor="{StaticResource Gray300}">
                                <Grid RowDefinitions="Auto,Auto,Auto,Auto" ColumnDefinitions="*,Auto" RowSpacing="5">
                                    
                                    <Label Grid.Row="0" Grid.Column="0"
                                           Text="{Binding NombreCompleto}"
                                           FontSize="16"
                                           FontAttributes="Bold"
                                           TextColor="{StaticResource Gray900}"/>
                                    
                                    <Label Grid.Row="1" Grid.Column="0"
                                           Text="{Binding NombreUsuario, StringFormat='?? Usuario: {0}'}"
                                           FontSize="14"
                                           TextColor="{StaticResource Gray700}"/>
                                    
                                    <Label Grid.Row="2" Grid.Column="0"
                                           Text="{Binding Telefono, StringFormat='?? {0}'}"
                                           FontSize="12"
                                           TextColor="{StaticResource Gray600}"/>
                                    
                                    <Label Grid.Row="3" Grid.Column="0"
                                           Text="{Binding UltimoAcceso, StringFormat='? Último acceso: {0:dd/MM/yyyy HH:mm}'}"
                                           FontSize="11"
                                           TextColor="{StaticResource Gray500}"/>
                                    
                                    <VerticalStackLayout Grid.Row="0" Grid.RowSpan="4" Grid.Column="1" 
                                                        Spacing="5" VerticalOptions="Center">
                                        <Button Text="?? Nueva Password"
                                                Command="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, Path=BindingContext.RegenerarPasswordCommand}"
                                                CommandParameter="{Binding .}"
                                                BackgroundColor="{StaticResource Warning}"
                                                TextColor="White"
                                                FontSize="11"
                                                CornerRadius="8"
                                                Padding="10,5"/>
                                        <Button Text="? Desactivar"
                                                Command="{Binding Source={RelativeSource AncestorType={x:Type ContentPage}}, Path=BindingContext.DesactivarUsuarioCommand}"
                                                CommandParameter="{Binding .}"
                                                BackgroundColor="{StaticResource Danger}"
                                                TextColor="White"
                                                FontSize="11"
                                                CornerRadius="8"
                                                Padding="10,5"/>
                                    </VerticalStackLayout>
                                </Grid>
                            </Frame>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
            </Frame>

            <ActivityIndicator IsRunning="{Binding EstaOcupado}"
                               IsVisible="{Binding EstaOcupado}"
                               Color="{StaticResource Primary}"
                               HorizontalOptions="Center"
                               Margin="0,20"/>

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

---

## ?? CARACTERÍSTICAS IMPLEMENTADAS

### ? Generación Automática de Contraseña
```csharp
private string GenerarPasswordTemporal()
{
    Random random = new Random();
    int password = random.Next(100000, 999999); // Entre 100000 y 999999
    return password.ToString();
}
```

### ? Login con DNI
- El cliente inicia sesión con su DNI como usuario
- No necesita recordar un nombre de usuario personalizado

### ? Flujo de Aprobación
1. Cliente solicita registro
2. Admin recibe solicitud en "Gestionar Usuarios"
3. Admin aprueba
4. Sistema genera contraseña de 6 dígitos
5. Se muestra alert con credenciales para enviar por WhatsApp

### ? Regeneración de Contraseña
- Botón "?? Nueva Password" para cada usuario
- Genera nueva contraseña de 6 dígitos
- Muestra alert con nueva contraseña

---

## ?? FLUJO DE USO

### Para el Administrador:

#### 1. Aprobar Solicitud de Cliente
```
1. Ir a "Gestionar Usuarios"
2. Ver "?? Solicitudes Pendientes"
3. Click en "? Aprobar"
4. Se muestra:
   ??????????????????????
   ? Cliente Aprobado
   
   Cliente: Juan Pérez
   
   ?? Usuario (DNI): 12345678
   ?? Contraseña: 847392
   
   ?? Envía estas credenciales
   al cliente por WhatsApp
   ??????????????????????
```

#### 2. Crear Usuario para Cliente Existente
```
1. Seleccionar cliente del dropdown
2. Click "?? GENERAR CREDENCIALES"
3. Se muestra:
   ??????????????????????
   ? Usuario Creado
   
   Usuario creado para: María García
   
   ?? Usuario (DNI): 87654321
   ?? Contraseña: 563829
   
   ?? Comunica estas credenciales
   al cliente por WhatsApp
   ??????????????????????
```

#### 3. Regenerar Contraseña
```
1. En lista de usuarios, click "?? Nueva Password"
2. Confirmar acción
3. Se muestra:
   ??????????????????????
   ? Contraseña Regenerada
   
   Cliente: Pedro López
   
   ?? Usuario (DNI): 45678912
   ?? Nueva Contraseña: 192847
   
   ?? Comunica la nueva contraseña
   al cliente
   ??????????????????????
```

### Para el Cliente:

```
1. Abrir app
2. Click "Cliente"
3. Ingresar credenciales:
   Usuario: [Su DNI]
   Contraseña: [6 dígitos recibidos]
4. ? Acceso al dashboard
```

---

## ?? SEGURIDAD

### Ventajas del Sistema:
? **Contraseña única** por cliente (6 dígitos aleatorios)
? **Usuario memorable** (su propio DNI)
? **Fácil comunicación** por WhatsApp
? **Regeneración** disponible si olvida o compromete
? **Balance** seguridad vs usabilidad

### Rango de Contraseñas:
- Mínimo: `100000`
- Máximo: `999999`
- Total posibilidades: **900,000 combinaciones**

---

## ?? PRUEBAS

### Compilar Proyecto
```bash
# Compilar
dotnet build

# O en Visual Studio
F6 o Build > Build Solution
```

### Probar Flujo Completo
1. ? Login como admin (admin/admin123)
2. ? Ver solicitudes pendientes (si hay)
3. ? Aprobar una solicitud
4. ? Verificar que se muestre la contraseña generada
5. ? Crear usuario para cliente existente
6. ? Verificar credenciales mostradas
7. ? Regenerar contraseña de un usuario
8. ? Logout de admin
9. ? Login como cliente con DNI y contraseña
10. ? Verificar acceso al dashboard del cliente

---

## ?? CHECKLIST DE IMPLEMENTACIÓN

- [x] Método `GenerarPasswordTemporal()` en AuthService
- [x] Modificar `AprobarClienteAsync()` para generar contraseña
- [x] Nuevo método `RegenerarPasswordClienteAsync()`
- [x] Modificar `RegistrarClientePendienteAsync()` para usar DNI
- [x] Modificar `RegistrarClienteUsuarioAsync()` para generar contraseña
- [x] Actualizar GestionarUsuariosViewModel
- [ ] **Actualizar GestionarUsuariosPage.xaml** ? PENDIENTE (cerrar archivo y copiar contenido arriba)
- [x] Agregar colección `SolicitudesPendientes`
- [x] Comandos de aprobar/rechazar/regenerar
- [x] Mostrar contraseñas en alerts

---

## ? PRÓXIMOS PASOS

1. **CERRAR** el archivo `Pages/GestionarUsuariosPage.xaml` en Visual Studio
2. **COPIAR** el contenido XAML de arriba
3. **PEGAR** en el archivo
4. **GUARDAR** el archivo
5. **COMPILAR** el proyecto (F6)
6. **EJECUTAR** y probar

---

## ?? MEJORAS FUTURAS (OPCIONALES)

### Integración con WhatsApp
```csharp
// En el futuro, podrías enviar automáticamente
private async Task EnviarCredencialesPorWhatsApp(string telefono, string dni, string password)
{
    var mensaje = $"¡Bienvenido a CrediVnzl!\\n\\n" +
                  $"Tus credenciales de acceso son:\\n" +
                  $"Usuario: {dni}\\n" +
                  $"Contraseña: {password}\\n\\n" +
                  $"Por seguridad, cambia tu contraseña desde la app.";
    
    // Usar WhatsAppService para enviar
    await _whatsAppService.EnviarMensajeAsync(telefono, mensaje);
}
```

### Historial de Cambios de Contraseña
- Registrar fecha/hora de cada cambio
- Mostrar último cambio en la UI

### Expiración de Contraseñas Temporales
- Forzar cambio al primer login
- Contraseñas temporales válidas por 7 días

---

¿Alguna pregunta sobre la implementación?
