using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Dragon.SecurityServer.AccountSTS.Controllers;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.SecurityServer.AccountSTS.Test.Controllers
{
    [TestClass]
    public class AccountApiControllerTest
    {
        [TestMethod]
        public void ClearCache_shouldInvokeClearCache()
        {
            var mockUserStore = Mock.Of<IDragonUserStore<AppMember>>();
            var controller = new AccountApiController(mockUserStore, null);

            var result = controller.ClearCache();
            Assert.IsInstanceOfType(result, typeof(OkResult));

            Mock.Get(mockUserStore).Verify(x => x.ClearCache(), Times.Once);
        }

        [TestMethod]
        public async Task Update_emailOnly_shouldInvokeUpdate()
        {
            var mockUserStore = Mock.Of<IDragonUserStore<AppMember>>(x =>
                x.FindByIdAsync(It.IsAny<string>()) == Task.FromResult(new AppMember()) &&
                x.UpdateAsync(It.IsAny<AppMember>()) == Task.FromResult<object>(null)
                );
            var controller = new AccountApiController(mockUserStore, null);

            const string email = "test22@localhost.test";
            var result = await controller.Update(new UpdateViewModel { Email = email });
            Assert.IsInstanceOfType(result, typeof(OkResult));

            Mock.Get(mockUserStore).Verify(x => x.UpdateAsync(It.Is<AppMember>(y => y.Email == email)), Times.Once);
            Mock.Get(mockUserStore).Verify(x => x.UpdateAsync(It.Is<AppMember>(y => y.Email == email)), Times.Once);
        }

        [TestMethod]
        public async Task Update_passwordOnly_shouldInvokeUpdate()
        {
            var mockUserStore = Mock.Of<IDragonUserStore<AppMember>>(x =>
                x.FindByIdAsync(It.IsAny<string>()) == Task.FromResult(new AppMember()) &&
                x.UpdateAsync(It.IsAny<AppMember>()) == Task.FromResult<object>(null)
                );
            var controller = new AccountApiController(mockUserStore, new ApplicationUserManager(mockUserStore));

            var password = Guid.NewGuid().ToString();
            var result = await controller.Update(new UpdateViewModel {Password = password});
            Assert.IsInstanceOfType(result, typeof(OkResult));

            Mock.Get(mockUserStore).Verify(x => x.UpdateAsync(It.Is<AppMember>(y => !string.IsNullOrEmpty(y.PasswordHash))), Times.Once);
            Mock.Get(mockUserStore).Verify(x => x.UpdateAsync(It.Is<AppMember>(y => !string.IsNullOrEmpty(y.PasswordHash))), Times.Once);
        }
    }
}
