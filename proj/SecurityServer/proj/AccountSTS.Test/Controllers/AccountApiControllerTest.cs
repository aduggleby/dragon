using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Dragon.Data.Interfaces;
using Dragon.Data.Repositories;
using Dragon.SecurityServer.AccountSTS.Controllers;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.AspNet.Identity;
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
            var controller = new AccountApiController(mockUserStore, null, null);

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
            var controller = new AccountApiController(mockUserStore, null, null);

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
            var controller = new AccountApiController(mockUserStore, new ApplicationUserManager(mockUserStore), null);

            var password = Guid.NewGuid().ToString();
            var result = await controller.Update(new UpdateViewModel {Password = password});
            Assert.IsInstanceOfType(result, typeof(OkResult));

            Mock.Get(mockUserStore).Verify(x => x.UpdateAsync(It.Is<AppMember>(y => !string.IsNullOrEmpty(y.PasswordHash))), Times.Once);
            Mock.Get(mockUserStore).Verify(x => x.UpdateAsync(It.Is<AppMember>(y => !string.IsNullOrEmpty(y.PasswordHash))), Times.Once);
        }

        [TestMethod]
        public async Task Delete_validId_shouldInvokeDelete()
        {
            var mockUserStore = Mock.Of<IDragonUserStore<AppMember>>(x =>
                x.FindByIdAsync(It.IsAny<string>()) == Task.FromResult(new AppMember()) &&
                x.GetRolesAsync(It.IsAny<AppMember>()) == Task.FromResult((IList<string>) new List<string>{"r1", "r2"}) &&
                (IUserRoleStore<AppMember, string>)(x).IsInRoleAsync(It.IsAny<AppMember>(), It.IsAny<string>()) == Task.FromResult(true) &&
                (IUserRoleStore<AppMember, string>)(x).RemoveFromRoleAsync(It.IsAny<AppMember>(), It.IsAny<string>()) == Task.FromResult(0) &&
                x.GetLoginsAsync(It.IsAny<AppMember>()) == Task.FromResult((IList<UserLoginInfo>) new List<UserLoginInfo>{new UserLoginInfo("p1", "k1")}) &&
                (IUserLoginStore<AppMember, string>)(x).RemoveLoginAsync(It.IsAny<AppMember>(), It.IsAny<UserLoginInfo>()) == Task.FromResult(true) &&
                x.RemoveServiceRegistrations(It.IsAny<AppMember>()) == Task.FromResult<object>(null) &&
                x.RemoveAppRegistrations(It.IsAny<AppMember>()) == Task.FromResult<object>(null)
                );
            var mockUserActivityRepository = Mock.Of<IRepository<UserActivity>>(x =>
                    x.GetByWhere(It.IsAny<Dictionary<string, object>>()) == new List<UserActivity> { new UserActivity() });
            var controller = new AccountApiController(mockUserStore, new ApplicationUserManager(mockUserStore), mockUserActivityRepository);

            var result = await controller.Delete(Guid.NewGuid().ToString());
            Assert.IsInstanceOfType(result, typeof(OkResult));

            Mock.Get(mockUserStore).Verify(x => x.RemoveServiceRegistrations(It.IsAny<AppMember>()), Times.Once);
            Mock.Get(mockUserStore).Verify(x => x.RemoveAppRegistrations(It.IsAny<AppMember>()), Times.Once);
            Mock.Get(mockUserStore).Verify(x => x.RemoveLoginAsync(It.IsAny<AppMember>(), It.IsAny<UserLoginInfo>()), Times.Once);
            Mock.Get(mockUserStore).Verify(x => x.RemoveFromRoleAsync(It.IsAny<AppMember>(), It.IsAny<string>()), Times.Exactly(2));
            Mock.Get(mockUserActivityRepository).Verify(x => x.Delete(It.IsAny<UserActivity>()), Times.Once);
            Mock.Get(mockUserStore).Verify(x => x.DeleteAsync(It.IsAny<AppMember>()), Times.Once);
        }
    }
}
