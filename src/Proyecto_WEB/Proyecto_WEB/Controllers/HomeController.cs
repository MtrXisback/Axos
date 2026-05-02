using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Proyecto_WEB.Models;
using Proyecto_WEB.Services;

namespace Proyecto_WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly OfertaService _ofertaService;
        private readonly MaestroService _maestroService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,
                              OfertaService ofertaService,
                              MaestroService maestroService)
        {
            _logger = logger;
            _ofertaService = ofertaService;
            _maestroService = maestroService;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Buscador");
            }
            return View("Landing");
        }

        public async Task<IActionResult> Buscador(int? especialidadId, string? modalidad, decimal? salarioMin, int pagina = 1)
        {
            ViewBag.Especialidades = await _maestroService.ListarEspecialidades();
            ViewBag.Ubicaciones = await _maestroService.ListarUbicaciones();

            var listaOfertas = await _ofertaService.ListarFiltrado(especialidadId, modalidad, salarioMin, pagina);

            ViewBag.EspSel = especialidadId;
            ViewBag.ModSel = modalidad;
            ViewBag.SalSel = salarioMin;

            return View("Index", listaOfertas);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}