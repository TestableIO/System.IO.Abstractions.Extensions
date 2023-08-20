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
        /// Checks if <paramref name="ancestor"/> is an ancestor of <paramref name="child"/>.
        /// If <paramref name="ancestor"/> is a parent this method will return <see cref="true"/>
        /// If <paramref name="ancestor"/> and <paramref name="child"/> are the same directory, this will return <see cref="false"/>
        /// </summary>
        /// <param name="ancestor">Ancestor directory</param>
        /// <param name="child">Child directory (sub-directory)</param>
        /// <returns>True if <paramref name="ancestor"/> is an ancestor of <paramref name="child"/> otherwise false</returns>
        public static bool IsAncestorOf(this IDirectoryInfo ancestor, IDirectoryInfo child)
        {
            return child.FullName.Length > ancestor.FullName.Length &&
                   child.FullName.StartsWith(ancestor.FullName);
        }

        /// <summary>
        /// Checks if <paramref name="ancestor"/> is an ancestor of <paramref name="child"/> and returns
        /// a list of segments of the paths of <paramref name="child"/> that are not in common with <paramref name="ancestor"/>
        /// </summary>
        /// <param name="ancestor">Ancestor directory</param>
        /// <param name="child">Child directory (sub-directory)</param>
        /// <returns>A <see cref="string[]"/> with the segments of the paths of <paramref name="child"/> not in common with <paramref name="ancestor"/></returns>
        /// <exception cref="ArgumentException">Exception thrown if <paramref name="ancestor"/> is not an ancestor of <paramref name="child"/></exception>
        public static string[] DiffPaths(this IDirectoryInfo ancestor, IDirectoryInfo child)
        {
            if (!ancestor.IsAncestorOf(child))
                throw new ArgumentException(StringResources.Format("NOT_AN_ANCESTOR", ancestor.FullName, child.FullName), nameof(child));

            return child.FullName.Substring(ancestor.FullName.Length + 1)
                .Split(ancestor.FileSystem.Path.PathSeparator);
        }

        /// <summary>
        /// Applies a <see cref="DiffPaths(IDirectoryInfo, IDirectoryInfo)"/> between <paramref name="ancestor1"/> and <paramref name="child"/>.
        /// The resulting diff of path segments is applied to <paramref name="ancestor2"/> and returned.
        /// If the flag <paramref name="create"/> is set to true the resulting subdirectory of <paramref name="ancestor2"/> will also be created.
        /// <paramref name="ancestor1"/> must be the same directory or an ancestor of <paramref name="child"/> otherwise this method will throw an <see cref="ArgumentException"/>
        /// </summary>
        /// <param name="ancestor1">Ancestor directory</param>
        /// <param name="child">Child directory (sub-directory)</param>
        /// <param name="ancestor2">Directory to apply the diff to</param>
        /// <param name="create">If set to true, the resulting directory will also be created</param>
        /// <returns>An <see cref="IDirectoryInfo"/> which is either a child of <paramref name="ancestor2"/> or <paramref name="ancestor2"/> ifself</returns>
        public static IDirectoryInfo TranslatePaths(
            this IDirectoryInfo ancestor1,
            IDirectoryInfo child,
            IDirectoryInfo ancestor2,
            bool create = false)
        {
            var ret = ancestor1.Equals(child)
                ? ancestor2
                : ancestor2.SubDirectory(ancestor1.DiffPaths(child));

            if (create)
                ret.Create();

            return ret;
        }

        /// <summary>
        /// Executes a <paramref name="fileAction"/> for each file in the <paramref name="info"/> directory
        /// A <paramref name="directoryAction"/> action is also executed before entering any directory, including <paramref name="info"/>
        /// The <see cref="IDirectoryInfo"/> returned by <paramref name="directoryAction"/> is passed as parameter into <paramref name="fileAction"/>
        /// </summary>
        /// <param name="info">Directory where to search for files</param>
        /// <param name="fileAction">Action to apply for each file found in <paramref name="info"/></param>
        /// <param name="directoryAction">Action to apply upon entering any directory including <paramref name="info"/></param>
        /// <param name="resurse">If true the search will be recursive and will include subfolders of <paramref name="info"/>. Defaults to true</param>
        /// <param name="filesSearchPattern">Search pattern to apply when searching files, defaults to '*'</param>
        /// <param name="directoriesSearchPattern">Search pattern to apply when searching directories, defaults to '*'</param>
        public static void ForEachFile(
            this IDirectoryInfo info, Action<IFileInfo, IDirectoryInfo> fileAction,
            Func<IDirectoryInfo, IDirectoryInfo> directoryAction,
            bool resurse = true,
            string filesSearchPattern = "*",
            string directoriesSearchPattern = "*")
        {
            var d = directoryAction?.Invoke(info) ?? info;
            foreach (var file in info.EnumerateFiles(filesSearchPattern))
            {
                fileAction.Invoke(file, d);
            }

            if (!resurse)
                return;

            foreach (var dir in info.EnumerateDirectories(directoriesSearchPattern))
            {
                dir.ForEachFile(fileAction, directoryAction, resurse, filesSearchPattern, directoriesSearchPattern);
            }
        }

        /// <summary>
        /// Copies files from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">Source directory</param>
        /// <param name="destination">Destination directory</param>
        /// <param name="resurse">If true the copy will be recursive and will include subfolders of <paramref name="info"/>. Defaults to true</param>
        /// <param name="filesSearchPattern">Search pattern to apply when searching files, defaults to '*'</param>
        /// <param name="directoriesSearchPattern">Search pattern to apply when searching directories, defaults to '*'</param>
        public static void CopyTo(
            this IDirectoryInfo source,
            IDirectoryInfo destination,
            bool recurse = true,
            string filesSearchPattern = "*",
            string directoriesSearchPattern = "*")
        {
            source.ForEachFile(
                (f, d) => f.CopyTo(d.File(f.Name).FullName),
                d => TranslatePaths(source, d, destination, true),
                recurse,
                filesSearchPattern,
                directoriesSearchPattern);
        }

        private static string[] GetPaths(IDirectoryInfo info, IEnumerable<string> names)
        {
            return new[] { info.FullName }
                .Concat(names.Where(n => !String.IsNullOrEmpty(n)))
                .ToArray();
        }
    }
}
