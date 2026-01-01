# INSTRUCCIONES PARA AGREGAR MENÚ HAMBURGUESA Y CONFIGURACIÓN DE CUENTA

## ? ARCHIVOS YA ACTUALIZADOS

### 1. AppShell.xaml ?
- Agregado menú hamburguesa con FlyoutHeader personalizado
- Opción "Dashboard" en el menú
- Opción "Mi Cuenta" en el menú  
- Opción "Cerrar Sesión" en el menú

### 2. AppShell.xaml.cs ?
- Agregado método `OnCerrarSesionClicked` para manejar el logout
- Registrada ruta "configuracioncuenta"

### 3. MauiProgram.cs ?
- Registrado `ConfiguracionCuentaViewModel`
- Registrado `ConfiguracionCuentaPage`

### 4. ViewModels/ConfiguracionCuentaViewModel.cs ?
- ViewModel completo para gestionar la configuración de cuenta

### 5. Pages/DashboardPage.xaml ?
- Cambiado Shell.NavBarIsVisible="True" para mostrar el menú hamburguesa

---

## ? ARCHIVOS POR AGREGAR MANUALMENTE

### Página de Configuración de Cuenta

Necesitas crear dos archivos en la carpeta `Pages`:

#### 1. ConfiguracionCuentaPage.xaml

```xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="App_CrediVnzl.Pages.ConfiguracionCuentaPage"
             Title="Mi Cuenta"
             BackgroundColor="#F5F5F5">

    <ScrollView>
        <VerticalStackLayout Spacing="20" Padding="20">

            <!-- Header -->
            <Frame BackgroundColor="{StaticResource Primary}"
                   CornerRadius="20"
                   Padding="25"
                   HasShadow="True"
                   Margin="0,0,0,10">
                <VerticalStackLayout Spacing="10">
                    <Label Text="??"
                           FontSize="50"
                           HorizontalOptions="Center" />
                    <Label Text="Configuración de Cuenta"
                           FontSize="22"
                           FontAttributes="Bold"
                           TextColor="White"
                           HorizontalOptions="Center" />
                    <Label Text="Administrador del Sistema"
                           FontSize="14"
                           TextColor="White"
                           Opacity="0.9"
                           HorizontalOptions="Center" />
                </VerticalStackLayout>
            </Frame>

            <!-- Datos del Administrador -->
            <Frame BackgroundColor="White"
                   CornerRadius="15"
                   Padding="20"
                   HasShadow="True">
                <VerticalStackLayout Spacing="20">
                    
                    <Label Text="?? Datos Personales"
                           FontSize="18"
                           FontAttributes="Bold"
                           TextColor="{StaticResource Gray900}" />

                    <!-- Nombre Completo -->
                    <VerticalStackLayout Spacing="8">
                        <Label Text="Nombre Completo"
                               FontSize="14"
                               TextColor="{StaticResource Gray700}"
                               FontAttributes="Bold" />
                        <Frame CornerRadius="10"
                               Padding="0"
                               HasShadow="False"
                               BorderColor="{StaticResource Gray300}"
                               BackgroundColor="#F9FAFB">
                            <Entry Placeholder="Ingrese su nombre completo"
                                   PlaceholderColor="#9CA3AF"
                                   Text="{Binding NombreCompleto}"
                                   TextColor="{StaticResource Gray900}"
                                   Margin="15,10" />
                        </Frame>
                    </VerticalStackLayout>

                    <!-- Teléfono -->
                    <VerticalStackLayout Spacing="8">
                        <Label Text="Teléfono"
                               FontSize="14"
                               TextColor="{StaticResource Gray700}"
                               FontAttributes="Bold" />
                        <Frame CornerRadius="10"
                               Padding="0"
                               HasShadow="False"
                               BorderColor="{StaticResource Gray300}"
                               BackgroundColor="#F9FAFB">
                            <Entry Placeholder="Ingrese su teléfono"
                                   PlaceholderColor="#9CA3AF"
                                   Text="{Binding Telefono}"
                                   Keyboard="Telephone"
                                   TextColor="{StaticResource Gray900}"
                                   Margin="15,10" />
                        </Frame>
                    </VerticalStackLayout>

                    <!-- Email -->
                    <VerticalStackLayout Spacing="8">
                        <Label Text="Email"
                               FontSize="14"
                               TextColor="{StaticResource Gray700}"
                               FontAttributes="Bold" />
                        <Frame CornerRadius="10"
                               Padding="0"
                               HasShadow="False"
                               BorderColor="{StaticResource Gray300}"
                               BackgroundColor="#F9FAFB">
                            <Entry Placeholder="Ingrese su email"
                                   PlaceholderColor="#9CA3AF"
                                   Text="{Binding Email}"
                                   Keyboard="Email"
                                   TextColor="{StaticResource Gray900}"
                                   Margin="15,10" />
                        </Frame>
                    </VerticalStackLayout>

                </VerticalStackLayout>
            </Frame>

            <!-- Cambiar Credenciales -->
            <Frame BackgroundColor="White"
                   CornerRadius="15"
                   Padding="20"
                   HasShadow="True">
                <VerticalStackLayout Spacing="20">
                    
                    <Label Text="?? Cambiar Credenciales"
                           FontSize="18"
                           FontAttributes="Bold"
                           TextColor="{StaticResource Gray900}" />

                    <!-- Nombre de Usuario -->
                    <VerticalStackLayout Spacing="8">
                        <Label Text="Nombre de Usuario"
                               FontSize="14"
                               TextColor="{StaticResource Gray700}"
                               FontAttributes="Bold" />
                        <Frame CornerRadius="10"
                               Padding="0"
                               HasShadow="False"
                               BorderColor="{StaticResource Gray300}"
                               BackgroundColor="#F9FAFB">
                            <Entry Placeholder="Ingrese nombre de usuario"
                                   PlaceholderColor="#9CA3AF"
                                   Text="{Binding NombreUsuario}"
                                   TextColor="{StaticResource Gray900}"
                                   Margin="15,10" />
                        </Frame>
                    </VerticalStackLayout>

                    <!-- Contraseña Actual -->
                    <VerticalStackLayout Spacing="8">
                        <Label Text="Contraseña Actual"
                               FontSize="14"
                               TextColor="{StaticResource Gray700}"
                               FontAttributes="Bold" />
                        <Frame CornerRadius="10"
                               Padding="0"
                               HasShadow="False"
                               BorderColor="{StaticResource Gray300}"
                               BackgroundColor="#F9FAFB">
                            <Entry Placeholder="Ingrese contraseña actual"
                                   PlaceholderColor="#9CA3AF"
                                   Text="{Binding PasswordActual}"
                                   IsPassword="True"
                                   TextColor="{StaticResource Gray900}"
                                   Margin="15,10" />
                        </Frame>
                    </VerticalStackLayout>

                    <!-- Nueva Contraseña -->
                    <VerticalStackLayout Spacing="8">
                        <Label Text="Nueva Contraseña"
                               FontSize="14"
                               TextColor="{StaticResource Gray700}"
                               FontAttributes="Bold" />
                        <Frame CornerRadius="10"
                               Padding="0"
                               HasShadow="False"
                               BorderColor="{StaticResource Gray300}"
                               BackgroundColor="#F9FAFB">
                            <Entry Placeholder="Ingrese nueva contraseña"
                                   PlaceholderColor="#9CA3AF"
                                   Text="{Binding PasswordNuevo}"
                                   IsPassword="True"
                                   TextColor="{StaticResource Gray900}"
                                   Margin="15,10" />
                        </Frame>
                    </VerticalStackLayout>

                    <!-- Confirmar Contraseña -->
                    <VerticalStackLayout Spacing="8">
                        <Label Text="Confirmar Contraseña"
                               FontSize="14"
                               TextColor="{StaticResource Gray700}"
                               FontAttributes="Bold" />
                        <Frame CornerRadius="10"
                               Padding="0"
                               HasShadow="False"
                               BorderColor="{StaticResource Gray300}"
                               BackgroundColor="#F9FAFB">
                            <Entry Placeholder="Confirme nueva contraseña"
                                   PlaceholderColor="#9CA3AF"
                                   Text="{Binding PasswordConfirmar}"
                                   IsPassword="True"
                                   TextColor="{StaticResource Gray900}"
                                   Margin="15,10" />
                        </Frame>
                    </VerticalStackLayout>

                    <Frame BackgroundColor="#FFF3E0"
                           CornerRadius="10"
                           Padding="15"
                           HasShadow="False"
                           BorderColor="#FF9800">
                        <HorizontalStackLayout Spacing="10">
                            <Label Text="??"
                                   FontSize="20"
                                   VerticalOptions="Center" />
                            <Label Text="Deja los campos de contraseña vacíos si no deseas cambiarla"
                                   FontSize="12"
                                   TextColor="#E65100" />
                        </HorizontalStackLayout>
                    </Frame>

                </VerticalStackLayout>
            </Frame>

            <!-- Botones -->
            <Grid ColumnDefinitions="*,*" ColumnSpacing="15">
                <Button Grid.Column="0"
                        Text="Cancelar"
                        BackgroundColor="{StaticResource Gray300}"
                        TextColor="{StaticResource Gray800}"
                        FontSize="16"
                        FontAttributes="Bold"
                        CornerRadius="12"
                        HeightRequest="55"
                        Command="{Binding CancelarCommand}" />
                
                <Button Grid.Column="1"
                        Text="?? Guardar Cambios"
                        BackgroundColor="{StaticResource Primary}"
                        TextColor="White"
                        FontSize="16"
                        FontAttributes="Bold"
                        CornerRadius="12"
                        HeightRequest="55"
                        Command="{Binding GuardarCommand}"
                        IsEnabled="{Binding EstaOcupado, Converter={StaticResource InvertedBoolConverter}}" />
            </Grid>

            <ActivityIndicator IsRunning="{Binding EstaOcupado}"
                               IsVisible="{Binding EstaOcupado}"
                               Color="{StaticResource Primary}"
                               HorizontalOptions="Center"
                               Margin="0,10,0,0" />

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

#### 2. ConfiguracionCuentaPage.xaml.cs

```csharp
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class ConfiguracionCuentaPage : ContentPage
    {
        public ConfiguracionCuentaPage(ConfiguracionCuentaViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
```

---

## ?? FUNCIONALIDADES IMPLEMENTADAS

### Menú Hamburguesa (Shell Flyout)
- ? **Header personalizado** con icono de usuario y nombre
- ? **Opción "Dashboard"** - Navega al panel de control
- ? **Opción "Mi Cuenta"** - Abre la página de configuración
- ? **Opción "Cerrar Sesión"** - Muestra confirmación y cierra sesión
- ? **Footer** con información de copyright

### Página de Configuración de Cuenta
- ? **Datos Personales:**
  - Nombre completo
  - Teléfono
  - Email
- ? **Cambiar Credenciales:**
  - Nombre de usuario
  - Contraseña actual (requerida para cambiar)
  - Nueva contraseña
  - Confirmar nueva contraseña
- ? **Validaciones:**
  - Campos requeridos
  - Contraseñas coincidentes
  - Mínimo 6 caracteres para contraseña
  - Verificación de contraseña actual
- ? **Actualización automática** de configuración del negocio

### Cerrar Sesión
- ? Confirmación antes de cerrar
- ? Limpia la sesión del usuario
- ? Redirige a la página de bienvenida

---

## ?? CÓMO USAR

### Acceder al Menú
1. Desde el Dashboard, toca el **ícono de hamburguesa** (?) en la esquina superior izquierda
2. Se desplegará el menú lateral con las opciones

### Configurar Cuenta
1. Abre el menú hamburguesa
2. Selecciona **"Mi Cuenta"**
3. Edita tus datos personales
4. Para cambiar contraseña:
   - Ingresa tu contraseña actual
   - Ingresa la nueva contraseña
   - Confirma la nueva contraseña
5. Toca **"Guardar Cambios"**

### Cerrar Sesión
1. Abre el menú hamburguesa
2. Selecciona **"Cerrar Sesión"**
3. Confirma la acción
4. Serás redirigido a la página de bienvenida

---

## ? ESTADO DEL PROYECTO

- ? AppShell actualizado con menú hamburguesa
- ? Logout implementado
- ? ViewModel de configuración creado
- ? Servicios registrados
- ? **Pendiente:** Agregar manualmente ConfiguracionCuentaPage.xaml y .xaml.cs
