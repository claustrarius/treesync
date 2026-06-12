# docs/coding-rules.md

## Ziel dieser Regeln

Diese Coding Rules stellen sicher, dass Implementierungen des Projekts:

- konsistent bleiben

- der definierten Architektur folgen

- keine funktionalen Anforderungen verändern

- vorhersehbar wartbar bleiben

Diese Regeln gelten für menschliche Entwickler ebenso wie für AI‑Codegeneratoren.

Wenn Unklarheiten auftreten, gelten immer die Spezifikationen in:

- `docs/sync-logic.md`

- `docs/algorithm.md`

- `docs/architecture.md`

---

# Allgemeine Entwicklungsprinzipien

## Architektur nicht verändern

Die in `docs/architecture.md` definierte Modulstruktur ist verbindlich.

Neue Funktionen dürfen ergänzt werden, aber:

- bestehende Module dürfen nicht zusammengelegt werden

- Verantwortlichkeiten dürfen nicht vermischt werden

- die Sync-Logik muss in `SyncEngine.cs` bleiben

---

## Single Responsibility

Jedes Modul hat genau eine Hauptverantwortung.

Beispiele:

- `IgnoreRuleSet.cs` → nur Ignore-Regeln

- `TreeSyncConfig.cs` → nur Konfiguration

- `SyncEngine.cs` → nur Synchronisation

Module dürfen keine versteckten Seiteneffekte erzeugen.

---

## Keine versteckte Logik

Die Kernentscheidungen müssen ausschließlich in der Sync-Engine stattfinden.

Insbesondere:

- Kopieren

- Aktualisieren

- Löschen

dürfen nicht in Hilfsfunktionen versteckt sein.

---

# C#/.NET Coding Style

## .NET Version

Mindestens:

```

.NET 10 SDK

```

---

## Pfadbehandlung

Immer verwenden:

```

System.IO.Path
System.IO.DirectoryInfo
System.IO.FileInfo

```

Nicht verwenden:

```

String-Konkatenation für Dateipfade

```

---

## Dateisystemoperationen

Erlaubt:

- `File.Copy`

- `Directory.CreateDirectory`

- `File.Delete`

- `Directory.Delete(path, recursive: true)`

Alle Operationen müssen den \*\*Dry‑Run Modus respektieren\*\*.

---

## Logging

Direkte `Console.WriteLine()` Aufrufe sind nicht erlaubt.

Alle Ausgaben müssen über das zentrale Logger-Modul erfolgen.

Beispiel:

```

logger.LogInformation("COPY {Source} -> {Target}", source, target);

```

---

# Fehlerbehandlung

## Erwartete Fehler

Beispiele:

- ungültige Konfiguration

- fehlende Dateien

- ungültige Pfade

Diese sollen mit verständlichen Fehlermeldungen behandelt werden.

---

## Unerwartete Fehler

Unbehandelte Exceptions sollen:

- geloggt werden

- zu Exitcode `3` führen

---

# Implementierungsregeln

## Extensions normalisieren

Alle Extensions aus der Konfiguration müssen intern als:

```

lowercase

```

gespeichert werden.

Der Vergleich erfolgt ebenfalls in lowercase.

---

## Relative Pfade

Alle Vergleiche (Ignore, Sync, Delete) erfolgen mit:

```

Pfaden relativ zur Quelle

```

Dies verhindert plattformabhängige Fehler.

---

## Ignore-Regeln

Ignore-Regeln werden \*\*immer zuerst\*\* angewendet.

Reihenfolge:

1. Ignore prüfen

2. Extension prüfen

3. Sync-Logik ausführen

Diese Reihenfolge darf nicht verändert werden.

---

# Dry‑Run Verhalten

Wenn `DryRun == true` gilt:

- es dürfen \*\*keine Dateisystemänderungen erfolgen\*\*

- alle Aktionen müssen trotzdem geloggt werden

Betroffene Aktionen:

- COPY

- UPDATE

- DELETE

- CREATE DIRECTORY

Logprefix:

```

DRYRUN

```

---

# Logging-Regeln

Alle Aktionen müssen geloggt werden.

Standardformat:

```

TIMESTAMP ACTION SOURCE -> TARGET

```

Beispiele:

```

2026-06-12 14:03:21 COPY src/app/a.php -> target/app/a.php

2026-06-12 14:03:22 UPDATE src/js/main.js -> target/js/main.js

2026-06-12 14:03:22 DELETE target/tmp/test.log

```

---

# Performance-Regeln

Das Tool soll auch große Verzeichnisstrukturen effizient verarbeiten.

Empfohlene Strategien:

- `IEnumerable`/Streaming statt großer Listen

- einmalige Extension-Normalisierung

- keine mehrfachen Dateisystemzugriffe

Nicht erlaubt:

- komplette Dateiinhalte zum Vergleich laden

- Hashberechnung für Dateien

Vergleich erfolgt ausschließlich über:

- Dateigröße

- Zeitstempel

---

# Sicherheit

Vor jeder Synchronisation müssen die Sicherheitsprüfungen ausgeführt werden.

Sie befinden sich ausschließlich in:

```

PathSafetyValidator.cs

```

Zu prüfen sind:

- source ≠ target

- target existiert

- target ist nicht Home-Verzeichnis

- target ist nicht Root-Verzeichnis

Wenn eine Prüfung fehlschlägt:

- Fehlermeldung loggen

- Programm abbrechen

Exitcode:

```

2

```

---

# Testbarkeit

Die Architektur muss Unit‑Tests ermöglichen.

Insbesondere sollen folgende Komponenten isoliert testbar sein:

- Ignore-Regeln

- Dateivergleich

- Sync-Engine

- Safety-Checks

Dateisystemtests sollten mit temporären Verzeichnissen erfolgen.

Empfohlen:

```

xUnit

temporäre Verzeichnisse über System.IO

```

---

# Erweiterungsregeln

Zukünftige Erweiterungen müssen kompatibel bleiben.

Mögliche Erweiterungen:

- paralleles Kopieren

- zusätzliche Dateifilter

- weitere Logging-Optionen

Nicht erlaubt:

- Änderung der bestehenden CLI-Parameter

- Änderung der Sync-Logik-Reihenfolge

- Entfernung bestehender Sicherheitsprüfungen

---

# Dokumentationspflicht

Neue Features müssen dokumentiert werden in:

- `docs/`

- ggf. `README.md`

Code ohne Dokumentation gilt als unvollständig.

---

# Zusammenfassung

Die wichtigsten Prinzipien:

- Architektur beibehalten

- klare Modulverantwortung

- deterministische Sync-Logik

- Dry‑Run strikt einhalten

- Logging zentralisieren

- Sicherheitsprüfungen verpflichtend

- Erweiterungen ohne Breaking Changes
