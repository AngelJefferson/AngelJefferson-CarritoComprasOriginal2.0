using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Cliente
    {
        public int Registrar(Cliente obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    using (SqlCommand cmd = new SqlCommand("tienda.sp_RegistrarCliente", oconexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Nombres", obj.Nombres ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@Apellidos", obj.Apellidos ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@Correo", obj.Correo ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@Clave", obj.Clave ?? Convert.DBNull);
                        cmd.Parameters.AddWithValue("@Provincia", obj.Provincia ?? Convert.DBNull);

                        cmd.Parameters.Add("@Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;

                        oconexion.Open();
                        cmd.ExecuteNonQuery();

                        idautogenerado = Convert.ToInt32(cmd.Parameters["@Resultado"].Value);
                        Mensaje = cmd.Parameters["@Mensaje"].Value?.ToString() ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                idautogenerado = 0;
                Mensaje = ex.Message;
            }
            return idautogenerado;
        }

        public Cliente Login(string correo, string clave, out string Mensaje)
        {
            Cliente obj = null;
            Mensaje = string.Empty;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    using (SqlCommand cmd = new SqlCommand("tienda.sp_LoginCliente", oconexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Correo", correo ?? string.Empty);
                        cmd.Parameters.AddWithValue("@Clave", clave ?? string.Empty);

                        cmd.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                        oconexion.Open();
                        cmd.ExecuteNonQuery();

                        int idCliente = Convert.ToInt32(cmd.Parameters["@Resultado"].Value);
                        Mensaje = cmd.Parameters["@Mensaje"].Value?.ToString() ?? string.Empty;

                        if (idCliente > 0)
                        {
                            string query = "SELECT IdCliente, Nombres, Apellidos, Correo FROM tienda.CLIENTE WHERE IdCliente = @id";
                            using (SqlCommand cmd2 = new SqlCommand(query, oconexion))
                            {
                                cmd2.CommandType = CommandType.Text;
                                cmd2.Parameters.AddWithValue("@id", idCliente);

                                using (SqlDataReader dr = cmd2.ExecuteReader())
                                {
                                    if (dr.Read())
                                    {
                                        obj = new Cliente()
                                        {
                                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                                            Nombres = dr["Nombres"].ToString(),
                                            Apellidos = dr["Apellidos"].ToString(),
                                            Correo = dr["Correo"].ToString()
                                        };
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                obj = null;
                Mensaje = ex.Message;
            }
            return obj;
        }

        public List<Cliente> Listar()
        {
            List<Cliente> lista = new List<Cliente>();
            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT IdCliente, Nombres, Apellidos, Correo, ISNULL(Provincia, 'No especificada') as Provincia, FechaRegistro FROM tienda.CLIENTE ORDER BY FechaRegistro DESC";

                    using (SqlCommand cmd = new SqlCommand(query, oconexion))
                    {
                        cmd.CommandType = CommandType.Text;
                        oconexion.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lista.Add(new Cliente()
                                {
                                    IdCliente = Convert.ToInt32(dr["IdCliente"]),
                                    Nombres = dr["Nombres"].ToString(),
                                    Apellidos = dr["Apellidos"].ToString(),
                                    Correo = dr["Correo"].ToString(),
                                    Provincia = dr["Provincia"].ToString(),
                                    FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"])
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Cliente>();
            }
            return lista;
        }
    }
}