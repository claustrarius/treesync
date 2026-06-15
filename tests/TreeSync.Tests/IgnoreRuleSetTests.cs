using clausTrarius.TreeSync.Core.Ignore;
using Xunit;

namespace clausTrarius.TreeSync.Tests;

public sealed class IgnoreRuleSetTests
{
    [Fact]
    public void IsIgnored_AppliesExactWildcardAndFolderRules()
    {
        using TemporaryDirectory directory = new();
        string ignorePath = directory.Combine(".treesyncignore");
        File.WriteAllText(
            ignorePath,
            """
            # local configuration
            config/local.php
            *.log
            cache/
            */tmp/
            """);

        IgnoreRuleSet rules = IgnoreRuleSet.Load(ignorePath);

        Assert.True(rules.IsIgnored("config/local.php"));
        Assert.True(rules.IsIgnored("logs/app.log"));
        Assert.True(rules.IsIgnored("assets/cache/image.png"));
        Assert.True(rules.IsIgnored("module/tmp/file.php"));
        Assert.False(rules.IsIgnored("config/prod.php"));
    }
}
