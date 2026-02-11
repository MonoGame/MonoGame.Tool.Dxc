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
        
        BuildForArchitecture(context, "x64", "windows-x64");
        BuildForArchitecture(context, "arm64", "windows-arm64");
    }

    private void BuildForArchitecture(BuildContext context, string arch, string rid)
    {
        context.CreateDirectory($"{context.ArtifactsDir}/{rid}");
        context.CopyFile($"cakebuild/bin/{arch}/dxc.exe", $"{context.ArtifactsDir}/{rid}/dxc.exe");
        context.CopyFile($"cakebuild/bin/{arch}/dxcompiler.dll", $"{context.ArtifactsDir}/{rid}/dxcompiler.dll");
        context.CopyFile($"cakebuild/LICENSE-LLVM.txt", $"{context.ArtifactsDir}/{rid}/LICENSE-LLVM.txt");
        context.CopyFile($"cakebuild/LICENSE-MIT.txt", $"{context.ArtifactsDir}/{rid}/LICENSE-MIT.txt");
        context.CopyFile($"cakebuild/LICENSE-MS.txt", $"{context.ArtifactsDir}/{rid}/LICENSE-MS.txt");
    }
}
