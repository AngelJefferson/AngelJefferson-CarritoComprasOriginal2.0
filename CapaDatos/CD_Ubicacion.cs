using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class CD_Ubicacion
    {
        public List<Departamento> ObtenerDepartamentos()
        {
            List<Departamento> lista = new List<Departamento>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("tienda.sp_ObtenerDepartamentos", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Departamento()
                            {
                                IdDepartamento = dr["IdDepartamento"] != DBNull.Value ? dr["IdDepartamento"].ToString() : string.Empty,
                                Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : string.Empty
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Departamento>();
            }

            return lista;
        }

        public List<Provincia> ObtenerProvincias(string idDepartamento)
        {
            List<Provincia> lista = new List<Provincia>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("tienda.sp_ObtenerProvincias", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add("@IdDepartamento", SqlDbType.VarChar, 10).Value = (object)idDepartamento ?? DBNull.Value;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Provincia()
                            {
                                IdProvincia = dr["IdProvincia"] != DBNull.Value ? dr["IdProvincia"].ToString() : string.Empty,
                                Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : string.Empty
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Provincia>();
            }

            return lista;
        }

        public List<Distrito> ObtenerDistritos(string idProvincia)
        {
            List<Distrito> lista = new List<Distrito>();

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    SqlCommand cmd = new SqlCommand("tienda.sp_ObtenerDistritos", oconexion)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.Add("@IdProvincia", SqlDbType.VarChar, 10).Value = (object)idProvincia ?? DBNull.Value;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Distrito()
                            {
                                IdDistrito = dr["IdDistrito"] != DBNull.Value ? dr["IdDistrito"].ToString() : string.Empty,
                                Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : string.Empty
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Distrito>();
            }

            return lista;
        }
    }
}