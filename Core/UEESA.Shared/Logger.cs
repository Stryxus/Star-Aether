public static class Logger
{
	private static string PreviousMessage;

	private static int PreviousMessageCount;

	private static bool IsWebPlatform { get; }

	static Logger()
	{
		try
		{
			IsWebPlatform = Runtime.IsWASM;
			if (!IsWebPlatform && Stryxus.Lib.OS.OperatingSystem.IsWindows)
			{
				Console.BufferWidth = Console.WindowWidth;
			}
			ClearBuffer();
		}
		catch
		{
		}
	}

	private static void PushLog(LogPackage pckg)
	{
		bool flag = true;
		if (PreviousMessage == pckg.Message)
		{
			PreviousMessageCount++;
		}
		else
		{
			flag = PreviousMessage == null;
			PreviousMessage = ((!string.IsNullOrEmpty(pckg.Message)) ? pckg.Message : string.Empty);
			PreviousMessageCount = 0;
		}
		if (pckg.Level == 1 && !IsWebPlatform)
		{
			try
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
			}
			catch
			{
			}
		}
		else if (pckg.Level == 2 && !IsWebPlatform)
		{
			try
			{
				Console.ForegroundColor = ConsoleColor.Red;
			}
			catch
			{
			}
		}
		else if (!IsWebPlatform)
		{
			try
			{
				Console.ForegroundColor = ConsoleColor.White;
			}
			catch
			{
			}
		}
		if (pckg.ClearMode == 0)
		{
			if (!flag && PreviousMessageCount == 0)
			{
				try
				{
					Console.Write("\n");
				}
				catch
				{
				}
			}
			if (pckg.Level == 0 && PreviousMessageCount == 0)
			{
				try
				{
					Console.Write("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][INFO]:  " + pckg.Message);
				}
				catch
				{
				}
			}
			else if (pckg.Level == 0)
			{
				ClearLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][INFO]:  [" + PreviousMessageCount + " Duplicates] " + pckg.Message);
			}
			else if (pckg.Level == 1 && PreviousMessageCount == 0)
			{
				Console.Write("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][WARN]:  " + pckg.Message);
			}
			else if (pckg.Level == 1)
			{
				ClearLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][WARN]:  [" + PreviousMessageCount + " Duplicates] " + pckg.Message);
			}
			else if (pckg.Level == 2 && PreviousMessageCount == 0)
			{
				Console.Write("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][ERROR]: " + pckg.Message);
			}
			else if (pckg.Level == 2)
			{
				ClearLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][ERROR]:  [" + PreviousMessageCount + " Duplicates] " + pckg.Message);
			}
			else if (pckg.Level == 3 && PreviousMessageCount == 0)
			{
				Console.Write("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][DEBUG]: " + pckg.Message);
			}
			else if (pckg.Level == 3)
			{
				ClearLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][DEBUG]:  [" + PreviousMessageCount + " Duplicates] " + pckg.Message);
			}
			else
			{
				try
				{
					Console.Write("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][INFO]: " + pckg.Message);
				}
				catch
				{
				}
			}
		}
		else if (pckg.ClearMode == 3 && !IsWebPlatform)
		{
			try
			{
				Console.Clear();
			}
			catch
			{
			}
		}
		else if (pckg.ClearMode == 2 && !IsWebPlatform)
		{
			try
			{
				Console.Write("\n");
			}
			catch
			{
			}
		}
		else if (pckg.ClearMode == 1 && !IsWebPlatform)
		{
			try
			{
				Console.Write(((!flag) ? "\n" : string.Empty) + pckg.Message);
			}
			catch
			{
			}
		}
		if (!IsWebPlatform)
		{
			try
			{
				Console.ForegroundColor = ConsoleColor.White;
			}
			catch
			{
			}
		}
		flag = false;
	}

	public static void LogInfo(object msg)
	{
		LogPackage pckg = default(LogPackage);
		pckg.PostTime = DateTime.Now;
		pckg.Level = 0;
		pckg.Message = ((msg != null) ? msg.ToString() : "null");
		PushLog(pckg);
	}

	public static void LogWarn(object msg)
	{
		LogPackage pckg = default(LogPackage);
		pckg.PostTime = DateTime.Now;
		pckg.Level = 1;
		pckg.Message = ((msg != null) ? msg.ToString() : "null");
		PushLog(pckg);
	}

	public static void LogError(object msg)
	{
		LogPackage pckg = default(LogPackage);
		pckg.PostTime = DateTime.Now;
		pckg.Level = 2;
		pckg.Message = ((msg != null) ? msg.ToString() : "null");
		PushLog(pckg);
	}

	public static void LogDebug(object msg)
	{
		LogPackage pckg = default(LogPackage);
		pckg.PostTime = DateTime.Now;
		pckg.Level = 3;
		pckg.Message = "Debug logs should not be called in Release mode!";
		PushLog(pckg);
	}

	public static void NewLine(int lines = 1)
	{
		if (lines < 1)
		{
			lines = 1;
		}
		for (int i = 0; i < lines; i++)
		{
			LogPackage pckg = default(LogPackage);
			pckg.ClearMode = 2;
			PushLog(pckg);
		}
	}

	public static void DivideBuffer()
	{
		string text = string.Empty;
		if (IsWebPlatform)
		{
			text = "----------------------------------------------------------------------------------------------------";
		}
		else
		{
			for (int i = 0; i < Console.BufferWidth - 1; i++)
			{
				text += "-";
			}
		}
		LogPackage pckg = default(LogPackage);
		pckg.ClearMode = 1;
		pckg.Message = text;
		PushLog(pckg);
	}

	public static void ClearLine(string content = null)
	{
		if (IsWebPlatform)
		{
			return;
		}
		if (string.IsNullOrEmpty(content))
		{
			content = string.Empty;
			for (int i = 0; i < Console.BufferWidth - 1; i++)
			{
				content += " ";
			}
		}
		Console.Write("\r{0}", content);
	}

	public static void ClearBuffer()
	{
		LogPackage pckg = default(LogPackage);
		pckg.ClearMode = 3;
		PushLog(pckg);
	}

	internal struct LogPackage
	{
		internal DateTime PostTime { get; set; }

		internal int ClearMode { get; set; }

		internal int Level { get; set; }

		internal string Message { get; set; }
	}
}
