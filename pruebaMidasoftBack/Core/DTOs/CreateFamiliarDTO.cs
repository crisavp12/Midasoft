using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaMidasoftBack.Core.DTOs
{
    public class CreateFamiliarDTO
    {
        public int FamiliarId { get; set; }
        public string Cedula { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public int Edad { get; set; }
        public string? Genero { get; set; }
        public string? Parentesco { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}
