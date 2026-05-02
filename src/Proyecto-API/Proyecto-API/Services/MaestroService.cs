using Proyecto_API.DAO;
using Proyecto_API.Entidades;

namespace Proyecto_API.Services
{
    public class MaestroService
    {
        private readonly MaestroDAO _maestroDAO;

        public MaestroService(MaestroDAO maestroDAO)
        {
            _maestroDAO = maestroDAO;
        }

        // =============================================
        // --- HABILIDADES ---
        // =============================================
        public List<Habilidad> ListarHabilidades() => _maestroDAO.Habilidad_Listar();

        public Habilidad? ObtenerHabilidadPorID(int id) => _maestroDAO.Habilidad_ObtenerPorID(id);

        public string RegistrarHabilidad(string nombre) =>
            _maestroDAO.Habilidad_Insertar(nombre) > 0 ? "OK" : "Error";

        public bool ActualizarHabilidad(Habilidad obj) => _maestroDAO.Habilidad_Actualizar(obj);

        public bool EliminarHabilidad(int id) => _maestroDAO.Habilidad_Eliminar(id);


        // =============================================
        // --- ESPECIALIDADES ---
        // =============================================
        public List<Especialidad> ListarEspecialidades() => _maestroDAO.Especialidad_Listar();

        public Especialidad? ObtenerEspecialidadPorID(int id) => _maestroDAO.Especialidad_ObtenerPorID(id);

        public string RegistrarEspecialidad(string nombre) =>
            _maestroDAO.Especialidad_Insertar(nombre) > 0 ? "OK" : "Error";

        public bool ActualizarEspecialidad(Especialidad obj) => _maestroDAO.Especialidad_Actualizar(obj);

        public bool EliminarEspecialidad(int id) => _maestroDAO.Especialidad_Eliminar(id);


        // =============================================
        // --- UBICACIONES ---
        // =============================================
        public List<Ubicacion> ListarUbicaciones() => _maestroDAO.Ubicacion_Listar();

        public Ubicacion? ObtenerUbicacionPorID(int id) => _maestroDAO.Ubicacion_ObtenerPorID(id);

        public string RegistrarUbicacion(string ciudad, string pais) =>
            _maestroDAO.Ubicacion_Insertar(ciudad, pais) > 0 ? "OK" : "Error";

        public bool ActualizarUbicacion(Ubicacion obj) => _maestroDAO.Ubicacion_Actualizar(obj);

        public bool EliminarUbicacion(int id) => _maestroDAO.Ubicacion_Eliminar(id);
    }
}