# Release Pipeline

Dieses Dokument beschreibt den Release-Prozess für TreeSync.

## Überblick

TreeSync verwendet GitHub Actions für CI und Release-Erstellung. Releases sind tag-getrieben:

```powershell
git tag v1.2.3
git push origin v1.2.3
```

Ein Tag im Format `vMAJOR.MINOR.PATCH` erstellt automatisch einen GitHub Draft Release.

## Projektanpassungen

Für die GitHub-Pipeline verwendet TreeSync die moderne .NET-Toolchain:

- `TreeSync.Cli.csproj`, `TreeSync.Core.csproj` und `TreeSync.Tests.csproj` sind SDK-style-Projekte
- Ziel-Framework ist `net10.0`
- Assembly-Metadaten liegen in den Projektdateien
- `AssemblyVersion` und `FileVersion` bleiben stabil auf `1.0.0.0`
- Die Release-Version wird erst beim Publish über `InformationalVersion` gesetzt
- `App.config` wird nicht benötigt
- `Properties/AssemblyInfo.cs` wird nicht benötigt

Das Release-Artefakt wird bewusst als Windows-x64-EXE gebaut, weil die erste Distribution als direkt startbare Windows-CLI erfolgen soll.

## Workflow-Dateien

Die Pipeline besteht aus zwei GitHub Actions Workflows:

```text
.github/workflows/ci.yml
.github/workflows/release.yml
```

`ci.yml` prüft jede Änderung auf `main` und in Pull Requests. `release.yml` erstellt ausschließlich für Versionstags einen GitHub Draft Release.

## CI

Der CI-Workflow läuft bei jedem Push auf `main` und bei Pull Requests gegen `main`.

Ablauf:

1. Repository auschecken
2. .NET 10 SDK installieren
3. `dotnet restore TreeSync.sln`
4. `dotnet build TreeSync.sln --configuration Release`
5. `dotnet test TreeSync.sln --configuration Release`
6. Windows-x64-Artefakt veröffentlichen
7. Smoke-Test mit `TreeSync.exe --help`

Die CI erstellt keinen GitHub Release.

Der Publish-Schritt in der CI dient nur dem Smoke-Test. Dadurch wird geprüft, ob das spätere Release-Artefakt grundsätzlich gebaut und gestartet werden kann.

## Release

Der Release-Workflow läuft nur bei Tags:

```text
v1.0.0
v1.1.0
v2.0.0
```

Ablauf:

1. Tag wird gepusht
2. GitHub Actions startet den Release-Workflow
3. TreeSync wird für `win-x64` veröffentlicht
4. Das Artefakt wird self-contained und als Single-File-EXE gebaut
5. Die veröffentlichte EXE wird mit `--help` gestartet
6. EXE und README werden in ein ZIP gepackt
7. GitHub Release wird als Draft erstellt
8. ZIP wird an den Draft Release angehängt
9. Release Notes werden manuell anhand von `CHANGELOG.md` geprüft und veröffentlicht

## Artefakt

Für Tag `v1.2.3` entsteht:

```text
TreeSync-1.2.3-win-x64.zip
```

Das ZIP enthält:

- `TreeSync.exe`
- `README.md`

Die EXE ist self-contained und benötigt auf dem Zielsystem keine installierte .NET Runtime.

## Versionierung

Die Release-Version wird ausschließlich über den Git-Tag definiert:

```text
vMAJOR.MINOR.PATCH
```

Die führende `v` wird für Dateinamen und `InformationalVersion` entfernt. `AssemblyVersion` und `FileVersion` bleiben stabil, damit die Assembly-Kompatibilität nicht an Release-Tags gekoppelt ist.

Beispiel:

```text
Tag:                  v1.2.3
InformationalVersion: 1.2.3
ZIP-Datei:            TreeSync-1.2.3-win-x64.zip
```

## Lokale Validierung

Vor dem Commit oder vor einem Release sollten die gleichen Kernschritte lokal ausführbar sein:

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

Optionaler Smoke-Test nach lokalem Publish:

```powershell
.\src\TreeSync.Cli\bin\Release\net10.0\win-x64\publish\TreeSync.exe --help
```

## Changelog und Release Notes

`CHANGELOG.md` ist die menschlich gepflegte Quelle für Release Notes. Neue Änderungen werden zunächst unter `## [Unreleased]` gesammelt und vor dem Release in einen versionierten Abschnitt verschoben.

Der Release-Workflow veröffentlicht den GitHub Release nicht direkt. Er erstellt einen Draft Release mit Artefakt, damit die Release Notes vor der Veröffentlichung anhand von `CHANGELOG.md` geprüft und bei Bedarf angepasst werden können.

Details zum Changelog-Prozess stehen in [`docs/versioning.md`](versioning.md).

## Release-Erstellung

Eine detaillierte Schritt-für-Schritt-Anleitung zum Erstellen eines Releases findest du in [`docs/create-release.md`](create-release.md).

Kurzfassung: Ein Release wird lokal vorbereitet, indem der gewünschte Commit auf `main` getaggt wird:

```powershell
git tag v1.2.3
git push origin v1.2.3
```

Der Push des Tags startet den Release-Workflow. Der Workflow erzeugt das ZIP-Artefakt und hängt es an den automatisch erstellten Draft Release an.

Da der Remote-Zugriff per SSH-Key passphrase-geschützt ist, müssen `git push`, `git pull` und `git fetch` in einer lokal freigegebenen Shell ausgeführt werden.

## Designentscheidungen

- Kein NuGet-Paket
- Distribution als CLI/EXE
- Windows-x64-Release-Artefakt für die erste Distribution
- Self-contained Single-File-Binary für einfache Nutzung
- Tests plus Smoke-Test gegen die veröffentlichte EXE
- Draft Release statt automatischer Veröffentlichung

## Reproduzierbarkeit

Releases werden bestimmt durch:

- Git-Tag
- .NET 10 SDK (`10.0.x`)
- GitHub Actions Runner `windows-latest`
- Runtime Identifier `win-x64`
