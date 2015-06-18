using System.Collections.Generic;
using System.Threading.Tasks;
using ManagementWeb.Areas.Hmac.Models;

namespace ManagementWeb.Areas.Hmac.Repositories
{
    public interface IGenericRepository<TModel, in TKey> where TModel : IModel<TKey>
    {
        string ServiceUrl { get; set; }

        Task<IList<TModel>> List();
        Task<string> Add(TModel model);
        Task<TModel> Details(TKey id);
        Task Edit(TModel model);
        Task Delete(TKey id);
    }
}