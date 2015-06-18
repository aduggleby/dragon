using System.Threading.Tasks;
using ManagementWeb.Areas.Hmac.Models;
using ManagementWeb.Areas.Hmac.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManagementWeb.Tests.Repositories
{
    /// <summary>
    /// Note: Requires a locally running Hmac Management Service!
    /// Only use with a test database, test data is not rolled back!
    /// </summary>
    [TestClass]
    public abstract class GenericRepositoryIntegrationTest<TRepository, TModel, TKey>
        where TRepository : IGenericRepository<TModel, TKey>, new()
        where TModel : IModel<TKey>
    {
        public string ServiceUrl { get; set; }

        [TestMethod]
        public async Task List_notEmpty_shouldReturnUsers()
        {
            var repository = new TRepository {ServiceUrl = ServiceUrl};
            await repository.Add(CreateElement());

            var actual = await repository.List();

            Assert.IsTrue(actual.Count > 0);
        }

        [TestMethod]
        public async Task Add_validModel_shouldInsertModel()
        {
            var repository = new TRepository {ServiceUrl = ServiceUrl};
            var numElements = (await repository.List()).Count;

            var id = Parse(await repository.Add(CreateElement()));

            Assert.AreEqual(numElements + 1, (await repository.List()).Count);
            Assert.IsTrue(IsIdValid(id));
        }
        
        [TestMethod]
        public async Task Detail_existingModel_shouldReturnDetails()
        {
            var repository = new TRepository {ServiceUrl = ServiceUrl};
            var expected = CreateElement();
            var id = Parse(await repository.Add(expected));
            expected.Id = id;

            var actual = (await repository.Details(id));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task Update_existingModel_shouldUpdateModel()
        {
            var repository = new TRepository {ServiceUrl = ServiceUrl};
            var model = CreateElement();
            var id = Parse(await repository.Add(model));
            model = CreateElement();
            Disable(model);
            model.Id = id;

            await (repository.Edit(model));

            var actual = await repository.Details(model.Id);
            Assert.AreEqual(model, actual);
        }

        [TestMethod]
        public async Task Delete_existingModel_shouldUpdateModel()
        {
            var repository = new TRepository {ServiceUrl = ServiceUrl};
            var model = CreateElement();
            var id = Parse(await repository.Add(model));
            model = CreateElement();
            Disable(model);
            model.Id = id;

            await (repository.Delete(id));

            var actual = await repository.Details(model.Id);

            Assert.AreEqual(null, actual);
        }

        # region helpers

        protected abstract TModel CreateElement();
        protected abstract TKey Parse(string result);
        protected abstract bool IsIdValid(TKey id);
        protected abstract void Disable(TModel model);

        # endregion
    }
}
