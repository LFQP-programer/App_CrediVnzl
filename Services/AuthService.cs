using System.Security.Cryptography;
using System.Text;
using App_CrediVnzl.Models;

namespace App_CrediVnzl.Services
{
    public class AuthService
    {
        private readonly DatabaseService _databaseService;
        private Usuario? _usuarioActual;

        public Usuario? UsuarioActual => _usuarioActual;
        public bool EstaAutenticado => _usuarioActual != null;
        public bool EsAdmin => _usuarioActual?.EsAdmin ?? false;
        public bool EsCliente => _usuarioActual?.EsCliente ?? false;

        public AuthService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<(bool exito, string mensaje, Usuario? usuario)> LoginAsync(string nombreUsuario, string password)
        {
            try
            {
                var usuario = await _databaseService.GetUsuarioByNombreUsuarioAsync(nombreUsuario);

                if (usuario == null)
                {
                    return (false, "Usuario no encontrado", null);
                }

                if (!usuario.Activo)
                {
                    return (false, "Usuario inactivo. Contacte al administrador.", null);
                }

                if (usuario.Estado == EstadosUsuario.Pendiente)
                {
                    return (false, "Tu solicitud está pendiente de aprobación por el administrador.", null);
                }

                if (usuario.Estado == EstadosUsuario.Rechazado)
                {
                    return (false, "Tu solicitud fue rechazada. Contacta al administrador.", null);
                }

                var passwordHash = HashPassword(password);
                if (usuario.PasswordHash != passwordHash)
                {
                    return (false, "Contraseña incorrecta", null);
                }

                usuario.UltimoAcceso = DateTime.Now;
                await _databaseService.SaveUsuarioAsync(usuario);

                _usuarioActual = usuario;
                return (true, "Login exitoso", usuario);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en Login: {ex.Message}");
                return (false, $"Error al iniciar sesión: {ex.Message}", null);
            }
        }

        public void SetUsuarioActual(Usuario usuario)
        {
            _usuarioActual = usuario;
            System.Diagnostics.Debug.WriteLine($"*** Usuario actual establecido: {usuario.NombreUsuario} ***");
        }

        public async Task<(bool exito, string mensaje, string? passwordGenerada)> RegistrarClienteUsuarioAsync(
            int clienteId,
            string nombreUsuario,
            string password)
        {
            try
            {
                var cliente = await _databaseService.GetClienteAsync(clienteId);
                if (cliente == null)
                {
                    return (false, "Cliente no encontrado", null);
                }

                var usuarioExistente = await _databaseService.GetUsuarioByClienteIdAsync(clienteId);
                if (usuarioExistente != null)
                {
                    return (false, "Este cliente ya tiene una cuenta de usuario", null);
                }

                var usuarioPorDNI = await _databaseService.GetUsuarioByNombreUsuarioAsync(cliente.Cedula);
                if (usuarioPorDNI != null)
                {
                    return (false, "Ya existe un usuario con este DNI", null);
                }

                string passwordTemporal = GenerarPasswordTemporal();

                var nuevoUsuario = new Usuario
                {
                    NombreUsuario = cliente.Cedula,
                    NombreCompleto = cliente.NombreCompleto,
                    PasswordHash = HashPassword(passwordTemporal),
                    Rol = RolesUsuario.Cliente,
                    ClienteId = clienteId,
                    Telefono = cliente.Telefono,
                    Email = "",
                    Estado = EstadosUsuario.Activo,
                    Activo = true
                };

                await _databaseService.SaveUsuarioAsync(nuevoUsuario);
                return (true, "Cuenta de cliente creada exitosamente", passwordTemporal);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registrando cliente usuario: {ex.Message}");
                return (false, $"Error al registrar: {ex.Message}", null);
            }
        }

        public async Task<(bool exito, string mensaje)> AprobarClienteAsync(int usuarioId)
        {
            try
            {
                var usuario = await _databaseService.GetUsuarioAsync(usuarioId);
                if (usuario == null)
                {
                    return (false, "Usuario no encontrado");
                }

                string passwordTemporal = GenerarPasswordTemporal();

                usuario.Estado = EstadosUsuario.Activo;
                usuario.Activo = true;
                usuario.FechaAprobacion = DateTime.Now;
                usuario.PasswordHash = HashPassword(passwordTemporal);
                await _databaseService.SaveUsuarioAsync(usuario);

                return (true, $"Cliente aprobado exitosamente. Contraseña temporal: {passwordTemporal}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aprobando cliente: {ex.Message}");
                return (false, $"Error al aprobar: {ex.Message}");
            }
        }

