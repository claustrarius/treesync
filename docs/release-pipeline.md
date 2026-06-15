# GitHub Release Pipeline für TreeSync

## Ziel

TreeSync wird als Open-Source-Projekt auf GitHub veröffentlicht. Bei jedem Release sollen automatisch ausführbare Artefakte erzeugt und als GitHub Release bereitgestellt werden.

### Anforderungen

* Build mit .NET 10
* Automatische Builds für Pull Requests und Main Branch
* Tag-basierte Releases
* Veröffentlichung von ausführbaren Binärdateien
* Bereitstellung der Artefakte über GitHub Releases
* Plattformübergreifende Distribution

---

# Build-Workflow

Der Build-Workflow wird bei Pushes auf den Main Branch und bei Pull Requests ausgeführt.

## Aufgaben

* Quellcode auschecken
* .NET SDK 10 installieren
* Restore aller Abhängigkeiten
* Projekt kompilieren
* Unit-Tests ausführen

## Beispiel

```yaml
name: Build

on:
  push:
    branches:
      - main

  pull_request:

jobs:
  build:

    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 10.0.x

      - run: dotnet restore

      - run: dotnet build -c Release --no-restore

      - run: dotnet test -c Release --no-build
```

---

# Release-Workflow

Releases werden über Git-Tags erstellt.

## Release erstellen

```bash
git tag v1.0.0
git push origin v1.0.0
```

Nach dem Push des Tags startet automatisch die Release-Pipeline.

---

# Veröffentlichungsstrategie

TreeSync wird als selbstständige Anwendung ausgeliefert.

Verwendet wird:

* Self-Contained Deployment
* Single-File Publishing
* Release Build

Dadurch benötigen Anwender keine separat installierte .NET Runtime.

---

# Unterstützte Plattformen

| Plattform   | Runtime Identifier |
| ----------- | ------------------ |
| Windows x64 | win-x64            |
| Linux x64   | linux-x64          |
| Linux ARM64 | linux-arm64        |
| macOS ARM64 | osx-arm64          |

---

# Publish-Konfiguration

## Windows

```bash
dotnet publish \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true
```

## Linux

```bash
dotnet publish \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true
```

## Linux ARM64

```bash
dotnet publish \
  -c Release \
  -r linux-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true
```

## macOS ARM64

```bash
dotnet publish \
  -c Release \
  -r osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true
```

---

# Erwartete Artefakte

Für jedes Release werden folgende Dateien erzeugt:

```text
TreeSync-v1.0.0-win-x64.zip
TreeSync-v1.0.0-linux-x64.tar.gz
TreeSync-v1.0.0-linux-arm64.tar.gz
TreeSync-v1.0.0-osx-arm64.zip
```

---

# GitHub Release Workflow

```yaml
name: Release

on:
  push:
    tags:
      - 'v*'

permissions:
  contents: write

jobs:

  release:

    runs-on: ubuntu-latest

    strategy:
      matrix:
        rid:
          - win-x64
          - linux-x64
          - linux-arm64
          - osx-arm64

    steps:

      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 10.0.x

      - name: Publish
        run: |
          dotnet publish \
            -c Release \
            -r ${{ matrix.rid }} \
            --self-contained true \
            -p:PublishSingleFile=true \
            -o publish/${{ matrix.rid }}

      - name: Package
        run: |
          cd publish
          zip -r ../TreeSync-${GITHUB_REF_NAME}-${{ matrix.rid }}.zip ${{ matrix.rid }}

      - name: Release
        uses: softprops/action-gh-release@v2
        with:
          generate_release_notes: true
          files: |
            TreeSync-${GITHUB_REF_NAME}-${{ matrix.rid }}.zip
```

---

# Empfohlene Repository-Einstellungen

## Workflow Permissions

GitHub → Settings → Actions → General

```text
Workflow permissions:
Read and write permissions
```

## Branch Protection

Main Branch schützen:

* Pull Requests erforderlich
* Erfolgreicher Build erforderlich
* Direktes Pushen auf Main verhindern

---

# Ergebnis

Nach dem Push eines Release-Tags wird automatisch:

1. Das Projekt gebaut
2. Für jede Zielplattform veröffentlicht
3. Ein GitHub Release erstellt
4. Die Binärartefakte angehängt
5. Automatische Release Notes generiert

Dadurch steht jede veröffentlichte Version von TreeSync unmittelbar über GitHub Releases zum Download bereit.
