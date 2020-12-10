using System;

namespace BramrApi.Utils
{
    public static class Utility
    {
        public static string CreateFileUri()
        {
            string Uri = Guid.NewGuid().ToString().Replace("-", "");
            return Uri;
        }
    }
}
