using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Usuarios
    {
        public List<Usuario> Listar()
        {
            List<Usuario> lista = new List<Usuario>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT IdUsuario, Nombre, Apellido, Correo, Clave, Reestablecer, Activo FROM tienda.USUARIO";

                    SqlCommand cmd = new SqlCommand(query, oconexion)
                    {
                        CommandType = CommandType.Text
                    };

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario()
                            {
                                IdUsuario = dr["IdUsuario"] != DBNull.Value ? Convert.ToInt32(dr["IdUsuario"]) : 0,
                                Nombre = dr["Nombre"] != DBNull.Value ? dr["Nombre"].ToString() : string.Empty,
                                Apellido = dr["Apellido"] != DBNull.Value ? dr["Apellido"].ToString() : string.Empty,
                                Correo = dr["Correo"] != DBNull.Value ? dr["Correo"].ToString() : string.Empty,
                                Clave = dr["Clave"] != DBNull.Value ? dr["Clave"].ToString() : string.Empty,
                                Reestablecer = dr["Reestablecer"] != DBNull.Value ? Convert.ToBoolean(dr["Reestablecer"]) : false,
                                Activo = dr["Activo"] != DBNull.Value ? Convert.ToBoolean(dr["Activo"]) : false
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Lanza el error real para poder identificar la falla de conexión o consulta
                throw new Exception("Error en CD_Usuarios.Listar: " + ex.Message, ex);
            }

            return lista;
        }

        public int Registrar(Usuario obj, out string Mensaje)
        {
            int idAutogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("tienda.SP_RegistrarUsuario", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add("@Nombres", SqlDbType.VarChar, 100).Value = (object)obj.Nombre ?? DBNull.Value;
                    cmd.Parameters.Add("@Apellidos", SqlDbType.VarChar, 100).Value = (object)obj.Apellido ?? DBNull.Value;
                    cmd.Parameters.Add("@Correo", SqlDbType.VarChar, 100).Value = (object)obj.Correo ?? DBNull.Value;
                    cmd.Parameters.Add("@Clave", SqlDbType.VarChar, 100).Value = (object)obj.Clave ?? DBNull.Value;
                    cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = obj.Activo;

                    SqlParameter pResultado = new SqlParameter("@Resultado", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    SqlParameter pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(pResultado);
                    cmd.Parameters.Add(pMensaje);

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    idAutogenerado = pResultado.Value != DBNull.Value ? Convert.ToInt32(pResultado.Value) : 0;
                    Mensaje = pMensaje.Value != DBNull.Value ? pMensaje.Value.ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                idAutogenerado = 0;
                Mensaje = ex.Message;
            }

            return idAutogenerado;
        }

        public bool Editar(Usuario obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("tienda.SP_EditarUsuario", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = obj.IdUsuario;
                    cmd.Parameters.Add("@Nombres", SqlDbType.VarChar, 100).Value = (object)obj.Nombre ?? DBNull.Value;
                    cmd.Parameters.Add("@Apellidos", SqlDbType.VarChar, 100).Value = (object)obj.Apellido ?? DBNull.Value;
                    cmd.Parameters.Add("@Correo", SqlDbType.VarChar, 100).Value = (object)obj.Correo ?? DBNull.Value;
                    cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = obj.Activo;

                    SqlParameter pResultado = new SqlParameter("@Resultado", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    SqlParameter pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(pResultado);
                    cmd.Parameters.Add(pMensaje);

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    resultado = pResultado.Value != DBNull.Value && Convert.ToBoolean(pResultado.Value);
                    Mensaje = pMensaje.Value != DBNull.Value ? pMensaje.Value.ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        public bool Eliminar(int id, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "DELETE TOP (1) FROM tienda.USUARIO WHERE IdUsuario = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, oconexion)
                    {
                        CommandType = CommandType.Text
                    };

                    cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = id;

                    oconexion.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        public bool CambiarClave(int idUsuario, string nuevaClave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "UPDATE tienda.USUARIO SET Clave = @NuevaClave, Reestablecer = 0 WHERE IdUsuario = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, oconexion)
                    {
                        CommandType = CommandType.Text
                    };

                    cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
                    cmd.Parameters.Add("@NuevaClave", SqlDbType.VarChar, 100).Value = (object)nuevaClave ?? DBNull.Value;

                    oconexion.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        public bool ReestablecerClave(int idUsuario, string clave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "UPDATE tienda.USUARIO SET Clave = @Clave, Reestablecer = 1 WHERE IdUsuario = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, oconexion)
                    {
                        CommandType = CommandType.Text
                    };

                    cmd.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
                    cmd.Parameters.Add("@Clave", SqlDbType.VarChar, 100).Value = (object)clave ?? DBNull.Value;

                    oconexion.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }
    }
}