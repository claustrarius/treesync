namespace clausTrarius.TreeSync.Core.Comparison;

public sealed class FileComparator
{
    public bool FileExists(string targetPath)
    {
        return File.Exists(targetPath);
    }

    public bool IsModified(string sourcePath, string targetPath)
    {
        FileInfo source = new(sourcePath);
        FileInfo target = new(targetPath);

        if (source.Length != target.Length)
        {
            return true;
        }

        return source.LastWriteTimeUtc != target.LastWriteTimeUtc;
    }
}
