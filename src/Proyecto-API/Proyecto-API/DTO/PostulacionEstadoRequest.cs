namespace Proyecto_API.DTOs
{
    public class PostulacionEstadoRequest
    {
        public int PostulacionId { get; set; }
        public string NuevoEstado { get; set; } = string.Empty;
    }
}