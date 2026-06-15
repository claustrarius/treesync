namespace clausTrarius.TreeSync.Core.Scanning;

public sealed record SourceEntry(string FullPath, string RelativePath, SourceEntryKind Kind);
