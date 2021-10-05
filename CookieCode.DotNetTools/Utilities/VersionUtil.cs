using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools.Utilities
{
    public static class VersionUtil
    {
        public static Version BumpMajor(Version version)
        {
            var result = new Version(
                Math.Max(version.Major, 0) + 1,
                0,
                version.Build == -1 ? -1 : 0,
                version.Revision == -1 ? -1 : 0);

            return result;
        }

        public static Version BumpMinor(Version version)
        {
            var result = new Version(
                Math.Max(version.Major, 0),
                Math.Max(version.Minor, 0) + 1,
                version.Build == -1 ? -1 : 0,
                version.Revision == -1 ? -1 : 0);

            return result;
        }

        public static Version BumpBuild(Version version)
        {
            var result = new Version(
                Math.Max(version.Major, 0),
                Math.Max(version.Minor, 0),
                Math.Max(version.Build, 0) + 1,
                version.Revision == -1 ? -1 : 0);

            return result;
        }

        public static Version BumpRevvision(Version version)
        {
            var result = new Version(
                Math.Max(version.Major, 0),
                Math.Max(version.Minor, 0),
                Math.Max(version.Build, 0),
                Math.Max(version.Revision, 0) + 1);

            return result;
        }
    }
}
