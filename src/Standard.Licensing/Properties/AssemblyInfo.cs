using System.Reflection;
//using System.Security.Permissions;

#if PORTABLE
using System.Linq;
#else
#endif

internal class AssemblyInfo
{
    private static string _version = null;

    public static string Version
    {
        get
        {
            if (_version == null)
            {
#if PORTABLE
#if NEW_REFLECTION
                var a = typeof(AssemblyInfo).GetTypeInfo().Assembly;
                var c = a.GetCustomAttributes(typeof(AssemblyVersionAttribute));
#else
                var a = typeof(AssemblyInfo).Assembly;
                var c = a.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
#endif
                var v = (AssemblyVersionAttribute)c.FirstOrDefault();
                if (v != null)
                {
                    version = v.Version;
                }
#else
                _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
#endif

                // if we're still here, then don't try again
                if (_version == null)
                {
                    _version = string.Empty;
                }
            }

            return _version;
        }
    }
}
