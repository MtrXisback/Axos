using Microsoft.AspNetCore.Mvc;
using Proyecto_API.DTOs;
using Proyecto_API.Entidades;
using Proyecto_API.Helpers;
using Proyecto_API.Services;

namespace Proyecto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostulacionController : ControllerBase
    {
        private readonly PostulacionService _service;

        public PostulacionController(PostulacionService service)
        {
            _service = service;
        }

        // 1. APLICAR A UNA OFERTA (POST)
        [HttpPost("aplicar")]
        public IActionResult Aplicar([FromBody] Postulacion obj)
        {
            var res = _service.Registrar(obj);
            return res == "OK"
                ? Ok(new ApiResponse<bool> { Success = true, Message = "Postulación enviada", Data = true })
                : BadRequest(new ApiResponse<bool> { Success = false, Message = res });
        }

        // 2. CAMBIAR ESTADO
        [HttpPatch("cambiar-estado")]
        public IActionResult CambiarEstado([FromBody] PostulacionEstadoRequest req)
        {
            var exito = _service.ActualizarEstado(req.PostulacionId, req.NuevoEstado);
            return exito
                ? Ok(new ApiResponse<bool> { Success = true, Message = "Estado actualizado", Data = true })
                : BadRequest(new ApiResponse<bool> { Success = false, Message = "No se pudo actualizar el estado" });
        }

        // 3. VER CANDIDATOS DE UNA OFERTA (Lo usa la Empresa)
        [HttpGet("oferta/{ofertaId}")]
        public IActionResult ListarPorOferta(int ofertaId)
        {
            var lista = _service.ListarPorOferta(ofertaId);
            return Ok(new ApiResponse<List<Postulacion>> { Success = true, Data = lista });
        }

        // 4. VER MIS POSTULACIONES (Lo usa el Candidato - EL QUE DABA ERROR)
        [HttpGet("usuario/{usuarioId}")]
        public IActionResult ListarPorUsuario(int usuarioId)
        {
            var lista = _service.ListarPorUsuario(usuarioId);
            // 💡 Ahora enviamos la caja ApiResponse con la Data dentro
            return Ok(new ApiResponse<List<Postulacion>> { Success = true, Data = lista });
        }

        // 5. CANCELAR POSTULACIÓN
        [HttpDelete("cancelar/{id}")]
        public IActionResult Cancelar(int id)
        {
            var exito = _service.Eliminar(id);
            return exito
                ? Ok(new ApiResponse<bool> { Success = true, Message = "Postulación cancelada", Data = true })
                : NotFound(new ApiResponse<bool> { Success = false, Message = "Postulación no encontrada" });
        }
    }
}
