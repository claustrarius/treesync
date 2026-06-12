# docs/algorithm.md

## Gesamtalgorithmus

Der Algorithmus besteht aus fünf Hauptphasen:

1. Initialisierung

2. Sicherheitsprüfung

3. Scan der Quelle

4. Synchronisation

5. Löschphase

---

## Pseudocode

### Programmstart

```

Main():

&#x20;   args = ParseCliArguments()

&#x20;   config = LoadConfig(args.Config)

&#x20;   logLevel = args.LogLevel OR config.Logging.Level

&#x20;   logger = InitializeLogger(args.LogFile, logLevel)

&#x20;   ValidatePaths(args.Source, args.Target)

&#x20;   ignoreRules = LoadIgnoreRules(args.Ignore)

&#x20;   syncEngine = new SyncEngine(

&#x20;       source=args.Source,

&#x20;       target=args.Target,

&#x20;       config=config,

&#x20;       ignoreRules=ignoreRules,

&#x20;       logger=logger,

&#x20;       dryRun=args.DryRun

&#x20;   )

&#x20;   syncEngine.Run()

```

---

## Sync Engine Ablauf

```

Run():

&#x20;   sourceEntries = ScanSource()

&#x20;   ProcessSourceEntries(sourceEntries)

&#x20;   PerformDeletePhase()

```

---

## Scan der Quelle

```

ScanSource():

&#x20;   for each path in EnumerateFileSystemEntries(sourceRoot):

&#x20;       relativePath = MakeRelative(path)

&#x20;       if ignoreRules.Match(relativePath):

&#x20;           LogDebug("IGNORED", relativePath)

&#x20;           continue

&#x20;       if IsDirectory(path):

&#x20;           yield DIRECTORY

&#x20;       else if ExtensionAllowed(path):

&#x20;           yield FILE

&#x20;       else:

&#x20;           LogDebug("SKIPPED_EXTENSION", relativePath)

```

---

## Verarbeitung der Quelle

```

ProcessSourceEntries(entries):

&#x20;   for entry in entries:

&#x20;       targetPath = MapToTarget(entry)

&#x20;       if entry is DIRECTORY:

&#x20;           EnsureDirectoryExists(targetPath)

&#x20;       if entry is FILE:

&#x20;           if target does not exist:

&#x20;               CopyFile(entry, targetPath)

&#x20;           else if FileChanged(entry, targetPath):

&#x20;               UpdateFile(entry, targetPath)

&#x20;           else:

&#x20;               LogDebug("UNCHANGED", entry)

```

---

## Vergleich zweier Dateien

```

FileChanged(source, target):

&#x20;   if source.size != target.size:

&#x20;       return true

&#x20;   if source.timestamp != target.timestamp:

&#x20;       return true

&#x20;   return false

```

---

## COPY

```

CopyFile(source, target):

&#x20;   if dryRun:

&#x20;       log("DRYRUN COPY", source, target)

&#x20;       return

&#x20;   EnsureParentDirectory(target)

&#x20;   Copy(source, target)

&#x20;   log("COPY", source, target)

```

---

## UPDATE

```

UpdateFile(source, target):

&#x20;   if dryRun:

&#x20;       log("DRYRUN UPDATE", source, target)

&#x20;       return

&#x20;   Copy(source, target)

&#x20;   log("UPDATE", source, target)

```

---

## Löschphase

Ziel: Dateien entfernen, die nicht mehr in der Quelle existieren.

```

PerformDeletePhase():

&#x20;   for each path in EnumerateFileSystemEntries(targetRoot):

&#x20;       relativePath = MakeRelative(path)

&#x20;       sourcePath = MapToSource(relativePath)

&#x20;       if sourcePath does not exist:

&#x20;           DeleteTarget(path)

```

---

## DELETE

```

DeleteTarget(path):

&#x20;   if dryRun:

&#x20;       log("DRYRUN DELETE", path)

&#x20;       return

&#x20;   if IsDirectory(path):

&#x20;       RemoveDirectory(path)

&#x20;   else:

&#x20;       RemoveFile(path)

&#x20;   log("DELETE", path)

```

---

## Reihenfolge der Löschphase

Um Probleme bei Verzeichnislöschungen zu vermeiden:

1. Dateien löschen

2. anschließend leere Verzeichnisse löschen

Empfohlen:

```

Zielbaum bottom-up durchlaufen

```

---

## Performance-Hinweise

Empfohlen:

- Verwendung von `Directory.EnumerateFileSystemEntries`

- Verwendung von `Path`, `DirectoryInfo` und `FileInfo`

- Verwendung von `Microsoft.Extensions.FileSystemGlobbing` oder einer dedizierten Matching-Komponente für Ignore-Regeln

- Extensions einmalig normalisieren

---

## Exit Codes

Empfohlene Exitcodes:

```

0  success

1  configuration error

2  safety check failed

3  runtime error

```
