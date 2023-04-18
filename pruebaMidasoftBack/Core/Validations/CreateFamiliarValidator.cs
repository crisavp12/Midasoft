using FluentValidation;
using pruebaMidasoftBack.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaMidasoftBack.Core.Validations
{
    public class CreateFamiliarValidator : AbstractValidator<CreateFamiliarDTO>
    {
        public CreateFamiliarValidator()
        {
            RuleFor(f => f.Cedula).NotEmpty().WithMessage("El campo Cedula no puede ser nulo").NotEmpty().WithMessage("El campo {0} no puede estar vacío");
            RuleFor(f => f.Nombres).NotEmpty().WithMessage("El campo Nombres no puede ser nulo").NotEmpty().WithMessage("El campo {0} no puede estar vacío");
            RuleFor(f => f.Apellidos).NotEmpty().WithMessage("El campo Apellidos no puede ser nulo").NotEmpty().WithMessage("El campo {0} no puede estar vacío");
            RuleFor(f => f.Edad).NotEmpty().WithMessage("El campo Edad no puede ser nulo").NotEmpty().WithMessage("El campo {0} no puede estar vacío");
            RuleFor(f => f.FechaNacimiento).NotEmpty().When(f => f.Edad < 18).WithMessage("El campo FechaNacimiento es requerido para menores de edad");
        }

    }
}
