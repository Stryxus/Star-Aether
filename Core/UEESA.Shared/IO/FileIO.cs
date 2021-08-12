using System.Collections.Concurrent;
using System.Text;

using Newtonsoft.Json;

namespace UEESA.Shared.IO
{
    public static class FileIO
    {
		private static ConcurrentDictionary<FileInfo, FileStream> ConcurrentOpenStreams { get; } = new ConcurrentDictionary<FileInfo, FileStream>();

		public static async Task WriteText(FileInfo info, object data, Encoding encoding, FileMode mode = FileMode.Open, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.ReadWrite)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (ConcurrentOpenStreams.ContainsKey(info))
			{
				byte[] bytes = encoding.GetBytes(data.ToString());
				await ConcurrentOpenStreams[info].WriteAsync(bytes, 0, bytes.Length);
			}
			else
			{
				using FileStream fs = info.Open(mode, access, share);
				await fs.WriteAsync(encoding.GetBytes(data.ToString()));
			}
		}

		public static async Task<string> ReadText(FileInfo info, FileMode mode = FileMode.Open, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.ReadWrite)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			string result = null;
			if (ConcurrentOpenStreams.ContainsKey(info))
			{
				using StreamReader streamReader = new StreamReader(ConcurrentOpenStreams[info]);
				result = await streamReader.ReadToEndAsync();
			}
			else
			{
				using StreamReader streamReader = new StreamReader(new FileStream(info.FullName, mode, access, share));
				result = await streamReader.ReadToEndAsync();
			}
			return result;
		}

		public static Task<FileStream> OpenStream(FileInfo info, FileMode mode = FileMode.Open, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.ReadWrite)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			FileStream fileStream = new FileStream(info.FullName, mode, access, share);
			return Task.FromResult(ConcurrentOpenStreams.TryAdd(info, fileStream) ? fileStream : null);
		}

		public static async Task CloseStream(FileStream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			await stream.DisposeAsync();
		}

		public static async Task CloseAllStreams()
		{
			foreach (FileStream value in ConcurrentOpenStreams.Values)
			{
				await value.DisposeAsync();
			}
		}

		public static async Task NullifyFile(FileInfo info)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			await File.WriteAllTextAsync(info.FullName, string.Empty);
		}

		public static class Json
        {
			public static async Task WriteJSON<T>(T data, FileInfo info)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				await FileIO.WriteText(info, JsonConvert.SerializeObject((object)data), Encoding.UTF8);
			}

			public static async Task<T> ReadJSON<T>(FileInfo info) where T : new()
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				return JsonConvert.DeserializeObject<T>(await FileIO.ReadText(info));
			}
		}
	}
}
