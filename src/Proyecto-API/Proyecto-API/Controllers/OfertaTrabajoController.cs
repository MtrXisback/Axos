using Microsoft.AspNetCore.Mvc;
using Proyecto_API.DTO;
using Proyecto_API.DTOs;
using Proyecto_API.Entidades;
using Proyecto_API.Services;

namespace Proyecto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfertaTrabajoController : ControllerBase
    {
        private readonly OfertaTrabajoService _service;
        public OfertaTrabajoController(OfertaTrabajoService service) => _service = service;

        // =============================================
        // --- SECCIÓN: GESTIÓN DE OFERTAS ---
        // =============================================

        // 1. PUBLICAR (USP_OfertasTrabajo_Insertar)
        [HttpPost("publicar")]
        public IActionResult Publicar([FromBody] OfertaTrabajo obj)
        {
            var res = _service.Registrar(obj);
            return res == "OK" ? Ok(new { mensaje = res }) : BadRequest(new { mensaje = res });
        }

        // 2. OBTENER DETALLE (USP_OfertasTrabajo_ObtenerPorID)
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var oferta = _service.ObtenerPorID(id);
            return oferta != null ? Ok(oferta) : NotFound(new { mensaje = "Oferta no encontrada" });
        }

        // 3. BUSCAR CON FILTROS (USP_OfertasTrabajo_ListarFiltrado)
        [HttpGet("buscar")]
        public IActionResult Buscar(int? especialidadId, string? modalidad, decimal? salarioMin, int pagina = 1)
            => Ok(_service.ListarFiltrado(especialidadId, modalidad, salarioMin, pagina));

        // 4. ACTUALIZAR (USP_OfertasTrabajo_Actualizar)
        [HttpPut("actualizar")]
        public IActionResult Actualizar([FromBody] OfertaTrabajo obj)
        {
            try
            {
                if (obj == null) return BadRequest("Objeto nulo.");

                bool actualizado = _service.Actualizar(obj);

                if (actualizado)
                    return Ok(new { mensaje = "Nodo AXON actualizado", id = obj.OfertaID });

                return NotFound(new { mensaje = "No se afectaron filas. ¿El ID existe o los datos son iguales?", id = obj.OfertaID });
            }
            catch (Exception ex)
            {
                // Esto te devuelve el error real en el JSON del Swagger
                return StatusCode(500, new { error = ex.Message, detalle = ex.InnerException?.Message });
            }
        }

        // 5. ELIMINAR/DESACTIVAR (USP_OfertasTrabajo_Eliminar)
        // En OfertaTrabajoController.cs (API)
        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id, [FromQuery] string estado = "Inactiva")
        {
            // El flujo Service -> DAO ya lo tienes perfecto
            bool respuesta = _service.Eliminar(id, estado);
            return Ok(respuesta);
        }


        // =============================================
        // --- SECCIÓN: HABILIDADES DE LA OFERTA ---
        // =============================================

        // 6. LISTAR POR OFERTA (USP_OfertaHabilidades_ListarPorOferta)
        [HttpGet("habilidades/{ofertaId}")]
        public IActionResult ListarHabilidades(int ofertaId)
            => Ok(_service.ObtenerHabilidadesPorOferta(ofertaId));

        // 7. ASIGNAR MÚLTIPLES (Usa USP_OfertaHabilidades_Limpiar y Asignar)
        [HttpPost("habilidades/asignar")]
        public IActionResult AsignarHabilidades([FromBody] HabilidadesRequest req)
            => Ok(_service.GuardarHabilidadesOferta(req.OfertaId, req.HabilidadesIds));

        // 8. QUITAR UNA SOLA (USP_OfertaHabilidades_Quitar) - EL QUE FALTABA
        [HttpDelete("habilidades/quitar")]
        public IActionResult QuitarHabilidad(int ofertaId, int habilidadId)
            => Ok(_service.EliminarHabilidadDeOferta(ofertaId, habilidadId));

        // 9. LIMPIAR TODAS (USP_OfertaHabilidades_Limpiar)
        [HttpDelete("habilidades/limpiar/{ofertaId}")]
        public IActionResult LimpiarHabilidades(int ofertaId)
            => Ok(_service.ResetearHabilidades(ofertaId));

        // GET: api/OfertaTrabajo/empresa/5
        [HttpGet("empresa/{id}")]
        public IActionResult ListarPorEmpresa(int id)
        {
          
            if (id <= 0)
            {
                return BadRequest("El ID de empresa proporcionado no es válido.");
            }

            var lista = _service.ListarPorEmpresa(id);

        
            return Ok(lista);
        }
    }
}