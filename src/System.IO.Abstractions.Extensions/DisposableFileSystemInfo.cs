namespace System.IO.Abstractions.Extensions
{
    /// <summary>
    /// Creates a class that wraps a <see cref="IFileSystemInfo"/>. That wrapped object will be deleted
    /// when the <see cref="Dispose()"/> method is called.
    /// </summary>
    /// <remarks>
    /// This class is designed for the <c>using</c> pattern to ensure that a directory is
    /// created and then deleted when it is no longer referenced. This is sometimes called
    /// the RAII pattern (see https://en.wikipedia.org/wiki/Resource_acquisition_is_initialization).
    /// </remarks>
    public class DisposableFileSystemInfo<T> : IDisposable where T : IFileSystemInfo
    {
        protected T fileSystemInfo;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableFileSystemInfoBase"/> class.
        /// </summary>
        /// <param name="fileSystemInfo">
        /// The directory to delete when this object is disposed.
        /// </param>
        public DisposableFileSystemInfo(T fileSystemInfo)
        {
            this.fileSystemInfo = fileSystemInfo ?? throw new ArgumentNullException(nameof(fileSystemInfo));

            // Do an attribute refresh so that the object we return to the caller
            // has up-to-date properties (like Exists).
            this.fileSystemInfo.Refresh();
        }

        /// <summary>
        /// Performs the actual work of releasing resources. This allows for subclasses to participate
        /// in resource release.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> if if the call comes from a <see cref="Dispose()"/> method, <c>false</c> if it
        /// comes from a finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    DeleteFileSystemInfo();

                    // Do an attribute refresh so that the object we returned to the
                    // caller has up-to-date properties (like Exists).
                    fileSystemInfo.Refresh();
                }

                isDisposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method.
            // See https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Deletes the wrapped <typeparamref name="T"/> object.
        /// </summary>
        /// <remarks>
        /// Different types of <typeparamref name="T"/> objects have different ways of deleting themselves (e.g.
        /// directories usually need to be deleted recursively). This method is called by the <see cref="Dispose()"/>
        /// </remarks>
        protected virtual void DeleteFileSystemInfo()
        {
            fileSystemInfo.Delete();
        }
    }
}
