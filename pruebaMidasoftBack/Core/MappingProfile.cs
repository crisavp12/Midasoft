using AutoMapper;
using pruebaMidasoftBack.Core.DTOs;
using pruebaMidasoftBack.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaMidasoftBack.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Familiar, FamiliarDTO>();
            CreateMap<User, UserDTO>();
        }
    }
}
