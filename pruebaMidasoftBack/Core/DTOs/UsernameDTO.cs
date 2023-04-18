using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaMidasoftBack.Core.DTOs
{
    public class UsernameDTO
    {
        [Required]
        [MaxLength(50)]
        public string Usuario { get; set; }
    }
}
