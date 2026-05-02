using Microsoft.AspNetCore.Mvc;
using Proyecto_WEB.Services;
using Proyecto_WEB.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Proyecto_WEB.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly EmpresaService _empresaService;

        public UsuarioController(UsuarioService usuarioService, EmpresaService empresaService)
        {
            _usuarioService = usuarioService;
            _empresaService = empresaService;
        }

        // ==========================================
        // 1. GESTIÓN DE PERFIL (LECTURA)
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> MiPerfil()
        {
            var claimId = User.FindFirst("UsuarioID")?.Value;
            if (string.IsNullOrEmpty(claimId)) return RedirectToAction("Salir", "Acceso");

            var usuarioId = int.Parse(claimId);
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if (rol == "Empresa")
            {
                var perfilEmpresa = await _empresaService.ObtenerPerfil(usuarioId);
                if (perfilEmpresa == null) return NotFound();
                return View("MiPerfilEmpresa", perfilEmpresa);
            }
            else
            {
                var perfilUsuario = await _usuarioService.ObtenerPorId(usuarioId);
                if (perfilUsuario == null) return NotFound();
                return View("MiPerfil", perfilUsuario);
            }
        }

        // ==========================================
        // 2. FORMULARIO DE EDICIÓN (GET)
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> ActualizarPerfil()
        {
            var claimId = User.FindFirst("UsuarioID")?.Value;
            if (string.IsNullOrEmpty(claimId)) return RedirectToAction("Salir", "Acceso");

            var usuarioId = int.Parse(claimId);
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if (rol == "Empresa")
            {
                var perfil = await _empresaService.ObtenerPerfil(usuarioId);
                return View("ActualizarPerfil", perfil);
            }
            else
            {
                var perfil = await _usuarioService.ObtenerPorId(usuarioId);
                return View("ActualizarPerfil", perfil);
            }
        }

        // ==========================================
        // 3. PROCESAR ACTUALIZACIÓN (POST)
        // ==========================================

        // POST: Para Candidatos y Admins
        [HttpPost]
        public async Task<IActionResult> ActualizarPerfil(Usuario obj)
        {
            ModelState.Remove("Password");
            ModelState.Remove("Rol");

            if (!ModelState.IsValid) return View("ActualizarPerfil", obj);

            var exito = await _usuarioService.Actualizar(obj);
            if (exito)
            {
                // 🔥 ACTUALIZAR NAVBAR (COOKIE) AL INSTANTE
                var identity = (ClaimsIdentity)User.Identity;
                var nameClaim = identity.FindFirst(ClaimTypes.Name);
                if (nameClaim != null)
                {
                    identity.RemoveClaim(nameClaim);
                    identity.AddClaim(new Claim(ClaimTypes.Name, obj.NombreCompleto));
                }

                await HttpContext.SignInAsync(
                    "CookieAuth",
                    new ClaimsPrincipal(identity)
                );

                TempData["Mensaje"] = "Perfil actualizado correctamente en la red AXON.";
                return RedirectToAction("MiPerfil");
            }

            TempData["Error"] = "Error de sincronización con el núcleo.";
            return View("ActualizarPerfil", obj);
        }

        // POST: Para Empresas
        [HttpPost]
        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> ActualizarEmpresa(Empresa obj)
        {
            ModelState.Remove("Password");

            var exito = await _empresaService.Actualizar(obj);
            if (exito)
            {
                // 🔥 ACTUALIZAR NAVBAR (COOKIE) AL INSTANTE
                var identity = (ClaimsIdentity)User.Identity;
                var nameClaim = identity.FindFirst(ClaimTypes.Name);
                if (nameClaim != null)
                {
                    identity.RemoveClaim(nameClaim);
                    // Aquí usamos el Nombre Comercial para el NavBar
                    identity.AddClaim(new Claim(ClaimTypes.Name, obj.NombreEmpresa));
                }

                await HttpContext.SignInAsync(
                    "CookieAuth",
                    new ClaimsPrincipal(identity)
                );

                TempData["Mensaje"] = "Ficha corporativa sincronizada con éxito.";
                return RedirectToAction("MiPerfil");
            }

            TempData["Error"] = "No se pudieron actualizar los datos corporativos.";
            return View("ActualizarPerfil", obj);
        }

        // ==========================================
        // 4. GESTIÓN ADMIN (Listados y estados)
        // ==========================================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListadoUsuarios(int pagina = 1)
        {
            var lista = await _usuarioService.ListarTodos(pagina);
            ViewBag.PaginaActual = pagina;
            return View(lista);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var exito = await _usuarioService.Eliminar(id);
            if (exito) TempData["Mensaje"] = "Estado del nodo de usuario actualizado.";
            else TempData["Error"] = "Error al cambiar estado del usuario.";
            return RedirectToAction("ListadoUsuarios");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListadoEmpresas()
        {
            var empresas = await _empresaService.ListarEmpresas();
            return View(empresas);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AlternarEstadoEmpresa(int id)
        {
            var exito = await _empresaService.Eliminar(id);
            if (exito) TempData["Mensaje"] = "Estado del partner corporativo actualizado.";
            else TempData["Error"] = "Error de sincronización empresarial.";
            return RedirectToAction("ListadoEmpresas");
        }

        // ==========================================
        // 5. VISTAS PÚBLICAS/DIRECTORIOS
        // ==========================================

        [Authorize(Roles = "Admin,Candidato")]
        public async Task<IActionResult> DetalleEmpresa(int id)
        {
            var empresa = await _empresaService.ObtenerPorEmpresa(id);
            if (empresa == null) return NotFound();
            return View(empresa);
        }

        [Authorize(Roles = "Candidato")]
        public async Task<IActionResult> DirectorioEmpresas()
        {
            var empresas = await _empresaService.ListarEmpresas();
            return View(empresas ?? new List<Empresa>());
        }
    }
}