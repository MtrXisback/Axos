// ... (Tus usings y namespace se mantienen igual)

using Microsoft.AspNetCore.Mvc;
using Proyecto_API.Entidades;
using Proyecto_API.Services;

[Route("api/[controller]")]
[ApiController]
public class MaestroController : ControllerBase
{
    private readonly MaestroService _service;
    public MaestroController(MaestroService service) => _service = service;

    private IActionResult ProcesarResultado(bool exito, string mensajeExito, string mensajeError)
    {
        if (exito)
            return Ok(new { success = true, message = mensajeExito });

        return BadRequest(new { success = false, message = mensajeError });
    }

    // --- HABILIDADES ---
    [HttpGet("habilidades")]
    public IActionResult ListarHabilidades()
    {
        var lista = _service.ListarHabilidades();
        return Ok(new { success = true, count = lista.Count, data = lista });
    }

    [HttpGet("habilidades/{id}")]
    public IActionResult ObtenerHabilidad(int id)
    {
        var obj = _service.ObtenerHabilidadPorID(id);
        if (obj == null)
            return NotFound(new { success = false, message = $"Habilidad {id} no encontrada." });

        return Ok(new { success = true, data = obj });
    }

    [HttpPost("habilidades")]
    public IActionResult RegistrarHabilidad([FromBody] Habilidad request) // 💡 Usar el modelo es más seguro que un string plano
    {
        var resultado = _service.RegistrarHabilidad(request.Nombre);
        return resultado == "OK"
            ? Created("", new { success = true, message = "Habilidad registrada." })
            : BadRequest(new { success = false, message = "Error al registrar." });
    }

    [HttpPut("habilidades")]
    public IActionResult ActualizarHabilidad([FromBody] Habilidad obj)
        => ProcesarResultado(_service.ActualizarHabilidad(obj), "Habilidad actualizada.", "Error al actualizar.");

    [HttpDelete("habilidades/{id}")]
    public IActionResult EliminarHabilidad(int id)
        => ProcesarResultado(_service.EliminarHabilidad(id), "Estado sincronizado.", "Error al cambiar estado.");


    // --- ESPECIALIDADES ---
    [HttpGet("especialidades")]
    public IActionResult ListarEspecialidades()
    {
        var lista = _service.ListarEspecialidades();
        return Ok(new { success = true, count = lista.Count, data = lista });
    }

    [HttpGet("especialidades/{id}")]
    public IActionResult ObtenerEspecialidad(int id)
    {
        var obj = _service.ObtenerEspecialidadPorID(id);
        if (obj == null)
            return NotFound(new { success = false, message = "Especialidad no encontrada." });

        return Ok(new { success = true, data = obj });
    }

    [HttpPost("especialidades")]
    public IActionResult RegistrarEspecialidad([FromBody] Especialidad request) // 💡 Sincronizado
    {
        var res = _service.RegistrarEspecialidad(request.Nombre);
        return res == "OK"
            ? Created("", new { success = true, message = "Especialidad creada." }) // 💡 Cambiado a Created
            : BadRequest(new { success = false, message = "Error al crear especialidad." });
    }

    [HttpPut("especialidades")]
    public IActionResult ActualizarEspecialidad([FromBody] Especialidad obj)
        => ProcesarResultado(_service.ActualizarEspecialidad(obj), "Especialidad modificada.", "Error al modificar.");

    [HttpDelete("especialidades/{id}")]
    public IActionResult EliminarEspecialidad(int id)
        => ProcesarResultado(_service.EliminarEspecialidad(id), "Estado actualizado.", "Error al cambiar estado.");


    // --- UBICACIONES ---
    [HttpGet("ubicaciones")]
    public IActionResult ListarUbicaciones()
    {
        var lista = _service.ListarUbicaciones();
        return Ok(new { success = true, count = lista.Count, data = lista });
    }

    [HttpGet("ubicaciones/{id}")]
    public IActionResult ObtenerUbicacion(int id)
    {
        var obj = _service.ObtenerUbicacionPorID(id);
        return obj != null
            ? Ok(new { success = true, data = obj })
            : NotFound(new { success = false, message = "Ubicación inexistente." });
    }

    [HttpPost("ubicaciones")]
    public IActionResult RegistrarUbicacion([FromBody] Ubicacion obj)
    {
        var res = _service.RegistrarUbicacion(obj.Ciudad, obj.Pais);
        return res == "OK"
            ? Created("", new { success = true, message = "Nodo geográfico establecido." })
            : BadRequest(new { success = false, message = "Error al insertar ubicación." });
    }

    [HttpPut("ubicaciones")]
    public IActionResult ActualizarUbicacion([FromBody] Ubicacion obj)
        => ProcesarResultado(_service.ActualizarUbicacion(obj), "Ubicación actualizada.", "Error en persistencia.");

    [HttpDelete("ubicaciones/{id}")]
    public IActionResult EliminarUbicacion(int id)
        => ProcesarResultado(_service.EliminarUbicacion(id), "Sede actualizada correctamente.", "Error en cambio de estado.");
}