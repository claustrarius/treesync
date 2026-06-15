using clausTrarius.TreeSync.Core.Ignore;
using clausTrarius.TreeSync.Core.Logging;
using clausTrarius.TreeSync.Core.Utilities;

namespace clausTrarius.TreeSync.Core.Scanning;

public sealed class SourceScanner
{
    private readonly string sourceRoot;
    private readonly IReadOnlySet<string> includeExtensions;
    private readonly IgnoreRuleSet ignoreRules;
    private readonly TreeSyncLogger logger;

    public SourceScanner(
        string sourceRoot,
        IReadOnlySet<string> includeExtensions,
        IgnoreRuleSet ignoreRules,
        TreeSyncLogger logger)
    {
        this.sourceRoot = PathUtils.NormalizeFullPath(sourceRoot);
        this.includeExtensions = includeExtensions;
        this.ignoreRules = ignoreRules;
        this.logger = logger;
    }

    public IEnumerable<SourceEntry> Scan()
    {
        if (!Directory.Exists(sourceRoot))
        {
            throw new DirectoryNotFoundException($"Source directory does not exist: {sourceRoot}");
        }

        foreach (SourceEntry entry in ScanDirectory(sourceRoot))
        {
            yield return entry;
        }
    }

    private IEnumerable<SourceEntry> ScanDirectory(string directory)
    {
        foreach (string entryPath in Directory.EnumerateFileSystemEntries(directory))
        {
            string relativePath = PathUtils.ToRelativePath(sourceRoot, entryPath);

            if (ignoreRules.IsIgnored(relativePath))
            {
                logger.LogDebug("IGNORED", relativePath);
                continue;
            }

            if (Directory.Exists(entryPath))
            {
                logger.LogDebug("SCANNED_DIRECTORY", relativePath);
                yield return new SourceEntry(entryPath, relativePath, SourceEntryKind.Directory);

                foreach (SourceEntry child in ScanDirectory(entryPath))
                {
                    yield return child;
                }

                continue;
            }

            string extension = PathUtils.GetNormalizedExtension(entryPath);
            if (includeExtensions.Contains(extension))
            {
                yield return new SourceEntry(entryPath, relativePath, SourceEntryKind.File);
            }
            else
            {
                logger.LogDebug("SKIPPED_EXTENSION", relativePath);
            }
        }
    }
}
