using Microsoft.AspNetCore.Mvc;
using Proyecto_WEB.Services;
using Proyecto_WEB.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_WEB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MaestroController : Controller
    {
        private readonly MaestroService _maestroService;
        private readonly UsuarioService _usuarioService;

        public MaestroController(MaestroService maestroService, UsuarioService usuarioService)
        {
            _maestroService = maestroService;
            _usuarioService = usuarioService;
        }

        // ==========================================
        // 1. ESPECIALIDADES
        // ==========================================
        public async Task<IActionResult> Especialidades() => View(await _maestroService.ListarEspecialidades());

        [HttpPost]
        public async Task<IActionResult> GuardarEspecialidad(string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) return RedirectToAction("Especialidades");

            var exito = await _maestroService.GuardarEspecialidad(nombre);

            TempData[exito ? "Mensaje" : "Error"] = exito ? "Especialidad registrada." : "Error al registrar.";
            return RedirectToAction("Especialidades");
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerEspecialidad(int id) => Json(await _maestroService.ObtenerEspecialidad(id));

        [HttpPost]
        public async Task<IActionResult> ActualizarEspecialidad(Especialidad obj)
        {
            await _maestroService.ActualizarEspecialidad(obj);
            return RedirectToAction("Especialidades");
        }

        // 💡 Cambiado a GET para que funcione con el enlace <a> de la tabla
        [HttpGet] 
        public async Task<IActionResult> EliminarEspecialidad(int id)
        {
            await _maestroService.EliminarEspecialidad(id);
            return RedirectToAction("Especialidades");
        }

        // ==========================================
        // 2. UBICACIONES
        // ==========================================
        public async Task<IActionResult> Ubicaciones() => View(await _maestroService.ListarUbicaciones());

        [HttpPost]
        public async Task<IActionResult> GuardarUbicacion(string ciudad, string pais)
        {
            // Validamos que ambos campos tengan datos
            if (string.IsNullOrEmpty(ciudad) || string.IsNullOrEmpty(pais)) return RedirectToAction("Ubicaciones");

            var exito = await _maestroService.GuardarUbicacion(ciudad, pais);

            TempData[exito ? "Mensaje" : "Error"] = exito ? "Ubicación sincronizada correctamente." : "Error al registrar la ubicación.";
            return RedirectToAction("Ubicaciones");
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerUbicacion(int id) => Json(await _maestroService.ObtenerUbicacion(id));

        [HttpPost]
        public async Task<IActionResult> ActualizarUbicacion(Ubicacion obj)
        {
            await _maestroService.ActualizarUbicacion(obj);
            return RedirectToAction("Ubicaciones");
        }

        [HttpGet] // 💡 Sincronizado con tu vista Infraestructura de Ubicaciones
        public async Task<IActionResult> EliminarUbicacion(int id)
        {
            await _maestroService.EliminarUbicacion(id);
            return RedirectToAction("Ubicaciones");
        }

        // ==========================================
        // 3. HABILIDADES
        // ==========================================
        public async Task<IActionResult> Habilidades() => View(await _maestroService.ListarHabilidades());

        [HttpPost]
        public async Task<IActionResult> GuardarHabilidad(string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) return RedirectToAction("Habilidades");

            var exito = await _maestroService.GuardarHabilidad(nombre);

            TempData[exito ? "Mensaje" : "Error"] = exito ? "Habilidad añadida al core." : "Error al registrar la habilidad.";
            return RedirectToAction("Habilidades");
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerHabilidad(int id)
        {
            var res = await _maestroService.ObtenerHabilidad(id);
            return Json(res); 
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarHabilidad(Habilidad obj)
        {
            await _maestroService.ActualizarHabilidad(obj);
            return RedirectToAction("Habilidades");
        }

        [HttpGet] // 💡 Estandarizado para evitar errores 405
        public async Task<IActionResult> EliminarHabilidad(int id)
        {
            await _maestroService.EliminarHabilidad(id);
            return RedirectToAction("Habilidades");
        }
    }
}