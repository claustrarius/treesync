using clausTrarius.TreeSync.Core.Comparison;
using clausTrarius.TreeSync.Core.Configuration;
using clausTrarius.TreeSync.Core.Ignore;
using clausTrarius.TreeSync.Core.Logging;
using clausTrarius.TreeSync.Core.Scanning;
using clausTrarius.TreeSync.Core.Utilities;

namespace clausTrarius.TreeSync.Core.Sync;

public sealed class SyncEngine
{
    private readonly string sourceRoot;
    private readonly string targetRoot;
    private readonly TreeSyncConfig config;
    private readonly IgnoreRuleSet ignoreRules;
    private readonly TreeSyncLogger logger;
    private readonly bool dryRun;
    private readonly FileComparator comparator;

    public SyncEngine(
        string sourceRoot,
        string targetRoot,
        TreeSyncConfig config,
        IgnoreRuleSet ignoreRules,
        TreeSyncLogger logger,
        bool dryRun,
        FileComparator? comparator = null)
    {
        this.sourceRoot = PathUtils.NormalizeFullPath(sourceRoot);
        this.targetRoot = PathUtils.NormalizeFullPath(targetRoot);
        this.config = config;
        this.ignoreRules = ignoreRules;
        this.logger = logger;
        this.dryRun = dryRun;
        this.comparator = comparator ?? new FileComparator();
    }

    public void Run()
    {
        HashSet<string> expectedRelativePaths = new(StringComparer.OrdinalIgnoreCase);
        SourceScanner scanner = new(sourceRoot, config.IncludeExtensions, ignoreRules, logger);

        foreach (SourceEntry entry in scanner.Scan())
        {
            expectedRelativePaths.Add(entry.RelativePath);
            string targetPath = PathUtils.MapRelativePath(targetRoot, entry.RelativePath);

            if (entry.Kind == SourceEntryKind.Directory)
            {
                EnsureDirectoryExists(targetPath);
                continue;
            }

            ProcessFile(entry.FullPath, targetPath);
        }

        PerformDeletePhase(expectedRelativePaths);
    }

    private void ProcessFile(string sourcePath, string targetPath)
    {
        if (!comparator.FileExists(targetPath))
        {
            CopyFile(sourcePath, targetPath);
            return;
        }

        if (comparator.IsModified(sourcePath, targetPath))
        {
            UpdateFile(sourcePath, targetPath);
            return;
        }

        logger.LogDebug("UNCHANGED", sourcePath);
    }

    private void EnsureDirectoryExists(string targetPath)
    {
        if (Directory.Exists(targetPath))
        {
            return;
        }

        if (dryRun)
        {
            logger.LogDryRun("CREATE DIRECTORY", targetPath);
            return;
        }

        Directory.CreateDirectory(targetPath);
        logger.LogAction("CREATE DIRECTORY", targetPath);
    }

    private void CopyFile(string sourcePath, string targetPath)
    {
        if (dryRun)
        {
            logger.LogDryRun("COPY", sourcePath, targetPath);
            return;
        }

        EnsureParentDirectory(targetPath);
        File.Copy(sourcePath, targetPath, overwrite: false);
        File.SetLastWriteTimeUtc(targetPath, File.GetLastWriteTimeUtc(sourcePath));
        logger.LogAction("COPY", sourcePath, targetPath);
    }

    private void UpdateFile(string sourcePath, string targetPath)
    {
        if (dryRun)
        {
            logger.LogDryRun("UPDATE", sourcePath, targetPath);
            return;
        }

        EnsureParentDirectory(targetPath);
        File.Copy(sourcePath, targetPath, overwrite: true);
        File.SetLastWriteTimeUtc(targetPath, File.GetLastWriteTimeUtc(sourcePath));
        logger.LogAction("UPDATE", sourcePath, targetPath);
    }

    private static void EnsureParentDirectory(string targetPath)
    {
        string? parent = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrEmpty(parent))
        {
            Directory.CreateDirectory(parent);
        }
    }

    private void PerformDeletePhase(IReadOnlySet<string> expectedRelativePaths)
    {
        foreach (string targetPath in EnumerateTargetEntriesBottomUp())
        {
            string relativePath = PathUtils.ToRelativePath(targetRoot, targetPath);
            if (ignoreRules.IsIgnored(relativePath))
            {
                logger.LogDebug("IGNORED", relativePath);
                continue;
            }

            if (expectedRelativePaths.Contains(relativePath))
            {
                continue;
            }

            DeleteTarget(targetPath);
        }
    }

    private IEnumerable<string> EnumerateTargetEntriesBottomUp()
    {
        foreach (string file in Directory.EnumerateFiles(targetRoot, "*", SearchOption.AllDirectories))
        {
            yield return file;
        }

        foreach (string directory in Directory
            .EnumerateDirectories(targetRoot, "*", SearchOption.AllDirectories)
            .OrderByDescending(path => path.Length))
        {
            yield return directory;
        }
    }

    private void DeleteTarget(string targetPath)
    {
        if (dryRun)
        {
            logger.LogDryRun("DELETE", targetPath);
            return;
        }

        if (Directory.Exists(targetPath))
        {
            if (Directory.EnumerateFileSystemEntries(targetPath).Any())
            {
                logger.LogDebug("SKIPPED_NONEMPTY_DIRECTORY", targetPath);
                return;
            }

            Directory.Delete(targetPath, recursive: false);
        }
        else if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }

        logger.LogAction("DELETE", targetPath);
    }
}
