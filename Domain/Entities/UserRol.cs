using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserRol
    {
        public int UsuarioIdFk { get; set; }
        public User Usuario { get; set; }
        public int RolIdFk { get; set; }
        public Rol Rol { get; set; }
    }
}