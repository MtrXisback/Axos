using Microsoft.AspNetCore.Mvc;
using Proyecto_WEB.Services;
using Proyecto_WEB.Models;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_WEB.Controllers
{
    [Authorize] // Bloqueo general: Debes estar logueado en la red AXON
    public class OfertaController : Controller
    {
        private readonly OfertaService _ofertaService;
        private readonly MaestroService _maestroService;
        private readonly EmpresaService _empresaService;
        private readonly PostulacionService _postulacionService;

        public OfertaController(
            OfertaService ofertaService,
            MaestroService maestroService,
            EmpresaService empresaService,
            PostulacionService postulacionService)
        {
            _ofertaService = ofertaService;
            _maestroService = maestroService;
            _empresaService = empresaService;
            _postulacionService = postulacionService;
        }

        // 1. EXPLORADOR DE NODOS (Público para logueados)
        public async Task<IActionResult> Index(int? especialidadId, string? modalidad, decimal? salarioMin)
        {
            var lista = await _ofertaService.ListarFiltrado(especialidadId, modalidad, salarioMin);
            return View(lista);
        }

        // 2. DETALLE TÉCNICO DEL NODO
        public async Task<IActionResult> Detalle(int id)
        {
            // 1. Obtenemos la oferta base
            var oferta = await _ofertaService.ObtenerPorID(id);
            if (oferta == null) return RedirectToAction("Index");

            // 2. Detective AXON: ¿Este usuario ya postuló?
            var usuarioIdStr = User.FindFirst("UsuarioID")?.Value;
            bool yaPostulo = false;

            if (!string.IsNullOrEmpty(usuarioIdStr))
            {
                int usuarioId = int.Parse(usuarioIdStr);

                // 💡 Llamamos al servicio de postulaciones para ver el historial del usuario
                var misPostulaciones = await _postulacionService.ListarPorUsuario(usuarioId);

                // 💡 Verificamos si este ID de oferta ya está en su lista de "sinapsis"
                yaPostulo = misPostulaciones.Any(p => p.OfertaID == id);
            }

            // 3. Pasamos las banderas a la vista
            ViewBag.YaPostulo = yaPostulo;
            ViewBag.Habilidades = await _ofertaService.ListarHabilidades(id);

            return View(oferta);
        }

        // 3. PUBLICAR (Solo Empresa) 
        // Nota: Quitamos Admin porque el flujo de POST requiere un perfil de empresa vinculado al UsuarioID
        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> Publicar()
        {
            ViewBag.Especialidades = await _maestroService.ListarEspecialidades();
            ViewBag.Ubicaciones = await _maestroService.ListarUbicaciones();
            return View();
        }

        // 4. PUBLICAR (POST)
        [HttpPost]
        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> Publicar(OfertaTrabajo obj)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Especialidades = await _maestroService.ListarEspecialidades();
                ViewBag.Ubicaciones = await _maestroService.ListarUbicaciones();
                return View(obj);
            }

            var userId = int.Parse(User.FindFirst("UsuarioID")!.Value);
            var perfilEmpresa = await _empresaService.BuscarPorId(userId);

            obj.EmpresaID = perfilEmpresa.EmpresaID;
            obj.FechaPublicacion = DateTime.Now;
            obj.Estado = "Activa";

            var exito = await _ofertaService.Registrar(obj);
            if (exito)
            {
                TempData["Mensaje"] = "Nodo AXON publicado exitosamente.";
                return RedirectToAction("MisOfertas");
            }

            return View(obj);
        }

        // 5. RECONFIGURAR NODO (GET)
        [HttpGet]
        [Authorize(Roles = "Empresa,Admin")]
        public async Task<IActionResult> Actualizar(int id)
        {
            var oferta = await _ofertaService.ObtenerPorID(id);
            if (oferta == null) return NotFound();

            ViewBag.Especialidades = await _maestroService.ListarEspecialidades();
            ViewBag.Ubicaciones = await _maestroService.ListarUbicaciones();
            return View(oferta);
        }

        // 6. RECONFIGURAR NODO (POST)
        [HttpPost]
        [Authorize(Roles = "Empresa,Admin")]
        public async Task<IActionResult> Actualizar(OfertaTrabajo obj)
        {
            string[] camposAExcluir = { "NombreEmpresa", "EspecialidadNombre", "Ciudad", "FechaPublicacion", "Modalidad", "Estado" };
            foreach (var campo in camposAExcluir) ModelState.Remove(campo);

            if (!ModelState.IsValid)
            {
                ViewBag.Especialidades = await _maestroService.ListarEspecialidades();
                ViewBag.Ubicaciones = await _maestroService.ListarUbicaciones();
                return View(obj);
            }

            bool exito = await _ofertaService.Actualizar(obj);
            if (exito)
            {
                TempData["Mensaje"] = "Nodo AXON sincronizado correctamente.";
                // Redirección inteligente según el rol
                if (User.IsInRole("Admin")) return RedirectToAction("VacantesPorEmpresa", new { id = obj.EmpresaID });
                return RedirectToAction("MisOfertas");
            }

            return View(obj);
        }

        // 7. DESACTIVAR NODO (Toggle de Estado)
        [HttpPost]
        [Authorize(Roles = "Empresa,Admin")]
        public async Task<IActionResult> Eliminar(int id, string estadoActual)
        {
            string nuevoEstado = (estadoActual == "Activa") ? "Inactiva" : "Activa";
            bool exito = await _ofertaService.Eliminar(id, nuevoEstado);

            if (exito) TempData["Mensaje"] = (nuevoEstado == "Activa") ? "¡Oferta reactivada!" : "Oferta pausada correctamente.";

            // 💡 Redirección inteligente: Si es Admin, vuelve a la vista de ese nodo.
            // Para esto necesitamos recuperar el objeto o el ID de la empresa.
            var oferta = await _ofertaService.ObtenerPorID(id);
            if (User.IsInRole("Admin") && oferta != null)
                return RedirectToAction("VacantesPorEmpresa", new { id = oferta.EmpresaID });

            return RedirectToAction("MisOfertas");
        }

        // 8. PANEL DE CONTROL EMPRESARIAL (Solo para Empresas)
        // 🚨 IMPORTANTE: Quitamos Admin para evitar el 404 al buscar perfil de empresa inexistente
        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> MisOfertas()
        {
            var userIdClaim = User.FindFirst("UsuarioID")?.Value;
            if (userIdClaim == null) return RedirectToAction("Login", "Acceso");

            int usuarioId = int.Parse(userIdClaim);
            var empresa = await _empresaService.BuscarPorId(usuarioId);

            if (empresa != null)
            {
                var lista = await _ofertaService.ListarPorEmpresa(empresa.EmpresaID);
                return View(lista);
            }

            return View(new List<OfertaTrabajo>());
        }

        // 9. GESTIÓN DE CANDIDATOS
        [Authorize(Roles = "Empresa,Admin")]
        public async Task<IActionResult> VerPostulantes(int id)
        {
            var lista = await _postulacionService.ListarPorOferta(id);
            ViewBag.OfertaID = id;
            return View(lista);
        }

        // 10. CAMBIO DE ESTADO DE POSTULACIÓN
        // 10. CAMBIO DE ESTADO DE POSTULACIÓN
        [HttpPost]
        [Authorize(Roles = "Empresa,Admin")]
        public async Task<IActionResult> ActualizarEstado(int postulacionId, int ofertaId, string nuevoEstado)
        {
            // 1. Ejecutamos el cambio en la base de datos
            var exito = await _postulacionService.CambiarEstado(postulacionId, nuevoEstado);

            if (exito)
            {
                TempData["Mensaje"] = $"Candidato marcado como: {nuevoEstado} exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo sincronizar el estado en el núcleo AXON.";
            }

            // 💡 LA CLAVE: Nos aseguramos de pasar el 'id' que espera el método 'VerPostulantes'
            // Usamos 'nameof' para evitar errores de escritura
            return RedirectToAction(nameof(VerPostulantes), new { id = ofertaId });
        }
        // 💡 AJUSTE: Permitimos Admin y Candidato
        [Authorize(Roles = "Admin,Candidato")]
        public async Task<IActionResult> VacantesPorEmpresa(int id)
        {
            var lista = await _ofertaService.ListarPorEmpresa(id);
            var empresa = await _empresaService.ObtenerPorEmpresa(id);

            ViewBag.NombreEmpresa = empresa?.NombreEmpresa ?? "Nodo Empresarial";
            ViewBag.EmpresaID = id;

            // 🧐 LÓGICA DE DESPACHO:
            if (User.IsInRole("Admin"))
            {
                // El Admin va a su vista técnica (la de la tabla oscura que ya tienes)
                return View("VacantesPorEmpresa", lista);
            }
            else
            {
                // El Candidato va a la vista de tarjetas (la que te pasé antes)
                // Filtramos para que el candidato solo vea las activas
                var listaActivas = lista.Where(o => o.Estado == "Activa").ToList();
                return View("ListaVacantesCandidato", listaActivas);
            }
        }
    }
}