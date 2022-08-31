using System;

namespace DotnetHsdpSdk.Utils
{
    public static class Validate
    {
        public static void NotNull(object o, string name)
        {
            if (o == null)
            {
                throw new ArgumentException($"Expected '{name}' to not be null.");
            }
        }

        public static void NotNullOrEmpty(string s, string name)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentException($"Expected '{name}' to not be null or empty.");
            }
        }
    }
}
