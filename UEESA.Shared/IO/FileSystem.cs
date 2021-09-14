using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace UEESA.IO
{
    public static class FileSystem
    {
        public static DirectoryInfo ApplicationDirectory { get; }

        public static DirectoryInfo AppDataDirectory { get; }

        static FileSystem()
        {
            ApplicationDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                AppDataDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }
            else
            {
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            }
        }

        public static Task Create(FileInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            File.Create(info.FullName).Dispose();
            return Task.CompletedTask;
        }

        public static Task Create(DirectoryInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            Directory.CreateDirectory(info.FullName);
            return Task.CompletedTask;
        }

        public static Task Delete(FileInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            File.Delete(info.FullName);
            return Task.CompletedTask;
        }

        public static Task Delete(DirectoryInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            Directory.Delete(info.FullName);
            return Task.CompletedTask;
        }

        public static Task Move(FileInfo from, FileInfo to)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            File.Move(from.FullName, to.FullName);
            return Task.CompletedTask;
        }

        public static Task Move(DirectoryInfo from, DirectoryInfo to)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            Directory.Move(from.FullName, to.FullName);
            return Task.CompletedTask;
        }

        public static Task Copy(FileInfo from, FileInfo to, bool overwrite = false)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            File.Copy(from.FullName, to.FullName, overwrite);
            return Task.CompletedTask;
        }

        public static Task Copy(DirectoryInfo from, DirectoryInfo to, bool overwrite = false)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            string[] directories = Directory.GetDirectories(from.FullName, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < directories.Length; i++)
            {
                Directory.CreateDirectory(directories[i].Replace(from.FullName, to.FullName));
            }
            directories = Directory.GetFiles(from.FullName, "*.*", SearchOption.AllDirectories);
            foreach (string obj in directories)
            {
                File.Copy(obj, obj.Replace(from.FullName, to.FullName), overwrite);
            }
            return Task.CompletedTask;
        }

        public static Task Cut(FileInfo from, FileInfo to, bool overwrite = false)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            File.Copy(from.FullName, to.FullName, overwrite);
            File.Delete(from.FullName);
            return Task.CompletedTask;
        }

        public static Task Cut(DirectoryInfo from, DirectoryInfo to, bool overwrite = false)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            string[] directories = Directory.GetDirectories(from.FullName, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < directories.Length; i++)
            {
                Directory.CreateDirectory(directories[i].Replace(from.FullName, to.FullName));
            }
            directories = Directory.GetFiles(from.FullName, "*.*", SearchOption.AllDirectories);
            foreach (string obj in directories)
            {
                File.Copy(obj, obj.Replace(from.FullName, to.FullName), overwrite);
            }
            directories = Directory.GetFiles(from.FullName, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < directories.Length; i++)
            {
                File.Delete(directories[i]);
            }
            directories = Directory.GetDirectories(from.FullName, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < directories.Length; i++)
            {
                Directory.Delete(directories[i]);
            }
            return Task.CompletedTask;
        }

        public static Task<bool> Exists(FileInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            return Task.FromResult(File.Exists(info.FullName));
        }

        public static Task<bool> Exists(DirectoryInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            return Task.FromResult(Directory.Exists(info.FullName));
        }
    }
}
