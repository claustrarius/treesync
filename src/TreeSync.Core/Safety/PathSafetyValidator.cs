using clausTrarius.TreeSync.Core.Utilities;

namespace clausTrarius.TreeSync.Core.Safety;

public static class PathSafetyValidator
{
    public static void ValidatePaths(string sourcePath, string targetPath)
    {
        string source = PathUtils.NormalizeFullPath(sourcePath);
        string target = PathUtils.NormalizeFullPath(targetPath);

        if (string.Equals(source, target, StringComparison.OrdinalIgnoreCase))
        {
            throw new SafetyValidationException("Source and target must not point to the same directory.");
        }

        if (!Directory.Exists(target))
        {
            throw new SafetyValidationException($"Target directory must already exist: {target}");
        }

        string? homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (!string.IsNullOrWhiteSpace(homeDirectory))
        {
            string home = PathUtils.NormalizeFullPath(homeDirectory);
            if (string.Equals(target, home, StringComparison.OrdinalIgnoreCase))
            {
                throw new SafetyValidationException("Target directory must not be the user's home directory.");
            }
        }

        string? root = Path.GetPathRoot(Path.GetFullPath(target));
        if (!string.IsNullOrEmpty(root))
        {
            string normalizedRoot = PathUtils.NormalizeFullPath(root);
            if (string.Equals(target, normalizedRoot, StringComparison.OrdinalIgnoreCase))
            {
                throw new SafetyValidationException("Target directory must not be a drive or filesystem root.");
            }
        }
    }
}
