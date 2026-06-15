using clausTrarius.TreeSync.Core.Comparison;
using Xunit;

namespace clausTrarius.TreeSync.Tests;

public sealed class FileComparatorTests
{
    [Fact]
    public void IsModified_ReturnsFalseForSameSizeAndTimestamp()
    {
        using TemporaryDirectory directory = new();
        string source = directory.Combine("source.php");
        string target = directory.Combine("target.php");
        File.WriteAllText(source, "same");
        File.WriteAllText(target, "same");
        DateTime timestamp = new(2026, 6, 12, 14, 3, 21, DateTimeKind.Utc);
        File.SetLastWriteTimeUtc(source, timestamp);
        File.SetLastWriteTimeUtc(target, timestamp);

        FileComparator comparator = new();

        Assert.False(comparator.IsModified(source, target));
    }

    [Fact]
    public void IsModified_ReturnsTrueForDifferentTimestamp()
    {
        using TemporaryDirectory directory = new();
        string source = directory.Combine("source.php");
        string target = directory.Combine("target.php");
        File.WriteAllText(source, "same");
        File.WriteAllText(target, "same");
        File.SetLastWriteTimeUtc(source, new DateTime(2026, 6, 12, 14, 3, 21, DateTimeKind.Utc));
        File.SetLastWriteTimeUtc(target, new DateTime(2026, 6, 12, 14, 4, 21, DateTimeKind.Utc));

        FileComparator comparator = new();

        Assert.True(comparator.IsModified(source, target));
    }
}
