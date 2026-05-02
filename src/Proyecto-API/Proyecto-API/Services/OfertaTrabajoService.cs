using Proyecto_API.DAO;
using Proyecto_API.Entidades;
using System.Net.Http;

namespace Proyecto_API.Services
{
    public class OfertaTrabajoService
    {
        private readonly OfertaTrabajoDAO _ofertaDAO;

        public OfertaTrabajoService(OfertaTrabajoDAO ofertaDAO)
        {
            _ofertaDAO = ofertaDAO;
        }

        // 1. REGISTRAR (USP_OfertasTrabajo_Insertar)
        public string Registrar(OfertaTrabajo obj)
        {
            if (obj.SalarioMin > obj.SalarioMax)
                return "El salario mínimo no puede ser mayor al máximo.";

            int id = _ofertaDAO.Insertar(obj);
            return id > 0 ? "OK" : "Error al registrar la oferta.";
        }

        // 2. OBTENER POR ID (USP_OfertasTrabajo_ObtenerPorID)
        public OfertaTrabajo? ObtenerPorID(int id) => _ofertaDAO.ObtenerPorID(id);

        // 3. BUSCAR FILTRADO (USP_OfertasTrabajo_ListarFiltrado)
        public List<OfertaTrabajo> ListarFiltrado(int? especialidadId, string? modalidad, decimal? salarioMin, int pagina)
        {
            int tamanoPagina = 10;
            if (pagina <= 0) pagina = 1;
            return _ofertaDAO.ListarFiltrado(especialidadId, modalidad, salarioMin, pagina, tamanoPagina);
        }

        // 4. ACTUALIZAR (USP_OfertasTrabajo_Actualizar)
        public bool Actualizar(OfertaTrabajo obj) => _ofertaDAO.Actualizar(obj);

        // 5. ELIMINAR (USP_OfertasTrabajo_Eliminar)
        public bool Eliminar(int id, string nuevoEstado = "Inactiva") =>
        _ofertaDAO.Eliminar(id, nuevoEstado);

        // --- MÉTODOS DE HABILIDADES (Sincronizados con los 4 SPs) ---

        // 6. LISTAR HABILIDADES (USP_OfertaHabilidades_ListarPorOferta)
        public List<Habilidad> ObtenerHabilidadesPorOferta(int ofertaId) =>
            _ofertaDAO.ListarHabilidadesPorOferta(ofertaId);

        // 7. GUARDAR MÚLTIPLES (Usa USP_OfertaHabilidades_Limpiar y Asignar)
        public bool GuardarHabilidadesOferta(int ofertaId, List<int> habilidadesIds)
        {
            _ofertaDAO.LimpiarHabilidades(ofertaId);
            bool todoOk = true;
            foreach (var hId in habilidadesIds)
            {
                if (!_ofertaDAO.AsignarHabilidad(ofertaId, hId) == false) todoOk = false;
            }
            return todoOk;
        }

        // 8. QUITAR UNA (USP_OfertaHabilidades_Quitar) -> ESTE COINCIDE CON EL CONTROLLER
        public bool EliminarHabilidadDeOferta(int ofertaId, int habilidadId)
        {
            if (ofertaId <= 0 || habilidadId <= 0) return false;
            return _ofertaDAO.QuitarHabilidad(ofertaId, habilidadId);
        }

        // 9. RESETEAR TODO (USP_OfertaHabilidades_Limpiar) -> ESTE COINCIDE CON EL CONTROLLER
        public bool ResetearHabilidades(int ofertaId)
        {
            if (ofertaId <= 0) return false;
            return _ofertaDAO.LimpiarHabilidades(ofertaId);
        }

        public List<OfertaTrabajo> ListarPorEmpresa(int empresaId)
        {
           
            if (empresaId <= 0)
            {
                return new List<OfertaTrabajo>();
            }

            var lista = _ofertaDAO.ListarPorEmpresa(empresaId);
            
            return lista.OrderByDescending(o => o.FechaPublicacion).ToList();
        }
    }
}