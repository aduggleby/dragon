﻿using System;
using System.IO;
using System.Linq;
using System.Web;
using Dragon.Common.Util;
using Dragon.Context.Configuration;
using Dragon.Context.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Context.Test.Helpers
{
    [TestClass]
    public class CookieHelperTest
    {
        [TestInitialize]
        public void Startup()
        {
            InitCryptUtil();           
        }

        [TestMethod]
        public void Add_validData_shouldAddEncryptedCookieToHttpResponse()
        {
            var httpRequest = new HttpRequest("", "http://localhost/", "");
            var httpResponse = new HttpResponse(new StringWriter());
            var cookieHelper = new CookieHelper(false)
            {
                HttpContext = new HttpContext(httpRequest, httpResponse)
            };
            var value = Guid.NewGuid().ToString();
            cookieHelper.Add("test", value);

            Assert.AreEqual(httpRequest.Cookies.Count + 1, httpResponse.Cookies.Count);
            Assert.IsTrue(httpResponse.Cookies.AllKeys.Any(x => x == "test"));
        }

        [TestMethod]
        public void Get_cookieExists_shouldReturnCookieValue()
        {
            var httpRequest = new HttpRequest("", "http://localhost/", "");
            var httpResponse = new HttpResponse(new StringWriter());
            var cookieHelper = new CookieHelper(false)
            {
                HttpContext = new HttpContext(httpRequest, httpResponse)
            };
            var value = Guid.NewGuid().ToString();
            cookieHelper.Add("test", value);
            // ReSharper disable once AssignNullToNotNullAttribute
            cookieHelper.HttpContext.Request.Cookies.Add(cookieHelper.HttpContext.Response.Cookies[0]);

            var actual = cookieHelper.Get("test");

            Assert.AreEqual(value, actual);
        }

        #region helpers

        private static void InitCryptUtil()
        {
            var configuration = new InMemoryConfiguration();
            configuration.Set("Dragon.Context.Encryption.Key", CryptUtil.GenerateKey());
            configuration.Set("Dragon.Context.Encryption.IV", CryptUtil.GenerateIV());
            // ReSharper disable once ObjectCreationAsStatement
            new CryptUtil(configuration);
        }

        #endregion
    }
}
