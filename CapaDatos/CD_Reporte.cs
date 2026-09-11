using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Reporte
    {
        public List<Reporte> Ventas(string fechainicio, string fechafin, string idtransaccion)
        {
            List<Reporte> lista = new List<Reporte>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("tienda.sp_ReporteVentas", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Tipado explícito de parámetros para optimizar consultas en SQL Server
                    cmd.Parameters.Add("@fechainicio", SqlDbType.VarChar, 10).Value = (object)fechainicio ?? DBNull.Value;
                    cmd.Parameters.Add("@fechafin", SqlDbType.VarChar, 10).Value = (object)fechafin ?? DBNull.Value;
                    cmd.Parameters.Add("@idtransaccion", SqlDbType.VarChar, 50).Value = (object)idtransaccion ?? DBNull.Value;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Reporte()
                            {
                                FechaVenta = dr["FechaVenta"] != DBNull.Value ? dr["FechaVenta"].ToString() : string.Empty,
                                Cliente = dr["Cliente"] != DBNull.Value ? dr["Cliente"].ToString() : string.Empty,
                                Producto = dr["Producto"] != DBNull.Value ? dr["Producto"].ToString() : string.Empty,
                                Precio = dr["Precio"] != DBNull.Value ? Convert.ToDecimal(dr["Precio"]) : 0m,
                                Cantidad = dr["Cantidad"] != DBNull.Value ? dr["Cantidad"].ToString() : "0",
                                Total = dr["Total"] != DBNull.Value ? Convert.ToDecimal(dr["Total"]) : 0m,
                                IdTransaccion = dr["IdTransaccion"] != DBNull.Value ? dr["IdTransaccion"].ToString() : string.Empty
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                lista = new List<Reporte>();
            }

            return lista;
        }

        public DashBoard VerDashBoard()
        {
            DashBoard objeto = new DashBoard();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("tienda.sp_ReporteDashboard", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // Se reemplaza 'while' por 'if' para evitar evaluar iteraciones innecesarias si solo retorna 1 fila
                        if (dr.Read())
                        {
                            objeto = new DashBoard()
                            {
                                TotalCliente = dr["TotalCliente"] != DBNull.Value ? Convert.ToInt32(dr["TotalCliente"]) : 0,
                                TotalVenta = dr["TotalVenta"] != DBNull.Value ? Convert.ToInt32(dr["TotalVenta"]) : 0,
                                TotalProducto = dr["TotalProducto"] != DBNull.Value ? Convert.ToInt32(dr["TotalProducto"]) : 0
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                objeto = new DashBoard();
            }

            return objeto;
        }
    }
}