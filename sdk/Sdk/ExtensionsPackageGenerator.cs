using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Locator;

namespace Microsoft.Azure.Functions.Worker.Sdk
{
    internal class ExtensionsPackageGenerator
    {
        private const string ExtensionsDirectoryName = ".azurefunctions";
        private const string ExtensionsProjectName = "WorkerExtensions.csproj";

        private readonly IDictionary<string, string> _extensions;
        private readonly string _outputPath;

        public static StringBuilder BuildLogs = new StringBuilder();

        public ExtensionsPackageGenerator(IDictionary<string, string> extensions, string outputPath)
        {
            _extensions = extensions ?? throw new ArgumentNullException(nameof(extensions));
            _outputPath = outputPath ?? throw new ArgumentNullException(nameof(outputPath));
        }

        public void GenerateExtensionAssemblies()
        {
            // Environment.SetEnvironmentVariable("MSBuildSDKsPath", @"C:\Program Files\dotnet\sdk\5.0.102\");
            // TODO: Get rid of MsBuildLocator to target netstandard again
            // throwing exception here because if msbuild is already loaded sometimes, this throws.
            try
            {
                MSBuildLocator.RegisterDefaults();
            }
            catch (Exception)
            {
            }

            // ApplyDotNetSdkEnvironmentVariables(@"C:\Program Files\dotnet\sdk\5.0.102\");
            GenerateAssemblies();
        }

        private void GenerateAssemblies()
        {
            BuildUsingProject();
            // BuildUsingProjectInstance();
        }

        private void BuildUsingProject()
        {
            var collection = ProjectCollection.GlobalProjectCollection;

            string csproj = GetCsProjContent();
            XmlReader csProjReader = XmlReader.Create(new StringReader(csproj));

            Project project = collection.LoadProject(csProjReader);
            SetupProject(project);
            BuildProject(project);
            CleanAndSetupEnvironment();
        }

        //private void BuildUsingProjectInstance()
        //{
        //    var collection = ProjectCollection.GlobalProjectCollection;

        //    string csproj = GetCsProjContent();
        //    XmlReader csProjReader = XmlReader.Create(new StringReader(csproj));

        //    var projInstance = SetupProjectInstance();

        //    // This didn't seem to help with parallel builds
        //    var paramsbuild = new BuildParameters()
        //    {
        //        MaxNodeCount = 3
        //    };

        //    var buildRequest = new BuildRequestData(projInstance, new[] { "Restore" });

        //    var br = BuildManager.DefaultBuildManager.Build(paramsbuild, buildRequest);

        //    if (br.Exception != null)
        //    {
        //        throw br.Exception;
        //    }

        //    buildRequest = new BuildRequestData(projInstance, new[] { "Build" });
        //    br = BuildManager.DefaultBuildManager.Build(paramsbuild, buildRequest);

        //    if (br.Exception != null)
        //    {
        //        throw br.Exception;
        //    }
        //}

        private void SetupProject(Project project)
        {
            string extensionsPath = Path.Combine(_outputPath, ExtensionsDirectoryName);
            string extensionsCsprojFakePath = Path.Combine(extensionsPath, ExtensionsProjectName);

            if (Directory.Exists(extensionsPath))
            {
                Directory.Delete(extensionsPath, recursive: true);
            }
            Directory.CreateDirectory(extensionsPath);

            // Unless a physical file with the path is present, Nuget restore fails
            // The file doesn't have to have any content. It just needs to exist.
            File.Create(extensionsCsprojFakePath);

            project.SetGlobalProperty("BuildInParallel", "true");

            project.SetProperty("Configuration", "Release");
            project.SetProperty("OutputPath", extensionsPath);
            project.FullPath = extensionsCsprojFakePath;
        }

        //private ProjectInstance SetupProjectInstance()
        //{
        //    string extensionsPath = Path.Combine(_outputPath, ExtensionsDirectoryName);
        //    string extensionsCsprojFakePath = Path.Combine(extensionsPath, ExtensionsProjectName);


        //    Directory.CreateDirectory(extensionsPath);

