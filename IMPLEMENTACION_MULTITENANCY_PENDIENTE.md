# ?? Implementación Multi-Tenancy - Archivos Pendientes y Configuración Final

## ? Archivos Ya Creados

### Modelos:
- ? `Models/Negocio.cs`
- ? `Models/Usuario.cs` (actualizado)
- ? `Models/Cliente.cs` (actualizado)
- ? `Models/Prestamo.cs` (actualizado)

### ViewModels:
- ? `ViewModels/BienvenidaViewModel.cs`
- ? `ViewModels/EleccionAdminViewModel.cs`
- ? `ViewModels/RegistroNegocioViewModel.cs`
- ? `ViewModels/ExitoNegocioViewModel.cs`
- ? `ViewModels/BuscarNegocioViewModel.cs`
- ? `ViewModels/RegistroClienteViewModel.cs`

### Páginas:
- ? `Pages/BienvenidaPage.xaml` + `.xaml.cs`
- ? `Pages/EleccionAdminPage.xaml` + `.xaml.cs`
- ? `Pages/RegistroNegocioPage.xaml` + `.xaml.cs`
- ? `Pages/ExitoNegocioPage.xaml` + `.xaml.cs`
- ? `Pages/BuscarNegocioPage.xaml` + `.xaml.cs`
- ? `Pages/RegistroClientePage.xaml.cs`

### Servicios:
- ? `Services/AuthService.cs` (actualizado)

---

## ?? Archivos Que Debes Crear Manualmente

