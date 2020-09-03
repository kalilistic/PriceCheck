#tool nuget:?package=GitVersion.CommandLine&version=5.3.7
#tool nuget:?package=NUnit.ConsoleRunner&version=3.10.0
#addin nuget:?package=Cake.FileHelpers&version=3.2.1
#addin nuget:?package=Cake.Git&version=0.22.0

var configuration = Argument ("configuration", "Release");
var version = GitVersion ().SemVer;

Task ("Clean")
    .Does (() => {
        CleanDirectory ("./src/PriceCheck/bin");
        Information("Clean complete.");
});

Task ("Restore")
    .IsDependentOn ("Clean")
    .Does (() => {
        NuGetRestore ("./src/PriceCheck.sln");
});

Task ("Update-Assembly-Info")
    .IsDependentOn ("Restore")
    .Does (() => {
        GitVersion (new GitVersionSettings {
            UpdateAssemblyInfo = true
        });
});

Task ("Update-Plugin-Json")
    .IsDependentOn ("Update-Assembly-Info")
    .Does (() => {
        string json = System.IO.File.ReadAllText("./src/PriceCheck/Properties/PriceCheck.json");
        json = TransformText(json).WithToken("version", version).ToString();
        System.IO.File.WriteAllText("./src/PriceCheck/bin/PriceCheck.json", json);
});

Task("Build")
    .IsDependentOn ("Update-Plugin-Json")
    .Does(() => {
        MSBuild ("./src/PriceCheck/PriceCheck.csproj", settings => settings.SetConfiguration (configuration));
});

Task("Run-Unit-Tests")
    .IsDependentOn ("Build")
    .Does (() => {
        MSBuild ("./src/PriceCheck.sln", settings =>
            settings.SetConfiguration (configuration));
        NUnit3("./src/PriceCheck.Test/bin/PriceCheck.Test.dll", new NUnit3Settings {
                WorkingDirectory = "./src/PriceCheck.Test/bin/",
                StopOnError = true
        });
});

Task("Install")
    .IsDependentOn ("Run-Unit-Tests")
    .Does(() => {
        var pluginDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/XIVLauncher/devPlugins/PriceCheck";
        EnsureDirectoryExists(pluginDir);
        CleanDirectory(pluginDir);
        CopyFile("./src/PriceCheck/bin/PriceCheck.json", pluginDir + "/PriceCheck.json");
        CopyFile("./src/PriceCheck/bin/PriceCheck.dll", pluginDir +  "/PriceCheck.dll");
        Information("Installed into devPlugins.");
});

Task("Publish")
    .IsDependentOn ("Install")
    .Does(() => {
        CreateDirectory("./src/PriceCheck/bin/latest");
        CopyFile("./src/PriceCheck/bin/PriceCheck.json", "./src/PriceCheck/bin/latest/PriceCheck.json");
        CopyFile("./src/PriceCheck/bin/PriceCheck.dll", "./src/PriceCheck/bin/latest/PriceCheck.dll");
        Zip("./src/PriceCheck/bin/latest", "./src/PriceCheck/bin/latest.zip");
        Information("Packaged plugin for publishing.");
        EnsureDirectoryExists("../DalamudPlugins/plugins/PriceCheck");
        CleanDirectory("../DalamudPlugins/plugins/PriceCheck");
        CopyFile("./src/PriceCheck/bin/PriceCheck.json", "../DalamudPlugins/plugins/PriceCheck/PriceCheck.json");
        CopyFile("./src/PriceCheck/bin/latest.zip", "../DalamudPlugins/plugins/PriceCheck/latest.zip");
        Information("Copied package into dalamud plugins workspace.");
});

Task("Cleanup")
    .IsDependentOn ("Publish")
    .Does(() => {
        var assemblyInfoPath1 = MakeAbsolute(File("./src/PriceCheck/Properties/AssemblyInfo.cs"));
        var assemblyInfoPath2 = MakeAbsolute(File("./src/PriceCheck.Test/Properties/AssemblyInfo.cs"));
        var assemblyInfoPath3 = MakeAbsolute(File("./src/PriceCheck.Mock/Properties/AssemblyInfo.cs"));
        var assemblyInfoPath4 = MakeAbsolute(File("./src/PriceCheck.UIDev/Properties/AssemblyInfo.cs"));
        GitCheckout("./", assemblyInfoPath1);
        GitCheckout("./", assemblyInfoPath2);
        GitCheckout("./", assemblyInfoPath3);
        GitCheckout("./", assemblyInfoPath4);
        Information("Reverted assembly info.");
});

Task ("Default")
    .IsDependentOn ("Clean")
    .IsDependentOn ("Restore")
    .IsDependentOn ("Update-Assembly-Info")
    .IsDependentOn ("Update-Plugin-Json")
    .IsDependentOn ("Build")
    .IsDependentOn ("Run-Unit-Tests")
    .IsDependentOn ("Install")
    .IsDependentOn ("Publish")
    .IsDependentOn ("Cleanup");

RunTarget ("Default");