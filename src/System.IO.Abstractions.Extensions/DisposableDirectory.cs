namespace System.IO.Abstractions.Extensions
{
    /// <summary>
    /// Creates a class that wraps a <see cref="IDirectoryInfo"/>. That wrapped directory will be
    /// deleted when the <see cref="Dispose()"/> method is called.
    /// </summary>
    /// <inheritdoc/>
    internal class DisposableDirectory : DisposableFileSystemInfo<IDirectoryInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableDirectory"/> class.
        /// </summary>
        /// <param name="directoryInfo">
        /// The directory to delete when this object is disposed.
        /// </param>
        public DisposableDirectory(IDirectoryInfo directoryInfo) : base(directoryInfo)
        {
        }

        /// <inheritdoc/>
        protected override void DeleteFileSystemInfo()
        {
            fileSystemInfo.Delete(recursive: true);
        }
    }
}
