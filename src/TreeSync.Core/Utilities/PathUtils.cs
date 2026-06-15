namespace clausTrarius.TreeSync.Core.Utilities;

public static class PathUtils
{
    public static string NormalizeFullPath(string path)
    {
        return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    public static string ToRelativePath(string rootPath, string path)
    {
        return NormalizeRelativePath(Path.GetRelativePath(rootPath, path));
    }

    public static string NormalizeRelativePath(string relativePath)
    {
        return relativePath
            .Replace(Path.DirectorySeparatorChar, '/')
            .Replace(Path.AltDirectorySeparatorChar, '/')
            .TrimStart('/');
    }

    public static string MapRelativePath(string rootPath, string relativePath)
    {
        string[] parts = NormalizeRelativePath(relativePath).Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return rootPath;
        }

        string[] allParts = new string[parts.Length + 1];
        allParts[0] = rootPath;
        Array.Copy(parts, 0, allParts, 1, parts.Length);
        return Path.Combine(allParts);
    }

    public static string GetNormalizedExtension(string path)
    {
        return Path.GetExtension(path).ToLowerInvariant();
    }
}