### 1. **Pages/RegistroClientePage.xaml**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="App_CrediVnzl.Pages.RegistroClientePage"
             Shell.NavBarIsVisible="False"
             BackgroundColor="#F5F5F5">

    <ScrollView>
        <VerticalStackLayout Spacing="20" Padding="30">

            <Button Text="? Volver"
                    Command="{Binding VolverCommand}"
                    BackgroundColor="Transparent"
                    TextColor="#2196F3"
                    FontSize="16"
                    HorizontalOptions="Start"
                    Margin="0,10,0,0"/>

            <Label Text="??" FontSize="80" HorizontalOptions="Center"/>

            <Label Text="Registrarse como Cliente" 
                   FontSize="28" 
                   FontAttributes="Bold" 
                   HorizontalOptions="Center"
                   TextColor="#212121"/>

            <Label Text="{Binding NombreNegocio}" 
                   FontSize="18" 
                   HorizontalOptions="Center"
                   TextColor="#2196F3"
                   FontAttributes="Bold"
                   Margin="0,0,0,20"/>

            <Frame CornerRadius="15" Padding="20" BackgroundColor="White" HasShadow="True">
                <VerticalStackLayout Spacing="15">

                    <Label Text="Datos Personales" FontSize="18" FontAttributes="Bold" TextColor="#212121"/>

                    <Label Text="Nombre completo *" FontSize="14" TextColor="#757575"/>
                    <Frame CornerRadius="10" Padding="0" HasShadow="False" BorderColor="#E0E0E0">
                        <Entry Placeholder="Nombre y apellidos"
                               Text="{Binding NombreCompleto}"
                               Margin="10,5"/>
                    </Frame>

                    <Label Text="Cédula *" FontSize="14" TextColor="#757575"/>
                    <Frame CornerRadius="10" Padding="0" HasShadow="False" BorderColor="#E0E0E0">
                        <Entry Placeholder="V-12345678"
                               Text="{Binding Cedula}"
                               Margin="10,5"/>
                    </Frame>

                    <Label Text="Teléfono *" FontSize="14" TextColor="#757575"/>
                    <Frame CornerRadius="10" Padding="0" HasShadow="False" BorderColor="#E0E0E0">
                        <Entry Placeholder="+58 412-1234567"
                               Text="{Binding Telefono}"
                               Keyboard="Telephone"
                               Margin="10,5"/>
                    </Frame>

                    <Label Text="Dirección" FontSize="14" TextColor="#757575"/>
                    <Frame CornerRadius="10" Padding="0" HasShadow="False" BorderColor="#E0E0E0">
                        <Editor Placeholder="Dirección completa"
                                Text="{Binding Direccion}"
                                HeightRequest="80"
                                Margin="10,5"/>
                    </Frame>

                    <Label Text="Email (opcional)" FontSize="14" TextColor="#757575"/>
                    <Frame CornerRadius="10" Padding="0" HasShadow="False" BorderColor="#E0E0E0">
                        <Entry Placeholder="correo@ejemplo.com"
                               Text="{Binding Email}"
                               Keyboard="Email"
                               Margin="10,5"/>
                    </Frame>

                    <BoxView HeightRequest="1" Color="#E0E0E0" Margin="0,10"/>

                    <Label Text="Datos de Acceso" FontSize="18" FontAttributes="Bold" TextColor="#212121"/>

                    <Label Text="Nombre de usuario *" FontSize="14" TextColor="#757575"/>
                    <Frame CornerRadius="10" Padding="0" HasShadow="False" BorderColor="#E0E0E0">
                        <Entry Placeholder="usuario123"
                               Text="{Binding NombreUsuario}"
                               Margin="10,5"/>
                    </Frame>

                    <Label Text="Contraseña *" FontSize="14" TextColor="#757575"/>
                    <Frame CornerRadius="10" Padding="0" HasShadow="False" BorderColor="#E0E0E0">
                        <Entry Placeholder="Mínimo 6 caracteres"
                               Text="{Binding Password}"
                               IsPassword="True"
                               Margin="10,5"/>
                    </Frame>

                    <Label Text="Confirmar contraseña *" FontSize="14" TextColor="#757575"/>
                    <Frame CornerRadius="10" Padding="0" HasShadow="False" BorderColor="#E0E0E0">
                        <Entry Placeholder="Repite la contraseña"
                               Text="{Binding ConfirmarPassword}"
                               IsPassword="True"
                               Margin="10,5"/>
                    </Frame>

                    <Frame BackgroundColor="#FFF3E0" CornerRadius="10" Padding="15" HasShadow="False" Margin="0,10,0,0">
                        <VerticalStackLayout Spacing="8">
                            <Label Text="? Proceso de aprobación"
                                   FontSize="14"
                                   FontAttributes="Bold"
                                   TextColor="#E65100"/>
                            <Label Text="Tu solicitud será revisada por el administrador. Recibirás una notificación cuando sea aprobada."
                                   FontSize="12"
                                   TextColor="#424242"
                                   LineBreakMode="WordWrap"/>
                        </VerticalStackLayout>
                    </Frame>

                    <Button Text="SOLICITAR REGISTRO"
                            Command="{Binding RegistrarCommand}"
                            BackgroundColor="#4CAF50"
                            TextColor="White"
                            CornerRadius="10"
                            HeightRequest="55"
                            FontSize="16"
                            FontAttributes="Bold"
                            Margin="0,20,0,0"
                            IsEnabled="{Binding EstaOcupado, Converter={StaticResource InvertedBoolConverter}}"/>

                    <ActivityIndicator IsRunning="{Binding EstaOcupado}"
                                       IsVisible="{Binding EstaOcupado}"
                                       Color="#4CAF50"
                                       HorizontalOptions="Center"/>

                </VerticalStackLayout>
            </Frame>

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

### 2. **ViewModels/SolicitudPendienteViewModel.cs**

```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace App_CrediVnzl.ViewModels
{
    [QueryProperty(nameof(NombreNegocio), "nombreNegocio")]
    public class SolicitudPendienteViewModel : INotifyPropertyChanged
    {
        private string _nombreNegocio = string.Empty;

        public string NombreNegocio
        {
            get => _nombreNegocio;
            set { _nombreNegocio = Uri.UnescapeDataString(value ?? string.Empty); OnPropertyChanged(); }
        }

        public ICommand VolverLoginCommand { get; }
        public ICommand VolverInicioCommand { get; }

        public SolicitudPendienteViewModel()
        {
            VolverLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//login"));
            VolverInicioCommand = new Command(async () => await Shell.Current.GoToAsync("//bienvenida"));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
```

### 3. **Pages/SolicitudPendientePage.xaml.cs**

```csharp
using App_CrediVnzl.ViewModels;

namespace App_CrediVnzl.Pages
{
    public partial class SolicitudPendientePage : ContentPage
    {
        public SolicitudPendientePage()
        {
            InitializeComponent();
            BindingContext = new SolicitudPendienteViewModel();
        }
    }
}
```

