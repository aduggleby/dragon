﻿using System;
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
        StreamReader Retrieve(String resourceID);
    }
}