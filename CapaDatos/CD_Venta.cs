using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Venta
    {
        public bool RegistrarVenta(Venta obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarVenta", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add("@IdCliente", SqlDbType.Int).Value = obj.IdCliente;
                    cmd.Parameters.Add("@TotalProducto", SqlDbType.Int).Value = obj.TotalProducto;
                    cmd.Parameters.Add("@MontoTotal", SqlDbType.Decimal).Value = obj.MontoTotal;
                    cmd.Parameters.Add("@Contacto", SqlDbType.VarChar, 50).Value = (object)obj.Contacto ?? DBNull.Value;
                    cmd.Parameters.Add("@IdDistrito", SqlDbType.VarChar, 10).Value = (object)obj.IdDistrito ?? DBNull.Value;
                    cmd.Parameters.Add("@Telefono", SqlDbType.VarChar, 50).Value = (object)obj.Telefono ?? DBNull.Value;
                    cmd.Parameters.Add("@Direccion", SqlDbType.VarChar, 500).Value = (object)obj.Direccion ?? DBNull.Value;
                    cmd.Parameters.Add("@IdTransaccion", SqlDbType.VarChar, 50).Value = (object)obj.IdTransaccion ?? DBNull.Value;

                    SqlParameter pResultado = new SqlParameter("@Resultado", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    SqlParameter pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(pResultado);
                    cmd.Parameters.Add(pMensaje);

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(pResultado.Value);
                    Mensaje = pMensaje.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        public Venta ObtenerVenta(string idTransaccion)
        {
            Venta obj = null;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_ObtenerVenta", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add("@IdTransaccion", SqlDbType.VarChar, 50).Value = (object)idTransaccion ?? DBNull.Value;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            obj = new Venta()
                            {
                                IdVenta = Convert.ToInt32(dr["IdVenta"]),
                                IdCliente = Convert.ToInt32(dr["IdCliente"]),
                                TotalProducto = Convert.ToInt32(dr["TotalProducto"]),
                                MontoTotal = dr["MontoTotal"] != DBNull.Value ? Convert.ToDecimal(dr["MontoTotal"]) : 0m,
                                Contacto = dr["Contacto"] != DBNull.Value ? dr["Contacto"].ToString() : string.Empty,
                                Telefono = dr["Telefono"] != DBNull.Value ? dr["Telefono"].ToString() : string.Empty,
                                Direccion = dr["Direccion"] != DBNull.Value ? dr["Direccion"].ToString() : string.Empty,
                                IdTransaccion = dr["IdTransaccion"] != DBNull.Value ? dr["IdTransaccion"].ToString() : string.Empty,
                                FechaTexto = dr["FechaVenta"] != DBNull.Value ? Convert.ToDateTime(dr["FechaVenta"]).ToString("dd/MM/yyyy") : string.Empty
                            };
                        }
                    }
                }
            }
            catch
            {
                obj = null;
            }

            return obj;
        }

        public List<DetalleVenta> DetalleVenta(string idTransaccion)
        {
            List<DetalleVenta> lista = new List<DetalleVenta>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("sp_DetalleVenta", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add("@IdTransaccion", SqlDbType.VarChar, 50).Value = (object)idTransaccion ?? DBNull.Value;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new DetalleVenta()
                            {
                                IdDetalleVenta = Convert.ToInt32(dr["IdDetalleVenta"]),
                                Cantidad = Convert.ToInt32(dr["Cantidad"]),
                                Total = dr["Total"] != DBNull.Value ? Convert.ToDecimal(dr["Total"]) : 0m,
                                oProducto = new Producto()
                                {
                                    Nombre = dr["Nombre"] != DBNull.Value ? dr["Nombre"].ToString() : string.Empty,
                                    Precio = dr["Precio"] != DBNull.Value ? Convert.ToDecimal(dr["Precio"]) : 0m
                                }
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<DetalleVenta>();
            }

            return lista;
        }
    }
}