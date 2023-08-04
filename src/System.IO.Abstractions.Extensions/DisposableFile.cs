namespace System.IO.Abstractions.Extensions
{
    /// <summary>
    /// Creates a class that wraps a <see cref="IFileInfo"/>. That wrapped file will be
    /// deleted when the <see cref="Dispose()"/> method is called.
    /// </summary>
    /// <inheritdoc/>
    internal class DisposableFile : DisposableFileSystemInfo<IFileInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableFile"/> class.
        /// </summary>
        /// <param name="fileInfo">
        /// The file to delete when this object is disposed.
        /// </param>
        public DisposableFile(IFileInfo fileInfo) : base(fileInfo)
        {
        }
    }
}