### 4. **Pages/SolicitudPendientePage.xaml**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="App_CrediVnzl.Pages.SolicitudPendientePage"
             Shell.NavBarIsVisible="False"
             BackgroundColor="#F5F5F5">

    <ScrollView>
        <VerticalStackLayout Spacing="30" Padding="30" VerticalOptions="Center">

            <Label Text="?" FontSize="120" HorizontalOptions="Center"/>

            <VerticalStackLayout Spacing="10">
                <Label Text="Solicitud Enviada" 
                       FontSize="32" 
                       FontAttributes="Bold" 
                       HorizontalOptions="Center"
                       TextColor="#FF9800"/>
            </VerticalStackLayout>

            <Frame CornerRadius="20" Padding="25" BackgroundColor="White" HasShadow="True">
                <VerticalStackLayout Spacing="20">
                    
                    <Label Text="{Binding NombreNegocio, StringFormat='Tu solicitud ha sido enviada a {0}'}"
                           FontSize="18"
                           HorizontalOptions="Center"
                           HorizontalTextAlignment="Center"
                           TextColor="#212121"
                           LineBreakMode="WordWrap"/>

                    <BoxView HeightRequest="1" Color="#E0E0E0"/>

                    <VerticalStackLayout Spacing="15">
                        <HorizontalStackLayout Spacing="15">
                            <Label Text="??" FontSize="30" VerticalOptions="Start"/>
                            <VerticalStackLayout Spacing="5" VerticalOptions="Center">
                                <Label Text="Revisión en proceso"
                                       FontSize="16"
                                       FontAttributes="Bold"
                                       TextColor="#212121"/>
                                <Label Text="El administrador revisará tu información"
                                       FontSize="13"
                                       TextColor="#757575"
                                       LineBreakMode="WordWrap"/>
                            </VerticalStackLayout>
                        </HorizontalStackLayout>

                        <HorizontalStackLayout Spacing="15">
                            <Label Text="??" FontSize="30" VerticalOptions="Start"/>
                            <VerticalStackLayout Spacing="5" VerticalOptions="Center">
                                <Label Text="Recibirás notificación"
                                       FontSize="16"
                                       FontAttributes="Bold"
                                       TextColor="#212121"/>
                                <Label Text="Te avisaremos cuando sea aprobada"
                                       FontSize="13"
                                       TextColor="#757575"
                                       LineBreakMode="WordWrap"/>
                            </VerticalStackLayout>
                        </HorizontalStackLayout>

                        <HorizontalStackLayout Spacing="15">
                            <Label Text="??" FontSize="30" VerticalOptions="Start"/>
                            <VerticalStackLayout Spacing="5" VerticalOptions="Center">
                                <Label Text="Tiempo estimado"
                                       FontSize="16"
                                       FontAttributes="Bold"
                                       TextColor="#212121"/>
                                <Label Text="Normalmente 1-2 días hábiles"
                                       FontSize="13"
                                       TextColor="#757575"
                                       LineBreakMode="WordWrap"/>
                            </VerticalStackLayout>
                        </HorizontalStackLayout>
                    </VerticalStackLayout>

                </VerticalStackLayout>
            </Frame>

            <Frame CornerRadius="15" Padding="20" BackgroundColor="#E3F2FD" HasShadow="False" BorderColor="#2196F3">
                <VerticalStackLayout Spacing="10">
                    <Label Text="?? Mientras tanto..."
                           FontSize="16"
                           FontAttributes="Bold"
                           TextColor="#1976D2"/>
                    <Label Text="• Guarda tus datos de acceso en un lugar seguro"
                           FontSize="13"
                           TextColor="#424242"/>
                    <Label Text="• Mantén tu teléfono activo para recibir notificaciones"
                           FontSize="13"
                           TextColor="#424242"/>
                    <Label Text="• Puedes intentar iniciar sesión más tarde"
                           FontSize="13"
                           TextColor="#424242"/>
                </VerticalStackLayout>
            </Frame>

            <VerticalStackLayout Spacing="15">
                <Button Text="Intentar Iniciar Sesión"
                        Command="{Binding VolverLoginCommand}"
                        BackgroundColor="#2196F3"
                        TextColor="White"
                        CornerRadius="15"
                        HeightRequest="55"
                        FontSize="16"
                        FontAttributes="Bold"/>

                <Button Text="Volver al Inicio"
                        Command="{Binding VolverInicioCommand}"
                        BackgroundColor="Transparent"
                        TextColor="#2196F3"
                        BorderColor="#2196F3"
                        BorderWidth="2"
                        CornerRadius="15"
                        HeightRequest="55"
                        FontSize="16"
                        FontAttributes="Bold"/>
            </VerticalStackLayout>

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

