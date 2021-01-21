using System.IO;
using System;
using System.Reflection;

namespace BramrApi.Utils
{
    public static class Utility
    {
        public static string CreateFileUri()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public static string CreatePathFromBegin(string destination)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @$"../../../../../../../../{destination}");
        }
    }
}
