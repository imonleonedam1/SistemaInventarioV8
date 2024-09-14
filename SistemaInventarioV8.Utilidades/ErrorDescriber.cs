using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarioV8.Utilidades
{
    public class ErrorDescriber: IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError() { 
            Code = nameof(PasswordRequiresLower),
            Description = "La contraseña debe tene al menos una letra minúscula"
            };
        }
    }
}
