using UEESA.IO;

namespace UEESA.Server.Data
{
    public class RuntimeState
    {
#if DEBUG
        public double ClientSize = new DirectoryInfo(FileSystem.ApplicationDirectory.FullName[..(FileSystem.ApplicationDirectory.FullName.LastIndexOf("Core") + 4)])
            .Combine("UEESA.Client").Combine("bin").Combine("Debug")
            .Combine("net" + AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName[(AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName.LastIndexOf('v') + 1)..])
            .Combine("wwwroot").GetFiles("*", SearchOption.AllDirectories).Where(o => o.Extension == ".gz").Sum(o => o.Length);
#else
        public double ClientSize = FileSystem.ApplicationDirectory.Combine("wwwroot").GetFiles().
            Where(o => o.Extension == ".br" || o.Extension == ".mp4" || o.Extension == ".avif" || o.Extension == ".svg" || o.Extension == ".woff2" || o.Extension == ".js").Sum(o => o.Length);
#endif
    }
}
