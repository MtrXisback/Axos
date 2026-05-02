using Microsoft.AspNetCore.Mvc;
using Proyecto_API.DTO;
using Proyecto_API.Entidades;
using Proyecto_API.Services;

namespace Proyecto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // Helper para estandarizar respuestas como en MaestroController
        private IActionResult ProcesarResultado(bool exito, string mensajeExito, string mensajeError, object? data = null)
        {
            if (exito)
                return Ok(new { success = true, message = mensajeExito, data = data });

            return BadRequest(new { success = false, message = mensajeError });
        }

        // 1. REGISTRO
        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] Usuario obj)
        {
            var respuesta = _usuarioService.Registrar(obj);
            return respuesta == "OK"
                ? Ok(new { success = true, message = "Usuario registrado correctamente" })
                : BadRequest(new { success = false, message = respuesta });
        }

        // 2. LOGIN
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var usuario = _usuarioService.Validar(request.Email, request.Password);
            if (usuario != null)
                return Ok(new { success = true, message = "Login exitoso", data = usuario });

            return Unauthorized(new { success = false, message = "Correo, contraseña incorrectos o cuenta inactiva" });
        }

        // 3. OBTENER POR ID
        [HttpGet("{id}")]
        public IActionResult ObtenerPorID(int id)
        {
            var usuario = _usuarioService.ObtenerPorID(id);
            if (usuario == null)
                return NotFound(new { success = false, message = "Usuario no encontrado" });

            return Ok(new { success = true, data = usuario });
        }

        // 4. ACTUALIZAR
        [HttpPut("actualizar")]
        public IActionResult Actualizar([FromBody] Usuario obj)
        {
            var exito = _usuarioService.Actualizar(obj);
            return ProcesarResultado(exito, "Usuario actualizado correctamente", "No se pudo actualizar el usuario");
        }

        // 5. LISTADO
        [HttpGet("listar")]
        public IActionResult Listar(int pagina = 1)
        {
            var lista = _usuarioService.Listar(pagina);
            return Ok(new { success = true, count = lista.Count, data = lista });
        }

        // 6. ELIMINAR (Toggle Lógico)
        [HttpDelete("eliminar/{id}")]
        public IActionResult Eliminar(int id)
        {
            var exito = _usuarioService.Eliminar(id);
            return ProcesarResultado(exito, "Estado de usuario actualizado", "Error al cambiar estado del usuario");
        }
    }
}