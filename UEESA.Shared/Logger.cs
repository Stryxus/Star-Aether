using System;

using UEESA.Machine;

public static class Logger
{
    private static string? PreviousMessage;
    private static int PreviousMessageCount;
    private static bool IsWebPlatform { get; }

    static Logger()
    {
        try
        {
            IsWebPlatform = Runtime.IsWASM;
            if (!IsWebPlatform && OS.IsWindows) Console.BufferWidth = Console.WindowWidth;
            ClearBuffer();
        }
        catch 
        {
            Console.WriteLine("[CRITICAL]: Logger is unable to initialise!");
        }
    }

    private static void PushLog(LogPackage pckg)
    {
        if (PreviousMessage == pckg.Message) PreviousMessageCount++;
        else
        {
            PreviousMessage = (!string.IsNullOrEmpty(pckg.Message) ? pckg.Message : string.Empty);
            PreviousMessageCount = 0;
        }
        if (pckg.Level == 1 && !IsWebPlatform) Console.ForegroundColor = ConsoleColor.Yellow;
        else if (pckg.Level == 2 && !IsWebPlatform) Console.ForegroundColor = ConsoleColor.Red;
        else if (!IsWebPlatform) Console.ForegroundColor = ConsoleColor.White;
        if (pckg.ClearMode == 0)
        {
            if (pckg.Level == -1) Console.WriteLine(pckg.Message);
            else if (pckg.Level == 0 && PreviousMessageCount == 0) Console.WriteLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][INFO]:  " + pckg.Message);
            else if (pckg.Level == 0) ClearLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][INFO]:  [" + PreviousMessageCount + " Duplicates] " + pckg.Message);
            else if (pckg.Level == 1 && PreviousMessageCount == 0) Console.WriteLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][WARN]:  " + pckg.Message);
            else if (pckg.Level == 1) ClearLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][WARN]:  [" + PreviousMessageCount + " Duplicates] " + pckg.Message);
            else if (pckg.Level == 2 && PreviousMessageCount == 0) Console.WriteLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][ERROR]: " + pckg.Message);
            else if (pckg.Level == 2) ClearLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][ERROR]:  [" + PreviousMessageCount + " Duplicates] " + pckg.Message);
            else if (pckg.Level == 3 && PreviousMessageCount == 0) Console.WriteLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][DEBUG]: " + pckg.Message);
            else if (pckg.Level == 3) ClearLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][DEBUG]:  [" + PreviousMessageCount + " Duplicates] " + pckg.Message);
            else Console.WriteLine("[" + pckg.PostTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "][INFO]: " + pckg.Message);
        }
        else if (pckg.ClearMode == 3 && !IsWebPlatform) Console.Clear();
        else if (pckg.ClearMode == 2 && !IsWebPlatform) Console.WriteLine();
        else if (pckg.ClearMode == 1 && !IsWebPlatform) Console.WriteLine(pckg.Message);
        if (!IsWebPlatform) Console.ForegroundColor = ConsoleColor.White;
    }

    public static void Log(object msg)
    {
        LogPackage pckg = default;
        pckg.Level = -1;
        pckg.Message = msg is not null ? msg.ToString() : "null";
        PushLog(pckg);
    }

    public static void LogInfo(object msg)
    {
        LogPackage pckg = default;
        pckg.PostTime = DateTime.Now;
        pckg.Level = 0;
        pckg.Message = msg is not null ? msg.ToString() : "null";
        PushLog(pckg);
    }

    public static void LogWarn(object msg)
    {
        LogPackage pckg = default;
        pckg.PostTime = DateTime.Now;
        pckg.Level = 1;
        pckg.Message = msg is not null ? msg.ToString() : "null";
        PushLog(pckg);
    }

    public static void LogError(object msg)
    {
        LogPackage pckg = default;
        pckg.PostTime = DateTime.Now;
        pckg.Level = 2;
        pckg.Message = msg is not null ? msg.ToString() : "null";
        PushLog(pckg);
    }

#if DEBUG
    public static void LogDebug(object msg)
    {
        LogPackage pckg = default;
        pckg.PostTime = DateTime.Now;
        pckg.Level = 3;
        pckg.Message = msg is not null ? msg.ToString() : "null";
        PushLog(pckg);
    }
#else
    public static void LogDebug(object msg)
    {
        LogPackage pckg = default;
        pckg.PostTime = DateTime.Now;
        pckg.Level = 3;
        pckg.Message = "Debug logs should not be called in Release mode!";
        PushLog(pckg);
    }
#endif

    public static void LogException<T>(T e) where T : Exception
    {
        LogPackage pckg = default;
        pckg.PostTime = DateTime.Now;
        pckg.Level = 2;
        pckg.Message = $"Source: {e.Source}\n | Data: {e.Data}\n | Message: {e.Message}\n | StackTrace: {e.StackTrace}";
        PushLog(pckg);
    }

    public static void NewLine(int lines = 1)
    {
        if (lines < 1) lines = 1;
        for (int i = 0; i < lines; i++)
        {
            LogPackage pckg = default;
            pckg.ClearMode = 2;
            PushLog(pckg);
        }
    }

    public static void DivideBuffer()
    {
        string text = string.Empty;
        if (IsWebPlatform) text = "----------------------------------------------------------------------------------------------------";
        else
        {
            for (int i = 0; i < Console.BufferWidth - 1; i++) text += "-";
        }
        LogPackage pckg = default;
        pckg.ClearMode = 1;
        pckg.Message = text;
        PushLog(pckg);
    }

    public static void ClearLine(string? content = null)
    {
        if (IsWebPlatform) return;
        if (string.IsNullOrEmpty(content))
        {
            content = string.Empty;
            for (int i = 0; i < Console.BufferWidth - 1; i++) content += " ";
        }
        Console.Write("\r{0}", content);
    }

    public static void ClearBuffer()
    {
        LogPackage pckg = default;
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