---

## ?? Actualizaciones Necesarias en DatabaseService.cs

Agrega estos métodos al archivo `Services/DatabaseService.cs`:

```csharp
// En la sección de InitializeAsync, agregar:
await _database.CreateTableAsync<Negocio>();

// Métodos para Negocios
public async Task<List<Negocio>> GetNegociosAsync()
{
    var db = await GetDatabaseAsync();
    return await db.Table<Negocio>().Where(n => n.Activo).ToListAsync();
}

public async Task<Negocio?> GetNegocioAsync(int id)
{
    var db = await GetDatabaseAsync();
    return await db.Table<Negocio>().Where(n => n.Id == id).FirstOrDefaultAsync();
}

public async Task<Negocio?> GetNegocioByCodigoAsync(string codigo)
{
    var db = await GetDatabaseAsync();
    return await db.Table<Negocio>()
        .Where(n => n.CodigoNegocio == codigo && n.Activo)
        .FirstOrDefaultAsync();
}

public async Task<List<Negocio>> BuscarNegociosPublicosAsync(string busqueda)
{
    var db = await GetDatabaseAsync();
    return await db.Table<Negocio>()
        .Where(n => n.Activo && n.PermitirBusquedaPublica && 
                   (n.NombreNegocio.Contains(busqueda) || 
                    n.NombreAdministrador.Contains(busqueda) || 
                    n.TelefonoAdministrador.Contains(busqueda)))
        .ToListAsync();
}

public async Task<int> SaveNegocioAsync(Negocio negocio)
{
    var db = await GetDatabaseAsync();
    
    if (negocio.Id != 0)
    {
        return await db.UpdateAsync(negocio);
    }
    else
    {
        return await db.InsertAsync(negocio);
    }
}

public async Task<Cliente?> GetClienteByCedulaAsync(int negocioId, string cedula)
{
    var db = await GetDatabaseAsync();
    return await db.Table<Cliente>()
        .Where(c => c.NegocioId == negocioId && c.Cedula == cedula)
        .FirstOrDefaultAsync();
}

// Actualizar TODOS los métodos existentes que obtengan datos para filtrar por NegocioId
// Ejemplo en GetClientesAsync:
public async Task<List<Cliente>> GetClientesAsync()
{
    var db = await GetDatabaseAsync();
    var negocioId = /* obtener negocioId del usuario actual */;
    return await db.Table<Cliente>()
        .Where(c => c.NegocioId == negocioId)
        .OrderByDescending(c => c.FechaRegistro)
        .ToListAsync();
}
```

---

## ?? Actualizar App.xaml.cs

```csharp
protected override Window CreateWindow(IActivationState? activationState)
{
    try
    {
        System.Diagnostics.Debug.WriteLine("*** CreateWindow - Iniciando ***");
        var window = new Window(new AppShell());
        
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                var authService = Handler?.MauiContext?.Services.GetService<AuthService>();
                var databaseService = Handler?.MauiContext?.Services.GetService<DatabaseService>();
                
                if (authService != null && databaseService != null)
                {
                    await databaseService.InitializeAsync();
                    
                    var esPrimerUso = await authService.VerificarPrimerUsoAsync();
                    
                    if (esPrimerUso)
                    {
                        // Primera vez - mostrar bienvenida
                        await Shell.Current.GoToAsync("//bienvenida");
                    }
                    else
                    {
                        // Ya hay negocios - ir a login
                        await Shell.Current.GoToAsync("//login");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR verificando primer uso: {ex.Message}");
            }
        });
        
        System.Diagnostics.Debug.WriteLine("*** CreateWindow - Window creado OK ***");
        return window;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"*** ERROR EN CreateWindow ***: {ex.Message}");
        throw;
    }
}
```

---

