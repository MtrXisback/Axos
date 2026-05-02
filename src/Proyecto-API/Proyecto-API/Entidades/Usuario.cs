namespace Proyecto_API.Entidades
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty; 
        public bool Activo { get; set; }
    }
}