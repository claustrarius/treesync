# docs/logging.md

## Logging

Alle Aktionen werden gleichzeitig

- auf der Konsole

- im Logfile

ausgegeben.

---

## Logformat

Beispiel:

```

2026-06-12 14:03:21 COPY src/app/a.php -> target/app/a.php

2026-06-12 14:03:22 UPDATE src/js/main.js -> target/js/main.js

2026-06-12 14:03:22 DELETE target/tmp/test.log

```

---

## Dry Run

Wenn `--dry-run` aktiviert ist, werden keine Änderungen durchgeführt.

Logbeispiel:

```

2026-06-12 14:03:21 DRYRUN COPY src/app/a.php -> target/app/a.php

```

---

## Logging-Level

### error

Nur Fehler werden geloggt.

---

### info

Standardmodus.

Folgende Aktionen werden geloggt:

- COPY

- UPDATE

- DELETE

---

### debug

Zusätzliche Informationen:

- übersprungene Dateien (nicht in Positivliste)

- durch Ignore-Regeln ausgeschlossene Dateien

- gescannte Verzeichnisse

- Entscheidungslogik
