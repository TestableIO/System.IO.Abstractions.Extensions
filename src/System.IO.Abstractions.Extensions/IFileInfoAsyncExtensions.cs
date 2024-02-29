#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Abstractions
{
    public static class FileInfoFileAsyncExtensions
    {
        /// <inheritdoc cref="IFile.AppendAllLinesAsync(string,System.Collections.Generic.IEnumerable{string},System.Threading.CancellationToken)"/>
        public static async Task AppendAllLinesAsync(this IFileInfo file, IEnumerable<string> contents, CancellationToken cancellationToken = default)
        {
            await file.FileSystem.File.AppendAllLinesAsync(file.FullName, contents, cancellationToken);
        }
        
        /// <inheritdoc cref="IFile.AppendAllTextAsync(string,string?,System.Threading.CancellationToken)"/>
        public static async Task AppendAllTextAsync(this IFileInfo file, string contents, CancellationToken cancellationToken = default)
        {
            await file.FileSystem.File.AppendAllTextAsync(file.FullName, contents, cancellationToken);
        }

        /// <inheritdoc cref="IFile.ReadAllBytesAsync(string,System.Threading.CancellationToken)"/>
        public static async Task<byte[]> ReadAllBytesAsync(this IFileInfo file, CancellationToken cancellationToken = default)
        {
            return await file.FileSystem.File.ReadAllBytesAsync(file.FullName, cancellationToken);
        }

        /// <inheritdoc cref="IFile.ReadAllLinesAsync(string,System.Threading.CancellationToken)"/>
        public static async Task<string[]> ReadAllLinesAsync(this IFileInfo file, CancellationToken cancellationToken = default)
        {
            return await file.FileSystem.File.ReadAllLinesAsync(file.FullName, cancellationToken);
        }
        
        /// <inheritdoc cref="IFile.ReadAllTextAsync(string,System.Threading.CancellationToken)" />
        public static async Task<string> ReadAllTextAsync(this IFileInfo file, CancellationToken cancellationToken = default)
        {
            return await file.FileSystem.File.ReadAllTextAsync(file.FullName, cancellationToken);
        }
        
        /// <inheritdoc cref="IFile.WriteAllBytesAsync(string,byte[],System.Threading.CancellationToken)" />
        public static async Task WriteAllBytesAsync(this IFileInfo file, byte[] bytes, CancellationToken cancellationToken = default)
        {
            await file.FileSystem.File.WriteAllBytesAsync(file.FullName, bytes, cancellationToken);
        }

        /// <inheritdoc cref="IFile.WriteAllLinesAsync(string,System.Collections.Generic.IEnumerable{string},System.Threading.CancellationToken)" />
        public static async Task WriteAllLinesAsync(this IFileInfo file, IEnumerable<string> contents, CancellationToken cancellationToken = default)
        {
            await file.FileSystem.File.WriteAllLinesAsync(file.FullName, contents, cancellationToken);
        }

        /// <inheritdoc cref="IFile.WriteAllTextAsync(string,string?,System.Threading.CancellationToken)" />
        public static async Task WriteAllTextAsync(this IFileInfo file, string contents, CancellationToken cancellationToken = default)
        {
            await file.FileSystem.File.WriteAllTextAsync(file.FullName, contents, cancellationToken);
        }
    }
}
#endif