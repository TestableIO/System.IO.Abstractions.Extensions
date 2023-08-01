namespace System.IO.Abstractions.Extensions
{
    /// <summary>
    /// Creates a file that will be deleted when the <see cref="Dispose()"/> method is called.
    /// </summary>
    /// <remarks>
    /// This class is designed for the <c>using</c> pattern to ensure that a file is
    /// created and then deleted when it is no longer referenced. This is sometimes called
    /// the RAII pattern (see https://en.wikipedia.org/wiki/Resource_acquisition_is_initialization).
    /// </remarks>
    internal class DisposableFile : IDisposable
    {
        private bool isDisposed;
        private readonly IFileInfo fileInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableFile"/> class.
        /// </summary>
        /// <param name="fileInfo">
        /// The file to delete when this object is disposed.
        /// </param>
        public DisposableFile(IFileInfo fileInfo)
        {
            this.fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
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
                    fileInfo.Delete();
                    
                    // Do an attribute refresh so that the object we returned to the
                    // caller has up-to-date properties (like Exists).
                    fileInfo.Refresh();
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
    }
}
