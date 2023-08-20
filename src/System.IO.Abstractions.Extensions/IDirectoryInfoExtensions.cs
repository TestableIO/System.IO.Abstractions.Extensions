using System.Collections.Generic;
using System.Linq;

namespace System.IO.Abstractions
{
    public static class IDirectoryInfoExtensions
    {
        /// <summary>
        /// Get an <see cref="IDirectoryInfo"/> for the specified sub-directory <paramref name="name"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="name">Sub-directory name (ex. "test")</param>
        /// <returns>An <see cref="IDirectoryInfo"/> for the specified sub-directory</returns>
        public static IDirectoryInfo SubDirectory(this IDirectoryInfo info, string name)
        {
            return info.FileSystem.DirectoryInfo.New(info.FileSystem.Path.Combine(info.FullName, name));
        }

        /// <summary>
        /// Get an <see cref="IDirectoryInfo"/> for the specified sub-directories <paramref name="names"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="names">Sub-directory names (ex. "test", "test2"). Empty or null names are automatically removed from this list</param>
        /// <returns>An <see cref="IDirectoryInfo"/> for the specified sub-directory</returns>
        public static IDirectoryInfo SubDirectory(this IDirectoryInfo info, params string[] names)
        {
            return info.SubDirectory((IEnumerable<string>)names);
        }

        /// <summary>
        /// Get an <see cref="IDirectoryInfo"/> for the specified sub-directories <paramref name="names"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="names">Sub-directory names (ex. "test", "test2"). Empty or null names are automatically removed from this list</param>
        /// <returns>An <see cref="IDirectoryInfo"/> for the specified sub-directory</returns>
        public static IDirectoryInfo SubDirectory(this IDirectoryInfo info, IEnumerable<string> names)
        {
            return info.FileSystem.DirectoryInfo.New(info.FileSystem.Path.Combine(GetPaths(info, names)));
        }

        /// <summary>
        /// Get an <see cref="IFileInfo"/> for the specified file <paramref name="name"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="name">File name (ex. "test.txt")</param>
        /// <returns>An <see cref="IFileInfo"/> for the specified file</returns>
        public static IFileInfo File(this IDirectoryInfo info, string name)
        {
            return info.FileSystem.FileInfo.New(info.FileSystem.Path.Combine(info.FullName, name));
        }

        /// <summary>
        /// Get an <see cref="IFileInfo"/> for the specified sub-directories file <paramref name="names"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="names">Sub-directories and file name (ex. "test", "test.txt"). Empty or null names are automatically removed from this list</param>
        /// <returns>An <see cref="IFileInfo"/> for the specified file</returns>
        public static IFileInfo File(this IDirectoryInfo info, params string[] names)
        {
            return info.File((IEnumerable<string>)names);
        }

        /// <summary>
        /// Get an <see cref="IFileInfo"/> for the specified sub-directories file <paramref name="names"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="names">Sub-directories and file name (ex. "test", "test.txt"). Empty or null names are automatically removed from this list</param>
        /// <returns>An <see cref="IFileInfo"/> for the specified file</returns>
        public static IFileInfo File(this IDirectoryInfo info, IEnumerable<string> names)
        {
            return info.FileSystem.FileInfo.New(info.FileSystem.Path.Combine(GetPaths(info, names)));
        }

        /// <summary>
        /// Throws an exception if the directory <paramref name="info"/> doesn't exists
        /// </summary>
        /// <param name="info">Directory that will be checked for existance</param>
        /// <exception cref="DirectoryNotFoundException">Exception thrown if the directory is not found</exception>
        public static void ThrowIfNotFound(this IDirectoryInfo info)
        {
            if (!info.Exists)
                throw new DirectoryNotFoundException(StringResources.Format("COULD_NOT_FIND_PART_OF_PATH_EXCEPTION", info.FullName));
        }

        /// <summary>
        /// Checks if <paramref name="ancestor"/> is an ancestor of <paramref name="child"/>
        /// </summary>
        /// <param name="ancestor">Ancestor directory</param>
        /// <param name="child">Child directory (sub-directory)</param>
        /// <returns>True if <paramref name="ancestor"/> is an ancestor of <paramref name="child"/> otherwise false</returns>
        public static bool IsAncestorOf(this IDirectoryInfo ancestor, IDirectoryInfo child)
        {
            return child.FullName.Length > ancestor.FullName.Length &&
                   child.FullName.StartsWith(ancestor.FullName);
        }

        public static string[] DiffPaths(this IDirectoryInfo parent, IDirectoryInfo child)
        {
            if (!parent.IsAncestorOf(child))
                throw new ArgumentException("Child is not a subdirectory of source", nameof(child));

            return child.FullName.Substring(parent.FullName.Length + 1)
                .Split(parent.FileSystem.Path.PathSeparator);
        }

        public static void ForEachFile(
            this IDirectoryInfo info, Action<IFileInfo, IDirectoryInfo> fileAction,
            Func<IDirectoryInfo, IDirectoryInfo> directoryAction,
            bool resurse)
        {
            if (resurse)
            {
                foreach (var dir in info.EnumerateDirectories())
                {
                    dir.ForEachFile(fileAction, directoryAction, resurse);
                }
            }

            var d = directoryAction?.Invoke(info) ?? info;
            foreach (var file in info.EnumerateFiles())
            {
                fileAction.Invoke(file, d);
            }
        }

        public static void Copy(this IDirectoryInfo source, IDirectoryInfo destination, bool recurse)
        {
            source.ForEachFile(
                (f, d) => f.CopyTo(d.File(f.Name).FullName),
                d => GetCopyDestination(source, d, destination),
                recurse);
        }

        private static string[] GetPaths(IDirectoryInfo info, IEnumerable<string> names)
        {
            return new[] { info.FullName }
                .Concat(names.Where(n => !String.IsNullOrEmpty(n)))
                .ToArray();
        }

        private static IDirectoryInfo GetCopyDestination(
            this IDirectoryInfo source,
            IDirectoryInfo current,
            IDirectoryInfo destination)
        {
            return source.Equals(current)
                ? destination
                : destination.SubDirectory(source.DiffPaths(current));
        }
    }
}
