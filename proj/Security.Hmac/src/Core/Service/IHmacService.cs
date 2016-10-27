using System.Collections.Specialized;
using System.IO;

namespace Dragon.Security.Hmac.Core.Service
{
    /// <summary>
    /// IHmacService is used to format the query string to a common format (<see cref="CreateSortedQueryString"/>)
    /// and calculate a hash value for such a formatted query string. This hash is meant to be added as additional parameter named "signature" to the query string.
    /// This is why the "signature" parameter is ignored in <see cref="CreateSortedQueryString"/>
    /// </summary>
    public interface IHmacService
    {
        /// <summary>
        /// Calculates the hash of a string using the provided secret.
        /// </summary>
        /// <param name="data">The data for which the hash should be created</param>
        /// <param name="secret">The secret used for hashing the data</param>
        /// <returns>The hash value for the given data/secret combination</returns>
        string CalculateHash(string data, string secret);

        /// <summary>
        /// Calculates the hash of a stream using the provided secret.
        /// </summary>
        /// <param name="data">The stream for which the hash should be created</param>
        /// <param name="secret">The secret used for hashing the data</param>
        /// <returns></returns>
        string CalculateHash(Stream data, string secret);

        /// <summary>
        /// Calculates the hash of a mixed input source (string and stream).
        /// </summary>
        /// <param name="dataString">The string which should be included as input for creating the hash, e.g. the query string of a HTTP request</param>
        /// <param name="dataStream">The stream which should be included as input for creating the hash, e.g. the content of a HTTP request</param>
        /// <param name="secret">The secret used for hashing the data</param>
        /// <returns></returns>
        string CalculateHash(string dataString, Stream dataStream, string secret);

        /// <summary>
        /// Returns a string representation of the query string, sorted by the keys.
        /// An optional "signature" parameter is excluded, because this parameter is used to transmit the hash.
        /// </summary>
        /// <param name="queryString">A collection of all parameters that should be included in the hash signature</param>
        /// <returns>A string that represents the sorted and concatenated key value pairs of the input</returns>
        string CreateSortedQueryString(NameValueCollection queryString);
    }
}
