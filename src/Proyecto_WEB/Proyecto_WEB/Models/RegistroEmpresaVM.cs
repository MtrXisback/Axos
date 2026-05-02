namespace Proyecto_WEB.Models
{
    public class RegistroEmpresaVM
    {
        // Datos para la cuenta (Tabla Usuario)
        public string NombreCompleto { get; set; } = string.Empty; // Representante
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Datos para la ficha (Tabla Empresa)
        public string NombreEmpresa { get; set; } = string.Empty;
        public string RUC { get; set; } = string.Empty;
        public string SitioWeb { get; set; } = string.Empty;
    }
}