        //    File.WriteAllText(extensionsCsprojFakePath, GetCsProjContent());

        //    var projectInstance = new ProjectInstance(extensionsCsprojFakePath);

        //    projectInstance.SetProperty("OutputPath", extensionsPath);
        //    projectInstance.SetProperty("BuildInParallel", "true");
        //    projectInstance.SetProperty("Configuration", "Release");

        //    return projectInstance;
        //}

        private static void BuildProject(Project project)
        {
            var logger = new ExtensionsBuildLogger();

            bool restoreSuccessful = project.Build("Restore", new List<ILogger>() { logger });

            if (!restoreSuccessful)
            {
                throw new Exception($"Failed to restore extensions: {logger.LogLines}");
            }

            bool buildSuccessful = project.Build(logger);

            if (!buildSuccessful)
            {
                throw new Exception($"Failed to build extensions: {logger.LogLines}");
            }
        }

        private void CleanAndSetupEnvironment()
        {
            string extensionsPath = Path.Combine(_outputPath, ExtensionsDirectoryName);
            string extensionsCsprojFakePath = Path.Combine(extensionsPath, ExtensionsProjectName);
            string dllPath = Path.Combine(extensionsPath, "netstandard2.0");

            MoveFiles(dllPath, extensionsPath);

            File.Delete(extensionsCsprojFakePath);
            Directory.Delete(dllPath, recursive: true);
            Directory.Delete(Path.Combine(extensionsPath, "obj"), recursive: true);
        }

        private static void MoveFiles(string from, string to)
        {
            var fromFiles = Directory.GetFiles(from, "*.*", SearchOption.AllDirectories);

            foreach (string fromFile in fromFiles)
            {
                var filename = Path.GetFileName(fromFile);
                var toFile = Path.Combine(to, filename);

                File.Move(fromFile, toFile);
            }
        }

        private string GetCsProjContent()
        {
            string extensionReferences = GetExtensionReferences();

            return $@"
                <Project Sdk=""Microsoft.NET.Sdk"">
                    <PropertyGroup>
                        <TargetFramework>netstandard2.0</TargetFramework>
                        <LangVersion>preview</LangVersion>
                        <Configuration>Release</Configuration>
                        <AssemblyName>Microsoft.Azure.Functions.Worker.Extensions</AssemblyName>
                        <RootNamespace>Microsoft.Azure.Functions.Worker.Extensions</RootNamespace>
                        <MajorMinorProductVersion>1.0</MajorMinorProductVersion>
                        <Version>$(MajorMinorProductVersion).0</Version>
                        <AssemblyVersion>$(MajorMinorProductVersion).0.0</AssemblyVersion>
                        <FileVersion>$(Version)</FileVersion>
                        <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
                    </PropertyGroup>
                    <ItemGroup>
                        {extensionReferences}
                    </ItemGroup>
                </Project>
            ";
        }

        private string GetExtensionReferences()
        {
            var packages = new StringBuilder();

            foreach (KeyValuePair<string, string> extensionPair in _extensions)
            {
                packages.AppendLine(GetPackageReferenceFromExtension(name: extensionPair.Key, version: extensionPair.Value));
            }

            return packages.ToString();
        }

        private static string GetPackageReferenceFromExtension(string name, string version)
        {
            return $@"<PackageReference Include=""{name}"" Version=""{version}"" />";
        }
    }

    internal class ExtensionsBuildLogger : ILogger
    {
        public LoggerVerbosity Verbosity { get; set; }

        public string? Parameters { get; set; }

        public StringBuilder LogLines { get; }

        public ExtensionsBuildLogger()
        {
            LogLines = new StringBuilder();
        }

        public void Initialize(IEventSource eventSource)
        {
            Parameters = "";
            Verbosity = LoggerVerbosity.Normal;

            eventSource.ErrorRaised += errorOccured;
        }

        void errorOccured(object sender, BuildEventArgs arg)
        {
            LogLines.AppendLine(arg.Message);
        }


        public void Shutdown()
        {
        }
    }
}
