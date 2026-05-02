using Microsoft.Data.SqlClient; 
using Microsoft.Extensions.Configuration;

namespace Proyecto_API.Helpers
{
    public class Conexion
    {
        private readonly IConfiguration _configuration;

        public Conexion(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }
    }
}