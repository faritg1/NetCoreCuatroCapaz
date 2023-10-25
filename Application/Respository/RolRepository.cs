using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Persistence.Data;

namespace Application.Respository
{
    public class RolRepository : GenericRepository<Rol>, IRol
    {
        private readonly ShopApiContext _context;

        public RolRepository(ShopApiContext context) : base(context)
        {
            
            _context = context;
        }
    }
}