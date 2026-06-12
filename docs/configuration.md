# docs/configuration.md

## Konfigurationsdatei

Dateiname:

```

config.json

```

Die Datei liegt standardmäßig im Root der Quelle.

---

## Beispiel

```

{

&#x20; "include_extensions": \[

&#x20;   ".php",

&#x20;   ".js",

&#x20;   ".css",

&#x20;   ".jpg",

&#x20;   ".png",

&#x20;   ".woff",

&#x20;   ".woff2",

&#x20;   ".treesyncme"

&#x20; ],

&#x20; "logging": {

&#x20;   "level": "info"

&#x20; }

}

```

---

## include_extensions

Liste erlaubter Dateiendungen.

Eigenschaften:

- Vergleich erfolgt \*\*case-insensitive\*\*

- nur Dateien mit diesen Endungen werden deployt

- `.treesyncme` kann verwendet werden, um sicherzustellen, dass ein Ordner auch ohne andere Dateien angelegt wird

Beispiele gültiger Einträge:

```

.php

.js

.css

.jpg

.png

.treesyncme

```

---

## logging.level

Standard Logging-Level des Tools.

Mögliche Werte:

- error

- info

- debug

Der Wert kann über CLI mit `--log-level` überschrieben werden.
