namespace BuildScripts;

[TaskName("Build macOS")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildMacOSTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnMacOs();

    public override void Run(BuildContext context)
    {
        var buildWorkingDir = "cakebuild/";
        context.CreateDirectory(buildWorkingDir);
        context.CreateDirectory($"{context.ArtifactsDir}/bin");
        context.CreateDirectory($"{context.ArtifactsDir}/lib");
        context.StartProcess("cmake", new ProcessSettings
        {
            WorkingDirectory = buildWorkingDir,
            Arguments = "-C ../dxc/cmake/caches/PredefinedParams.cmake -DCMAKE_BUILD_TYPE=Release -DCMAKE_OSX_ARCHITECTURES=\"x86_64;arm64\" ../dxc/"
        });
        context.StartProcess("make", new ProcessSettings { WorkingDirectory = buildWorkingDir });
        context.CopyFile($"{buildWorkingDir}/bin/dxc-3.7", $"{context.ArtifactsDir}/bin/dxc");
        context.CopyFile($"{buildWorkingDir}/lib/libdxcompiler.dylib", $"{context.ArtifactsDir}/lib/libdxcompiler.dylib");
    }
}
