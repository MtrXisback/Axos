using Microsoft.AspNetCore.Mvc;
using Proyecto_WEB.Services;
using Proyecto_WEB.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Proyecto_WEB.Controllers
{
    public class AccesoController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly EmpresaService _empresaService;

        public AccesoController(UsuarioService usuarioService, EmpresaService empresaService)
        {
            _usuarioService = usuarioService;
            _empresaService = empresaService;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string clave)
        {
            // 💡 El Service ahora abre la caja ApiResponse<Usuario> y nos da el objeto
            var usuario = await _usuarioService.Login(correo, clave);

            if (usuario != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Rol),
                    new Claim("UsuarioID", usuario.UsuarioID.ToString())
                };

                // 💡 Usamos "CookieAuth" para que coincida con tu Program.cs
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                };

                // 💡 Iniciamos sesión vinculando al esquema "CookieAuth"
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }

            // Si llegamos aquí, el Login falló (Usuario inactivo o datos mal)
            ViewBag.Error = "Credenciales incorrectas o cuenta suspendida, mi rey.";
            return View("Index");
        }

        public async Task<IActionResult> Salir()
        {
            // 💡 Limpiamos la cookie usando el nombre del esquema
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Acceso");
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            // Retorna la vista con el formulario en blanco
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(Usuario obj)
        {
            // 💡 Por defecto, el sistema los registra como Candidatos
            obj.Rol = "Candidato";
            obj.Activo = true;

            // Limpiamos validaciones que no aplican al registro inicial
            ModelState.Remove("UsuarioID");

            if (!ModelState.IsValid) return View(obj);

            // 💡 Llamamos al Service para que guarde en la BD a través de la API
            var exito = await _usuarioService.Registrar(obj);

            if (exito)
            {
                TempData["Mensaje"] = "¡Registro exitoso! Ya puedes iniciar sesión en AXON.";
                return RedirectToAction("Index"); // Nos manda al Login (Index)
            }

            ViewBag.Error = "No se pudo completar el registro. ¿Quizás el correo ya existe?";
            return View(obj);
        }

        // --- REGISTRO PARA EMPRESAS ---
        [HttpGet]
        public IActionResult RegistrarEmpresa()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarEmpresa(RegistroEmpresaVM modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

           

            var exito = await _empresaService.RegistrarNuevoPartner(modelo);

            if (exito)
            {
                TempData["Mensaje"] = "Registro corporativo exitoso.";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Error al vincular el RUC con la cuenta de usuario.";
            return View(modelo);
        }
    }
}