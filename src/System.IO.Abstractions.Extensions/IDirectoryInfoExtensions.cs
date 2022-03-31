namespace System.IO.Abstractions
{
    public static class IDirectoryInfoExtensions
    {
        /// <summary>
        ///     Get an <see cref="IDirectoryInfo" /> for the specified Directory at <paramref name="destDirectoryName" />.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="destDirectoryName">Destination Path for the directory to be copied to.</param>
        /// <returns>An <see cref="IDirectoryInfo" /> for the specified path.</returns>
        public static IDirectoryInfo CopyTo(this IDirectoryInfo info, string destDirectoryName, bool recursive = false)
        {
            var dest = info.FileSystem.DirectoryInfo.FromDirectoryName(destDirectoryName);

            return info.CopyTo(dest, recursive);
        }

        /// <summary>
        ///     Get an <see cref="IDirectoryInfo" /> for the specified Directory at <paramref name="destDirectoryName" />.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="destDirectory">Destination Path for the directory to be copied to.</param>
        /// <returns>An <see cref="IDirectoryInfo" /> for the specified path.</returns>
        public static IDirectoryInfo CopyTo(this IDirectoryInfo info, IDirectoryInfo destDirectory, bool recursive = false)
        {
            // Check if the source directory exists
            info.Refresh();
            if (!info.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: '{info.FullName}'");
            }

            destDirectory.Refresh();
            if (destDirectory.Exists)
            {
                throw new IOException($"The directory '{destDirectory.FullName}' already exists.");
            }

            // Cache directories before we start copying
            var subDirectories = info.GetDirectories();

            // Create the destination directory
            var fs = info.FileSystem;
            fs.Directory.CreateDirectory(destDirectory.FullName);

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (var directoryInfo in subDirectories)
                {
                    var newDestDirectory = fs.DirectoryInfo.FromDirectoryName(fs.Path.Combine(destDirectory.FullName, directoryInfo.Name));
                    directoryInfo.CopyTo(newDestDirectory, true);
                }
            }

            // Get the files in the source directory and copy to the destination directory
            foreach (var fileInfo in info.GetFiles())
            {
                string targetFilePath = Path.Combine(destDirectory.FullName, fileInfo.Name);
                fileInfo.CopyTo(targetFilePath);
            }

            destDirectory.Refresh();
            return destDirectory;
        }

        /// <summary>
        ///     Get an <see cref="IFileInfo" /> for the specified file <paramref name="name" />
        /// </summary>
        /// <param name="info"></param>
        /// <param name="name">File name (ex. "test.txt")</param>
        /// <returns>An <see cref="IFileInfo" /> for the specified file</returns>
        public static IFileInfo File(this IDirectoryInfo info, string name)
        {
            return info.FileSystem.FileInfo.FromFileName(info.FileSystem.Path.Combine(info.FullName, name));
        }

        /// <summary>
        ///     Get an <see cref="IDirectoryInfo" /> for the specified sub-directory <paramref name="name" />
        /// </summary>
        /// <param name="info"></param>
        /// <param name="name">Sub-directory name (ex. "test")</param>
        /// <returns>An <see cref="IDirectoryInfo" /> for the specified sub-directory</returns>
        public static IDirectoryInfo SubDirectory(this IDirectoryInfo info, string name)
        {
            return info.FileSystem.DirectoryInfo.FromDirectoryName(info.FileSystem.Path.Combine(info.FullName, name));
        }
    }
}