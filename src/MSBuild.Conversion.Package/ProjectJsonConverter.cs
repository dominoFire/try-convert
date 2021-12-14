using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.LibraryModel;
using NuGet.ProjectModel;
using NuGet.Common;

namespace MSBuild.Conversion.Package
{
    public static class ProjectJsonConverter
    {
        public static IEnumerable<PackageReferencePackage> GetProjectJsonPackageReferences(PackageSpec packageSpec)
        {
            if (packageSpec.TargetFrameworks.Count > 1)
            {
                throw new Exception("Multiple frameworks detected in frameworks section");
            }

            var dependencies = new List<PackageReferencePackage>();
            foreach (var targetFramework in packageSpec.TargetFrameworks)
            {
                foreach (var fxdep in targetFramework.Dependencies)
                {
                    var pkg = FromLibraryDependency(fxdep);
                    if (pkg != null)
                    {
                        dependencies.Add(pkg);
                    }
                }
            }

            foreach (var dep in packageSpec.Dependencies)
            {
                var pkg = FromLibraryDependency(dep);
                if (pkg != null)
                {
                    dependencies.Add(pkg);
                }
            }

            return dependencies;
        }

        public static PackageReferencePackage? FromLibraryDependency(LibraryDependency libDep)
        {
            if (libDep.ReferenceType == LibraryDependencyReferenceType.Direct)
            {
                var prp = new PackageReferencePackage
                {
                    ID = libDep.Name,
                    Version = libDep.LibraryRange.VersionRange.ToNormalizedString(),
                };

                if (libDep.IncludeType != LibraryIncludeFlags.All)
                {
                    prp.IncludeAssets = MSBuildStringUtility.Convert(LibraryIncludeFlagUtils.GetFlagString(libDep.IncludeType));
                }

                if (libDep.SuppressParent != LibraryIncludeFlagUtils.DefaultSuppressParent)
                {
                    prp.PrivateAssets = MSBuildStringUtility.Convert(LibraryIncludeFlagUtils.GetFlagString(libDep.SuppressParent));
                }

                return prp;
            }

            return null;
        }

        public static string GetRuntimes(PackageSpec packageSpec)
        {
            var runtimes = packageSpec.RuntimeGraph.Runtimes;
            var supports = packageSpec.RuntimeGraph.Supports;
            var runtimeIdentifiers = new List<string>();
            var runtimeSupports = new List<string>();
            if (runtimes != null && runtimes.Count > 0)
            {
                runtimeIdentifiers.AddRange(runtimes.Keys);

            }

            if (supports != null && supports.Count > 0)
            {
                runtimeSupports.AddRange(supports.Keys);
            }

            var union = string.Join(";", runtimeIdentifiers.Union(runtimeSupports));
            return union;
        }
    }
}
