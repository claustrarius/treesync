# README.md

## Projekt: treesync

`treesync` ist ein CLI-Tool in C# auf Basis von .NET 10 zum synchronen Copy-Deployment einer Verzeichnisstruktur.

Das Tool kopiert Dateien aus einer Quelle in ein Zielverzeichnis, erhält die Ordnerstruktur, berücksichtigt eine Positivliste von Dateitypen und wendet Ignore‑Regeln an. Dateien oder Verzeichnisse, die im Ziel existieren, aber nicht mehr in der Quelle vorhanden sind, werden gelöscht.

Das Zielverzeichnis stellt damit stets eine synchronisierte Kopie der Quelle dar.

---

## Hauptfunktionen

- rekursives Kopieren einer Verzeichnisstruktur

- Positivliste erlaubter Dateiendungen

- Ignore‑Datei ähnlich `.gitignore` (vereinfachte Syntax)

- automatisches Update geänderter Dateien

- Entfernen nicht mehr vorhandener Dateien im Ziel

- Dry‑Run Modus

- Logging in Datei und Konsole

- Sicherheitsprüfungen vor Ausführung

---

## Technologie

- Programmiersprache: C#
- Runtime/SDK: .NET 10
- Anwendungstyp: .NET Console App
- Default Namespace: `clausTrarius.TreeSync`

---

## Voraussetzungen

- .NET 10 SDK

Prüfen mit:

```
dotnet --list-sdks
```

Für Entwicklung, Build und Tests muss ein `10.x` SDK installiert sein, da alle Projekte `net10.0` targeten.

---

## Projektstruktur

Der Code folgt der Architektur aus `docs/architecture.md`:

- `src/TreeSync.Cli`: CLI-Parsing, Top-Level-Fehlerbehandlung und Exitcodes
- `src/TreeSync.Core`: Konfiguration, Logging, Ignore-Regeln, Scanning, Vergleich, Safety und Sync-Engine
- `tests/TreeSync.Tests`: xUnit-Tests für die zentralen Komponenten

---

## Nutzung als EXE

Für ein direkt startbares Windows-Executable muss das CLI-Projekt veröffentlicht werden:

```powershell
dotnet publish src/TreeSync.Cli/TreeSync.Cli.csproj `
  -c Release `
  -p:PublishProfile=win-x64-folder
```

Die erzeugten Dateien liegen danach hier:

```text
publish/TreeSync.exe
publish/TreeSync.dll
```

Nach dem Publish kann `TreeSync.exe` direkt ohne Sourcecode verwendet werden.

```powershell
.\TreeSync.exe `
  --source "C:\pfad\zur\quelle" `
  --target "C:\pfad\zum\ziel" `
  --config "C:\pfad\zur\quelle\config.json" `
  --ignore "C:\pfad\zur\quelle\.treesyncignore" `
  --log "treesync.log" `
  --log-level info
```

`--config`, `--ignore`, `--log` und `--log-level` sind optional.

Dry Run:

```powershell
.\TreeSync.exe `
  --source "C:\pfad\zur\quelle" `
  --target "C:\pfad\zum\ziel" `
  --dry-run
```

---

## Beispielaufruf

```

treesync \\

&#x20; --source ./src \\

&#x20; --target /var/www/app \\

&#x20; --config ./config.json \\

&#x20; --ignore .treesyncignore \\

&#x20; --log treesync.log \\

&#x20; --log-level info

```

---

## Dry Run

```

treesync --source ./src --target /var/www/app --dry-run

```

Zeigt alle Aktionen an, ohne Änderungen vorzunehmen.

---

## Build und Tests

```
dotnet build src/TreeSync.Cli/TreeSync.Cli.csproj
dotnet test tests/TreeSync.Tests/TreeSync.Tests.csproj
```

`dotnet build` ist für Entwicklung und Tests gedacht. Für eine verteilbare `.exe` bitte `dotnet publish` wie oben verwenden.

---

## Exitcodes

- `0`: Erfolg
- `1`: CLI- oder Konfigurationsfehler
- `2`: Sicherheitsprüfung fehlgeschlagen
- `3`: unerwarteter Laufzeitfehler

---

## Standarddateien

Wenn nicht anders angegeben, erwartet das Tool folgende Dateien im Root der Quelle:

- `config.json`

- `.treesyncignore`

---

## Konfiguration

Siehe: `docs/configuration.md`

---

## Ignore-Regeln

Siehe: `docs/ignore-rules.md`

---

## Synchronisationslogik

Siehe: `docs/sync-logic.md`

---

## CLI-Interface

Siehe: `docs/cli.md`
