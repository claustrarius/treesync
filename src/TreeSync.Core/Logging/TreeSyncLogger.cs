namespace clausTrarius.TreeSync.Core.Logging;

public sealed class TreeSyncLogger : IDisposable
{
    private readonly StreamWriter logWriter;
    private readonly TextWriter consoleWriter;
    private readonly Func<DateTimeOffset> clock;
    private bool disposed;

    public TreeSyncLogger(
        string logFilePath,
        TreeSyncLogLevel logLevel,
        TextWriter? consoleWriter = null,
        Func<DateTimeOffset>? clock = null)
    {
        LogLevel = logLevel;
        this.consoleWriter = consoleWriter ?? Console.Out;
        this.clock = clock ?? DateTimeOffset.Now;

        string? directory = Path.GetDirectoryName(Path.GetFullPath(logFilePath));
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        logWriter = new StreamWriter(new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
        {
            AutoFlush = true
        };
    }

    public TreeSyncLogLevel LogLevel { get; }

    public void LogAction(string action, string source, string? target = null)
    {
        Write(TreeSyncLogLevel.Info, Format(action, source, target));
    }

    public void LogDryRun(string action, string source, string? target = null)
    {
        Write(TreeSyncLogLevel.Info, Format($"DRYRUN {action}", source, target));
    }

    public void LogDebug(string action, string message)
    {
        Write(TreeSyncLogLevel.Debug, Format(action, message, null));
    }

    public void LogError(string message)
    {
        Write(TreeSyncLogLevel.Error, Format("ERROR", message, null));
    }

    private string Format(string action, string source, string? target)
    {
        string timestamp = clock().ToString("yyyy-MM-dd HH:mm:ss");
        return target is null
            ? $"{timestamp} {action} {source}"
            : $"{timestamp} {action} {source} -> {target}";
    }

    private void Write(TreeSyncLogLevel level, string message)
    {
        if (level > LogLevel)
        {
            return;
        }

        consoleWriter.WriteLine(message);
        logWriter.WriteLine(message);
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        logWriter.Dispose();
    }
}
