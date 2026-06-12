# docs/ignore-rules.md

## Ignore-Datei

Dateiname:

```

.treesyncignore

```

Standardmäßig im Root der Quelle.

Die Syntax ist bewusst einfacher als `.gitignore`.

---

## Regeln

- eine Regel pro Zeile

- `#` beginnt einen Kommentar

- Regeln sind relativ zum Root der Quelle

---

## Unterstützte Muster

### Datei ignorieren

```

config/local.php

```

---

### Dateityp ignorieren

```

\*.log

\*.tmp

```

---

### Ordner ignorieren

```

cache/

node_modules/

```

Alle Inhalte dieser Ordner werden ignoriert.

---

### Ordnerstruktur

Wildcards sind erlaubt.

Beispiel:

```

\*/cache/

```

---

## Standardempfehlung

Folgende Dateien sollten typischerweise ausgeschlossen werden:

```

config.json

.treesyncignore

treesync.log

```

Damit werden Konfigurations‑ und Logdateien nicht deployt.
