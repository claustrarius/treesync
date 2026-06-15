using clausTrarius.TreeSync.Core.Logging;

namespace clausTrarius.TreeSync.Cli;

public sealed class CliOptions
{
    private CliOptions(
        string sourcePath,
        string targetPath,
        string configPath,
        string ignorePath,
        string logFilePath,
        TreeSyncLogLevel? logLevelOverride,
        bool dryRun)
    {
        SourcePath = sourcePath;
        TargetPath = targetPath;
        ConfigPath = configPath;
        IgnorePath = ignorePath;
        LogFilePath = logFilePath;
        LogLevelOverride = logLevelOverride;
        DryRun = dryRun;
    }

    public string SourcePath { get; }

    public string TargetPath { get; }

    public string ConfigPath { get; }

    public string IgnorePath { get; }

    public string LogFilePath { get; }

    public TreeSyncLogLevel? LogLevelOverride { get; }

    public bool DryRun { get; }

    public static CliOptions Parse(string[] args, string? currentDirectory = null)
    {
        currentDirectory ??= Directory.GetCurrentDirectory();

        Dictionary<string, string?> values = new(StringComparer.OrdinalIgnoreCase);
        bool dryRun = false;

        for (int index = 0; index < args.Length; index++)
        {
            string argument = args[index];

            if (string.Equals(argument, "--dry-run", StringComparison.OrdinalIgnoreCase))
            {
                dryRun = true;
                continue;
            }

            if (!argument.StartsWith("--", StringComparison.Ordinal))
            {
                throw new CliOptionsException($"Unexpected argument '{argument}'.");
            }

            if (!IsKnownValueOption(argument))
            {
                throw new CliOptionsException($"Unknown option '{argument}'.");
            }

            if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
            {
                throw new CliOptionsException($"Option '{argument}' requires a value.");
            }

            values[argument] = args[++index];
        }

        string sourcePath = Require(values, "--source");
        string targetPath = Require(values, "--target");
        string configPath = values.TryGetValue("--config", out string? config)
            ? config!
            : Path.Combine(sourcePath, "config.json");
        string ignorePath = values.TryGetValue("--ignore", out string? ignore)
            ? ignore!
            : Path.Combine(sourcePath, ".treesyncignore");
        string logFilePath = values.TryGetValue("--log", out string? log)
            ? log!
            : Path.Combine(currentDirectory, "treesync.log");
        TreeSyncLogLevel? logLevelOverride = values.TryGetValue("--log-level", out string? logLevel)
            ? LogLevelParser.Parse(logLevel)
            : null;

        return new CliOptions(
            Path.GetFullPath(sourcePath),
            Path.GetFullPath(targetPath),
            Path.GetFullPath(configPath),
            Path.GetFullPath(ignorePath),
            Path.GetFullPath(logFilePath),
            logLevelOverride,
            dryRun);
    }

    private static string Require(IReadOnlyDictionary<string, string?> values, string optionName)
    {
        if (!values.TryGetValue(optionName, out string? value) || string.IsNullOrWhiteSpace(value))
        {
            throw new CliOptionsException($"Required option '{optionName}' is missing.");
        }

        return value;
    }

    private static bool IsKnownValueOption(string option)
    {
        return option is "--source" or "--target" or "--config" or "--ignore" or "--log" or "--log-level";
    }
}
