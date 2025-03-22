using System.Diagnostics;

namespace Daric.Shared;

internal class AppInfo : IAppInfo
{
    public AppInfo()
    {
        var executingAssembly = System.Reflection.Assembly.GetEntryAssembly();
        
        if (executingAssembly is not null)
        {
            var fieVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            Version = fieVersionInfo.FileVersion ?? "Unknown";
        }
        else
            Version = "Unknown";
    }
    public string Version
    {
        get;
    }
}
