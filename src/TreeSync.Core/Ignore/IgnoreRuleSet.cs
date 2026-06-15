using System.Text;
using System.Text.RegularExpressions;
using clausTrarius.TreeSync.Core.Utilities;

namespace clausTrarius.TreeSync.Core.Ignore;

public sealed class IgnoreRuleSet
{
    private readonly IReadOnlyList<IgnoreRule> rules;

    private IgnoreRuleSet(IReadOnlyList<IgnoreRule> rules)
    {
        this.rules = rules;
    }

    public static IgnoreRuleSet Empty { get; } = new(Array.Empty<IgnoreRule>());

    public static IgnoreRuleSet Load(string path)
    {
        if (!File.Exists(path))
        {
            return Empty;
        }

        List<IgnoreRule> rules = new();

        foreach (string line in File.ReadLines(path))
        {
            string value = StripComment(line).Trim();
            if (value.Length == 0)
            {
                continue;
            }

            rules.Add(IgnoreRule.Create(value));
        }

        return new IgnoreRuleSet(rules);
    }

    public bool IsIgnored(string relativePath)
    {
        string normalized = PathUtils.NormalizeRelativePath(relativePath);
        return rules.Any(rule => rule.IsMatch(normalized));
    }

    private static string StripComment(string line)
    {
        int index = line.IndexOf('#', StringComparison.Ordinal);
        return index < 0 ? line : line[..index];
    }

    private sealed class IgnoreRule
    {
        private readonly string pattern;
        private readonly Regex? regex;
        private readonly bool isFolderRule;
        private readonly bool matchesFileNameOnly;

        private IgnoreRule(string pattern, Regex? regex, bool isFolderRule, bool matchesFileNameOnly)
        {
            this.pattern = pattern;
            this.regex = regex;
            this.isFolderRule = isFolderRule;
            this.matchesFileNameOnly = matchesFileNameOnly;
        }

        public static IgnoreRule Create(string rawPattern)
        {
            string normalized = PathUtils.NormalizeRelativePath(rawPattern.Trim());
            bool isFolderRule = normalized.EndsWith('/', StringComparison.Ordinal);
            normalized = normalized.TrimEnd('/');
            bool matchesFileNameOnly = !normalized.Contains('/', StringComparison.Ordinal);

            Regex? regex = normalized.Contains('*', StringComparison.Ordinal)
                ? new Regex(ToRegexPattern(normalized, isFolderRule), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
                : null;

            return new IgnoreRule(normalized, regex, isFolderRule, matchesFileNameOnly);
        }

        public bool IsMatch(string relativePath)
        {
            string normalized = PathUtils.NormalizeRelativePath(relativePath).TrimEnd('/');
            string matchValue = matchesFileNameOnly ? Path.GetFileName(normalized) : normalized;

            if (regex is not null)
            {
                return regex.IsMatch(matchValue);
            }

            if (isFolderRule)
            {
                if (matchesFileNameOnly)
                {
                    return normalized
                        .Split('/', StringSplitOptions.RemoveEmptyEntries)
                        .Any(segment => string.Equals(segment, pattern, StringComparison.OrdinalIgnoreCase));
                }

                return string.Equals(matchValue, pattern, StringComparison.OrdinalIgnoreCase)
                    || normalized.StartsWith($"{pattern}/", StringComparison.OrdinalIgnoreCase);
            }

            return string.Equals(matchValue, pattern, StringComparison.OrdinalIgnoreCase);
        }

        private static string ToRegexPattern(string wildcardPattern, bool isFolderRule)
        {
            StringBuilder builder = new("^");

            foreach (char character in wildcardPattern)
            {
                builder.Append(character switch
                {
                    '*' => "[^/]*",
                    '/' => "/",
                    _ => Regex.Escape(character.ToString())
                });
            }

            builder.Append(isFolderRule ? "(/.*)?$" : "$");
            return builder.ToString();
        }
    }
}
