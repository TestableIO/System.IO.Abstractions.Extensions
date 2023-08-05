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
        /// Creates a new <see cref="IDirectoryInfo"/> using a random name from the temp path, and returns an <see cref="IDisposable"/>
        /// that deletes the directory when disposed.
        /// </summary>
        /// <param name="fileSystem">
        /// The <see cref="IFileSystem"/> in use.
        /// </param>
        /// <param name="directoryInfo">
        /// The <see cref="IDirectoryInfo"/> for the directory that was created.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> to manage the directory's lifetime.
        /// </returns>
        public static IDisposable CreateDisposableDirectory(this IFileSystem fileSystem, out IDirectoryInfo directoryInfo)
        {
            return fileSystem.CreateDisposableDirectory(fileSystem.Path.GetRandomTempPath(), out directoryInfo);
        }

        /// <inheritdoc cref="CreateDisposableDirectory(IFileSystem, out IDirectoryInfo)"/>
        /// <param name="path">
        /// The full path to the directory to create.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the directory already exists.
        /// </exception>
        public static IDisposable CreateDisposableDirectory(this IFileSystem fileSystem, string path, out IDirectoryInfo directoryInfo)
        {
            return fileSystem.CreateDisposableDirectory(path, dir => new DisposableDirectory(dir), out directoryInfo);
        }

        /// <inheritdoc cref="CreateDisposableDirectory(IFileSystem, string, out IDirectoryInfo)"/>
        /// <param name="disposableFactory">
        /// A <see cref="Func{T, TResult}"/> that acts as a factory method. Given the <see cref="IDirectoryInfo"/>, create the
        /// <see cref="IDisposable"/> that will manage the its lifetime.
        /// </param>
        public static T CreateDisposableDirectory<T>(
            this IFileSystem fileSystem,
            string path,
            Func<IDirectoryInfo, T> disposableFactory,
            out IDirectoryInfo directoryInfo) where T : IDisposable
        {
            directoryInfo = fileSystem.DirectoryInfo.New(path);

            if (directoryInfo.Exists)
            {
                throw CreateAlreadyExistsException(nameof(path), path);
            }

            directoryInfo.Create();

            return disposableFactory(directoryInfo);
        }

        /// <summary>
        /// Creates a new <see cref="IFileInfo"/> using a random name from the temp path, and returns an <see cref="IDisposable"/>
        /// that deletes the file when disposed.
        /// </summary>
        /// <param name="fileSystem">
        /// The <see cref="IFileSystem"/> in use.
        /// </param>
        /// <param name="fileInfo">
        /// The <see cref="IFileInfo"/> for the file that was created.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> to manage the file's lifetime.
        /// </returns>
        public static IDisposable CreateDisposableFile(this IFileSystem fileSystem, out IFileInfo fileInfo)
        {
            return fileSystem.CreateDisposableFile(fileSystem.Path.GetRandomTempPath(), out fileInfo);
        }

        /// <inheritdoc cref="CreateDisposableFile(IFileSystem, out IFileInfo)"/>
        /// <param name="path">
        /// The full path to the file to create.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the file already exists.
        /// </exception>
        public static IDisposable CreateDisposableFile(this IFileSystem fileSystem, string path, out IFileInfo fileInfo)
        {
            return fileSystem.CreateDisposableFile(path, file => new DisposableFile(file), out fileInfo);
        }

        /// <inheritdoc cref="CreateDisposableFile(IFileSystem, string, out IFileInfo)"/>
        /// <param name="disposableFactory">
        /// A <see cref="Func{T, TResult}"/> that acts as a factory method. Given the <see cref="IFileInfo"/>, create the
        /// <see cref="IDisposable"/> that will manage the its lifetime.
        /// </param>
        public static T CreateDisposableFile<T>(
            this IFileSystem fileSystem,
            string path,
            Func<IFileInfo, T> disposableFactory,
            out IFileInfo fileInfo) where T : IDisposable
        {
            fileInfo = fileSystem.FileInfo.New(path);

            if (fileInfo.Exists)
            {
                throw CreateAlreadyExistsException(nameof(path), path);
            }

            // Ensure we close the handle to the file after we create it, otherwise
            // callers may get an access denied error.
            fileInfo.Create().Dispose();

            return disposableFactory(fileInfo);
        }

        private static string GetRandomTempPath(this IPath path)
        {
            var temp = path.GetTempPath();
            var fileName = path.GetRandomFileName();
            return path.Combine(temp, fileName);
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
