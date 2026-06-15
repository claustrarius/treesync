namespace clausTrarius.TreeSync.Core.Logging;

public static class LogLevelParser
{
    public static TreeSyncLogLevel Parse(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            null or "" => TreeSyncLogLevel.Info,
            "error" => TreeSyncLogLevel.Error,
            "info" => TreeSyncLogLevel.Info,
            "debug" => TreeSyncLogLevel.Debug,
            _ => throw new ArgumentException($"Unsupported log level '{value}'. Allowed values: error, info, debug.")
        };
    }
}
