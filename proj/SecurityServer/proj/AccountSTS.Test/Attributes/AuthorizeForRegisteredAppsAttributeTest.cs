using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using Dragon.SecurityServer.AccountSTS.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dragon.SecurityServer.AccountSTS.Test.Attributes
{
    [TestClass]
    public class AuthorizeForRegisteredAppsAttributeTest
    {
        [TestMethod]
        public void AuthorizeCore_requiredRequestParameterMissing_shouldDenyAccess()
        {
            // Arrange
            var helper = new AuthorizeAttributeHelper();
            var request = new Mock<HttpRequestBase>();
            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(ctx => ctx.User.Identity).Returns(CreateClaimsIdentityMock(CreateClaims(), true).Object);
            mockHttpContext.Setup(ctx => ctx.Request).Returns(request.Object);

            // Act
            var retVal = helper.PublicAuthorizeCore(mockHttpContext.Object);

            // Assert
            Assert.IsFalse(retVal);
        }

        [TestMethod]
        public void AuthorizeCore_userIsNotAuthenticated_shouldAllowAccess()
        {
            // Arrange
            var helper = new AuthorizeAttributeHelper();
            var request = new Mock<HttpRequestBase>();
            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(ctx => ctx.User.Identity).Returns(CreateClaimsIdentityMock(CreateClaims(), false).Object);
            mockHttpContext.Setup(ctx => ctx.Request).Returns(request.Object);

            // Act
            var retVal = helper.PublicAuthorizeCore(mockHttpContext.Object);

            // Assert
            Assert.IsTrue(retVal);
        }

        private static IEnumerable<Claim> CreateClaims()
        {
            return new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
        }

        private static Mock<ClaimsIdentity> CreateClaimsIdentityMock(IEnumerable<Claim> claims, bool isAuthenticated)
        {
            var cp = new Mock<ClaimsIdentity>();
            cp.Setup(m => m.HasClaim(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            cp.Setup(m => m.IsAuthenticated).Returns(isAuthenticated);
            cp.Setup(m => m.Name).Returns("SomeName");
            cp.Setup(m => m.Claims).Returns(claims);
            return cp;
        }
    }

    public class AuthorizeAttributeHelper : AuthorizeForRegisteredAppsAttribute
    {
        public virtual bool PublicAuthorizeCore(HttpContextBase httpContext)
        {
            return base.AuthorizeCore(httpContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return PublicAuthorizeCore(httpContext);
        }

        public virtual HttpValidationStatus PublicOnCacheAuthorization(HttpContextBase httpContext)
        {
            return base.OnCacheAuthorization(httpContext);
        }

        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            return PublicOnCacheAuthorization(httpContext);
        }
    }
}
