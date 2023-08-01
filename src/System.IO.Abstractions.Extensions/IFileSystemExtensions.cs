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
                throw CreateAlreadyExistsException(nameof(path), path);
            }

            directoryInfo.Create();

            // Do an attribute refresh so that the object we return to the caller
            // has up-to-date properties (like Exists).
            directoryInfo.Refresh();

            return new DisposableDirectory(directoryInfo);
        }

        /// <summary>
        /// Creates a new <see cref="DisposableFile"/> using a random name from the temp path.
        /// </summary>
        /// <param name="fileSystem">
        /// The <see cref="IFileSystem"/> in use.
        /// </param>
        /// <param name="fileInfo">
        /// The <see cref="IFileInfo"/> for the file that was created.
        /// </param>
        /// <returns>
        /// A <see cref="DisposableFile"/> to manage the file's lifetime.
        /// </returns>
        public static DisposableFile CreateDisposableFile(this IFileSystem fileSystem, out IFileInfo fileInfo)
        {
            var temp = fileSystem.Path.GetTempPath();
            var fileName = fileSystem.Path.GetRandomFileName();
            var path = fileSystem.Path.Combine(temp, fileName);

            return fileSystem.CreateDisposableFile(path, out fileInfo);
        }

        /// <summary>
        /// Creates a new <see cref="DisposableFile"/> using the provided <paramref name="path"/>.
        /// </summary>
        /// <param name="fileSystem">
        /// The <see cref="IFileSystem"/> in use.
        /// </param>
        /// <param name="path">
        /// The full path to the file to create.
        /// </param>
        /// <param name="fileInfo">
        /// The <see cref="IFileInfo"/> for the file that was craeted.
        /// </param>
        /// <returns>
        /// A <see cref="DisposableFile"/> to manage the file's lifetime.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the file already exists.
        /// </exception>
        public static DisposableFile CreateDisposableFile(this IFileSystem fileSystem, string path, out IFileInfo fileInfo)
        {
            fileInfo = fileSystem.FileInfo.New(path);

            if (fileInfo.Exists)
            {
                throw CreateAlreadyExistsException(nameof(path), path);
            }

            // Ensure we close the handle to the file after we create it, otherwise
            // callers may get an access denied error.
            fileInfo.Create().Dispose();

            // Do an attribute refresh so that the object we return to the caller
            // has up-to-date properties (like Exists).
            fileInfo.Refresh();

            return new DisposableFile(fileInfo);
        }

        private static ArgumentException CreateAlreadyExistsException(string argumentName, string path)
        {
            // Having the colliding path availabe as part of the exception is very useful for debugging.
            // However, paths can be considered sensitive information in some contexts (like web servers).
            // Thus, we add the path to the exception's data , rather than the message.
            var ex = new ArgumentException("File already exists", argumentName);
            ex.Data.Add("path", path);

            return ex;
        }
    }
}
