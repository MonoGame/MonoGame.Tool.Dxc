namespace BuildScripts;

[TaskName("Build Windows")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildWindowsTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnWindows();

    public override void Run(BuildContext context)
    {
        var buildWorkingDir = "cakebuild/";
        context.CreateDirectory(buildWorkingDir);
        context.StartProcess("cmake", new ProcessSettings
        {
            WorkingDirectory = buildWorkingDir,
            Arguments = "-C ../dxc/cmake/caches/PredefinedParams.cmake -DCMAKE_BUILD_TYPE=Release -G \"Visual Studio 17 2022\" ../dxc/"
        });
    }
}