## ?? Actualizar AppShell.xaml

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Shell
    x:Class="App_CrediVnzl.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:App_CrediVnzl"
    xmlns:pages="clr-namespace:App_CrediVnzl.Pages"
    Title="CrediVnzl">

    <!-- Bienvenida (Inicial) -->
    <ShellContent
        Title="Bienvenida"
        ContentTemplate="{DataTemplate pages:BienvenidaPage}"
        Route="bienvenida" />

    <!-- Elección Admin -->
    <ShellContent
        Title="Elección Admin"
        ContentTemplate="{DataTemplate pages:EleccionAdminPage}"
        Route="eleccionadmin"
        Shell.FlyoutBehavior="Disabled" />

    <!-- Login Page -->
    <ShellContent
        Title="Login"
        ContentTemplate="{DataTemplate pages:LoginPage}"
        Route="login"
        Shell.FlyoutBehavior="Disabled" />

    <!-- Dashboard Admin -->
    <ShellContent
        Title="Dashboard"
        ContentTemplate="{DataTemplate pages:DashboardPage}"
        Route="dashboard"
        Shell.FlyoutBehavior="Disabled" />

    <!-- Dashboard Cliente -->
    <ShellContent
        Title="Mi Dashboard"
        ContentTemplate="{DataTemplate pages:DashboardClientePage}"
        Route="dashboardcliente"
        Shell.FlyoutBehavior="Disabled" />

</Shell>
```

---

## ?? Actualizar AppShell.xaml.cs

```csharp
public AppShell()
{
    try
    {
        System.Diagnostics.Debug.WriteLine("*** AppShell Constructor - Iniciando ***");
        InitializeComponent();

        // Registrar rutas de navegación
        Routing.RegisterRoute("registronegocio", typeof(RegistroNegocioPage));
        Routing.RegisterRoute("exitonegocio", typeof(ExitoNegocioPage));
        Routing.RegisterRoute("buscarnegocio", typeof(BuscarNegocioPage));
        Routing.RegisterRoute("registrocliente", typeof(RegistroClientePage));
        Routing.RegisterRoute("solicitudpendiente", typeof(SolicitudPendientePage));
        
        // Rutas existentes
        Routing.RegisterRoute("clientes", typeof(ClientesPage));
        Routing.RegisterRoute("nuevocliente", typeof(NuevoClientePage));
        Routing.RegisterRoute("detallecliente", typeof(DetalleClientePage));
        // ... resto de rutas
        
        System.Diagnostics.Debug.WriteLine("*** AppShell Constructor - Rutas registradas OK ***");
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"*** ERROR EN AppShell Constructor ***: {ex.Message}");
        throw;
    }
}
```

---

## ?? Actualizar MauiProgram.cs

```csharp
// Registrar nuevas páginas
builder.Services.AddTransient<BienvenidaPage>();
builder.Services.AddTransient<EleccionAdminPage>();
builder.Services.AddTransient<RegistroNegocioPage>();
builder.Services.AddTransient<ExitoNegocioPage>();
builder.Services.AddTransient<BuscarNegocioPage>();
builder.Services.AddTransient<RegistroClientePage>();
builder.Services.AddTransient<SolicitudPendientePage>();
```

---

## ? Checklist Final

- [ ] Crear `Pages/RegistroClientePage.xaml`
- [ ] Crear `ViewModels/SolicitudPendienteViewModel.cs`
- [ ] Crear `Pages/SolicitudPendientePage.xaml` + `.xaml.cs`
- [ ] Actualizar `DatabaseService.cs` con métodos de Negocio
- [ ] Actualizar `DatabaseService.cs` para filtrar por NegocioId
- [ ] Actualizar `App.xaml.cs` con nueva lógica de inicio
- [ ] Actualizar `AppShell.xaml` con nuevas rutas
- [ ] Actualizar `AppShell.xaml.cs` con nuevos registros
- [ ] Actualizar `MauiProgram.cs` con nuevas páginas
- [ ] **Compilar y probar** ??

---

## ?? Próximos Pasos Después de Implementar

1. **Crear página de gestión de solicitudes pendientes** (para admin)
2. **Agregar notificaciones** cuando un cliente sea aprobado
3. **Implementar búsqueda de negocios** con más filtros
4. **Agregar opción de "Olvidé mi contraseña"**
5. **Implementar migración a Firebase** cuando estés listo

---

**¿Necesitas ayuda con alguno de estos archivos o implementaciones?** ??
