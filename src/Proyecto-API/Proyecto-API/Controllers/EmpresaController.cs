using Microsoft.AspNetCore.Mvc;
using Proyecto_API.Entidades;
using Proyecto_API.Services;
using Proyecto_API.Helpers;

namespace Proyecto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly EmpresaService _empresaService;

        public EmpresaController(EmpresaService empresaService)
        {
            _empresaService = empresaService;
        }

        // ============================================================
        // 🚀 REGISTRO COMPLETO (ORQUESTADO POR EL SERVICE)
        // ============================================================
        [HttpPost("registrar-completo")]
        public IActionResult RegistrarCompleto([FromBody] RegistroEmpresaVM modelo)
        {
            // El Service ahora maneja la transacción SQL internamente
            var resultado = _empresaService.RegistrarPartnerCompleto(modelo);

            if (resultado == "OK")
            {
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Partner corporativo sincronizado con éxito en la red AXON."
                });
            }

            // Si el resultado no es OK, es un mensaje de error (correo duplicado, RUC, etc.)
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = resultado
            });
        }

        // 1. REGISTRAR EMPRESA (INDIVIDUAL)
        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] Empresa obj)
        {
            var res = _empresaService.Registrar(obj);
            return Ok(new ApiResponse<string>
            {
                Success = res == "OK",
                Message = res == "OK" ? "Empresa registrada correctamente" : res,
                Data = res
            });
        }

        // 2. OBTENER PERFIL POR USUARIO
        [HttpGet("perfil/{usuarioId}")]
        public IActionResult GetPerfil(int usuarioId)
        {
            var empresa = _empresaService.ObtenerPorUsuario(usuarioId);
            return Ok(new ApiResponse<Empresa>
            {
                Success = empresa != null,
                Message = empresa != null ? "Perfil encontrado" : "El usuario no tiene una empresa vinculada",
                Data = empresa
            });
        }

        // 2.1 OBTENER POR ID (EMPRESA)
        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var empresa = _empresaService.ObtenerPorEmpresa(id);
            return Ok(new ApiResponse<Empresa>
            {
                Success = empresa != null,
                Message = empresa != null ? "Empresa encontrada" : "ID empresarial no reconocido",
                Data = empresa
            });
        }

        // 3. LISTAR TODAS LAS EMPRESAS (PAGINADO)
        [HttpGet("listar")]
        public IActionResult Listar(int pagina = 1)
        {
            var lista = _empresaService.Listar(pagina);
            return Ok(new ApiResponse<List<Empresa>>
            {
                Success = true,
                Message = "Listado de partners AXON obtenido con éxito",
                Data = lista
            });
        }

        // 4. ACTUALIZAR
        [HttpPut("actualizar")]
        public IActionResult Actualizar([FromBody] Empresa obj)
        {
            var exito = _empresaService.Actualizar(obj);
            return Ok(new ApiResponse<bool>
            {
                Success = exito,
                Message = exito ? "Datos corporativos actualizados" : "Error al actualizar la ficha",
                Data = exito
            });
        }

        // 5. ELIMINAR (TOGGLE LÓGICO)
        [HttpDelete("eliminar/{id}")]
        public IActionResult Eliminar(int id)
        {
            var exito = _empresaService.Eliminar(id);
            return Ok(new ApiResponse<bool>
            {
                Success = exito,
                Message = exito ? "Estado de partner actualizado" : "No se pudo modificar el estado del nodo",
                Data = exito
            });
        }
    }
}