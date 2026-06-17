# Git Helper

Hilfsskripte für lokale Release-Vorbereitung.

## Changelog aktualisieren

```powershell
./githelper/Update-Changelog.ps1 -Version 1.2.3 -DryRun
./githelper/Update-Changelog.ps1 -Version 1.2.3
```

Das Skript verschiebt den Inhalt von `## [Unreleased]` in einen versionierten Abschnitt:

```text
## [1.2.3] - YYYY-MM-DD
```
