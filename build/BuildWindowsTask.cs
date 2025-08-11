using System.IO.Compression;
using Cake.Common.Net;

namespace BuildScripts;

[TaskName("Build Windows")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildWindowsTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnWindows();

    public override void Run(BuildContext context)
    {
        context.ShouldSkipTest = true;
        context.CreateDirectory("cakebuild");
        var downloadUrl = "https://github.com/microsoft/DirectXShaderCompiler/releases/download/v1.8.2505.1/dxc_2025_07_14.zip";
        context.DownloadFile(downloadUrl, "cakebuild/dxc.zip");
        ZipFile.ExtractToDirectory("cakebuild/dxc.zip", $"cakebuild/");
        context.CreateDirectory($"{context.ArtifactsDir}/bin/");
        context.CopyFile($"cakebuild/bin/x64/dxc.exe", $"{context.ArtifactsDir}/bin/dxc.exe");
        context.CopyFile($"cakebuild/bin/x64/dxcompiler.dll", $"{context.ArtifactsDir}/bin/dxcompiler.dll");
        context.CopyFile($"cakebuild/LICENSE-LLVM.txt", $"{context.ArtifactsDir}/bin/LICENSE-LLVM.txt");
        context.CopyFile($"cakebuild/LICENSE-MIT.txt", $"{context.ArtifactsDir}/bin/LICENSE-MIT.txt");
        context.CopyFile($"cakebuild/LICENSE-MS.txt", $"{context.ArtifactsDir}/bin/LICENSE-MS.txt");
    }
}
