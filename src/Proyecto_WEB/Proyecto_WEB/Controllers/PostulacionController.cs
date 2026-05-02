using Microsoft.AspNetCore.Mvc;
using Proyecto_WEB.Services;
using Proyecto_WEB.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IO;

namespace Proyecto_WEB.Controllers
{
    [Authorize]
    public class PostulacionController : Controller
    {
        private readonly PostulacionService _postulacionService;

        public PostulacionController(PostulacionService postulacionService)
        {
            _postulacionService = postulacionService;
        }

        // 1. APLICAR A UNA OFERTA
        [HttpPost]
        public async Task<IActionResult> Aplicar(int ofertaId, IFormFile ArchivoCV)
        {
            var usuarioIdStr = User.FindFirst("UsuarioID")?.Value;
            if (string.IsNullOrEmpty(usuarioIdStr) || ArchivoCV == null)
                return RedirectToAction("Index", "Acceso");

            int usuarioId = int.Parse(usuarioIdStr);

            // 1. Definir la ruta donde se guardará (wwwroot/cvs)
            string carpetaCvs = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cvs");
            if (!Directory.Exists(carpetaCvs)) Directory.CreateDirectory(carpetaCvs);

            // 2. Crear un nombre único para el archivo
            string nombreArchivo = $"CV_{usuarioId}_{DateTime.Now.Ticks}.pdf";
            string rutaCompleta = Path.Combine(carpetaCvs, nombreArchivo);

            // 3. Guardar el archivo físicamente
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await ArchivoCV.CopyToAsync(stream);
            }

            // 4. Crear el objeto para enviar a la API con la URL relativa
            var postulacion = new Postulacion
            {
                OfertaID = ofertaId,
                UsuarioID = usuarioId,
                CV_AdjuntoURL = "/cvs/" + nombreArchivo // 👈 Esta es la ruta que leerá la Empresa
            };

            var exito = await _postulacionService.Aplicar(postulacion);

            if (exito) TempData["Mensaje"] = "¡Sinapsis completada! Tu CV ha sido enviado.";
            else TempData["Error"] = "Hubo un fallo en la conexión neuronal.";

            return RedirectToAction("Detalle", "Oferta", new { id = ofertaId });
        }

        // 2. VER MIS POSTULACIONES (Candidato)
        public async Task<IActionResult> MisPostulaciones()
        {
            var claim = User.FindFirst("UsuarioID")?.Value;
            if (string.IsNullOrEmpty(claim)) return RedirectToAction("Index", "Acceso");

            var usuarioId = int.Parse(claim);
            var lista = await _postulacionService.ListarPorUsuario(usuarioId);

            return View(lista);
        }

        // 3. VER POSTULANTES DE UNA OFERTA (Empresa)
        public async Task<IActionResult> VerPostulantes(int id)
        {
            // 💡 "id" aquí es el OfertaID
            var lista = await _postulacionService.ListarPorOferta(id);

            // Pasamos el ID a la vista por si necesitamos regresar o refrescar
            ViewBag.OfertaID = id;
            return View(lista);
        }

        // 4. CAMBIAR ESTADO (Aceptado, Rechazado, etc.)
        [HttpPost]
        public async Task<IActionResult> ActualizarEstado(int postulacionId, string nuevoEstado, int ofertaId)
        {
            var exito = await _postulacionService.CambiarEstado(postulacionId, nuevoEstado);

            if (exito) TempData["Mensaje"] = "Estado actualizado correctamente.";
            else TempData["Error"] = "No se pudo sincronizar el cambio de estado.";

            // 💡 Corregido: El parámetro debe llamarse "id" para que coincida con VerPostulantes(int id)
            return RedirectToAction("VerPostulantes", new { id = ofertaId });
        }

        // 5. CANCELAR POSTULACIÓN (ELIMINAR)
        [HttpPost] // 💡 Recomendado usar Post para acciones que alteran datos
        public async Task<IActionResult> Cancelar(int id)
        {
            var exito = await _postulacionService.CancelarPostulacion(id);

            if (exito) TempData["Mensaje"] = "Postulación cancelada satisfactoriamente.";
            else TempData["Error"] = "No se pudo cancelar la postulación.";

            return RedirectToAction("MisPostulaciones");
        }
    }
}