        public async Task<(bool exito, string mensaje)> RechazarClienteAsync(int usuarioId, string razon)
        {
            try
            {
                var usuario = await _databaseService.GetUsuarioAsync(usuarioId);
                if (usuario == null)
                {
                    return (false, "Usuario no encontrado");
                }

                if (usuario.ClienteId.HasValue && usuario.ClienteId.Value > 0)
                {
                    await _databaseService.EliminarClienteConDatosRelacionadosAsync(usuario.ClienteId.Value);
                }
                
                await _databaseService.DeleteUsuarioAsync(usuario);

                return (true, $"Solicitud rechazada: {razon}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error rechazando cliente: {ex.Message}");
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool exito, string mensaje)> RegistrarAdminAsync(
            string nombreNegocio,
            string nombreAdmin,
            string telefono,
            string email,
            string direccion,
            string nombreUsuario,
            string password)
        {
            try
            {
                var usuarios = await _databaseService.GetUsuariosAsync();
                var adminExistente = usuarios.FirstOrDefault(u => u.Rol == RolesUsuario.Admin);
                
                if (adminExistente != null)
                {
                    return (false, "Ya existe un administrador configurado");
                }

                var usuarioAdmin = new Usuario
                {
                    NombreUsuario = nombreUsuario,
                    NombreCompleto = nombreAdmin,
                    PasswordHash = HashPassword(password),
                    Rol = RolesUsuario.Admin,
                    ClienteId = null,
                    Telefono = telefono,
                    Email = email,
                    Estado = EstadosUsuario.Activo,
                    Activo = true
                };

                await _databaseService.SaveUsuarioAsync(usuarioAdmin);

                var capitalConfig = new CapitalConfig
                {
                    CapitalInicial = 0,
                    CapitalDisponible = 0,
                    GananciaTotal = 0,
                    FechaActualizacion = DateTime.Now
                };
                await _databaseService.SaveCapitalConfigAsync(capitalConfig);

                return (true, "Administrador registrado exitosamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registrando admin: {ex.Message}");
                return (false, $"Error al registrar: {ex.Message}");
            }
        }

        public async Task<(bool exito, string mensaje, string? passwordGenerada)> RegenerarPasswordClienteAsync(int usuarioId)
        {
            try
            {
                var usuario = await _databaseService.GetUsuarioAsync(usuarioId);
                if (usuario == null)
                {
                    return (false, "Usuario no encontrado", null);
                }

                var nuevaPassword = GenerarPasswordAleatoria();
                
                usuario.PasswordHash = HashPassword(nuevaPassword);
                await _databaseService.SaveUsuarioAsync(usuario);

                return (true, "Contraseña regenerada exitosamente", nuevaPassword);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error regenerando contraseña: {ex.Message}");
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool exito, string mensaje)> CambiarPasswordAsync(string passwordActual, string passwordNuevo)
        {
            try
            {
                if (UsuarioActual == null)
                {
                    return (false, "No hay sesión activa");
                }

                if (!VerifyPassword(passwordActual, UsuarioActual.PasswordHash))
                {
                    return (false, "La contraseña actual es incorrecta");
                }

                if (string.IsNullOrWhiteSpace(passwordNuevo))
                {
                    return (false, "La nueva contraseña no puede estar vacía");
                }

                if (passwordNuevo.Length < 6)
                {
                    return (false, "La nueva contraseña debe tener al menos 6 caracteres");
                }

                UsuarioActual.PasswordHash = HashPassword(passwordNuevo);
                await _databaseService.SaveUsuarioAsync(UsuarioActual);

                return (true, "Contraseña cambiada exitosamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cambiando contraseña: {ex.Message}");
                return (false, $"Error: {ex.Message}");
            }
        }

        public void Logout()
        {
            _usuarioActual = null;
        }

        public async Task<bool> VerificarPrimerUsoAsync()
        {
            try
            {
                var usuarios = await _databaseService.GetUsuariosAsync();
                var adminExistente = usuarios.FirstOrDefault(u => u.Rol == RolesUsuario.Admin);
                
                if (adminExistente == null)
                {
                    await CrearAdministradorPorDefectoAsync();
                }
                
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verificando primer uso: {ex.Message}");
                return false;
            }
        }

        private async Task CrearAdministradorPorDefectoAsync()
        {
            try
            {
                var usuarios = await _databaseService.GetUsuariosAsync();
                var adminExistente = usuarios.FirstOrDefault(u => u.Rol == RolesUsuario.Admin);
                
                if (adminExistente != null)
                {
                    return;
                }

                var usuarioAdmin = new Usuario
                {
                    NombreUsuario = "admin",
                    NombreCompleto = "Administrador",
                    PasswordHash = HashPassword("admin123"),
                    Rol = RolesUsuario.Admin,
                    ClienteId = null,
                    Telefono = "",
                    Email = "",
                    Estado = EstadosUsuario.Activo,
                    Activo = true
                };

                await _databaseService.SaveUsuarioAsync(usuarioAdmin);

                var capitalConfig = new CapitalConfig
                {
                    CapitalInicial = 0,
                    CapitalDisponible = 0,
                    GananciaTotal = 0,
                    FechaActualizacion = DateTime.Now
                };
                await _databaseService.SaveCapitalConfigAsync(capitalConfig);

                System.Diagnostics.Debug.WriteLine("*** Administrador por defecto creado exitosamente ***");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando administrador por defecto: {ex.Message}");
            }
        }

        public async Task<bool> EliminarDatosExistentesAsync()
        {
            try
            {
                await _databaseService.EliminarBaseDeDatosCompletaAsync();
                _usuarioActual = null;
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error eliminando datos: {ex.Message}");
                return false;
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private string GenerarPasswordTemporal()
        {
            Random random = new Random();
            int password = random.Next(100000, 999999);
            return password.ToString();
        }

        private bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }

        private string GenerarPasswordAleatoria()
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] password = new char[6];
            Random random = new Random();

            for (int i = 0; i < 6; i++)
            {
                password[i] = caracteres[random.Next(caracteres.Length)];
            }

            return new string(password);
        }
    }
}
