namespace Proyecto_API.DTO
{
    public class HabilidadesRequest
    {
        public int OfertaId { get; set; }
        public List<int> HabilidadesIds { get; set; } = new List<int>();
    }
}
