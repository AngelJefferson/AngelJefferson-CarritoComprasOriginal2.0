using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace CapaDatos
{
    public static class Conexion
    {
        // Propiedad de solo lectura con comprobación contra valores nulos
        public static string cn => ConfigurationManager.ConnectionStrings["cadena"]?.ConnectionString ?? string.Empty;
    }
}
