using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace DotNetPack
{
    public static class NugetZZZZ
    {
        public static PackageSource[] GetPackageSources()
        {
            var result = GetPackageSources(@"%APPDATA%\NuGet\NuGet.Config");
            return result;
        }

        public static PackageSource[] GetPackageSources(string nugetConfigPath)
        {
            var nugetConfig = XDocument.Load(nugetConfigPath);

            var packageSources = nugetConfig.Descendants()
                .Where(element => element.Name.LocalName == "packageSources")
                .Single();

            var list = new List<PackageSource>();
            foreach (var packageSource in packageSources.Elements())
            {
                var item = new PackageSource
                {
                    Name = packageSource.Attribute("key").Value,
                    Location = packageSource.Attribute("value").Value,
                    ProtocolVersion = int.Parse(packageSource.Attribute("protocolVersion").Value)
                };

                list.Add(item);
            }

            return list.ToArray();
        }

        public static async Task<NuGetVersion[]> FindVersions(string packageId)
        {
            ILogger logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            SourceCacheContext cache = new SourceCacheContext();
            SourceRepository repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            FindPackageByIdResource resource = await repository.GetResourceAsync<FindPackageByIdResource>();
            IEnumerable<NuGetVersion> versions = await resource.GetAllVersionsAsync(packageId, cache, logger, cancellationToken);

            return versions.ToArray();
        }

        ////public void Test()
        ////{
        ////    //ID of the package to be looked up 
        ////    string packageID = "EntityFramework";
        ////
        ////    //Connect to the official package repository 
        ////    IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
        ////
        ////    //Get the list of all NuGet packages with ID 'EntityFramework' 
        ////    List<IPackage> packages = repo.FindPackagesById(packageID).ToList();
        ////
        ////    //Filter the list of packages that are not Release (Stable) versions 
        ////    packages = packages.Where(item => (item.IsReleaseVersion() == false)).ToList();
        ////    //Iterate through the list and print the full name of the pre-release packages to console 
        ////    foreach (IPackage p in packages)
        ////    {
        ////        Console.WriteLine(p.GetFullName());
        ////    }
        ////}

        ////public void Install()
        ////{
        ////    //ID of the package to be looked 
        ////    up string packageID = "EntityFramework";
        ////
        ////    //Connect to the official package repository IPackageRepository
        ////    repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
        ////
        ////    //Initialize the package manager string path = <PATH_TO_WHERE_THE_PACKAGES_SHOULD_BE_INSTALLED>
        ////    PackageManager packageManager = new PackageManager(repo, path);
        ////
        ////    //Download and unzip the package 
        ////    packageManager.InstallPackage(packageID, SemanticVersion.Parse("5.0.0"));
        ////}
    }
}