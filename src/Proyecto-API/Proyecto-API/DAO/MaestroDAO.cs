using Microsoft.Data.SqlClient;
using System.Data;
using Proyecto_API.Entidades;
using Proyecto_API.Helpers;

namespace Proyecto_API.DAO
{
    public class MaestroDAO
    {
        private readonly Conexion _conexion;

        public MaestroDAO(Conexion conexion)
        {
            _conexion = conexion;
        }

        // =============================================
        // --- HABILIDADES ---
        // =============================================

        public int Habilidad_Insertar(string nombre)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Habilidades_Insertar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public List<Habilidad> Habilidad_Listar()
        {
            List<Habilidad> lista = new List<Habilidad>();
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Habilidades_Listar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Habilidad
                        {
                            HabilidadID = (int)dr["HabilidadID"],
                            Nombre = dr["Nombre"].ToString()!,
                            Estado = dr["Estado"].ToString()! // 💡 Mapeo del nuevo campo
                        });
                    }
                }
            }
            return lista;
        }

        public Habilidad? Habilidad_ObtenerPorID(int id)
        {
            Habilidad? obj = null;
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Habilidades_ObtenerPorID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@HabilidadID", id);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        obj = new Habilidad
                        {
                            HabilidadID = (int)dr["HabilidadID"],
                            Nombre = dr["Nombre"].ToString()!,
                            Estado = dr["Estado"].ToString()! // 💡 Mapeo del nuevo campo
                        };
                    }
                }
            }
            return obj;
        }

        public bool Habilidad_Actualizar(Habilidad obj)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Habilidades_Actualizar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@HabilidadID", obj.HabilidadID);
                cmd.Parameters.AddWithValue("@Nombre", obj.Nombre);
                cn.Open();
                return cmd.ExecuteNonQuery() != 0;
            }
        }

        public bool Habilidad_Eliminar(int id)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("USP_Habilidades_Eliminar", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@HabilidadID", id);
                    cn.Open();
                    // 💡 Retorna true si se afectó al menos una fila (Toggle Lógico)
                    return cmd.ExecuteNonQuery() != 0;
                }
            }
            catch (Exception) { return false; }
        }

        // =============================================
        // --- ESPECIALIDADES ---
        // =============================================

        public List<Especialidad> Especialidad_Listar()
        {
            List<Especialidad> lista = new List<Especialidad>();
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Especialidades_Listar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Especialidad
                        {
                            EspecialidadID = (int)dr["EspecialidadID"],
                            Nombre = dr["Nombre"].ToString()!,
                            Estado = dr["Estado"].ToString()! // 💡 Mapeo del nuevo campo
                        });
                    }
                }
            }
            return lista;
        }

        public Especialidad? Especialidad_ObtenerPorID(int id)
        {
            Especialidad? obj = null;
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Especialidades_ObtenerPorID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EspecialidadID", id);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        obj = new Especialidad
                        {
                            EspecialidadID = (int)dr["EspecialidadID"],
                            Nombre = dr["Nombre"].ToString()!,
                            Estado = dr["Estado"].ToString()! // 💡 Mapeo del nuevo campo
                        };
                    }
                }
            }
            return obj;
        }

        public int Especialidad_Insertar(string nombre)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Especialidades_Insertar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Especialidad_Actualizar(Especialidad obj)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Especialidades_Actualizar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EspecialidadID", obj.EspecialidadID);
                cmd.Parameters.AddWithValue("@Nombre", obj.Nombre);
                cn.Open();
                return cmd.ExecuteNonQuery() != 0;
            }
        }

        public bool Especialidad_Eliminar(int id)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("USP_Especialidades_Eliminar", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EspecialidadID", id);
                    cn.Open();
                    return cmd.ExecuteNonQuery() != 0;
                }
            }
            catch (Exception) { return false; }
        }

        // =============================================
        // --- UBICACIONES ---
        // =============================================

        public List<Ubicacion> Ubicacion_Listar()
        {
            List<Ubicacion> lista = new List<Ubicacion>();
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Ubicaciones_Listar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Ubicacion
                        {
                            UbicacionID = (int)dr["UbicacionID"],
                            Ciudad = dr["Ciudad"].ToString()!,
                            Pais = dr["Pais"].ToString()!,
                            Estado = dr["Estado"].ToString()! // 💡 Mapeo del nuevo campo
                        });
                    }
                }
            }
            return lista;
        }

        public Ubicacion? Ubicacion_ObtenerPorID(int id)
        {
            Ubicacion? obj = null;
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Ubicaciones_ObtenerPorID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UbicacionID", id);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        obj = new Ubicacion
                        {
                            UbicacionID = (int)dr["UbicacionID"],
                            Ciudad = dr["Ciudad"].ToString()!,
                            Pais = dr["Pais"].ToString()!,
                            Estado = dr["Estado"].ToString()! // 💡 Mapeo del nuevo campo
                        };
                    }
                }
            }
            return obj;
        }

        public int Ubicacion_Insertar(string ciudad, string pais)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Ubicaciones_Insertar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ciudad", ciudad);
                cmd.Parameters.AddWithValue("@Pais", pais);
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Ubicacion_Actualizar(Ubicacion obj)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Ubicaciones_Actualizar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UbicacionID", obj.UbicacionID);
                cmd.Parameters.AddWithValue("@Ciudad", obj.Ciudad);
                cmd.Parameters.AddWithValue("@Pais", obj.Pais);
                cn.Open();
                return cmd.ExecuteNonQuery() != 0;
            }
        }

        public bool Ubicacion_Eliminar(int id)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("USP_Ubicaciones_Eliminar", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UbicacionID", id);
                    cn.Open();
                    return cmd.ExecuteNonQuery() != 0;
                }
            }
            catch (Exception) { return false; }
        }
    }
}