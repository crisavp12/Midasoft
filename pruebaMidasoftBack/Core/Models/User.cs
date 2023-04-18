using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaMidasoftBack.Core.Models
{
    [Table("usuario", Schema = "dbo")]
    public class User
    {
        [Key]
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
    }
}