using System;
using System.IO;
using System.Linq;
using System.Text;
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
        private const string Content = @"{""key"":""val""}";

        [TestClass]
        public class CalculateHashFromString : HmacSha256ServiceTest
        {
            [TestMethod]
            public void ReturnsHashForValidInput()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                var actual = service.CalculateHash(QueryString, Secret);

                // Assert
                Assert.AreEqual(44, actual.Length);
            }

            [TestMethod]
            public void ReturnsHexEncodedHashForValidQueryStringUsingHexEncoding()
            {
                // Arrange
                var service = new HmacSha256Service {UseHexEncoding = true};

                // Act
                var actual = service.CalculateHash(QueryString, Secret);

                // Assert
                Assert.IsTrue(actual.All(IsHex));
            }

            [TestMethod]
            public void ReturnsDifferentHashesForDifferentQueryStrings()
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
            public void ReturnsEqualHashesForEqualQueryStrings()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                var hash1 = service.CalculateHash(QueryString, Secret);
                var hash2 = service.CalculateHash(QueryString, Secret);

                // Assert
                Assert.AreEqual(hash1, hash2);
            }

            [TestMethod]
            public void ReturnsDifferentHashesForDifferentSecrets()
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
            public void ReturnsNoSpecialCharacters()
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
            public void ThrowsExceptionForEmptyQueryStrings()
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
            public void ThrowsExceptionForEmptySecrets()
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
            public void ThrowExceptionForQueryStringsThatAreNull()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                service.CalculateHash((string)null, Secret);

                // Assert
                // see expected exception
            }

            [TestMethod]
            [ExpectedException(typeof(HmacInvalidArgumentException))]
            public void ThrowExceptionForSecretsThatAreNull()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                service.CalculateHash(QueryString, null);

                // Assert
                // see expected exception
            }
        }

        [TestClass]
        public class CalculateHashFromStream : HmacSha256ServiceTest
        {
            [TestMethod]
            public void ReturnsHashForValidInput()
            {
                // Arrange
                IHmacService service = new HmacSha256Service();

                // Act
                var actual = service.CalculateHash(ToStream(QueryString), Secret);

                // Assert
                Assert.AreEqual(44, actual.Length);
            }

            [TestMethod]
            public void ReturnsHexEncodedHashForValidQueryStringUsingHexEncoding()
            {
                // Arrange
                var service = new HmacSha256Service {UseHexEncoding = true};

                // Act
                var actual = service.CalculateHash(ToStream(QueryString), Secret);

                // Assert
                Assert.IsTrue(actual.All(IsHex));
            }

            [TestMethod]
            public void ReturnsDifferentHashesForDifferentQueryStrings()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                var hash1 = service.CalculateHash(ToStream(QueryString), Secret);
                var hash2 = service.CalculateHash(ToStream(QueryString + "a"), Secret);

                // Assert
                Assert.AreNotEqual(hash1, hash2);
            }

            [TestMethod]
            public void ReturnsEqualHashesForEqualQueryStrings()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                var hash1 = service.CalculateHash(ToStream(QueryString), Secret);
                var hash2 = service.CalculateHash(ToStream(QueryString), Secret);

                // Assert
                Assert.AreEqual(hash1, hash2);
            }

            [TestMethod]
            public void ReturnsDifferentHashesForDifferentSecrets()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                var hash1 = service.CalculateHash(ToStream(QueryString), Secret);
                var hash2 = service.CalculateHash(ToStream(QueryString), Secret + "1");

                // Assert
                Assert.AreNotEqual(hash1, hash2);
            }

            /// <summary>
            /// Ensure that the hash can be transmitted as query string value.
            /// </summary>
            [TestMethod]
            public void ReturnsNoSpecialCharacters()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                var actual = service.CalculateHash(ToStream(QueryString), Secret);

                // Assert
                Assert.AreEqual(Uri.EscapeDataString(actual), actual);
            }

            [TestMethod]
            [ExpectedException(typeof(HmacInvalidArgumentException))]
            public void ThrowsExceptionForEmptyQueryStrings()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                service.CalculateHash(ToStream(""), Secret);

                // Assert
                // see expected exception
            }

            [TestMethod]
            [ExpectedException(typeof(HmacInvalidArgumentException))]
            public void ThrowsExceptionForEmptySecrets()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                service.CalculateHash(ToStream(QueryString), "");

                // Assert
                // see expected exception
            }

            [TestMethod]
            [ExpectedException(typeof(HmacInvalidArgumentException))]
            public void ThrowExceptionForQueryStringsThatAreNull()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                service.CalculateHash((Stream)null, Secret);

                // Assert
                // see expected exception
            }

            [TestMethod]
            [ExpectedException(typeof(HmacInvalidArgumentException))]
            public void ThrowExceptionForSecretsThatAreNull()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                service.CalculateHash(ToStream(QueryString), null);

                // Assert
                // see expected exception
            }
        }

        [TestClass]
        public class CalculateHashFromStringAndStream : HmacSha256ServiceTest
        {
            [TestMethod]
            public void ReturnsHashForValidInput()
            {
                // Arrange
                IHmacService service = new HmacSha256Service();

                // Act
                var actual = service.CalculateHash(QueryString, ToStream(Content), Secret);

                // Assert
                Assert.AreEqual(44, actual.Length);
            }

            [TestMethod]
            public void ReturnsHexEncodedHashForValidQueryStringUsingHexEncoding()
            {
                // Arrange
                var service = new HmacSha256Service { UseHexEncoding = true };

                // Act
                var actual = service.CalculateHash(QueryString, ToStream(Content), Secret);

                // Assert
                Assert.IsTrue(actual.All(IsHex));
            }

            [TestMethod]
            public void ReturnsDifferentHashesForDifferentQueryStrings()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                var hash1 = service.CalculateHash(QueryString, ToStream(Content), Secret);
                var hash2 = service.CalculateHash(QueryString + "a", ToStream(Content), Secret);

                // Assert
                Assert.AreNotEqual(hash1, hash2);
            }

            [TestMethod]
            public void ReturnsEqualHashesForEqualQueryStrings()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                var hash1 = service.CalculateHash(QueryString, ToStream(Content), Secret);
                var hash2 = service.CalculateHash(QueryString, ToStream(Content), Secret);

                // Assert
                Assert.AreEqual(hash1, hash2);
            }

            [TestMethod]
            public void ReturnsDifferentHashesForDifferentContent()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                var hash1 = service.CalculateHash(QueryString, ToStream(Content), Secret);
                var hash2 = service.CalculateHash(QueryString, ToStream(Content + "a"), Secret);

                // Assert
                Assert.AreNotEqual(hash1, hash2);
            }

            [TestMethod]
            public void ReturnsEqualHashesForEqualContent()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                var hash1 = service.CalculateHash(QueryString, ToStream(Content), Secret);
                var hash2 = service.CalculateHash(QueryString, ToStream(Content), Secret);

                // Assert
                Assert.AreEqual(hash1, hash2);
            }

            [TestMethod]
            public void ReturnsDifferentHashesForDifferentSecrets()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                var hash1 = service.CalculateHash(QueryString, ToStream(Content), Secret);
                var hash2 = service.CalculateHash(QueryString, ToStream(Content), Secret + "1");

                // Assert
                Assert.AreNotEqual(hash1, hash2);
            }

            /// <summary>
            /// Ensure that the hash can be transmitted as query string value.
            /// </summary>
            [TestMethod]
            public void ReturnsNoSpecialCharacters()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                var actual = service.CalculateHash(QueryString, ToStream(Content), Secret);

                // Assert
                Assert.AreEqual(Uri.EscapeDataString(actual), actual);
            }

            [TestMethod]
            [ExpectedException(typeof(HmacInvalidArgumentException))]
            public void ThrowsExceptionForEmptyQueryStrings()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                service.CalculateHash("", ToStream(Content), Secret);

                // Assert
                // see expected exception
            }

            [TestMethod]
            [ExpectedException(typeof(HmacInvalidArgumentException))]
            public void ThrowsExceptionForEmptyContent()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                service.CalculateHash(QueryString, ToStream(""), Secret);

                // Assert
                // see expected exception
            }


            [TestMethod]
            [ExpectedException(typeof(HmacInvalidArgumentException))]
            public void ThrowsExceptionForEmptySecrets()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                service.CalculateHash(QueryString, ToStream(Content), "");

                // Assert
                // see expected exception
            }

            [TestMethod]
            [ExpectedException(typeof(HmacInvalidArgumentException))]
            public void ThrowExceptionForQueryStringsThatAreNull()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                service.CalculateHash(null, ToStream(Content), Secret);

                // Assert
                // see expected exception
            }

            [TestMethod]
            [ExpectedException(typeof(HmacInvalidArgumentException))]
            public void ThrowExceptionForContentThatIsNull()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                service.CalculateHash(QueryString, null, Secret);

                // Assert
                // see expected exception
            }


            [TestMethod]
            [ExpectedException(typeof(HmacInvalidArgumentException))]
            public void ThrowExceptionForSecretsThatAreNull()
            {
                // Arrange
                var service = new HmacSha256Service();

                // Act
                service.CalculateHash(QueryString, ToStream(Content), null);

                // Assert
                // see expected exception
            }
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

        private static bool IsHex(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        private static MemoryStream ToStream(string data)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(data));
        }
    }
}
