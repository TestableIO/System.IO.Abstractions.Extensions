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
        /// Get an <see cref="IFileInfo"/> for the specified file <paramref name="name"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="name">File name (ex. "test.txt")</param>
        /// <returns>An <see cref="IFileInfo"/> for the specified file</returns>
        public static IFileInfo File(this IDirectoryInfo info, string name)
        {
            return info.FileSystem.FileInfo.New(info.FileSystem.Path.Combine(info.FullName, name));
        }

        /// <summary>
        /// Copies this <see cref="IDirectoryInfo"/> instance and its contents to a new path.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="destDirectoryName">The name and path to wich to copy this directory. The destination must not exist.</param>
        /// <returns>An <see cref="IDirectoryInfo"/> for the specified path.</returns>
        public static IDirectoryInfo CopyTo(this IDirectoryInfo info, string destDirectoryName)
        {
            return info.CopyTo(destDirectoryName, false);
        }

        /// <summary>
        /// Copies this <see cref="IDirectoryInfo"/> instance and its contents to a new path.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="destDirectoryName">The name and path to wich to copy this directory. The destination must not exist.</param>
        /// <param name="recursive"><see langword="true"/> to copy this directory, its subfolders, and all files; otherwise <see langword="false"/>.</param>
        /// <returns>An <see cref="IDirectoryInfo"/> for the specified path.</returns>
        public static IDirectoryInfo CopyTo(this IDirectoryInfo info, string destDirectoryName, bool recursive)
        {
            var dest = info.FileSystem.DirectoryInfo.New(destDirectoryName);
            return info.CopyTo(dest, recursive);
        }

        /// <summary>
        /// Copies this <see cref="IDirectoryInfo"/> instance and its contents to a new path.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="destDirectory">The <see cref="IDirectoryInfo"/> to wich to copy this directory. The destination must not exist.</param>
        /// <returns>An <see cref="IDirectoryInfo"/> for the specified path.</returns>
        public static IDirectoryInfo CopyTo(this IDirectoryInfo info, IDirectoryInfo destDirectory)
        {
            return info.CopyTo(destDirectory, false);
        }

        /// <summary>
        /// Copies this <see cref="IDirectoryInfo"/> instance and its contents to a new path.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="destDirectory">The <see cref="IDirectoryInfo"/> to wich to copy this directory. The destination must not exist.</param>
        /// <param name="recursive"><see langword="true"/> to copy this directory, its subfolders, and all files; otherwise <see langword="false"/>.</param>
        /// <returns>An <see cref="IDirectoryInfo"/> for the specified path.</returns>
        public static IDirectoryInfo CopyTo(this IDirectoryInfo info, IDirectoryInfo destDirectory, bool recursive)
        {
            info.Refresh();
            if (!info.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: '{info.FullName}'");
            }

            destDirectory.Refresh();
            if (!destDirectory.Exists)
            {
                destDirectory.Create();
            }

            var fileSystem = info.FileSystem;
            if (recursive)
            {
                foreach (var directoryInfo in info.EnumerateDirectories())
                {
                    var newDestDirectory = fileSystem.DirectoryInfo.New(fileSystem.Path.Combine(destDirectory.FullName, directoryInfo.Name));
                    directoryInfo.CopyTo(newDestDirectory, true);
                }
            }

            foreach (var fileInfo in info.EnumerateFiles())
            {
                var targetFilePath = fileSystem.Path.Combine(destDirectory.FullName, fileInfo.Name);
                fileInfo.CopyTo(targetFilePath);
            }

            destDirectory.Refresh();
            return destDirectory;
        }
    }
}
