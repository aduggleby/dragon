using System; using System.Linq;
using Dragon.Security.Hmac.Module.Models;
using Dragon.Security.Hmac.Module.Repositories;
using ManagementService.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ManagementService.Tests.Controllers
{
    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        public void Get_allUsersExist_shouldReturnUsers()
        {
            var expected = CreateUser();
            var mockRepository = new Mock<IUserRepository>();
            mockRepository.Setup(x => x.GetAll()).Returns(new[]{ expected, expected });
            var controller = new UserController {UserRepository = mockRepository.Object};

            var actual = controller.Get();

            Assert.AreEqual(expected, actual.First());
        }

        [TestMethod]
        public void Get_userExists_shouldReturnUser()
        {
            var expected = CreateUser();
            var mockRepository = new Mock<IUserRepository>();
            mockRepository.Setup(x => x.Get(expected.Id.Value)).Returns(expected);
            var controller = new UserController {UserRepository = mockRepository.Object};

            var actual = controller.Get(expected.Id.Value);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Post_validModel_shouldInsertModel()
        {
            var newUser = CreateUser();
            var mockRepository = new Mock<IUserRepository>();
            var controller = new UserController {UserRepository = mockRepository.Object};

            controller.Post(newUser);

            mockRepository.Verify(x => x.Insert(newUser));
        }

        [TestMethod]
        public void Delete_existingModel_shouldRemoveModel()
        {
            var newUser = CreateUser();
            var mockRepository = new Mock<IUserRepository>();
            var controller = new UserController {UserRepository = mockRepository.Object};

            controller.Delete(newUser.Id.Value);

            mockRepository.Verify(x => x.Delete(newUser.Id.Value));
        }

        [TestMethod]
        public void Put_existingIdvalidModel_shouldUpdateModel()
        {
            var newUser = CreateUser();
            var mockRepository = new Mock<IUserRepository>();
            mockRepository.Setup(x => x.Get(newUser.Id.Value)).Returns(newUser);
            var controller = new UserController {UserRepository = mockRepository.Object};
            newUser.ServiceId = Guid.NewGuid();

            controller.Put(newUser.Id.Value, newUser);

            mockRepository.Verify(x => x.Update(newUser.Id.Value, newUser));
        }

        # region helpers

        private static UserModel CreateUser()
        {
            return new UserModel
            {
                AppId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Enabled = true,
                Id = 1L,
                ServiceId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };
        }

        # endregion
    }
}
