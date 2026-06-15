using clausTrarius.TreeSync.Core.Safety;
using Xunit;

namespace clausTrarius.TreeSync.Tests;

public sealed class PathSafetyValidatorTests
{
    [Fact]
    public void ValidatePaths_RejectsSameSourceAndTarget()
    {
        using TemporaryDirectory directory = new();

        Assert.Throws<SafetyValidationException>(() =>
            PathSafetyValidator.ValidatePaths(directory.Path, directory.Path));
    }

    [Fact]
    public void ValidatePaths_RejectsMissingTarget()
    {
        using TemporaryDirectory source = new();
        string missingTarget = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"treesync-missing-{Guid.NewGuid():N}");

        Assert.Throws<SafetyValidationException>(() =>
            PathSafetyValidator.ValidatePaths(source.Path, missingTarget));
    }

    [Fact]
    public void ValidatePaths_AllowsDifferentExistingDirectories()
    {
        using TemporaryDirectory source = new();
        using TemporaryDirectory target = new();

        PathSafetyValidator.ValidatePaths(source.Path, target.Path);
    }
}
