using System;

namespace Dragon.Interfaces.Files
{
    public interface IFileRestriction
    {
        /// <summary>
        ///     Determines if it is allowed to store a file in the IFileStorage.
        /// </summary>
        Boolean IsAllowed(string filePath);
    }
}
