using clausTrarius.TreeSync.Cli;
using clausTrarius.TreeSync.Core.Configuration;
using clausTrarius.TreeSync.Core.Ignore;
using clausTrarius.TreeSync.Core.Logging;
using clausTrarius.TreeSync.Core.Safety;
using clausTrarius.TreeSync.Core.Sync;

return ProgramRunner.Run(args);

public static class ProgramRunner
{
    public static int Run(string[] args)
    {
        try
        {
            CliOptions options = CliOptions.Parse(args);
            TreeSyncConfig config = TreeSyncConfig.Load(options.ConfigPath);
            TreeSyncLogLevel logLevel = options.LogLevelOverride ?? config.LogLevel;

            using TreeSyncLogger logger = new(options.LogFilePath, logLevel);

            try
            {
                PathSafetyValidator.ValidatePaths(options.SourcePath, options.TargetPath);
            }
            catch (SafetyValidationException ex)
            {
                logger.LogError(ex.Message);
                return 2;
            }

            IgnoreRuleSet ignoreRules = IgnoreRuleSet.Load(options.IgnorePath);
            SyncEngine syncEngine = new(
                options.SourcePath,
                options.TargetPath,
                config,
                ignoreRules,
                logger,
                options.DryRun);

            syncEngine.Run();
            return 0;
        }
        catch (CliOptionsException ex)
        {
            LogStartupError(ex.Message);
            return 1;
        }
        catch (ConfigurationException ex)
        {
            LogStartupError(ex.Message);
            return 1;
        }
        catch (ArgumentException ex)
        {
            LogStartupError(ex.Message);
            return 1;
        }
        catch (DirectoryNotFoundException ex)
        {
            LogStartupError(ex.Message);
            return 1;
        }
        catch (Exception ex)
        {
            LogStartupError(ex.ToString());
            return 3;
        }
    }

    private static void LogStartupError(string message)
    {
        using TreeSyncLogger logger = new("treesync.log", TreeSyncLogLevel.Error);
        logger.LogError(message);
    }
}
