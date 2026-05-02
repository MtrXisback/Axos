using Proyecto_API.DAO;
using Proyecto_API.Entidades;

namespace Proyecto_API.Services
{
    public class UsuarioService
    {
        private readonly UsuarioDAO _usuarioDAO;

        public UsuarioService(UsuarioDAO usuarioDAO)
        {
            _usuarioDAO = usuarioDAO;
        }

        // 1. REGISTRAR
        public string Registrar(Usuario obj)
        {
            if (string.IsNullOrEmpty(obj.Email) || string.IsNullOrEmpty(obj.PasswordHash))
                return "Email y Password son obligatorios.";

            return _usuarioDAO.RegistrarUsuario(obj);
        }

        // 💡 2. VALIDAR (El antiguo 'Login', ahora sincronizado con el Controller)
        public Usuario? Validar(string email, string password)
        {
            // Buscamos al usuario por su email en la base de datos
            var usuario = _usuarioDAO.ValidarLogin(email);

            if (usuario != null)
            {
                // 🛡️ REGLA AXON: Si el nodo está inactivo (Activo = 0), denegamos acceso
                if (!usuario.Activo) return null;

                // Comparamos la clave (luego le metemos BCrypt si quieres)
                if (usuario.PasswordHash == password)
                {
                    usuario.PasswordHash = string.Empty; // Limpiamos la clave por seguridad
                    return usuario;
                }
            }
            return null;
        }

        // 3. LISTAR
        public List<Usuario> Listar(int pagina)
        {
            int tamanoPagina = 10;
            if (pagina <= 0) pagina = 1;
            return _usuarioDAO.ListarUsuarios(pagina, tamanoPagina);
        }

        // 4. ACTUALIZAR
        public bool Actualizar(Usuario obj)
        {
            if (obj.UsuarioID <= 0) return false;
            return _usuarioDAO.ActualizarUsuario(obj);
        }

        // 5. ELIMINAR (Toggle Lógico)
        public bool Eliminar(int id)
        {
            if (id <= 0) return false;
            return _usuarioDAO.EliminarUsuario(id);
        }

        // 6. OBTENER POR ID
        public Usuario? ObtenerPorID(int id)
        {
            if (id <= 0) return null;
            return _usuarioDAO.ObtenerPorID(id);
        }
    }
}