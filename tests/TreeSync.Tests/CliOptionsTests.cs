using clausTrarius.TreeSync.Cli;
using clausTrarius.TreeSync.Core.Logging;
using Xunit;

namespace clausTrarius.TreeSync.Tests;

public sealed class CliOptionsTests
{
    [Fact]
    public void Parse_UsesDocumentedDefaults()
    {
        using TemporaryDirectory directory = new();
        string source = directory.Combine("source");
        string target = directory.Combine("target");

        CliOptions options = CliOptions.Parse(
            new[] { "--source", source, "--target", target },
            currentDirectory: directory.Path);

        Assert.Equal(System.IO.Path.GetFullPath(System.IO.Path.Combine(source, "config.json")), options.ConfigPath);
        Assert.Equal(System.IO.Path.GetFullPath(System.IO.Path.Combine(source, ".treesyncignore")), options.IgnorePath);
        Assert.Equal(System.IO.Path.GetFullPath(System.IO.Path.Combine(directory.Path, "treesync.log")), options.LogFilePath);
        Assert.False(options.DryRun);
    }

    [Fact]
    public void Parse_AcceptsLogLevelOverrideAndDryRun()
    {
        using TemporaryDirectory directory = new();

        CliOptions options = CliOptions.Parse(
            new[]
            {
                "--source", directory.Combine("source"),
                "--target", directory.Combine("target"),
                "--log-level", "error",
                "--dry-run"
            },
            currentDirectory: directory.Path);

        Assert.Equal(TreeSyncLogLevel.Error, options.LogLevelOverride);
        Assert.True(options.DryRun);
    }

    [Fact]
    public void Parse_RejectsMissingRequiredParameters()
    {
        Assert.Throws<CliOptionsException>(() => CliOptions.Parse(Array.Empty<string>()));
    }
}
