namespace clausTrarius.TreeSync.Cli;

public sealed class CliOptionsException : Exception
{
    public CliOptionsException(string message)
        : base(message)
    {
    }
}
