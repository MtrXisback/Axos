using Microsoft.Data.SqlClient;
using System.Data;
using Proyecto_API.Entidades;
using Proyecto_API.Helpers;

namespace Proyecto_API.DAO
{
    public class OfertaTrabajoDAO
    {
        private readonly Conexion _conexion;

        public OfertaTrabajoDAO(Conexion conexion)
        {
            _conexion = conexion;
        }

        // 1. INSERTAR OFERTA
        public int Insertar(OfertaTrabajo obj)
        {
            int idGenerado = 0;
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_OfertasTrabajo_Insertar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpresaID", obj.EmpresaID);
                cmd.Parameters.AddWithValue("@Titulo", obj.Titulo);
                cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                cmd.Parameters.AddWithValue("@EspecialidadID", obj.EspecialidadID);
                cmd.Parameters.AddWithValue("@UbicacionID", obj.UbicacionID);
                cmd.Parameters.AddWithValue("@Modalidad", obj.Modalidad);
                cmd.Parameters.AddWithValue("@SalarioMin", obj.SalarioMin);
                cmd.Parameters.AddWithValue("@SalarioMax", obj.SalarioMax);

                cn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null) idGenerado = Convert.ToInt32(result);
            }
            return idGenerado;
        }

        // 2. OBTENER POR ID (Detalle para la vista del candidato)
        public OfertaTrabajo? ObtenerPorID(int id)
        {
            OfertaTrabajo? oferta = null;
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_OfertasTrabajo_ObtenerPorID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OfertaID", id);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        oferta = new OfertaTrabajo
                        {
                            OfertaID = Convert.ToInt32(dr["OfertaID"]),
                            EmpresaID = Convert.ToInt32(dr["EmpresaID"]),
                            NombreEmpresa = dr["NombreEmpresa"].ToString()!,
                            Titulo = dr["Titulo"].ToString()!,
                            Descripcion = dr["Descripcion"].ToString()!,

                            // ESTAS LÍNEAS SON LAS QUE SALVAN EL ACTUALIZAR
                            EspecialidadID = Convert.ToInt32(dr["EspecialidadID"]),
                            EspecialidadNombre = dr["EspecialidadNombre"].ToString()!,
                            UbicacionID = Convert.ToInt32(dr["UbicacionID"]),
                            Ciudad = dr["Ciudad"].ToString()!,

                            Modalidad = dr["Modalidad"].ToString()!,
                            SalarioMin = Convert.ToDecimal(dr["SalarioMin"]),
                            SalarioMax = Convert.ToDecimal(dr["SalarioMax"]),
                            Estado = dr["Estado"].ToString()!,
                            FechaPublicacion = Convert.ToDateTime(dr["FechaPublicacion"])
                        };
                    }
                }
            }
            return oferta;
        }

        // 3. LISTADO FILTRADO (Reporte con Paginación - Clave para la Rúbrica)
        public List<OfertaTrabajo> ListarFiltrado(int? especialidadId, string? modalidad, decimal? salarioMin, int pagina, int tamanoPagina)
        {
            List<OfertaTrabajo> lista = new List<OfertaTrabajo>();
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_OfertasTrabajo_ListarFiltrado", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Manejo de nulos para que el SP use sus valores por defecto
                cmd.Parameters.AddWithValue("@EspecialidadID", (object?)especialidadId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Modalidad", (object?)modalidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SalarioMin", (object?)salarioMin ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Pagina", pagina);
                cmd.Parameters.AddWithValue("@TamanoPagina", tamanoPagina);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new OfertaTrabajo
                        {
                            OfertaID = Convert.ToInt32(dr["OfertaID"]),
                            Titulo = dr["Titulo"].ToString()!,
                            NombreEmpresa = dr["NombreEmpresa"].ToString()!,
                            Modalidad = dr["Modalidad"].ToString()!,
                            SalarioMin = Convert.ToDecimal(dr["SalarioMin"]),
                            SalarioMax = Convert.ToDecimal(dr["SalarioMax"]),
                            FechaPublicacion = Convert.ToDateTime(dr["FechaPublicacion"])
                        });
                    }
                }
            }
            return lista;
        }

        // 4. ACTUALIZAR ESTADO (Borrado Lógico o Activar)
        public bool Eliminar(int id, string nuevoEstado = "Inactiva")
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_OfertasTrabajo_Eliminar", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@OfertaID", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = nuevoEstado;

                cn.Open();
          
                int filas = cmd.ExecuteNonQuery();
                return filas != 0;
            }
        }
        // 5. ACTUALIZAR OFERTA COMPLETA
        public bool Actualizar(OfertaTrabajo obj)
        {
            bool respuesta = false;
            try
            {
                using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("USP_OfertasTrabajo_Actualizar", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@OfertaID", SqlDbType.Int).Value = obj.OfertaID;
                    cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 150).Value = obj.Titulo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, -1).Value = obj.Descripcion ?? (object)DBNull.Value; 
                    cmd.Parameters.Add("@EspecialidadID", SqlDbType.Int).Value = obj.EspecialidadID;
                    cmd.Parameters.Add("@UbicacionID", SqlDbType.Int).Value = obj.UbicacionID;
                    cmd.Parameters.Add("@Modalidad", SqlDbType.NVarChar, 50).Value = obj.Modalidad ?? "Presencial";
                    cmd.Parameters.Add("@SalarioMin", SqlDbType.Decimal).Value = obj.SalarioMin;
                    cmd.Parameters.Add("@SalarioMax", SqlDbType.Decimal).Value = obj.SalarioMax;
                    cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = obj.Estado ?? "Activa";

                    // Seteamos la precisión manualmente para evitar líos
                    ((SqlParameter)cmd.Parameters["@SalarioMin"]).Precision = 18;
                    ((SqlParameter)cmd.Parameters["@SalarioMin"]).Scale = 2;
                    ((SqlParameter)cmd.Parameters["@SalarioMax"]).Precision = 18;
                    ((SqlParameter)cmd.Parameters["@SalarioMax"]).Scale = 2;

                    cn.Open();
                    int filas = cmd.ExecuteNonQuery();
                    respuesta = (filas != 0);
                }
            }
            catch (SqlException ex)
            {
                // Esto imprime el error detallado en la consola de depuración de VS
                System.Diagnostics.Debug.WriteLine("==== ERROR EN AXON DATABASE ====");
                System.Diagnostics.Debug.WriteLine("Mensaje: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Número de Error SQL: " + ex.Number); // Ej: 547 (FK), 2627 (PK), etc.
                System.Diagnostics.Debug.WriteLine("Procedimiento: " + ex.Procedure);
                System.Diagnostics.Debug.WriteLine("================================");

                // También lo mandamos a la consola estándar por si acaso
                Console.WriteLine($"Error SQL [{ex.Number}]: {ex.Message}");

                return false;
            }
            return respuesta;
        }

        // =============================================
        // MÉTODOS PARA RELACIÓN MUCHOS A MUCHOS (OfertaHabilidades)
        // =============================================

        // 1. ASIGNAR UNA HABILIDAD A LA OFERTA
        public bool AsignarHabilidad(int ofertaId, int habilidadId)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_OfertaHabilidades_Asignar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OfertaID", ofertaId);
                cmd.Parameters.AddWithValue("@HabilidadID", habilidadId);
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // 2. LISTAR HABILIDADES DE UNA OFERTA (Para ver los tags en el detalle)
        public List<Habilidad> ListarHabilidadesPorOferta(int ofertaId)
        {
            List<Habilidad> lista = new List<Habilidad>();
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_OfertaHabilidades_ListarPorOferta", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OfertaID", ofertaId);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Habilidad
                        {
                            HabilidadID = (int)dr["HabilidadID"],
                            Nombre = dr["Nombre"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }

        // 3. LIMPIAR HABILIDADES (Vital para cuando editas la oferta en el Front)
        public bool LimpiarHabilidades(int ofertaId)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_OfertaHabilidades_Limpiar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OfertaID", ofertaId);
                cn.Open();
                return cmd.ExecuteNonQuery() >= 0; // Puede ser 0 si no tenía habilidades antes
            }
        }

        // 4. QUITAR UNA HABILIDAD ESPECÍFICA (USP_OfertaHabilidades_Quitar)
        public bool QuitarHabilidad(int ofertaId, int habilidadId)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_OfertaHabilidades_Quitar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OfertaID", ofertaId);
                cmd.Parameters.AddWithValue("@HabilidadID", habilidadId);
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<OfertaTrabajo> ListarPorEmpresa(int empresaId)
        {
            List<OfertaTrabajo> lista = new List<OfertaTrabajo>();

            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                // Usamos el SP que creaste hace un momento
                SqlCommand cmd = new SqlCommand("USP_OFERTASTRABAJO_LISTARPOREMPRESA", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EMPRESAID", empresaId);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new OfertaTrabajo
                        {
                            OfertaID = Convert.ToInt32(dr["OfertaID"]),
                            EmpresaID = Convert.ToInt32(dr["EmpresaID"]),
                            NombreEmpresa = dr["NombreEmpresa"].ToString()!,
                            Titulo = dr["Titulo"].ToString()!,
                            Descripcion = dr["Descripcion"].ToString()!,                            
                            EspecialidadNombre = dr["EspecialidadNombre"].ToString()!,
                            Ciudad = dr["Ciudad"].ToString()!,
                            Modalidad = dr["Modalidad"].ToString()!,
                            SalarioMin = Convert.ToDecimal(dr["SalarioMin"]),
                            SalarioMax = Convert.ToDecimal(dr["SalarioMax"]),
                            Estado = dr["Estado"].ToString()!,
                            FechaPublicacion = Convert.ToDateTime(dr["FechaPublicacion"])
                        });
                    }
                }
            }
            return lista;
        }
    }
}