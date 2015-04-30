using System;
using System.Web;
using Dragon.Security.Hmac.Core.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dragon.Security.Hmac.Core.Test.Service
{
    [TestClass]
    public class HmacSha256ServiceTest
    {
        private const string QueryString = "c2=wrgh&b1=yrgh&a1=zrgh";
        private const string SortedQueryString = "a1=zrgh&b1=yrgh&c2=wrgh";
        private const string Secret = "a=42";

        [TestMethod]
        public void CalculateHash_validQueryString_shouldCalculateHash()
        {
            // Arrange
            var service = new HmacSha256Service();

            // Act
            var actual = service.CalculateHash(QueryString, Secret);

            // Assert
            Assert.AreEqual(44, actual.Length);
        }

        [TestMethod]
        public void CalculateHash_differentQueryStrings_shouldReturnDifferentHashes()
        {
            // Arrange
            var service = new HmacSha256Service();

            // Act
            var hash1 = service.CalculateHash(QueryString, Secret);
            var hash2 = service.CalculateHash(QueryString + "a", Secret);

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void CalculateHash_differentSecrets_shouldReturnDifferentHashes()
        {
            // Arrange
            var service = new HmacSha256Service();

            // Act
            var hash1 = service.CalculateHash(QueryString, Secret);
            var hash2 = service.CalculateHash(QueryString, Secret + "1");

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        /// <summary>
        /// Ensure that the hash can be transmitted as query string value.
        /// </summary>
        [TestMethod]
        public void CalculateHash_validQueryString_shouldReturnNoSpecialCharacters()
        {
            // Arrange
            var service = new HmacSha256Service();

            // Act
            var actual = service.CalculateHash(QueryString, Secret);

            // Assert
            Assert.AreEqual(Uri.EscapeDataString(actual), actual);
        }

        [TestMethod]
        [ExpectedException(typeof(HmacInvalidArgumentException))]
        public void CalculateHash_emptyQueryString_shouldThrowException()
        {
            // Arrange
            var service = new HmacSha256Service();

            // Act
            service.CalculateHash("", Secret);

            // Assert
            // see expected exception
        }

        [TestMethod]
        [ExpectedException(typeof(HmacInvalidArgumentException))]
        public void CalculateHash_emptySecret_shouldThrowException()
        {
            // Arrange
            var service = new HmacSha256Service();

            // Act
            service.CalculateHash(QueryString, "");

            // Assert
            // see expected exception
        }

        [TestMethod]
        [ExpectedException(typeof(HmacInvalidArgumentException))]
        public void CalculateHash_queryStringNull_shouldThrowException()
        {
            // Arrange
            var service = new HmacSha256Service();

            // Act
            service.CalculateHash(null, Secret);

            // Assert
            // see expected exception
        }

        [TestMethod]
        [ExpectedException(typeof(HmacInvalidArgumentException))]
        public void CalculateHash_secretNull_shouldThrowException()
        {
            // Arrange
            var service = new HmacSha256Service();

            // Act
            service.CalculateHash(QueryString, null);

            // Assert
            // see expected exception
        }

        [TestMethod]
        public void CreateSortedQueryString_unsortedInput_shouldReturnSortedValues()
        {
            // Arrange
            var service = new HmacSha256Service();
            var nameValues = HttpUtility.ParseQueryString(QueryString);

            // Act
            var actual = service.CreateSortedQueryString(nameValues);

            // Assert
            Assert.AreEqual(SortedQueryString, actual);
        }

        [TestMethod]
        public void CreateSortedQueryString_unsortedInputCustomSignatureParameterKey_shouldReturnSortedValuesIncludingDefaultSignature()
        {
            // Arrange
            var service = new HmacSha256Service { SignatureParameterKey = "s" };
            var nameValues = HttpUtility.ParseQueryString(QueryString + "&signature=somesignature");

            // Act
            var actual = service.CreateSortedQueryString(nameValues);

            // Assert
            Assert.AreEqual(SortedQueryString + "&signature=somesignature", actual);
        }

        [TestMethod]
        public void CreateSortedQueryString_inputContainsSignature_shouldIgnoreSignature()
        {
            // Arrange
            var service = new HmacSha256Service();
            var nameValues = HttpUtility.ParseQueryString(QueryString + "&signature=sig23");

            // Act
            var actual = service.CreateSortedQueryString(nameValues);

            // Assert
            Assert.AreEqual(SortedQueryString, actual);
        }

        [TestMethod]
        public void CreateSortedQueryString_inputContainsCustomSignature_shouldIgnoreSignature()
        {
            // Arrange
            var service = new HmacSha256Service { SignatureParameterKey = "s" };
            var nameValues = HttpUtility.ParseQueryString(QueryString + "&s=sig23");

            // Act
            var actual = service.CreateSortedQueryString(nameValues);

            // Assert
            Assert.AreEqual(SortedQueryString, actual);
        }

        [TestMethod]
        public void CreateSortedQueryString_inputContainsUnderscore_shouldIgnoreUnderscore()
        {
            // Arrange
            var service = new HmacSha256Service();
            var nameValues = HttpUtility.ParseQueryString(QueryString + "&_=somecachebustvalue");

            // Act
            var actual = service.CreateSortedQueryString(nameValues);

            // Assert
            Assert.AreEqual(SortedQueryString, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(HmacInvalidArgumentException))]
        public void CreateSortedQueryString_empty_shouldThrowException()
        {
            // Arrange
            var service = new HmacSha256Service();
            var nameValues = HttpUtility.ParseQueryString("");

            // Act
            service.CreateSortedQueryString(nameValues);

            // Assert
            // see expected exception
        }

        [TestMethod]
        [ExpectedException(typeof(HmacInvalidArgumentException))]
        public void CreateSortedQueryString_null_shouldThrowException()
        {
            // Arrange
            var service = new HmacSha256Service();

            // Act
            service.CreateSortedQueryString(null);

            // Assert
            // see expected exception
        }
    }
}
