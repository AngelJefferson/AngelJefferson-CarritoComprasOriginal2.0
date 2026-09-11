using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CapaDatos
{
    public class CD_Producto
    {
        public List<Producto> Listar()
        {
            List<Producto> lista = new List<Producto>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT p.IdProducto, p.Nombre, p.Descripcion, m.IdMarca, m.Descripcion AS DesMarca,");
                    query.AppendLine("c.IdCategoria, c.Descripcion AS DesCategoria, p.Precio, p.Stock, p.RutaImagen,");
                    query.AppendLine("p.NombreImagen, p.Activo");
                    query.AppendLine("FROM tienda.PRODUCTO p");
                    query.AppendLine("LEFT JOIN tienda.MARCA m ON m.IdMarca = p.IdMarca");
                    query.AppendLine("LEFT JOIN tienda.CATEGORIA c ON c.IdCategoria = p.IdCategoria");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion)
                    {
                        CommandType = CommandType.Text
                    };

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Producto()
                            {
                                IdProducto = dr["IdProducto"] != DBNull.Value ? Convert.ToInt32(dr["IdProducto"]) : 0,
                                Nombre = dr["Nombre"] != DBNull.Value ? dr["Nombre"].ToString() : string.Empty,
                                Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : string.Empty,
                                oMarca = new Marca()
                                {
                                    IdMarca = dr["IdMarca"] != DBNull.Value ? Convert.ToInt32(dr["IdMarca"]) : 0,
                                    Descripcion = dr["DesMarca"] != DBNull.Value ? dr["DesMarca"].ToString() : string.Empty
                                },
                                oCategoria = new Categoria()
                                {
                                    IdCategoria = dr["IdCategoria"] != DBNull.Value ? Convert.ToInt32(dr["IdCategoria"]) : 0,
                                    Descripcion = dr["DesCategoria"] != DBNull.Value ? dr["DesCategoria"].ToString() : string.Empty
                                },
                                Precio = dr["Precio"] != DBNull.Value ? Convert.ToDecimal(dr["Precio"]) : 0m,
                                Stock = dr["Stock"] != DBNull.Value ? Convert.ToInt32(dr["Stock"]) : 0,
                                RutaImagen = dr["RutaImagen"] != DBNull.Value ? dr["RutaImagen"].ToString() : string.Empty,
                                NombreImagen = dr["NombreImagen"] != DBNull.Value ? dr["NombreImagen"].ToString() : string.Empty,
                                Activo = dr["Activo"] != DBNull.Value ? Convert.ToBoolean(dr["Activo"]) : false
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Lanza el error real si hay fallas con Azure SQL para poder identificar la causa exacta
                    throw new Exception("Error en CD_Producto.Listar: " + ex.Message, ex);
                }
            }

            return lista;
        }

        public int Registrar(Producto obj, out string Mensaje)
        {
            int idProductoGenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("tienda.SP_EditarProducto", oconexion) // Se apunta al procedimiento dentro del esquema tienda
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = (object)obj.Nombre ?? DBNull.Value;
                    cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 500).Value = (object)obj.Descripcion ?? DBNull.Value;
                    cmd.Parameters.Add("@IdMarca", SqlDbType.Int).Value = obj.oMarca != null ? obj.oMarca.IdMarca : 0;
                    cmd.Parameters.Add("@IdCategoria", SqlDbType.Int).Value = obj.oCategoria != null ? obj.oCategoria.IdCategoria : 0;
                    cmd.Parameters.Add("@Precio", SqlDbType.Decimal).Value = obj.Precio;
                    cmd.Parameters.Add("@Stock", SqlDbType.Int).Value = obj.Stock;
                    cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = obj.Activo;

                    SqlParameter pResult = new SqlParameter("@Resultado", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    SqlParameter pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(pResult);
                    cmd.Parameters.Add(pMensaje);

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    idProductoGenerado = pResult.Value != DBNull.Value ? Convert.ToInt32(pResult.Value) : 0;
                    Mensaje = pMensaje.Value != DBNull.Value ? pMensaje.Value.ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                idProductoGenerado = 0;
                Mensaje = ex.Message;
            }

            return idProductoGenerado;
        }

        public bool Editar(Producto obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("tienda.SP_EditarProducto", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add("@IdProducto", SqlDbType.Int).Value = obj.IdProducto;
                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = (object)obj.Nombre ?? DBNull.Value;
                    cmd.Parameters.Add("@Descripcion", SqlDbType.VarChar, 500).Value = (object)obj.Descripcion ?? DBNull.Value;
                    cmd.Parameters.Add("@IdMarca", SqlDbType.Int).Value = obj.oMarca != null ? obj.oMarca.IdMarca : 0;
                    cmd.Parameters.Add("@IdCategoria", SqlDbType.Int).Value = obj.oCategoria != null ? obj.oCategoria.IdCategoria : 0;
                    cmd.Parameters.Add("@Precio", SqlDbType.Decimal).Value = obj.Precio;
                    cmd.Parameters.Add("@Stock", SqlDbType.Int).Value = obj.Stock;
                    cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = obj.Activo;

                    SqlParameter pResult = new SqlParameter("@Resultado", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    SqlParameter pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(pResult);
                    cmd.Parameters.Add(pMensaje);

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    resultado = pResult.Value != DBNull.Value && Convert.ToBoolean(pResult.Value);
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

        public bool GuardarDatosImagen(Producto obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "UPDATE tienda.PRODUCTO SET RutaImagen = @RutaImagen, NombreImagen = @NombreImagen WHERE IdProducto = @IdProducto";

                    SqlCommand cmd = new SqlCommand(query, oconexion)
                    {
                        CommandType = CommandType.Text
                    };

                    cmd.Parameters.Add("@RutaImagen", SqlDbType.VarChar, 100).Value = (object)obj.RutaImagen ?? DBNull.Value;
                    cmd.Parameters.Add("@NombreImagen", SqlDbType.VarChar, 100).Value = (object)obj.NombreImagen ?? DBNull.Value;
                    cmd.Parameters.Add("@IdProducto", SqlDbType.Int).Value = obj.IdProducto;

                    oconexion.Open();

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        resultado = true;
                    }
                    else
                    {
                        Mensaje = "No se pudo actualizar la imagen del producto.";
                    }
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
                    SqlCommand cmd = new SqlCommand("tienda.SP_EliminarProducto", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add("@IdProducto", SqlDbType.Int).Value = id;

                    SqlParameter pResult = new SqlParameter("@Resultado", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    SqlParameter pMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(pResult);
                    cmd.Parameters.Add(pMensaje);

                    oconexion.Open();
                    cmd.ExecuteNonQuery();

                    resultado = pResult.Value != DBNull.Value && Convert.ToBoolean(pResult.Value);
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
    }
}