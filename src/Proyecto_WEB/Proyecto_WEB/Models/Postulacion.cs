using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_WEB.Models
{
    public class Postulacion
    {
        public int PostulacionID { get; set; }
        public int OfertaID { get; set; }
        public string OfertaTitulo { get; set; } = string.Empty;
        public int UsuarioID { get; set; }
        public string CandidatoNombre { get; set; } = string.Empty;
        public DateTime FechaPostulacion { get; set; }
        public string EstadoPostulacion { get; set; } = "Pendiente";
        public string CV_AdjuntoURL { get; set; } = string.Empty;
        public bool Activo { get; set; }

        [NotMapped]
        public IFormFile? ArchivoCV { get; set; }
    }
}