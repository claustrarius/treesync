namespace clausTrarius.TreeSync.Core.Safety;

public sealed class SafetyValidationException : Exception
{
    public SafetyValidationException(string message)
        : base(message)
    {
    }
}
