using System.Collections.Generic;
using System.Text;

namespace System.IO.Abstractions
{
    public static class IFileInfoExtensions
    {
        /// <summary>
        /// Throws an exception if the file <paramref name="info"/> doesn't exists
        /// </summary>
        /// <param name="info">File that will be checked for existance</param>
        /// <exception cref="FileNotFoundException">Exception thrown if the file is not found</exception>
        public static void ThrowIfNotFound(this IFileInfo info)
        {
            if (!info.Exists)
                throw new FileNotFoundException(StringResources.Format("COULD_NOT_FIND_FILE_EXCEPTION", info.FullName));
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{String}"/> that can enumerate the lines of text in the <paramref name="info"/> file
        /// </summary>
        /// <param name="info">File to enumerate content</param>
        /// <returns>Returns an <see cref="IEnumerable{String}"/> to enumerate the content of the file</returns>
        public static IEnumerable<string> EnumerateLines(this IFileInfo info)
        {
            return new LineEnumerable(info, null);
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{String}"/> that can enumerate the lines of text in the <paramref name="info"/> file
        /// using the specified <paramref name="encoding"/>
        /// </summary>
        /// <param name="info">File to enumerate content</param>
        /// <returns>Returns an <see cref="IEnumerable{String}"/> to enumerate the content of the file</returns>
        public static IEnumerable<string> EnumerateLines(this IFileInfo info, Encoding encoding)
        {
            return new LineEnumerable(info, encoding);
        }

        /// <summary>
        /// Opens a <see cref="FileSystemStream"/> for the <paramref name="file"/> in the specified <paramref name="mode"/>
        /// </summary>
        /// <param name="file">File to open stream on</param>
        /// <param name="mode">Mode to use when opening the file</param>
        /// <returns>A <see cref="FileSystemStream"/> to read or write data to the specified <paramref name="file"/></returns>
        public static FileSystemStream OpenFileStream(this IFileInfo file, FileMode mode)
        {
            return file.FileSystem.FileStream.New(file.FullName, mode);
        }

        /// <summary>
        /// Writes the specified <paramref name="lines"/> to the specified <paramref name="info"/> file using the UTF-8 encoding.
        /// If the file already exists and the <paramref name="overwrite"/> flag is set to true, the file will be truncated.
        /// </summary>
        /// <param name="info">File to write to</param>
        /// <param name="lines">Lines to write to file as text</param>
        /// <param name="overwrite">Flag that specifies if the file can be overwritten if it exists</param>
        /// <exception cref="IOException">Exception thrown if the file already exists and the <paramref name="overwrite"/> flag is set to <see cref="false"/></exception>
        public static void WriteLines(this IFileInfo info, IEnumerable<string> lines, bool overwrite = false)
        {
            using (var stream = info.OpenFileStream(GetWriteFileMode(info, overwrite)))
            using (var writer = new StreamWriter(stream))
            foreach(var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        /// <summary>
        /// Writes the specified <paramref name="lines"/> to the specified <paramref name="info"/> file
        /// using the specified <paramref name="encoding"/>.
        /// If the file already exists and the <paramref name="overwrite"/> flag is set to true, the file will be truncated.
        /// </summary>
        /// <param name="info">File to write to</param>
        /// <param name="lines">Lines to write to file as text</param>
        /// <param name="encoding">Encoding to use when writing the <paramref name="lines"/> to the text file</param>
        /// <param name="overwrite">Flag that specifies if the file can be overwritten if it exists</param>
        /// <exception cref="IOException">Exception thrown if the file already exists and the <paramref name="overwrite"/> flag is set to <see cref="false"/></exception>
        public static void WriteLines(this IFileInfo info, IEnumerable<string> lines, Encoding encoding, bool overwrite = false)
        {
            using (var stream = info.OpenFileStream(GetWriteFileMode(info, overwrite)))
            using (var writer = new StreamWriter(stream, encoding))
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        /// <summary>
        /// Appends the specified <paramref name="lines"/> to the specified <paramref name="info"/> file
        /// </summary>
        /// <param name="info">File to append to</param>
        /// <param name="lines">Lines to append to file as text</param>
        public static void AppendLines(this IFileInfo info, IEnumerable<string> lines)
        {
            using (var writer = info.AppendText())
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        private static FileMode GetWriteFileMode(IFileInfo info, bool overwrite)
        {
            if (!overwrite && info.Exists)
            {
                throw new IOException(StringResources.Format("CANNOT_OVERWRITE", info.FullName));
            }

            //if the file already exists it will be truncated
            return FileMode.Create;
        }
    }
}
