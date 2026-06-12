# docs/sync-logic.md

## Synchronisationslogik

Das Tool führt eine gerichtete Synchronisation von Quelle → Ziel durch.

---

## 1. Scan der Quelle

Die Quelle wird rekursiv durchsucht.

Für jedes Element wird entschieden, ob es verarbeitet wird.

---

## 2. Verarbeitung von Ordnern

Regeln:

1. Prüfen, ob Ordner durch `.treesyncignore` ausgeschlossen ist  

2. Wenn nicht ausgeschlossen:

&#x20;  - Zielordner wird angelegt, falls er nicht existiert

---

## 3. Verarbeitung von Dateien

Schritt 1 – Ignore prüfen

Wenn eine Datei einer Regel aus `.treesyncignore` entspricht → überspringen.

---

Schritt 2 – Positivliste prüfen

Die Dateiendung muss in `include_extensions` enthalten sein.

Der Vergleich erfolgt case-insensitive.

Wenn die Endung nicht enthalten ist → Datei überspringen.

---

Schritt 3 – Vergleich mit Ziel

Wenn Datei im Ziel nicht existiert:

Aktion:

```

COPY

```

Wenn Datei existiert:

Vergleich von

- Dateigröße

- Zeitstempel

Wenn unterschiedlich:

Aktion:

```

UPDATE

```

Wenn identisch:

keine Aktion.

---

## 4. Löschphase

Nach Abschluss des Kopiervorgangs wird das Zielverzeichnis überprüft.

Alle Dateien oder Ordner, die im Ziel existieren, aber nicht mehr in der Quelle vorhanden sind, werden gelöscht.

Aktion:

```

DELETE

```

Die Löschung kann rekursiv komplette Ordner betreffen.
