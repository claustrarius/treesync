using clausTrarius.TreeSync.Core.Configuration;
using clausTrarius.TreeSync.Core.Logging;
using Xunit;

namespace clausTrarius.TreeSync.Tests;

public sealed class ConfigurationTests
{
    [Fact]
    public void Load_NormalizesExtensionsAndLogLevel()
    {
        using TemporaryDirectory directory = new();
        string configPath = directory.Combine("config.json");
        File.WriteAllText(
            configPath,
            """
            {
              "include_extensions": [".PHP", ".js"],
              "logging": {
                "level": "debug"
              }
            }
            """);

        TreeSyncConfig config = TreeSyncConfig.Load(configPath);

        Assert.Contains(".php", config.IncludeExtensions);
        Assert.Contains(".js", config.IncludeExtensions);
        Assert.Equal(TreeSyncLogLevel.Debug, config.LogLevel);
    }

    [Fact]
    public void Load_RejectsExtensionWithoutDot()
    {
        using TemporaryDirectory directory = new();
        string configPath = directory.Combine("config.json");
        File.WriteAllText(configPath, """{"include_extensions":["php"]}""");

        Assert.Throws<ConfigurationException>(() => TreeSyncConfig.Load(configPath));
    }
}
