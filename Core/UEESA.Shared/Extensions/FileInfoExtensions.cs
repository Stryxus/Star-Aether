﻿namespace UEESA
{
    public static class FileInfoExtensions
    {
		public static FileInfo Combine(this FileInfo dir, string filename, params string[] extensions)
		{
			if (dir == null)
			{
				throw new ArgumentNullException("dir");
			}
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			if (extensions == null)
			{
				throw new ArgumentNullException("extensions");
			}
			string text = dir.FullName;
			foreach (string path in extensions)
			{
				text = Path.Combine(text, path);
			}
			return new FileInfo(text + Path.DirectorySeparatorChar + filename);
		}

		public static FileInfo Combine(this FileInfo dir, string filename, params DirectoryInfo[] extensions)
		{
			if (dir == null)
			{
				throw new ArgumentNullException("dir");
			}
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			if (extensions == null)
			{
				throw new ArgumentNullException("extensions");
			}
			string text = dir.FullName;
			foreach (DirectoryInfo directoryInfo in extensions)
			{
				text = Path.Combine(text, directoryInfo.FullName);
			}
			return new FileInfo(text + Path.DirectorySeparatorChar + filename);
		}

		public static bool ContainsDirectory(this FileInfo dir, string lookup)
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

		public static bool ContainsAnyDirectory(this FileInfo dir, params string[] lookup)
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

		public static bool ContainsAllDirectories(this FileInfo dir, params string[] lookup)
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
