namespace Proyecto_API.Entidades
{
    public class Especialidad
    {
        public int EspecialidadID { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public string Estado { get; set; } = "Activo"; 
    }
}
