using Microsoft.Data.SqlClient;
using System.Data;
using Proyecto_API.Entidades;
using Proyecto_API.Helpers;

namespace Proyecto_API.DAO
{
    public class UsuarioDAO
    {
        private readonly Conexion _conexion;
        public UsuarioDAO(Conexion conexion) => _conexion = conexion;

        // 1. REGISTRAR USUARIO
        public string RegistrarUsuario(Usuario obj)
        {
            string respuesta = "";
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                // 💡 Usamos ExecuteScalar para obtener el ID o un mensaje de error del SP
                SqlCommand cmd = new SqlCommand("USP_Usuarios_Insertar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", obj.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", obj.PasswordHash);
                cmd.Parameters.AddWithValue("@NombreCompleto", obj.NombreCompleto);
                cmd.Parameters.AddWithValue("@Rol", obj.Rol);

                try
                {
                    cn.Open();
                    var result = cmd.ExecuteScalar();
                    respuesta = (result != null) ? "OK" : "ERROR";
                }
                catch (Exception ex) { respuesta = ex.Message; }
            }
            return respuesta;
        }

        // 2. VALIDAR LOGIN (Clave para seguridad)
        public Usuario? ValidarLogin(string email)
        {
            Usuario? usuario = null;
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Usuarios_ValidarLogin", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        usuario = new Usuario
                        {
                            UsuarioID = Convert.ToInt32(dr["UsuarioID"]),
                            Email = dr["Email"].ToString()!,
                            PasswordHash = dr["PasswordHash"].ToString()!,
                            NombreCompleto = dr["NombreCompleto"].ToString()!,
                            Rol = dr["Rol"].ToString()!,
                            Activo = Convert.ToBoolean(dr["Activo"]) // 💡 Vital para bloquear login si está inactivo
                        };
                    }
                }
            }
            return usuario;
        }

        // 3. LISTAR USUARIOS (Con Activo para la UI)
        public List<Usuario> ListarUsuarios(int pagina, int tamanoPagina)
        {
            List<Usuario> lista = new List<Usuario>();
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Usuarios_Listar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Pagina", pagina);
                cmd.Parameters.AddWithValue("@TamanoPagina", tamanoPagina);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Usuario
                        {
                            UsuarioID = Convert.ToInt32(dr["UsuarioID"]),
                            Email = dr["Email"].ToString()!,
                            NombreCompleto = dr["NombreCompleto"].ToString()!,
                            Rol = dr["Rol"].ToString()!,
                            Activo = Convert.ToBoolean(dr["Activo"])
                        });
                    }
                }
            }
            return lista;
        }

        // 4. ACTUALIZAR USUARIO (Refinado a bool)
        public bool ActualizarUsuario(Usuario obj)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Usuarios_Actualizar", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UsuarioID", obj.UsuarioID);
                cmd.Parameters.AddWithValue("@NombreCompleto", obj.NombreCompleto);
                cmd.Parameters.AddWithValue("@Email", obj.Email);
                

                cn.Open();
                return cmd.ExecuteNonQuery() != 0;
            }
        }

        // 5. ELIMINAR USUARIO (Toggle Lógico)
        public bool EliminarUsuario(int id)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Usuarios_Eliminar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UsuarioID", id);
                cn.Open();
                return cmd.ExecuteNonQuery() != 0;
            }
        }

        // 6. OBTENER POR ID
        public Usuario? ObtenerPorID(int id)
        {
            Usuario? usuario = null;
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Usuarios_ObtenerPorID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UsuarioID", id);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        usuario = new Usuario
                        {
                            UsuarioID = Convert.ToInt32(dr["UsuarioID"]),
                            Email = dr["Email"].ToString()!,
                            NombreCompleto = dr["NombreCompleto"].ToString()!,
                            Rol = dr["Rol"].ToString()!,
                            Activo = Convert.ToBoolean(dr["Activo"])
                        };
                    }
                }
            }
            return usuario;
        }

        // 7. REGISTRAR USUARIO CON TRANSACCIÓN (Para el registro doble)
        public int RegistrarUsuarioConTransaccion(Usuario obj, SqlConnection cn, SqlTransaction tra)
        {
            int idGenerado = 0;
            // 💡 Pasamos la conexión y la transacción al comando
            SqlCommand cmd = new SqlCommand("USP_Usuarios_Insertar", cn, tra);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", obj.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", obj.PasswordHash);
            cmd.Parameters.AddWithValue("@NombreCompleto", obj.NombreCompleto);
            cmd.Parameters.AddWithValue("@Rol", obj.Rol);

            // ExecuteScalar devuelve el ID generado por el SCOPE_IDENTITY() del SP
            var result = cmd.ExecuteScalar();
            if (result != null)
            {
                idGenerado = Convert.ToInt32(result);
            }

            return idGenerado;
        }
    }
}