# Versionierung und Changelog

Dieses Dokument beschreibt, wie Anwendungs-Versionen und `CHANGELOG.md` gepflegt werden.

Diese Anwendung nutzt [Semantic Versioning](https://semver.org/lang/de/) (`MAJOR.MINOR.PATCH`) und ein Changelog nach [Keep a Changelog](https://keepachangelog.com/de/1.1.0/).

## Grundsätze

- Die verbindliche Release-Version ist der Git-Tag im Format `vMAJOR.MINOR.PATCH`
- Es gibt keine separate `VERSION`-Datei
- `CHANGELOG.md` enthält die menschlich gepflegte Änderungshistorie
- Neue Einträge werden während der Entwicklung unter `## [Unreleased]` gesammelt
- Vor einem Release wird `Unreleased` in einen versionierten Abschnitt verschoben
- GitHub Releases werden als Draft erstellt und vor der Veröffentlichung manuell geprüft

## Beteiligte Dateien

| Datei | Rolle |
|-------|-------|
| [`CHANGELOG.md`](../CHANGELOG.md) | Änderungshistorie mit `Unreleased` und Release-Abschnitten |
| [`githelper/Update-Changelog.ps1`](../githelper/Update-Changelog.ps1) | Verschiebt `Unreleased` in einen neuen Release-Abschnitt |
| [`docs/release-pipeline.md`](release-pipeline.md) | Technische Beschreibung der GitHub Actions Pipeline |

## Changelog pflegen

Änderungen werden laufend unter `## [Unreleased]` eingetragen. Kategorien:

- `Neue Features` für neue Funktionalität
- `Fixes` für Fehlerbehebungen
- `Änderungen` für geändertes Verhalten
- `Dokumentation` für reine Dokumentationsänderungen
- `Entfernt` für entfernte Funktionen, Dateien oder Artefakte

Beispiel:

```markdown
## [Unreleased]

### Neue Features

- Parameter `--example` ergänzt

### Fixes

- Fehlerhafte Ausgabe bei leerem Zertifikatsspeicher korrigiert
```

## Version vorbereiten

Das Helper-Script verschiebt den Inhalt von `Unreleased` in einen neuen Release-Abschnitt:

```powershell
./githelper/Update-Changelog.ps1 -Version 1.2.3 -DryRun
./githelper/Update-Changelog.ps1 -Version 1.2.3
```

Das Script:

1. akzeptiert `1.2.3` oder `v1.2.3`
2. prüft das SemVer-Format
3. prüft, ob der Release-Abschnitt noch nicht existiert
4. verschiebt den Inhalt von `## [Unreleased]` nach `## [1.2.3] - YYYY-MM-DD`
5. lässt `## [Unreleased]` leer für die nächste Entwicklung

Wenn ein anderes Releasedatum benötigt wird:

```powershell
./githelper/Update-Changelog.ps1 -Version 1.2.3 -Date 2026-06-17
```

## Release-Commit und Tag

Ein Git-Tag muss auf den Commit zeigen, dessen Repository-Zustand veröffentlicht wird.

Vor dem Taggen prüfen:

- `CHANGELOG.md` enthält den passenden Abschnitt `## [X.Y.Z] - YYYY-MM-DD`
- `## [Unreleased]` ist leer oder enthält nur spätere Änderungen
- alle Code-, Dokumentations- und Pipeline-Änderungen für das Release sind committed
- lokale Validierung aus `docs/release-pipeline.md` war erfolgreich

Beispiel:

```powershell
git add CHANGELOG.md docs/ githelper/ .github/ README.md
git commit -m "docs: prepare changelog for v1.2.3"
git push origin main
git tag v1.2.3
```

## Tag prüfen

Vor dem Push prüfen, ob Tag und Changelog zusammenpassen:

```powershell
git rev-list -n 1 v1.2.3
git show v1.2.3:CHANGELOG.md
git log --oneline v1.0.0..v1.2.3
git diff --stat v1.0.0..v1.2.3
```

Bereits gepushte Tags werden nicht verschoben. Fehler in einem gepushten Release werden mit einem neuen Patch-Release korrigiert.

## Veröffentlichen

Nach erfolgreicher Prüfung:

```powershell
git push origin v1.2.3
```

Der Tag-Push startet den Release-Workflow. GitHub erstellt einen Draft Release mit Artefakt. Die Release Notes werden anschließend manuell anhand von `CHANGELOG.md` geprüft und veröffentlicht.

Da der Remote-Zugriff per SSH-Key passphrase-geschützt ist, müssen `git push`, `git pull` und `git fetch` in einer lokal freigegebenen Shell ausgeführt werden.

## Checkliste vor dem Release

- [ ] Alle relevanten Änderungen stehen unter `## [Unreleased]`
- [ ] Bump-Typ (`patch`, `minor`, `major`) passt zur Änderung
- [ ] `Update-Changelog.ps1 -DryRun` geprüft
- [ ] `CHANGELOG.md` enthält den neuen Release-Abschnitt
- [ ] Lokaler Build und Publish waren erfolgreich
- [ ] Release-Commit enthält alle Release-Dateien
- [ ] Annotierter Tag zeigt auf den Release-Commit
- [ ] Commit und Tag wurden gepusht
- [ ] Draft Release wurde in GitHub geprüft und veröffentlicht

## Siehe auch

- [`CHANGELOG.md`](../CHANGELOG.md)
- [`docs/create-release.md`](create-release.md) - Praktische Schritt-für-Schritt-Anleitung
- [`githelper/README.md`](../githelper/README.md)
- [`docs/release-pipeline.md`](release-pipeline.md)
