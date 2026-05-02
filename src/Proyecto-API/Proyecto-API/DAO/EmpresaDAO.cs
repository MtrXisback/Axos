using Microsoft.Data.SqlClient;
using System.Data;
using Proyecto_API.Entidades;
using Proyecto_API.Helpers;

namespace Proyecto_API.DAO
{
    public class EmpresaDAO
    {
        private readonly Conexion _conexion;
        public EmpresaDAO(Conexion conexion) => _conexion = conexion;

        // 1. REGISTRAR EMPRESA
        public int RegistrarEmpresa(Empresa obj)
        {
            int idGenerado = 0;
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Empresas_Insertar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UsuarioID", obj.UsuarioID);
                cmd.Parameters.AddWithValue("@NombreEmpresa", obj.NombreEmpresa);
                cmd.Parameters.AddWithValue("@RUC", obj.RUC);
                cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SitioWeb", obj.SitioWeb ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LogoURL", obj.LogoURL ?? (object)DBNull.Value);

                cn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null) idGenerado = Convert.ToInt32(result);
            }
            return idGenerado;
        }

        public int RegistrarEmpresaConTransaccion(Empresa obj, SqlConnection cn, SqlTransaction tra)
        {
            SqlCommand cmd = new SqlCommand("USP_Empresas_Insertar", cn, tra);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UsuarioID", obj.UsuarioID);
            cmd.Parameters.AddWithValue("@NombreEmpresa", obj.NombreEmpresa);
            cmd.Parameters.AddWithValue("@RUC", obj.RUC);

            // 💡 Para los campos que no pides en el registro, mandas DBNull
            cmd.Parameters.AddWithValue("@Descripcion", (object)obj.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SitioWeb", (object)obj.SitioWeb ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LogoURL", (object)obj.LogoURL ?? DBNull.Value);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }


        // 2. OBTENER POR USUARIO ID (Ajustado)
        public Empresa? ObtenerPorUsuario(int usuarioId)
        {
            Empresa? empresa = null;
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Empresas_ObtenerPorUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UsuarioID", usuarioId);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        empresa = MapearEmpresa(dr);
                    }
                }
            }
            return empresa;
        }

        // 2.1 OBTENER POR EMPRESA ID
        public Empresa? ObtenerPorEmpresa(int empresaId)
        {
            Empresa? empresa = null;
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Empresas_ObtenerPorID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpresaID", empresaId);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        empresa = MapearEmpresa(dr);
                    }
                }
            }
            return empresa;
        }

        // 3. ACTUALIZAR
        public bool ActualizarEmpresa(Empresa obj)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Empresas_Actualizar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpresaID", obj.EmpresaID);
                cmd.Parameters.AddWithValue("@NombreEmpresa", obj.NombreEmpresa);
                cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SitioWeb", obj.SitioWeb ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LogoURL", obj.LogoURL ?? (object)DBNull.Value);

                cn.Open();
                return cmd.ExecuteNonQuery() != 0; // 💡 Aplicamos la "vacuna" del != 0
            }
        }

        // 4. LISTAR
        public List<Empresa> ListarEmpresas(int pagina, int tamanoPagina)
        {
            List<Empresa> lista = new List<Empresa>();
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Empresas_Listar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Pagina", pagina);
                cmd.Parameters.AddWithValue("@TamanoPagina", tamanoPagina);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(MapearEmpresa(dr));
                    }
                }
            }
            return lista;
        }

        // 5. ALTERNAR ESTADO (Toggle)
        public bool AlternarEstado(int empresaId)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Empresas_Eliminar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpresaID", empresaId);
                cn.Open();
                return cmd.ExecuteNonQuery() != 0;
            }
        }

        // 💡 MÉTODO HELPER: Para no repetir código de mapeo
        private Empresa MapearEmpresa(SqlDataReader dr)
        {
            return new Empresa
            {
                EmpresaID = Convert.ToInt32(dr["EmpresaID"]),
                UsuarioID = Convert.ToInt32(dr["UsuarioID"]),
                NombreEmpresa = dr["NombreEmpresa"]?.ToString() ?? "",
                RUC = dr["RUC"]?.ToString() ?? "",
                Descripcion = dr["Descripcion"]?.ToString() ?? "",
                SitioWeb = dr["SitioWeb"]?.ToString() ?? "",
                LogoURL = dr["LogoURL"]?.ToString() ?? "",
                Activo = dr["Activo"] != DBNull.Value && Convert.ToBoolean(dr["Activo"]) // 💡 Mapeo seguro
            };
        }
    }
}