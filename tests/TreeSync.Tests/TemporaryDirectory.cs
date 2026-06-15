namespace clausTrarius.TreeSync.Tests;

internal sealed class TemporaryDirectory : IDisposable
{
    public TemporaryDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"treesync-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path);
    }

    public string Path { get; }

    public string Combine(params string[] parts)
    {
        string[] allParts = new string[parts.Length + 1];
        allParts[0] = Path;
        Array.Copy(parts, 0, allParts, 1, parts.Length);
        return System.IO.Path.Combine(allParts);
    }

    public void Dispose()
    {
        if (Directory.Exists(Path))
        {
            Directory.Delete(Path, recursive: true);
        }
    }
}
