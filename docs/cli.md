# docs/cli.md

## CLI Interface

### Nutzung als EXE

Die produktive Nutzung erfolgt über die veröffentlichte `TreeSync.exe`.

Beispiel:

```powershell
.\TreeSync.exe `
  --source "C:\pfad\zur\quelle" `
  --target "C:\pfad\zum\ziel" `
  --config "C:\pfad\zur\quelle\config.json" `
  --ignore "C:\pfad\zur\quelle\.treesyncignore" `
  --log "treesync.log" `
  --log-level info
```

Dry Run:

```powershell
.\TreeSync.exe `
  --source "C:\pfad\zur\quelle" `
  --target "C:\pfad\zum\ziel" `
  --dry-run
```

---

### Publish

Beispiel für eine Windows-x64-EXE:

```powershell
dotnet publish .\src\TreeSync.Cli\TreeSync.Cli.csproj `
  -c Release `
  -p:PublishProfile=win-x64-folder
```

Die EXE und DLL liegen danach unter:

```text
publish\TreeSync.exe
publish\TreeSync.dll
```

`dotnet build` ist für Entwicklung und Tests gedacht. Für eine verteilbare `.exe` muss `dotnet publish` verwendet werden.

---

### Grundsyntax

```

treesync --source <path> --target <path> \[options]

```

### Pflichtparameter

`--source <path>`

Pfad zum Quellverzeichnis.

`--target <path>`

Pfad zum Zielverzeichnis.

---

### Optionale Parameter

`--config <file>`

Pfad zur Konfigurationsdatei.

Standard:

```

config.json

```

im Root der Quelle.

---

`--ignore <file>`

Pfad zur Ignore-Datei.

Standard:

```

.treesyncignore

```

im Root der Quelle.

---

`--log <file>`

Pfad zur Logdatei.

Standard:

```

treesync.log

```

im aktuellen Arbeitsverzeichnis.

---

`--log-level <level>`

Überschreibt den Logging-Level aus der Konfiguration.

Mögliche Werte:

- error

- info

- debug

---

`--dry-run`

Simuliert alle Aktionen ohne Änderungen am Dateisystem.

Alle geplanten Aktionen werden trotzdem geloggt.
