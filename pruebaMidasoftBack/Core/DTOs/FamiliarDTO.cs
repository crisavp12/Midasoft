using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaMidasoftBack.Core.DTOs
{
    public class FamiliarDTO :CreateFamiliarDTO
    {

        [Required(ErrorMessage = "El campo {0} no puede ser nulo", AllowEmptyStrings = false)]
        public string Usuario { get; set; }
        public bool MenorEdad { get; set; }
    }
}
