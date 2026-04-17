using App_CrediVnzl.Models;

namespace App_CrediVnzl.Services
{
    public class AuthService
    {
        private readonly DatabaseService _databaseService;
        private Usuario? _usuarioActual;

        public Usuario? UsuarioActual => _usuarioActual;

        public bool IsAuthenticated => _usuarioActual != null;
        
        public int? ClienteId => _usuarioActual?.ClienteId;

        public AuthService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public bool Login(string username, string password, TipoUsuario tipo)
        {
            if (tipo == TipoUsuario.Administrador)
            {
                if (username == "admin" && password == "admin123")
                {
                    _usuarioActual = new Usuario
                    {
                        Username = username,
                        Tipo = TipoUsuario.Administrador,
                        NombreUsuario = username,
                        NombreCompleto = "Administrador"
                    };
                    return true;
                }
            }

            return false;
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

                if (usuario.Password != password)
                {
                    return (false, "Contraseña incorrecta", null);
                }

                if (!usuario.Activo)
                {
                    return (false, "Usuario inactivo", null);
                }

                _usuarioActual = usuario;
                return (true, "Login exitoso", usuario);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en LoginAsync: {ex.Message}");
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public void LoginCliente(int clienteId, string nombreCliente, string dni)
        {
            _usuarioActual = new Usuario
            {
                ClienteId = clienteId,
                Username = nombreCliente,
                Tipo = TipoUsuario.Cliente,
                Dni = dni,
                NombreUsuario = dni,
                NombreCompleto = nombreCliente
            };
        }

        public void Logout()
        {
            _usuarioActual = null;
        }

        public async Task<(bool exito, string mensaje)> RegistrarAdminAsync(
            string nombreNegocio,
            string nombreCompleto,
            string telefono,
            string email,
            string direccion,
            string nombreUsuario,
            string password)
        {
            try
            {
                // Verificar si ya existe un administrador
                var usuarioExistente = await _databaseService.GetUsuarioByNombreUsuarioAsync(nombreUsuario);
                if (usuarioExistente != null)
                {
                    return (false, "El nombre de usuario ya existe");
                }

                var nuevoUsuario = new Usuario
                {
                    NombreUsuario = nombreUsuario,
                    Password = password,
                    NombreCompleto = nombreCompleto,
                    Telefono = telefono,
                    Email = email,
                    Tipo = TipoUsuario.Administrador,
                    Activo = true,
                    Estado = EstadosUsuario.Activo,
                    FechaCreacion = DateTime.Now
                };

                await _databaseService.SaveUsuarioAsync(nuevoUsuario);
                return (true, "Administrador registrado exitosamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en RegistrarAdminAsync: {ex.Message}");
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool exito, string mensaje, string? password)> RegistrarClienteUsuarioAsync(
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

                // Verificar si ya existe un usuario para este cliente
                var usuariosClientes = await _databaseService.GetUsuariosClientesAsync();
                if (usuariosClientes.Any(u => u.ClienteId == clienteId))
                {
                    return (false, "El cliente ya tiene un usuario creado", null);
                }

                // Generar contraseña automática de 6 dígitos
                var random = new Random();
                var passwordGenerada = random.Next(100000, 999999).ToString();

                var nuevoUsuario = new Usuario
                {
                    NombreUsuario = cliente.Cedula, // Usar DNI como nombre de usuario
                    Password = passwordGenerada,
                    NombreCompleto = cliente.NombreCompleto,
                    Telefono = cliente.Telefono,
                    Email = string.Empty,
                    Tipo = TipoUsuario.Cliente,
                    ClienteId = clienteId,
                    Dni = cliente.Cedula,
                    Activo = true,
                    Estado = EstadosUsuario.Activo,
                    FechaCreacion = DateTime.Now
                };

                await _databaseService.SaveUsuarioAsync(nuevoUsuario);
                return (true, "Usuario creado exitosamente", passwordGenerada);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en RegistrarClienteUsuarioAsync: {ex.Message}");
                return (false, $"Error: {ex.Message}", null);
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

                // Generar contraseña automática de 6 dígitos
                var random = new Random();
                var password = random.Next(100000, 999999).ToString();

                usuario.Password = password;
                usuario.Estado = EstadosUsuario.Activo;
                usuario.Activo = true;

                await _databaseService.SaveUsuarioAsync(usuario);
                return (true, $"Cliente aprobado exitosamente. Contraseña temporal: {password}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en AprobarClienteAsync: {ex.Message}");
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool exito, string mensaje)> RechazarClienteAsync(int usuarioId, string motivo)
        {
            try
            {
                var usuario = await _databaseService.GetUsuarioAsync(usuarioId);
                if (usuario == null)
                {
                    return (false, "Usuario no encontrado");
                }

                usuario.Estado = EstadosUsuario.Rechazado;
                usuario.Activo = false;

                await _databaseService.SaveUsuarioAsync(usuario);
                return (true, "Cliente rechazado exitosamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en RechazarClienteAsync: {ex.Message}");
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool exito, string mensaje, string? password)> RegenerarPasswordClienteAsync(int usuarioId)
        {
            try
            {
                var usuario = await _databaseService.GetUsuarioAsync(usuarioId);
                if (usuario == null)
                {
                    return (false, "Usuario no encontrado", null);
                }

                // Generar nueva contraseña automática de 6 dígitos
                var random = new Random();
                var passwordGenerada = random.Next(100000, 999999).ToString();

                usuario.Password = passwordGenerada;

                await _databaseService.SaveUsuarioAsync(usuario);
                return (true, "Contraseña regenerada exitosamente", passwordGenerada);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en RegenerarPasswordClienteAsync: {ex.Message}");
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool exito, string mensaje)> CambiarPasswordAsync(string passwordActual, string passwordNueva)
        {
            try
            {
                if (_usuarioActual == null)
                {
                    return (false, "No hay sesión activa");
                }

                if (_usuarioActual.Password != passwordActual)
                {
                    return (false, "La contraseña actual es incorrecta");
                }

                if (passwordNueva.Length < 6)
                {
                    return (false, "La nueva contraseña debe tener al menos 6 caracteres");
                }

                _usuarioActual.Password = passwordNueva;
                await _databaseService.SaveUsuarioAsync(_usuarioActual);

                return (true, "Contraseña cambiada exitosamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CambiarPasswordAsync: {ex.Message}");
                return (false, $"Error: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"Error en EliminarDatosExistentesAsync: {ex.Message}");
                return false;
            }
        }
    }
}
