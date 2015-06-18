using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using ManagementWeb.Areas.Hmac.Controllers;
using ManagementWeb.Areas.Hmac.Models;
using ManagementWeb.Areas.Hmac.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ManagementWeb.Tests.Controllers
{
    [TestClass]
    public abstract class GenericControllerTest<TController, TModel, TKey>
        where TModel : class, IModel<TKey>, new() where TController : GenericController<TModel, TKey>, new()
    {
        [TestMethod]
        public async Task List_repositoryNotEmpty_shouldReturnElements()
        {
            IList<TModel> expected = new List<TModel> {CreateElement(), CreateElement()};
            var mockRepository = new Mock<IGenericRepository<TModel, TKey>> ();
            mockRepository.Setup(x => x.List()).Returns(Task.FromResult(expected));
            var controller = new TController {Repository = mockRepository.Object};

            var actual = ((ViewResult)(await controller.Index())).Model as List<TModel>;
            
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task Details_elementExists_shouldReturnDetails()
        {
            var expected = CreateElement();
            var mockRepository = new Mock<IGenericRepository<TModel, TKey>> ();
            mockRepository.Setup(x => x.Details(expected.Id)).Returns(Task.FromResult(expected));
            var controller = new TController {Repository = mockRepository.Object};

            var actual = ((ViewResult)(await controller.Details(expected.Id))).Model as TModel;
            
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task Edit_elementExists_shouldUpdateDetails()
        {
            var element = CreateElement();
            var expected = CreateElement();
            var mockRepository = new Mock<IGenericRepository<TModel, TKey>> ();
            mockRepository.Setup(x => x.Details(element.Id)).Returns(Task.FromResult(element));
            var controller = new TController {Repository = mockRepository.Object};

            await controller.Edit(element.Id, expected);

            mockRepository.Verify(x => x.Edit(expected), Times.Once());
        }

        [TestMethod]
        public async Task Delete_elementExists_shouldDelete()
        {
            var element = CreateElement();
            var mockRepository = new Mock<IGenericRepository<TModel, TKey>> ();
            var controller = new TController {Repository = mockRepository.Object};

            await controller.Delete(element.Id, element);

            mockRepository.Verify(x => x.Delete(element.Id), Times.Once());
        }

        protected abstract TModel CreateElement();
    }
}
