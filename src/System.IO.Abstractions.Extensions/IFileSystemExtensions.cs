namespace System.IO.Abstractions.Extensions
{
    public static class IFileSystemExtensions
    {
        /// <summary>
        /// Get the current directory for the specified <paramref name="fileSystem"/>
        /// </summary>
        /// <param name="fileSystem">FileSystem in use</param>
        /// <returns>An <see cref="IDirectoryInfo"/> for the current directory</returns>
        public static IDirectoryInfo CurrentDirectory(this IFileSystem fileSystem)
        {
            return fileSystem.DirectoryInfo.New(fileSystem.Directory.GetCurrentDirectory());
        }

        /// <summary>
        /// Creates a new <see cref="DisposableDirectory"/> using a random name from the temp path.
        /// </summary>
        /// <param name="fileSystem">
        /// The <see cref="IFileSystem"/> in use.
        /// </param>
        /// <param name="directoryInfo">
        /// The <see cref="IDirectoryInfo"/> for the directory that was created.
        /// </param>
        /// <returns>
        /// A <see cref="DisposableDirectory"/> to manage the directory's lifetime.
        /// </returns>
        public static DisposableDirectory CreateDisposableDirectory(this IFileSystem fileSystem, out IDirectoryInfo directoryInfo)
        {
            var temp = fileSystem.Path.GetTempPath();
            var fileName = fileSystem.Path.GetRandomFileName();
            var path = fileSystem.Path.Combine(temp, fileName);

            return fileSystem.CreateDisposableDirectory(path, out directoryInfo);
        }

        /// <summary>
        /// Creates a new <see cref="DisposableDirectory"/> using the provided <paramref name="path"/>.
        /// </summary>
        /// <param name="fileSystem">
        /// The <see cref="IFileSystem"/> in use.
        /// </param>
        /// <param name="path">
        /// The full path to the directory to create.
        /// </param>
        /// <param name="directoryInfo">
        /// The <see cref="IDirectoryInfo"/> for the directory that was craeted.
        /// </param>
        /// <returns>
        /// A <see cref="DisposableDirectory"/> to manage the directory's lifetime.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the directory already exists.
        /// </exception>
        public static DisposableDirectory CreateDisposableDirectory(this IFileSystem fileSystem, string path, out IDirectoryInfo directoryInfo)
        {
            directoryInfo = fileSystem.DirectoryInfo.New(path);

            if (directoryInfo.Exists)
            {
                // Having the colliding path availabe as part of the exception is very useful for debugging.
                // However, paths can be considered sensitive information in some contexts (like web servers).
                // Thus, we add the path to the exception's data , rather than the message.
                var ex = new ArgumentException("Directory already exists", nameof(path));
                ex.Data.Add("path", path);

                throw ex;
            }

            directoryInfo.Create();

            // Do an attribute refresh so that the object we return to the caller
            // has up-to-date properties (like Exists).
            directoryInfo.Refresh();

            return new DisposableDirectory(directoryInfo);
        }
    }
}
