using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaMidasoftBack.Core.DTOs
{
    public class UserDTO : UsernameDTO
    {
        [Required]
        public string Contrasena { get; set; }
    }
}
