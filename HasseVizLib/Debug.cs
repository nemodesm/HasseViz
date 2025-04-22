using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HasseVizLib;

/// <summary>
/// A class for logging messages
/// </summary>
public static class Debug
{
    public enum LogLevel
    {
        Trace = 1,
        Log = 2,
        Info = 3,
        Warn = 4,
        Error = 5
    }

    /// <summary>
    /// The name of the file in which the logs are stored
    /// </summary>
    private const string LogPath = "log.log";
    /// <summary>
    /// The name of the file in which the logs of the previous session are stored
    /// </summary>
    private const string PreviousLogPath = "prev.log";
    /// <summary>
    /// The name of the application, used in determining the path of the log files
    /// </summary>
    private const string ApplicationName = "HasseViz";
    
    /// <summary>
    /// The writer used to write to the log file
    /// </summary>
    private static readonly StreamWriter Writer;
    
    /// <summary>
    /// The path to the application's AppData folder, where the log files are stored
    /// </summary>
    private static readonly string AppDataPath;

    /// <summary>
    /// Initializes the <see cref="Debug"/> class
    /// </summary>
    static Debug()
    {
        AppDomain.CurrentDomain.ProcessExit += Debug_Dtor;
        
        AppDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/{ApplicationName}/";

        if (!Directory.Exists(AppDataPath))
        {
            Directory.CreateDirectory(AppDataPath);
        }
        
        AttemptMovePreviousFile();

        // var logFile = File.Create(appDataPath + LogPath);
        Writer = new StreamWriter(AppDataPath + LogPath, false)
        {
            NewLine = "\n",
            AutoFlush = true
        };

        Log($"{ApplicationName}: Application opened at {GetTimeStamp()} on {GetDateFormat()}", 0);
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"Started logging in {AppDataPath}{LogPath}");
        Console.ResetColor();
    }

    /// <summary>
    /// The destructor of the <see cref="Debug"/> class, should not be called directly
    /// </summary>
    private static void Debug_Dtor(object sender, EventArgs e)
    {
        Log($"Program closed at {GetTimeStamp()} on {GetDateFormat()}", 0);
        
        Writer.Close();
    }

    /// <summary>
    /// Renames the file currently at <see cref="LogPath"/> to <see cref="PreviousLogPath"/>
    /// </summary>
    private static void AttemptMovePreviousFile()
    {
        if (File.Exists(AppDataPath + LogPath))
        {
            File.Move(AppDataPath + LogPath, AppDataPath + PreviousLogPath, true);
        }
    }

    /// <summary>
    /// Get the current date in the format "DD-MM-YYYY"
    /// </summary>
    /// <returns>The string representation of the date</returns>
    private static string GetDateFormat()
    {
        var date = DateTime.Now;
        return $"{date.Day:00}-{date.Month:00}-{date.Year:0000}";
    }

    /// <summary>
    /// Get the current time in the format "HH:MM:SS"
    /// </summary>
    /// <returns>The string representation of the time</returns>
    private static string GetTimeStamp()
    {
        var time = DateTime.Now;
        return $"{time.Hour:00}:{time.Minute:00}:{time.Second:00}";
    }

    /// <summary>
    /// Get the appropriate prefix for the log message
    /// </summary>
    /// <returns>The string to be prepended to the log message</returns>
    private static string GetPrefix()
    {
        // skip both the frames for this method and the caller as the caller is the Debug.Log[X]()
        var stackTrace = new StackTrace(2);
        return $"{GetTimeStamp()}:{stackTrace.GetFrame(0)?.GetMethod()?.DeclaringType!.Name}";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteMessage(string message, ConsoleColor color = ConsoleColor.White)
    {
#if VISUAL_STUDIO
        System.Diagnostics.Debug.WriteLine(toLog);
#else
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
#endif

        Writer.WriteLine(message);
    }
    
    /// <summary>
    /// Logs a message to the log file, having the specified <paramref name="logLevel"/>
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="logLevel">How important the message is</param>
    public static void Log(object? message, LogLevel logLevel)
    {
        var toLog = (logLevel is 0
            ? $"{message}"
            : $"[{GetPrefix()}/{logLevel}]: {message}{(logLevel == LogLevel.Error ? $"\n{new StackTrace(1)}" : "")}").TrimEnd();

        WriteMessage(toLog, ConsoleColor.DarkCyan);
    }
    
    /// <summary>
    /// Logs an informative message to the log file
    /// </summary>
    /// <param name="message">The message to log</param>
    [Conditional("TRACE")]
    public static void Trace(object? message)
    {
#if TRACE
        var toLog = $"[{GetPrefix()}/TRACE]: {message}".TrimEnd();
        
        WriteMessage(toLog, ConsoleColor.Gray);
#endif
    }
    
    /// <summary>
    /// Logs an informative message to the log file
    /// </summary>
    /// <param name="message">The message to log</param>
    [Conditional("DEBUG")]
    public static void Log(object? message)
    {
        var toLog = $"[{GetPrefix()}/INFO]: {message}".TrimEnd();
        
        WriteMessage(toLog);
    }

    /// <summary>
    /// Logs a warning message to the log file
    /// </summary>
    /// <param name="message">The message to log</param>
    public static void LogWarning(object? message)
    {
        var toLog = $"[{GetPrefix()}/WARN]: {message}".TrimEnd();
        
        WriteMessage(toLog, ConsoleColor.DarkYellow);
    }

    /// <summary>
    /// Logs an error message to the log file, along with the stack trace from where this method was called
    /// </summary>
    /// <param name="message">The message to log</param>
    public static void LogError(object? message)
    {
        var stackTrace = new StackTrace(1);

        var toLog = $"[{GetPrefix()}/ERROR]: {message}\n{stackTrace}".TrimEnd();
        
        WriteMessage(toLog, ConsoleColor.DarkRed);
    }

    /// <summary>
    /// Logs an error message to the log file, along with the given stack trace
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="stackTrace">The stack trace to log</param>
    public static void LogError(object? message, string stackTrace)
    {
        var outerStackTrace = new StackTrace(1);
        var toLog = $"[{GetPrefix()}/ERROR]: {message}\n{stackTrace}\n-- End of inner stack trace --\n{outerStackTrace}".TrimEnd();
        
        WriteMessage(toLog, ConsoleColor.DarkRed);
    }
}