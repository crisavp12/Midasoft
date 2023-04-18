using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaMidasoftBack.Core.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string Peticion { get; set; }
        public string Respuesta { get; set; }
        public DateTime Fecha { get; set; }
    }
}
