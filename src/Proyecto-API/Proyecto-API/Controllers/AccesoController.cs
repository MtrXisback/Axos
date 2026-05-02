using Microsoft.AspNetCore.Mvc;
using Proyecto_API.DTO; // Asegúrate de tener aquí tu clase LoginRequest
using Proyecto_API.Helpers;
using Proyecto_API.Services;
using Proyecto_API.Entidades;

namespace Proyecto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public AccesoController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // 💡 Llamamos al Validar que ya tienes en tu UsuarioService (DAO)
            var usuario = _usuarioService.Validar(request.Email, request.Password);

            if (usuario == null)
            {
                return Ok(new ApiResponse<Usuario> { Success = false, Message = "Usuario o clave incorrectos" });
            }

            return Ok(new ApiResponse<Usuario> { Success = true, Data = usuario });
        }
    }
}