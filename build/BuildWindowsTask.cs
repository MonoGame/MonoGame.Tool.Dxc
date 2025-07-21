using Cake.Common.Tools.VSWhere.Latest;

namespace BuildScripts;

[TaskName("Build Windows")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildToolTask))]
public sealed class BuildWindowsTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnWindows();

    public override void Run(BuildContext context)
    {
        var vswhere = new VSWhereLatest(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools);

        // Generate projects
        string cmake = "cmake";

        if (!IsOnPATH(cmake))
            cmake = vswhere.Latest(new VSWhereLatestSettings()).FullPath + @"\Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe";

        var buildWorkingDir = "cakebuild/";
        context.CreateDirectory(buildWorkingDir);
        context.StartProcess(cmake, new ProcessSettings
        {
            WorkingDirectory = buildWorkingDir,
            Arguments = "-C ../dxc/cmake/caches/PredefinedParams.cmake -D CMAKE_MSVC_RUNTIME_LIBRARY=MultiThreaded -DCMAKE_BUILD_TYPE=Release -G \"Visual Studio 17 2022\" ../dxc/"
        });

        // Build
        string msbuild = "msbuild";

        if (!IsOnPATH(msbuild))
            msbuild = vswhere.Latest(new VSWhereLatestSettings()).FullPath + @"\MSBuild\Current\Bin\MSBuild.exe";

        context.StartProcess(msbuild, new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "LLVM.sln /p:Configuration=Release" });

        // Copy artifact
        context.CreateDirectory(context.ArtifactsDir);
        context.CopyFile($"{buildWorkingDir}/Release/bin/dxc.exe", $"{context.ArtifactsDir}/dxc.exe");
    }

    private bool IsOnPATH(string process)
    {
        if (string.IsNullOrEmpty(process))
            return false;

        if (!process.EndsWith(".exe"))
            process += ".exe";

        // Check if process exist on PATH env

        var split = Environment.GetEnvironmentVariable("PATH")?.Split(';');
        if (split != null)
        {
            foreach (var path in split)
            {
                string processPath = System.IO.Path.Combine(path, process);
                if (File.Exists(processPath))
                    return true;
            }
        }

        return false;
    }
}
