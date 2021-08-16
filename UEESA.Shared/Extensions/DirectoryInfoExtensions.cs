namespace UEESA
{
    public static class DirectoryInfoExtensions
    {
        public static DirectoryInfo Combine(this DirectoryInfo dir, params string[] extensions)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }
            string text = dir.FullName;
            foreach (string path in extensions)
            {
                text = Path.Combine(text, path);
            }
            return new DirectoryInfo(text);
        }

        public static DirectoryInfo Combine(this DirectoryInfo dir, params DirectoryInfo[] extensions)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }
            string text = dir.FullName;
            foreach (DirectoryInfo directoryInfo in extensions)
            {
                text = Path.Combine(text, directoryInfo.FullName);
            }
            return new DirectoryInfo(text);
        }

        public static FileInfo CombineToFile(this DirectoryInfo dir, string filename)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }
            return new FileInfo(dir.FullName + Path.DirectorySeparatorChar + filename);
        }

        public static FileInfo CombineToFile(this DirectoryInfo dir, string filename, params string[] extensions)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }
            string text = dir.FullName;
            foreach (string path in extensions)
            {
                text = Path.Combine(text, path);
            }
            return new FileInfo(text + Path.DirectorySeparatorChar + filename);
        }

        public static FileInfo CombineToFile(this DirectoryInfo dir, string filename, params DirectoryInfo[] extensions)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }
            string text = dir.FullName;
            foreach (DirectoryInfo directoryInfo in extensions)
            {
                text = Path.Combine(text, directoryInfo.FullName);
            }
            return new FileInfo(text + Path.DirectorySeparatorChar + filename);
        }

        public static bool ContainsDirectory(this DirectoryInfo dir, string lookup)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }
            if (lookup == null)
            {
                throw new ArgumentNullException("lookup");
            }
            return dir.FullName.Contains(Path.DirectorySeparatorChar + lookup);
        }

        public static bool ContainsAnyDirectory(this DirectoryInfo dir, params string[] lookup)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }
            if (lookup == null)
            {
                throw new ArgumentNullException("lookup");
            }
            foreach (string text in lookup)
            {
                if (dir.FullName.Contains(Path.DirectorySeparatorChar + text))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAllDirectories(this DirectoryInfo dir, params string[] lookup)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }
            if (lookup == null)
            {
                throw new ArgumentNullException("lookup");
            }
            foreach (string text in lookup)
            {
                if (!dir.FullName.Contains(Path.DirectorySeparatorChar + text))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
