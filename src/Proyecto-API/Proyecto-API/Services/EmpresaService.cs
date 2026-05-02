using Microsoft.Data.SqlClient;
using Proyecto_API.DAO;
using Proyecto_API.Entidades;
using Proyecto_API.Helpers;

namespace Proyecto_API.Services
{
    public class EmpresaService
    {        
            private readonly EmpresaDAO _empresaDAO;
            private readonly UsuarioDAO _usuarioDAO; // 👈 Necesitamos al vecino
            private readonly Conexion _conexion;      // 👈 Y la llave de la casa

            public EmpresaService(EmpresaDAO empresaDAO, UsuarioDAO usuarioDAO, Conexion conexion)
            {
                _empresaDAO = empresaDAO;
                _usuarioDAO = usuarioDAO;
                _conexion = conexion;
            }

        public string RegistrarPartnerCompleto(RegistroEmpresaVM modelo)
        {
            // 1. Validaciones de Negocio previas
            if (string.IsNullOrEmpty(modelo.RUC) || modelo.RUC.Length != 11)
                return "El RUC debe tener exactamente 11 dígitos.";

            if (string.IsNullOrEmpty(modelo.Email))
                return "El correo electrónico es obligatorio.";

            // 2. Iniciamos la sinapsis con la base de datos
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                cn.Open();
                SqlTransaction tra = cn.BeginTransaction(); // 👈 El "Pacto de Sangre"

                try
                {
                    // PASO A: Crear el Usuario de acceso
                    var nuevoUsuario = new Usuario
                    {
                        NombreCompleto = modelo.NombreCompleto,
                        Email = modelo.Email,
                        PasswordHash = modelo.Password, // Deberías hashearla aquí si tienes el helper
                        Rol = "Empresa"
                    };

                    // Llamamos al UsuarioDAO pasando la transacción
                    int usuarioId = _usuarioDAO.RegistrarUsuarioConTransaccion(nuevoUsuario, cn, tra);

                    if (usuarioId <= 0) throw new Exception("No se pudo generar el perfil de acceso.");

                    // PASO B: Crear la Ficha de Empresa vinculada
                    var nuevaEmpresa = new Empresa
                    {
                        UsuarioID = usuarioId,
                        NombreEmpresa = modelo.NombreEmpresa,
                        RUC = modelo.RUC,
                        SitioWeb = modelo.SitioWeb
                    };

                    // Llamamos al EmpresaDAO pasando la transacción
                    int empresaId = _empresaDAO.RegistrarEmpresaConTransaccion(nuevaEmpresa, cn, tra);

                    if (empresaId <= 0) throw new Exception("Error al vincular los datos corporativos.");

                    // FINAL: Si llegamos aquí, todo fue un éxito
                    tra.Commit();
                    return "OK";
                }
                catch (Exception ex)
                {
                    tra.Rollback(); // 🚩 Si algo falló (correo duplicado, RUC inválido), borramos TODO.
                    return "Falla en la sincronización: " + ex.Message;
                }
            }
        }

        // 1. REGISTRAR (Con validaciones de negocio)
        public string Registrar(Empresa obj)
        {
            // 💡 Regla AXON: Validación estricta de RUC
            if (string.IsNullOrEmpty(obj.RUC) || obj.RUC.Length != 11)
                return "El RUC debe tener exactamente 11 dígitos.";

            if (string.IsNullOrEmpty(obj.NombreEmpresa))
                return "El nombre de la empresa es obligatorio.";

            try
            {
                int resultado = _empresaDAO.RegistrarEmpresa(obj);
                return resultado > 0 ? "OK" : "No se pudo completar el registro corporativo.";
            }
            catch (Exception ex)
            {
                return "Error en el nodo central: " + ex.Message;
            }
        }

        // 2. OBTENER POR USUARIO ID
        public Empresa? ObtenerPorUsuario(int usuarioId)
        {
            if (usuarioId <= 0) return null;
            return _empresaDAO.ObtenerPorUsuario(usuarioId);
        }

        // 2.1 OBTENER POR EMPRESA ID
        public Empresa? ObtenerPorEmpresa(int empresaId)
        {
            if (empresaId <= 0) return null;
            return _empresaDAO.ObtenerPorEmpresa(empresaId);
        }

        // 3. ACTUALIZAR
        public bool Actualizar(Empresa obj)
        {
            if (obj.EmpresaID <= 0) return false;

            // 💡 Evitamos descripciones vacías o demasiado cortas
            if (string.IsNullOrEmpty(obj.Descripcion) || obj.Descripcion.Length < 10)
                return false;

            return _empresaDAO.ActualizarEmpresa(obj);
        }

        // 4. LISTAR (Paginado)
        public List<Empresa> Listar(int pagina)
        {
            int tamanoPagina = 10;
            if (pagina <= 0) pagina = 1;
            return _empresaDAO.ListarEmpresas(pagina, tamanoPagina);
        }

        // 5. ELIMINAR (Toggle Lógico)
        public bool Eliminar(int id)
        {
            if (id <= 0) return false;
            return _empresaDAO.AlternarEstado(id);
        }
    }
}