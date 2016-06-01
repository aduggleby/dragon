using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Dragon.SecurityServer.ChainedIdentity.Stores
{
    public class RoleStore<TRole> : IQueryableRoleStore<TRole>
        where TRole : class, IRole
    {
        private readonly List<RoleStore<TRole>> _roleStores;

        public RoleStore(List<RoleStore<TRole>> roleStores)
        {
            _roleStores = roleStores;
        }

        public void Dispose()
        {
            _roleStores.ForEach(x => x.Dispose());
        }

        public Task CreateAsync(TRole role)
        {
            _roleStores.ForEach(x => x.CreateAsync(role));
            return Task.FromResult<object>(null);
        }

        public Task UpdateAsync(TRole role)
        {
            _roleStores.ForEach(x => x.UpdateAsync(role));
            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TRole role)
        {
             _roleStores.ForEach(x => x.DeleteAsync(role));
            return Task.FromResult<object>(null);
        }

        public Task<TRole> FindByIdAsync(string roleId)
        {
             return _roleStores.First().FindByIdAsync(roleId);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            return _roleStores.First().FindByNameAsync(roleName);
        }

        public IQueryable<TRole> Roles { get; private set; }

        public Task DeleteAllAsync()
        {
            _roleStores.ForEach(x => x.DeleteAllAsync());
            return Task.FromResult<object>(null);
        }

        private bool UsesCache()
        {
            return _roleStores.Count > 1;
        }

        public Task ClearCache()
        {
            if (!UsesCache()) return Task.FromResult<object>(null);
            var cacheRoleStore = _roleStores.First();
            var roleStore = _roleStores[1];
            cacheRoleStore.DeleteAllAsync();
            foreach (var role in roleStore.Roles)
            {
                cacheRoleStore.CreateAsync(role);
            }
            return Task.FromResult<object>(null);
        }
    }
}
