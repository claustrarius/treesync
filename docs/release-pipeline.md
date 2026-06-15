# TreeSync Release Pipeline

Dieses Dokument beschreibt den offiziellen Release-Prozess für TreeSync.

---

# 🔄 Überblick

TreeSync verwendet eine GitHub Actions basierte CI/CD Pipeline.
Der Prozess ist vollständig tag-getrieben:

```
git tag v1.2.3
git push origin v1.2.3
```

→ triggert automatisch einen Release

---

# ⚙️ Build Pipeline (CI)

Bei jedem Push auf `main` und bei Pull Requests:

- Restore
- Build (Release)
- Tests ausführen

Kein Publish erfolgt in dieser Phase.

---

# 🚀 Release Pipeline

Die Release Pipeline wird durch Git Tags ausgelöst:

```
v1.0.0
v1.1.0
v2.0.0
```

## Ablauf

1. Git Tag wird gepusht
2. GitHub Actions startet Matrix Build
3. Builds werden erstellt für:

- win-x64
- linux-x64
- linux-arm64
- osx-arm64

4. Jede Plattform wird:

- published (`dotnet publish`)
- self-contained gebaut
- single-file executable erzeugt
- in ZIP archiviert

---

# 📦 Output Artefakte

Nach dem Release entstehen folgende Dateien:

```
TreeSync-1.0.0-win-x64.zip
TreeSync-1.0.0-linux-x64.zip
TreeSync-1.0.0-linux-arm64.zip
TreeSync-1.0.0-osx-arm64.zip
```

Jede ZIP enthält:

- TreeSync executable
- benötigte runtime files (falls vorhanden)
- Konfigurationsdateien

---

# 🧪 Runtime

TreeSync ist:

- self-contained
- benötigt keine .NET Installation
- direkt ausführbar

---

# 🏷 Versionierung

Die Version wird ausschließlich über Git Tags definiert:

```
vMAJOR.MINOR.PATCH
```

Beispiel:

```
v1.3.0
```

---

# 📤 Release Erstellung

Release erfolgt automatisch via GitHub Actions:

- GitHub Release wird erstellt
- Artefakte werden angehängt
- Automatische Release Notes werden generiert

---

# 🧠 Designentscheidungen

- Kein NuGet Paket
- Fokus auf CLI/EXE Distribution
- Cross-platform Builds
- Self-contained deployment
- Single-file binaries für einfache Nutzung

---

# 🔐 Reproduzierbarkeit

Alle Builds sind deterministisch über:

- Git Tag
- Fixed SDK Version (.NET 10)
- GitHub Actions Runner

---

# 🚀 Optional (empfohlen für später)

Für "Enterprise-grade" als nächste Schritte:

* 🔢 automatische Versionierung via GitVersion oder Nerdbank.GitVersioning
* 🧾 CHANGELOG auto generation
* 🔐 Code Signing für Windows EXE
* 📦 Installer (MSI / DEB / Homebrew tap)
* ⚡ rolling pre-releases (`v1.2.0-beta.1`)
