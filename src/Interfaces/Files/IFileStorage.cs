using System;
using System.IO;

namespace Dragon.Interfaces.Files
{
    public interface IFileStorage
    {
        /// <summary>
        ///     Uploads a file to a storage provider.
        /// </summary>
        /// <param name="filePath">The path of the file that is to be uploaded</param>
        /// <returns>A unique id of the stored resource.</returns>
        String Store(String filePath);

        /// <summary>
        ///     Retrieves a file from a storage provider.
        /// </summary>
        /// <param name="resourceID">The unique id of the resource to retrieve.</param>
        /// <returns>The resource.</returns>
        Stream Retrieve(string resourceID);

        /// <summary>
        ///     Deletes a file from the storage provider.
        /// </summary>
        /// <param name="resourceID">The unique id of the resource to delete.</param>
        void Delete(string resourceID);

        /// <summary>
        ///     Checks if a file is available on the storage provider.
        /// </summary>
        /// <param name="resourceID">The unique id of the resource to search for.</param>
        /// <returns></returns>
        bool Exists(string resourceID);
    }
}