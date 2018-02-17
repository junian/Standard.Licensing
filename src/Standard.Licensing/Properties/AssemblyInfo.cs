using System;
using System.Reflection;
//using System.Security.Permissions;

#if PORTABLE
using System.Linq;
#else
using System.Runtime.InteropServices;
#endif

internal class AssemblyInfo
{
    private static string version = null;

    public static string Version
    {
        get
        {
            if (version == null)
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
                version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
#endif

                // if we're still here, then don't try again
                if (version == null)
                {
                    version = string.Empty;
                }
            }

            return version;
        }
    }
}
