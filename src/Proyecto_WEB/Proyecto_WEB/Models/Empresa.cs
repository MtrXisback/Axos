namespace Proyecto_WEB.Models
{
    public class Empresa
    {
        public int EmpresaID { get; set; }
        public int UsuarioID { get; set; } // FK al dueño de la cuenta
        public string NombreEmpresa { get; set; } = string.Empty;
        public string RUC { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string SitioWeb { get; set; } = string.Empty;
        public string LogoURL { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}