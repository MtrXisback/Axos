using Microsoft.Data.SqlClient;
using System.Data;
using Proyecto_API.Entidades;
using Proyecto_API.Helpers;

namespace Proyecto_API.DAO
{
    public class PostulacionDAO
    {
        private readonly Conexion _conexion;

        public PostulacionDAO(Conexion conexion)
        {
            _conexion = conexion;
        }

        // 1. REGISTRAR POSTULACIÓN
        public int Registrar(Postulacion obj)
        {
            int idGenerado = 0;
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Postulaciones_Registrar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OfertaID", obj.OfertaID);
                cmd.Parameters.AddWithValue("@UsuarioID", obj.UsuarioID);
                cmd.Parameters.AddWithValue("@CV_AdjuntoURL", obj.CV_AdjuntoURL);

                cn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null) idGenerado = Convert.ToInt32(result);
            }
            return idGenerado;
        }

        // 2. ACTUALIZAR ESTADO (Aceptado, Rechazado, etc.)
        public bool ActualizarEstado(int postulacionId, string nuevoEstado)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Postulaciones_ActualizarEstado", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PostulacionID", postulacionId);
                cmd.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
                cn.Open();
                return cmd.ExecuteNonQuery() != 0;
            }
        }

        // 3. LISTAR POSTULACIONES POR OFERTA (Para la Empresa)
        public List<Postulacion> ListarPorOferta(int ofertaId)
        {
            List<Postulacion> lista = new List<Postulacion>();
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Postulaciones_ListarPorOferta", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OfertaID", ofertaId);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Postulacion
                        {
                            PostulacionID = Convert.ToInt32(dr["PostulacionID"]),                          
                            OfertaID = Convert.ToInt32(dr["OfertaID"]),
                            UsuarioID = Convert.ToInt32(dr["UsuarioID"]),
                            CandidatoNombre = dr["CandidatoNombre"].ToString()!,                           
                            FechaPostulacion = Convert.ToDateTime(dr["FechaPostulacion"]),
                            EstadoPostulacion = dr["EstadoPostulacion"].ToString()!,
                            CV_AdjuntoURL = dr["CV_AdjuntoURL"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }

        // 4. ELIMINAR / CANCELAR POSTULACIÓN (Borrado Lógico)
        public bool Eliminar(int id)
        {
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Postulaciones_Eliminar", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PostulacionID", id);
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        
        // 3.1 LISTAR POSTULACIONES POR USUARIO (Sincronizado con tu Entidad)
        public List<Postulacion> ListarPorUsuario(int usuarioId)
        {
            List<Postulacion> lista = new List<Postulacion>();
            using (SqlConnection cn = new SqlConnection(_conexion.GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("USP_Postulaciones_ListarPorUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UsuarioID", usuarioId);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Postulacion
                        {
                            PostulacionID = Convert.ToInt32(dr["PostulacionID"]),
                            OfertaID = Convert.ToInt32(dr["OfertaID"]),
                            OfertaTitulo = dr["OfertaTitulo"].ToString()!, // <-- Ahora coincide
                            UsuarioID = Convert.ToInt32(dr["UsuarioID"]),
                            CandidatoNombre = dr["CandidatoNombre"].ToString()!, // <-- Coincide
                            FechaPostulacion = Convert.ToDateTime(dr["FechaPostulacion"]),
                            EstadoPostulacion = dr["EstadoPostulacion"].ToString()!,
                            CV_AdjuntoURL = dr["CV_AdjuntoURL"].ToString()!,
                            Activo = Convert.ToBoolean(dr["Activo"])
                        });
                    }
                }
            }
            return lista;
        }
    }
}