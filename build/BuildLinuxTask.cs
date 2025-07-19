namespace BuildScripts;

[TaskName("Build Linux")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildLinuxTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnLinux();

    public override void Run(BuildContext context)
    {
        var buildWorkingDir = "cakebuild/";
        context.CreateDirectory(buildWorkingDir);
        context.ReplaceTextInFiles("dxc/tools/clang/tools/dxcompiler/CMakeLists.txt", "add_clang_library(dxcompiler SHARED ${SOURCES})", "add_clang_library(dxcompiler STATIC ${SOURCES})");
        context.StartProcess("cmake", new ProcessSettings
        {
            WorkingDirectory = buildWorkingDir,
            Arguments = "-C ../dxc/cmake/caches/PredefinedParams.cmake -DCMAKE_BUILD_TYPE=Release ../dxc/"
        });
        context.StartProcess("make", new ProcessSettings { WorkingDirectory = buildWorkingDir });
        context.CopyFile($"{buildWorkingDir}/bin/dxc-3.7", $"{context.ArtifactsDir}/dxc");
    }
}
