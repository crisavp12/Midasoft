using pruebaMidasoftBack.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaMidasoftBack.Core.Interfaces
{
    public interface IMiddlewareService
    {
        Task CreateLog(LogDTO logDTO);
    }
}
