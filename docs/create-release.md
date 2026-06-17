# Release Erstellen

Diese Anleitung beschreibt das vollständige Vorgehen zum Erstellen eines neuen TreeSync-Releases.

## Voraussetzungen

- PowerShell, Windows Terminal oder VS Code Terminal
- Git ist installiert und konfiguriert
- .NET 10 SDK ist installiert
- SSH-Key für GitHub ist eingerichtet
- Schreibzugriff auf `git@github.com:claustrarius/treesync.git`

## Übersicht

```text
1. Changelog aktualisieren
2. Änderungen committen
3. Auf main pushen
4. Versionstag erstellen
5. Tag pushen
6. GitHub Actions prüfen
7. Draft Release prüfen und veröffentlichen
```

## 1. Auf Aktuellen Stand Bringen

```powershell
cd X:\dev\internal\treesync
git checkout main
git pull --ff-only origin main
```

Da der Remote-Zugriff per SSH-Key passphrase-geschützt ist, müssen `git push`, `git pull` und `git fetch` in einer lokal freigegebenen Shell ausgeführt werden.

## 2. Changelog Vorbereiten

Stelle sicher, dass alle Änderungen unter `## [Unreleased]` in [`CHANGELOG.md`](../CHANGELOG.md) dokumentiert sind.

```powershell
./githelper/Update-Changelog.ps1 -Version 1.2.3 -DryRun
./githelper/Update-Changelog.ps1 -Version 1.2.3
```

Das Skript verschiebt den Inhalt von `## [Unreleased]` in einen neuen Abschnitt:

```text
## [1.2.3] - YYYY-MM-DD
```

## 3. Lokal Validieren

```powershell
dotnet restore TreeSync.sln
dotnet build TreeSync.sln --configuration Release
dotnet test TreeSync.sln --configuration Release
dotnet publish src/TreeSync.Cli/TreeSync.Cli.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:EnableCompressionInSingleFile=true `
  -p:DebugType=none `
  -p:DebugSymbols=false
```

Smoke-Test:

```powershell
.\src\TreeSync.Cli\bin\Release\net10.0\win-x64\publish\TreeSync.exe --help
```

## 4. Release-Commit Erstellen

```powershell
git add CHANGELOG.md docs/ githelper/ .github/ README.md src/ tests/
git commit -m "docs: prepare release v1.2.3"
git push origin main
```

## 5. Tag Erstellen Und Pushen

Der Tag muss im Format `vMAJOR.MINOR.PATCH` sein:

```powershell
git tag v1.2.3
git push origin v1.2.3
```

Der Tag-Push startet den Release-Workflow.

## 6. GitHub Actions Prüfen

1. Öffne [GitHub Actions](https://github.com/claustrarius/treesync/actions)
2. Prüfe den Workflow-Lauf für den Tag, z. B. `v1.2.3`
3. Warte, bis der Workflow erfolgreich abgeschlossen ist
4. Prüfe bei Fehlern die Workflow-Logs

## 7. Draft Release Prüfen Und Veröffentlichen

Nach erfolgreichem Workflow-Lauf:

1. Öffne [GitHub Releases](https://github.com/claustrarius/treesync/releases)
2. Öffne den Draft Release für den Tag
3. Prüfe das ZIP-Artefakt, z. B. `TreeSync-1.2.3-win-x64.zip`
4. Kopiere oder prüfe die Release Notes anhand von `CHANGELOG.md`
5. Veröffentliche den Release manuell

## Häufige Fehler Vermeiden

Tag zeigt auf falschen Commit:

```powershell
git log --oneline --decorate -5
```

Tag existiert bereits lokal:

```powershell
git tag --list "v1.2.3"
git tag -d v1.2.3
```

Bereits gepushte Tags werden nicht verschoben. Fehler in einem gepushten Release werden mit einem neuen Patch-Release korrigiert.

## Siehe Auch

- [`CHANGELOG.md`](../CHANGELOG.md)
- [`docs/versioning.md`](versioning.md)
- [`docs/release-pipeline.md`](release-pipeline.md)
- [`githelper/Update-Changelog.ps1`](../githelper/Update-Changelog.ps1)
