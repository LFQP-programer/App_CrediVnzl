using App_CrediVnzl.Models;

namespace App_CrediVnzl.Services
{
    public class AuthService
    {
        private Usuario? _usuarioActual;

        public Usuario? UsuarioActual => _usuarioActual;

        public bool IsAuthenticated => _usuarioActual != null;
        
        public int? ClienteId => _usuarioActual?.ClienteId;

        public bool Login(string username, string password, TipoUsuario tipo)
        {
            if (tipo == TipoUsuario.Administrador)
            {
                if (username == "admin" && password == "admin123")
                {
                    _usuarioActual = new Usuario
                    {
                        Username = username,
                        Tipo = TipoUsuario.Administrador
                    };
                    return true;
                }
            }

            return false;
        }

        public void LoginCliente(int clienteId, string nombreCliente, string dni)
        {
            _usuarioActual = new Usuario
            {
                ClienteId = clienteId,
                Username = nombreCliente,
                Tipo = TipoUsuario.Cliente,
                Dni = dni
            };
        }

        public void Logout()
        {
            _usuarioActual = null;
        }
    }
}
