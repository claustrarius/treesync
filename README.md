# README.md

## Projekt: treesync

`treesync` ist ein CLI-Tool in C# auf Basis von .NET 10 zum synchronen Copy-Deployment einer Verzeichnisstruktur.

Das Tool kopiert Dateien aus einer Quelle in ein Zielverzeichnis, erhält die Ordnerstruktur, berücksichtigt eine Positivliste von Dateitypen und wendet Ignore‑Regeln an. Dateien oder Verzeichnisse, die im Ziel existieren, aber nicht mehr in der Quelle vorhanden sind, werden gelöscht.

Das Zielverzeichnis stellt damit stets eine synchronisierte Kopie der Quelle dar.

---

## Hauptfunktionen

- rekursives Kopieren einer Verzeichnisstruktur

- Positivliste erlaubter Dateiendungen

- Ignore‑Datei ähnlich `.gitignore` (vereinfachte Syntax)

- automatisches Update geänderter Dateien

- Entfernen nicht mehr vorhandener Dateien im Ziel

- Dry‑Run Modus

- Logging in Datei und Konsole

- Sicherheitsprüfungen vor Ausführung

---

## Technologie

- Programmiersprache: C#
- Runtime/SDK: .NET 10
- Anwendungstyp: .NET Console App

---

## Beispielaufruf

```

treesync \\

&#x20; --source ./src \\

&#x20; --target /var/www/app \\

&#x20; --config ./config.json \\

&#x20; --ignore .treesyncignore \\

&#x20; --log treesync.log \\

&#x20; --log-level info

```

---

## Dry Run

```

treesync --source ./src --target /var/www/app --dry-run

```

Zeigt alle Aktionen an, ohne Änderungen vorzunehmen.

---

## Standarddateien

Wenn nicht anders angegeben, erwartet das Tool folgende Dateien im Root der Quelle:

- `config.json`

- `.treesyncignore`

---

## Konfiguration

Siehe: `docs/configuration.md`

---

## Ignore-Regeln

Siehe: `docs/ignore-rules.md`

---

## Synchronisationslogik

Siehe: `docs/sync-logic.md`

---

## CLI-Interface

Siehe: `docs/cli.md`
