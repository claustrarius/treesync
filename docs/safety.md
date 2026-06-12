# docs/safety.md

## Sicherheitsprüfungen

Vor Beginn der Synchronisation führt das Tool mehrere Sicherheitsprüfungen durch.

Wenn eine Prüfung fehlschlägt, wird die Ausführung abgebrochen.

---

## Quelle und Ziel dürfen nicht identisch sein

Das Tool darf nicht ausgeführt werden, wenn Source und Target auf dasselbe Verzeichnis zeigen.

---

## Zielverzeichnis muss existieren

Das Root des Zielverzeichnisses muss bereits existieren.

Das Tool legt das Zielroot nicht automatisch an.

---

## Ziel darf nicht Home-Verzeichnis sein

Das Home-Verzeichnis eines Benutzers darf nicht als Ziel verwendet werden.

---

## Ziel darf nicht Root-Verzeichnis sein

Das Root eines Laufwerks darf nicht als Ziel verwendet werden.

Beispiele verbotener Ziele:

Linux

```

/

```

Windows

```

C:\\

```

Diese Einschränkung verhindert versehentliche Massendeletionen durch falsche Parameter.
