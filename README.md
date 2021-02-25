# BwInf-39.2.2-Spiessgesellen

Lösung der Aufgabe "Spiessgesellen" aus der zweiten Runde des 39ten Bundeswettbewerb Informatik.

## Aufgabe

Auf Daisys Party gibt es Obstspieße, die die Gäste selbst zusammenstellen. Sechs Obstsorten
stehen zur Auswahl: Apfel, Banane, Brombeere, Erdbeere, Pflaume und Weintraube. Alle
Stücke einer Sorte befinden sich in einer eigenen Schüssel. Daisy liebt Überraschungen, und so
hat sie die sechs Schüsseln abgedeckt und nur mit Nummern versehen, damit die Gäste nicht
vorher wissen, welche Obstsorte sie erwartet, wenn sie es bis an die Spitze der Warteschlange
vor einer bestimmten Nummer geschafft haben.
Donald gefallen Daisys Überraschungen gar nicht. Er will nur bestimmte Obstsorten essen,
gibt dieses aber ungern zu. Deshalb beobachtet er andere Gäste und notiert sich, in welchen
Schlangen sie anstehen, bevor sie ihre Spieße zusammenstellen, und wie die Spieße am Ende
aussehen. So erfährt er allerdings nicht, aus welcher Schüssel welches Obststück gekommen
ist. Hier sind seine ersten Beobachtungen:
- Micky hat einen Spieß mit Apfel, Banane und Brombeere, dessen Obststücke aus den
Schüsseln 1, 4 und 5 stammen (aber nicht notwendigerweise in dieser Reihenfolge).
- Minnies Spieß besteht aus Banane, Pflaume undWeintraube und wurde aus den Schüsseln
3, 5 und 6 zusammengestellt.
- Gustav hat einen Spieß mit Apfel, Brombeere und Erdbeere und hat die Schüsseln 1, 2
und 4 aufgesucht.
Jetzt will Donald sich einen Obstspieß machen, und zwar mit Weintraube, Brombeere und Apfel.

Leider kann er aus den vorhandenen Informationen nicht eindeutig bestimmen, in welchen
Schüsseln diese Obstsorten zu finden sind. Daher beobachtet er zum Schluss auch noch
Daisy.
- Daisy hat einen Spieß mit Erdbeere und Pflaume und war an den Schüsseln 2 und 6.

a) Hilf Donald und sage ihm, aus welchen Schüsseln er sich bedienen soll. Skizziere, wie
du die Menge der Schüsseln bestimmt hast.
b) Nach der Party ist Daisy begeistert: Das war ein voller Erfolg! Sie ist fest entschlossen,
fulminante Obstspieß-Happenings zu veranstalten, mit zig Obstsorten und vielen Gästen.
Donald will natürlich dabei sein. Aber jetzt wird er sich erst recht anstrengen müssen, um
seinen Wunschspieß zusammenzustellen.
Schreibe ein Programm, das Donald helfen kann. Es soll die folgenden Daten einlesen:
- die Menge der verfügbaren Obstsorten;
- Informationen über einige Obstspieße: für jeden Spieß
– die Menge der Obstsorten auf dem Spieß und
– die gleich große Menge von Nummern der Schüsseln, aus denen diese Obstsorten
stammen;
- eine Menge von Wunschsorten.

Ausgeben soll das Programm die Menge der Schüsseln, in denen die Wunschsorten zu
finden sind, sofern die vorliegenden Informationen ausreichen, um diese Menge eindeutig
zu bestimmen. Ansonsten soll das Programm eine möglichst informative Meldung
ausgeben.

## Lösungsansatz

1. iterativ werden alle Spiesse in kleinstmögliche Spieße aufgeteilt. Dazu vergleicht das Programm alle Spieße paarweise 
und bildet aus den gemeinsamen Obstsorten und Schüsselnummern des Paares einen neuen, kleineren Spieß. SO kann man die 
Grundaussage der Daten, welche Sorten zu welchen Schüsseln gehören, extrahieren.
2. nicht-beobachtete Obstsorten und Schüsselnummern werden als eigener Spieß ergänzt
3. bei jedem Spieß wird überprüft, ob seine Obstsorten im Wunschspieß vorkommen.
