using Proyecto_API.DAO;
using Proyecto_API.Entidades;

namespace Proyecto_API.Services
{
    public class PostulacionService
    {
        private readonly PostulacionDAO _postulacionDAO;

        public PostulacionService(PostulacionDAO postulacionDAO)
        {
            _postulacionDAO = postulacionDAO;
        }

        // 1. REGISTRAR (USP_Postulaciones_Registrar)
        public string Registrar(Postulacion obj)
        {
            if (string.IsNullOrEmpty(obj.CV_AdjuntoURL))
                return "Debe adjuntar la URL de su CV para postular.";

            int id = _postulacionDAO.Registrar(obj);
            return id > 0 ? "OK" : "No se pudo registrar la postulación.";
        }

        // 2. ACTUALIZAR ESTADO (USP_Postulaciones_ActualizarEstado)
        public bool ActualizarEstado(int postulacionId, string nuevoEstado)
        {
            if (postulacionId <= 0 || string.IsNullOrEmpty(nuevoEstado)) return false;
            return _postulacionDAO.ActualizarEstado(postulacionId, nuevoEstado);
        }

        // 3. LISTAR POR OFERTA (USP_Postulaciones_ListarPorOferta) - Para la Empresa
        public List<Postulacion> ListarPorOferta(int ofertaId)
        {
            if (ofertaId <= 0) return new List<Postulacion>();
            return _postulacionDAO.ListarPorOferta(ofertaId);
        }

        // 4. LISTAR POR USUARIO (USP_Postulaciones_ListarPorUsuario) - Para el Candidato
        public List<Postulacion> ListarPorUsuario(int usuarioId)
        {
            if (usuarioId <= 0) return new List<Postulacion>();
            return _postulacionDAO.ListarPorUsuario(usuarioId);
        }

        // 5. ELIMINAR / CANCELAR (USP_Postulaciones_Eliminar)
        public bool Eliminar(int id)
        {
            if (id <= 0) return false;
            return _postulacionDAO.Eliminar(id);
        }
    }
}