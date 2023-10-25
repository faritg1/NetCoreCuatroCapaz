using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Respository;
using Domain.Interfaces;
using Persistence.Data;

namespace Application.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ShopApiContext _context;
        private IRol _roles; 

        private IUser _users;

        public UnitOfWork(ShopApiContext context)
        {
            _context = context;
        }

        public IRol Roles{
            get{
                if(_roles == null){
                    _roles = new RolRepository(_context);
                }
                return _roles;
            }
        }
        public IUser Users{
            get{
                if(_users == null){
                    _users = new UserRepository(_context);
                }
                return _users;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}