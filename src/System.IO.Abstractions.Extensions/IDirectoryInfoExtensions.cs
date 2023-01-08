namespace System.IO.Abstractions
{
    public static class IDirectoryInfoExtensions
    {
        /// <summary>
        /// Get an <see cref="IDirectoryInfo"/> for the specified sub-directory <paramref name="name"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="name">Sub-directory name (ex. "test")</param>
        /// <returns>An <see cref="IDirectoryInfo"/> for the specified sub-directory</returns>
        public static IDirectoryInfo SubDirectory(this IDirectoryInfo info, string name)
        {
            return info.FileSystem.DirectoryInfo.New(info.FileSystem.Path.Combine(info.FullName, name));
        }

        /// <summary>
        /// Get full path for the specified file <paramref name="name"/>
        /// </summary>
        /// <param name="info">Containing directory</param>
        /// <param name="name">File name (ex. "test.txt")</param>
        /// <returns>An <see cref="IFileInfo"/> for the specified file</returns>
        public static string FilePath(this IDirectoryInfo info, string name)
        {
            return info.FileSystem.Path.Combine(info.FullName, name);
        }

        /// <summary>
        /// Get an <see cref="IFileInfo"/> for the specified file <paramref name="name"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="name">File name (ex. "test.txt")</param>
        /// <returns>An <see cref="IFileInfo"/> for the specified file</returns>
        public static IFileInfo File(this IDirectoryInfo info, string name)
        {
            return info.FileSystem.FileInfo.New(info.FilePath(name));
        }

        /// <summary>
        /// Copies the <paramref name="source"/> directory to the specified <paramref name="destinationPath"/>
        /// </summary>
        /// <param name="source">Source directory to copy the files from</param>
        /// <param name="destinationPath">The full path to wich to copy this directory to. If the <paramref name="destinationPath"/> doesn't exist, it will be created</param>
        /// <param name="recursive"><see langword="true"/> to copy this directory, its subfolders, and all files; otherwise <see langword="false"/>.</param>
        /// <returns>An <see cref="IDirectoryInfo"/> for the specified <paramref name="destinationPath"/></returns>
        public static IDirectoryInfo CopyTo(this IDirectoryInfo source, string destinationPath, bool recursive = false, bool overwrite = false)
        {
            var dest = source.FileSystem.DirectoryInfo.New(destinationPath);
            return source.CopyTo(dest, recursive, overwrite);
        }

        /// <summary>
        /// Copies the <paramref name="source"/> directory to the specified <paramref name="destination"/> directory
        /// </summary>
        /// <param name="source">Source directory to copy the files from</param>
        /// <param name="destination">The <see cref="IDirectoryInfo"/> to wich to copy the <paramref name="source"/> directory to. If the <paramref name="destination"/> directory doesn't exist, it will be created</param>
        /// <param name="recursive"><see langword="true"/> to copy this directory, its subfolders, and all files; otherwise <see langword="false"/> to copy only the files at the first level</param>
        /// <param name="overwrite"><see langword="true"/> to overwrite the files in the destination; otherwise <see langword="false"/> to throw an <see cref="IOException"/> if the file already exists in the destination.</param>
        /// <returns>Returns the <paramref name="destination"/> directory reference to allow for chaining methods</returns>
        public static IDirectoryInfo CopyTo(this IDirectoryInfo source, IDirectoryInfo destination, bool recursive = false, bool overwrite = false)
        {
            if (!source.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: '{source.FullName}'");

            destination.Create();

            if (recursive)
            {
                foreach (var sourceDir in source.EnumerateDirectories())
                    sourceDir.CopyTo(destination.SubDirectory(sourceDir.Name), recursive, overwrite);
            }

            foreach (var srcFile in source.EnumerateFiles())
                srcFile.CopyTo(destination.FilePath(srcFile.Name), overwrite);

            return destination;
        }
    }
}
