using System.Text.Json;
using System.Text.Json.Serialization;
using clausTrarius.TreeSync.Core.Logging;

namespace clausTrarius.TreeSync.Core.Configuration;

public sealed class TreeSyncConfig
{
    public TreeSyncConfig(IReadOnlySet<string> includeExtensions, TreeSyncLogLevel logLevel)
    {
        IncludeExtensions = includeExtensions;
        LogLevel = logLevel;
    }

    public IReadOnlySet<string> IncludeExtensions { get; }

    public TreeSyncLogLevel LogLevel { get; }

    public static TreeSyncConfig Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new ConfigurationException($"Configuration file does not exist: {path}");
        }

        try
        {
            using FileStream stream = File.OpenRead(path);
            ConfigFile? configFile = JsonSerializer.Deserialize<ConfigFile>(
                stream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (configFile is null)
            {
                throw new ConfigurationException("Configuration file is empty or invalid.");
            }

            IReadOnlySet<string> includeExtensions = NormalizeExtensions(configFile.IncludeExtensions);
            TreeSyncLogLevel logLevel = LogLevelParser.Parse(configFile.Logging?.Level);

            return new TreeSyncConfig(includeExtensions, logLevel);
        }
        catch (JsonException ex)
        {
            throw new ConfigurationException($"Configuration file is not valid JSON: {path}", ex);
        }
        catch (ArgumentException ex)
        {
            throw new ConfigurationException(ex.Message, ex);
        }
    }

    private static IReadOnlySet<string> NormalizeExtensions(IReadOnlyCollection<string>? extensions)
    {
        if (extensions is null || extensions.Count == 0)
        {
            throw new ConfigurationException("Configuration must contain at least one include_extensions entry.");
        }

        HashSet<string> normalized = new(StringComparer.OrdinalIgnoreCase);

        foreach (string extension in extensions)
        {
            string value = extension.Trim().ToLowerInvariant();

            if (value.Length == 0)
            {
                throw new ConfigurationException("include_extensions must not contain empty values.");
            }

            if (!value.StartsWith('.', StringComparison.Ordinal))
            {
                throw new ConfigurationException($"include_extensions entry '{extension}' must start with '.'.");
            }

            normalized.Add(value);
        }

        return normalized;
    }

    private sealed class ConfigFile
    {
        [JsonPropertyName("include_extensions")]
        public string[]? IncludeExtensions { get; init; }

        [JsonPropertyName("logging")]
        public LoggingConfigFile? Logging { get; init; }
    }

    private sealed class LoggingConfigFile
    {
        [JsonPropertyName("level")]
        public string? Level { get; init; }
    }
}
