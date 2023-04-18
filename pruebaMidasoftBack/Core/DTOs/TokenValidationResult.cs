using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaMidasoftBack.Core.DTOs
{
    //DTO de la verificacion del token
    public class TokenValidationResult
    {
        public bool Success { get; set; }
        public UserDTO UserDTO { get; set; }
    }
}
