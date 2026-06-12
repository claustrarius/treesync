# docs/cli.md

## CLI Interface

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
