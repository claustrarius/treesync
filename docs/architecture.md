# docs/architecture.md

## Ziel der Architektur

Die Architektur soll:
- klar strukturiert
- leicht testbar
- gut wartbar
- für zukünftige Erweiterungen offen

sein.

Das Tool ist bewusst modular aufgebaut, sodass einzelne Komponenten unabhängig getestet werden können.

---

## Empfohlene Projektstruktur

```

treesync/
│
├─ src/
│  ├─ TreeSync.Cli/
│  │  ├─ TreeSync.Cli.csproj
│  │  ├─ Program.cs
│  │  └─ CliOptions.cs
│  │
│  └─ TreeSync.Core/
│     ├─ TreeSync.Core.csproj
│     ├─ Configuration/
│     │  └─ TreeSyncConfig.cs
│     ├─ Logging/
│     │  └─ TreeSyncLogger.cs
│     ├─ Ignore/
│     │  └─ IgnoreRuleSet.cs
│     ├─ Scanning/
│     │  └─ SourceScanner.cs
│     ├─ Comparison/
│     │  └─ FileComparator.cs
│     ├─ Sync/
│     │  └─ SyncEngine.cs
│     ├─ Safety/
│     │  └─ PathSafetyValidator.cs
│     └─ Utilities/
│        └─ PathUtils.cs
│
├─ docs/
├─ tests/
│  └─ TreeSync.Tests/
│     └─ TreeSync.Tests.csproj
│
├─ config.json
├─ .treesyncignore
├─ treesync.sln
└─ README.md

```

---

## Default Namespace

Der Default Namespace des Projekts lautet:

```
clausTrarius.TreeSync
```

Die Teilprojekte verwenden daraus abgeleitete Namespaces:

- `clausTrarius.TreeSync.Cli`
- `clausTrarius.TreeSync.Core`
- `clausTrarius.TreeSync.Tests`

---

## Program.cs

Startpunkt der Anwendung.

Verantwortlich für:
- Start des CLI
- Fehlerbehandlung auf Top-Level
- Rückgabe von Exit-Codes

Beispielablauf:

1. CLI parsen

2. Konfiguration laden

3. Logger initialisieren

4. Sicherheitsprüfungen durchführen

5. Synchronisation starten

---

## CliOptions.cs

Verantwortlich für:

- Parsing der CLI-Argumente

- Validierung der Eingaben

Empfohlen: Verwendung von `System.CommandLine` oder einer kleinen eigenen Parser-Komponente, solange die dokumentierte CLI stabil bleibt.

Ausgabe:

```

CliOptions

```

Objekt mit allen Parametern.

---

## TreeSyncConfig.cs

Verantwortlich für:

- Laden der `config.json`

- Validierung der Konfiguration

- Normalisierung der Dateiendungen

Beispielstruktur:

```

public sealed class TreeSyncConfig
{
    public ISet<string> IncludeExtensions { get; init; }
    public string LogLevel { get; init; }
}

```

Alle Extensions werden intern \*\*lowercase\*\* gespeichert.

---

## TreeSyncLogger.cs

Zentrale Logging-Komponente.

Funktionen:

- Schreiben ins Logfile

- Ausgabe auf Konsole

- Unterstützung der Level:

&#x20; - error

&#x20; - info

&#x20; - debug

Empfohlen: Wrapper um `Microsoft.Extensions.Logging`.

---

## IgnoreRuleSet.cs

Verarbeitet `.treesyncignore`.

Verantwortlich für:

- Einlesen der Regeln

- Parsing der Patterns

- Prüfung, ob ein Pfad ignoriert werden soll

Beispielmethoden:

```

Load(path)

IsIgnored(relativePath)

```

Wildcard-Matching kann mit `Matcher` aus `Microsoft.Extensions.FileSystemGlobbing` oder einer kleinen dedizierten Matching-Komponente umgesetzt werden.

---

## SourceScanner.cs

Durchläuft die Quellstruktur.

Verantwortlich für:

- rekursives Scannen der Quelle

- Anwendung von Ignore-Regeln

- Filterung nach erlaubten Extensions

Ausgabe:

`IEnumerable` oder `IAsyncEnumerable` von Dateien und Ordnern.

---

## FileComparator.cs

Vergleicht Dateien zwischen Quelle und Ziel.

Vergleichskriterien:

- Dateigröße

- Zeitstempel

Beispielmethoden:

```

FileExists(targetPath)

IsModified(source, target)

```

---

## SyncEngine.cs

Zentrale Logik der Anwendung.

Verantwortlich für:

- Kopieren neuer Dateien

- Aktualisieren geänderter Dateien

- Anlegen von Ordnern

- Löschen veralteter Dateien

Aktionen:

```

COPY

UPDATE

DELETE

```

Die Sync-Engine kennt auch den \*\*Dry-Run Modus\*\*.

---

## PathSafetyValidator.cs

Implementiert Sicherheitsprüfungen.

Funktionen:

```

ValidatePaths(source, target)

```

Prüft:

- source ≠ target

- target existiert

- target ist nicht Home-Verzeichnis

- target ist nicht Root-Verzeichnis

---

## PathUtils.cs

Hilfsfunktionen:

Beispiele:

- Pfadnormalisierung

- Extension-Ermittlung

- Zeitstempelvergleiche

- relative Pfadberechnung

---

## tests/

Empfohlene Testbereiche:

- ignore-Regeln

- Dateivergleich

- Synchronisationslogik

- Sicherheitsprüfungen

Empfohlenes Framework:

```

xUnit

```
