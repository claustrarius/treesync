using clausTrarius.TreeSync.Core.Configuration;
using clausTrarius.TreeSync.Core.Ignore;
using clausTrarius.TreeSync.Core.Logging;
using clausTrarius.TreeSync.Core.Sync;
using Xunit;

namespace clausTrarius.TreeSync.Tests;

public sealed class SyncEngineTests
{
    [Fact]
    public void Run_CopiesAllowedFilesAndDeletesObsoleteTargets()
    {
        using TemporaryDirectory root = new();
        string source = root.Combine("source");
        string target = root.Combine("target");
        Directory.CreateDirectory(System.IO.Path.Combine(source, "app"));
        Directory.CreateDirectory(target);
        File.WriteAllText(System.IO.Path.Combine(source, "app", "index.php"), "source");
        File.WriteAllText(System.IO.Path.Combine(source, "app", "notes.txt"), "skip");
        File.WriteAllText(System.IO.Path.Combine(target, "obsolete.php"), "delete");

        RunSync(source, target, dryRun: false);

        Assert.True(File.Exists(System.IO.Path.Combine(target, "app", "index.php")));
        Assert.False(File.Exists(System.IO.Path.Combine(target, "app", "notes.txt")));
        Assert.False(File.Exists(System.IO.Path.Combine(target, "obsolete.php")));
    }

    [Fact]
    public void Run_UpdatesChangedFiles()
    {
        using TemporaryDirectory root = new();
        string source = root.Combine("source");
        string target = root.Combine("target");
        Directory.CreateDirectory(source);
        Directory.CreateDirectory(target);
        string sourceFile = System.IO.Path.Combine(source, "index.php");
        string targetFile = System.IO.Path.Combine(target, "index.php");
        File.WriteAllText(sourceFile, "new");
        File.WriteAllText(targetFile, "old");

        RunSync(source, target, dryRun: false);

        Assert.Equal("new", File.ReadAllText(targetFile));
    }

    [Fact]
    public void Run_DoesNotDeleteIgnoredTargetPaths()
    {
        using TemporaryDirectory root = new();
        string source = root.Combine("source");
        string target = root.Combine("target");
        Directory.CreateDirectory(source);
        Directory.CreateDirectory(System.IO.Path.Combine(target, "cache"));
        File.WriteAllText(System.IO.Path.Combine(target, "cache", "local.log"), "keep");
        string ignorePath = root.Combine(".treesyncignore");
        File.WriteAllText(ignorePath, "cache/");

        RunSync(source, target, dryRun: false, ignorePath: ignorePath);

        Assert.True(File.Exists(System.IO.Path.Combine(target, "cache", "local.log")));
    }

    [Fact]
    public void Run_DryRunLogsActionsWithoutChangingFiles()
    {
        using TemporaryDirectory root = new();
        string source = root.Combine("source");
        string target = root.Combine("target");
        Directory.CreateDirectory(source);
        Directory.CreateDirectory(target);
        File.WriteAllText(System.IO.Path.Combine(source, "index.php"), "source");
        StringWriter writer = new();

        RunSync(source, target, dryRun: true, ignorePath: null, writer: writer);

        Assert.False(File.Exists(System.IO.Path.Combine(target, "index.php")));
        Assert.Contains("DRYRUN COPY", writer.ToString(), StringComparison.Ordinal);
    }

    private static void RunSync(
        string source,
        string target,
        bool dryRun,
        string? ignorePath = null,
        StringWriter? writer = null)
    {
        TreeSyncConfig config = new(new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".php" }, TreeSyncLogLevel.Debug);
        IgnoreRuleSet ignoreRules = ignorePath is null ? IgnoreRuleSet.Empty : IgnoreRuleSet.Load(ignorePath);
        string logRoot = Directory.GetParent(source)?.FullName ?? System.IO.Path.GetTempPath();
        string logFile = System.IO.Path.Combine(logRoot, "treesync.log");

        using TreeSyncLogger logger = TestLoggerFactory.Create(logFile, writer);
        SyncEngine engine = new(source, target, config, ignoreRules, logger, dryRun);
        engine.Run();
    }
}
