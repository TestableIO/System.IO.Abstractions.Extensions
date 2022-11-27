using System.Collections.Generic;

namespace System.IO.Abstractions
{
    public static class IPathExtensions
    {
        public static string Combine(this IPath path, string root, params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return root;

            for(int i = 0; i < paths.Length; i++)
                root = path.Combine(root, paths[i]);

            return root;
        }

        public static string Combine(this IPath path, IEnumerable<string> paths)
        {
            using(var enumerator = paths.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return null;

                var ret = enumerator.Current;

                while(enumerator.MoveNext())
                    ret = path.Combine(ret, enumerator.Current);

                return ret;
            }
        }
    }
}
