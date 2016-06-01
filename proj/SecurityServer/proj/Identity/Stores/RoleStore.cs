using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dragon.Data.Interfaces;
using Microsoft.AspNet.Identity;

namespace Dragon.SecurityServer.Identity.Stores
{
    /// <summary>
    /// See <see href="http://www.asp.net/identity/overview/extensibility/overview-of-custom-storage-providers-for-aspnet-identity">asp.net</see>
    /// </summary>
    public class RoleStore<TRole> : IQueryableRoleStore<TRole>
        where TRole : class, IRole
    {
        private IRepository<TRole> RoleRepository { get; set; }

        public RoleStore(IRepository<TRole> roleRepository)
        {
            RoleRepository = roleRepository;
        }

        public void Dispose()
        {
            // nothing to be done
        }

        public Task CreateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            RoleRepository.Insert(role);

            return Task.FromResult<object>(null);
        }

        public Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            RoleRepository.Update(role);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            RoleRepository.Delete(role);

            return Task.FromResult<object>(null);
        }

        public Task<TRole> FindByIdAsync(string roleId)
        {
            var result = RoleRepository.Get(roleId);

            return Task.FromResult(result);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            var result = RoleRepository.GetByWhere(new Dictionary<string, object>{{"Name", roleName}}).ToList();
            return result.Count() > 1 ? Task.FromResult(result.First()) : Task.FromResult<TRole>(null);
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public IQueryable<TRole> Roles { get; private set; }
    }
}
