namespace Proyecto_WEB.Models 
{ 
    public class OfertaTrabajo
    {
        public int OfertaID { get; set; }
        public int EmpresaID { get; set; }
        public string? NombreEmpresa { get; set; } = string.Empty; // Para mostrar en el listado
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int EspecialidadID { get; set; }
        public string? EspecialidadNombre { get; set; } = string.Empty;
        public int UbicacionID { get; set; }
        public string? Ciudad { get; set; } = string.Empty;
        public string Modalidad { get; set; } = string.Empty;
        public decimal SalarioMin { get; set; }
        public decimal SalarioMax { get; set; }
        public string Estado { get; set; } = "Activa";
        public DateTime FechaPublicacion { get; set; }
    }
}