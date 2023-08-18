using System.Reflection;
using System.Resources;

namespace System.IO.Abstractions
{
    internal static class StringResources
    {
        public static ResourceManager Manager { get; } = new ResourceManager(
            $"{typeof(StringResources).Namespace}.Resources",
            typeof(StringResources).GetTypeInfo().Assembly);

        public static string Format(string name, params object[] objects)
        {
            return String.Format(Manager.GetString(name), objects);
        }
    }
}
