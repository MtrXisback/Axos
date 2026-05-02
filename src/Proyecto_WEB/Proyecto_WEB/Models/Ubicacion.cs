namespace Proyecto_WEB.Models
{
    public class Ubicacion
    {
        public int UbicacionID { get; set; }
        public string Ciudad { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;

        public string Estado { get; set; } = "Activo"; 
    }
}