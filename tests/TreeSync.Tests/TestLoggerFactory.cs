using clausTrarius.TreeSync.Core.Logging;

namespace clausTrarius.TreeSync.Tests;

internal static class TestLoggerFactory
{
    public static TreeSyncLogger Create(string logFilePath, StringWriter? writer = null)
    {
        return new TreeSyncLogger(
            logFilePath,
            TreeSyncLogLevel.Debug,
            writer ?? new StringWriter(),
            () => new DateTimeOffset(2026, 6, 12, 14, 3, 21, TimeSpan.Zero));
    }
}
