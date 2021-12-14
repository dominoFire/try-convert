using System.Linq;
using NuGet.ProjectModel;
using Xunit;

namespace MSBuild.Conversion.Package.Tests
{
    public class ProjectJsonConverterTests
    {
        [Fact]
        public void GetProjectJsonPackageReferences_IncludeAssets_Succeeds()
        {
            var strProjectJson = @"{
    ""dependencies"": {
        ""packageA"": {
            ""version"": ""1.0.0"",
            ""include"": ""build, native""
        }
    }
}";
            var projectJson = JsonPackageSpecReader.GetPackageSpec(strProjectJson, "myJson.project.json", "x");

            var prps = ProjectJsonConverter.GetProjectJsonPackageReferences(projectJson);
            Assert.NotNull(prps);
            Assert.Equal("build; native", prps.First().IncludeAssets);
        }

        [Fact]
        public void GetProjectJsonPackageReferences_ExcludeAssets_InIncludeAssets_Succeeds()
        {
            var strProjectJson = @"{
    ""dependencies"": {
        ""packageA"": {
            ""version"": ""1.0.0"",
            ""include"": ""contentFiles, build"",
            ""exclude"": ""build""
        }
    }
}";
            // NuGet resolves include and exclude assets, leaves final assets in include
            var projectJson = JsonPackageSpecReader.GetPackageSpec(strProjectJson, "myJson.project.json", "x");

            var prps = ProjectJsonConverter.GetProjectJsonPackageReferences(projectJson);

            Assert.NotNull(prps);
            Assert.Equal("contentfiles", prps.First().IncludeAssets);
        }

        [Fact]
        public void GetProjectJsonPackageReferences_PrivateAssets_Succeeds()
        {
            var strProjectJson = @"{
    ""dependencies"": {
        ""packageA"": {
            ""version"": ""1.0.0"",
            ""suppressParent"": ""contentFiles"",
        }
    }
}";
            var projectJson = JsonPackageSpecReader.GetPackageSpec(strProjectJson, "myJson.project.json", "x");

            var prps = ProjectJsonConverter.GetProjectJsonPackageReferences(projectJson);

            Assert.NotNull(prps);
            Assert.Equal("contentfiles", prps.First().PrivateAssets);
        }
    }
}
