using System.Collections;
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

        public static IEnumerable<string> EnumerateLines(this IFileInfo info)
        {
            return new LineEnumerable(info, null);
        }

        public static IEnumerable<string> EnumerateLines(this IFileInfo info, Encoding encoding)
        {
            return new LineEnumerable(info, encoding);
        }
    }
}
