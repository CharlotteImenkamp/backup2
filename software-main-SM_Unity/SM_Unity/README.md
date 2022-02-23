# Umsetzung des Smith-Milner-Paradigmas in Augmented-Reality

Umsetzung eines etablierten Tests der Visuokonstruktion mit der HoloLens2. \
Zu Beginn werden Objektnamen und Positionen aus \Assets\Resources\DataFiles\generalSettings.json ausgelesen. 
Die Datei muss vorhanden sein. 

## Ändern einer einzulesenden Datei
Ein neuer Datensatz kann über das Programm oder manuell angelegt werden. 
Eine manuelle Änderung erfolgt über das Hinzufügen von .json Dateien in \Assets\Resources\DataFiles\userData\userX und die Registrierung der Datei in der generalSettings Datei.
Zu einem Datensatz gehört eine Nutzer-Datei sowie eine Objektpositionsdatei.

## Auslesen von erhobenen Daten
- Test in Unity: 
    - Dateien werden im persistent Path (LocalAppData/YourApp/LocalState) gespeichert
- Auslesen aus **Windows Device Portal** (WDP): 
    - in HoloLens: 
        - Device Portal akivieren (Einstellungen > Für Entwickler)
        - IP Adresse unter Einstellung > Für Entwickler > Ethernet kopieren  
    - Achtung: WDP funktioniert nur vollständig unter **Microsoft Edge** (ermöglicht Datei download)
    - Username: **FHKiel**, Passwort: **Labor2.62**
    - in WDP: File explorer > LocalAppData

## Erstellen des Builds
- File > Build Settings (in Unity)
- Platform: Universal Windows Platform
- **Achtung**: Neuer Name pro Build erleichert das Nachvollziehen einer erfolgreichen Übertragung

## Fehlerbehebung auf HoloLens
- Schließen aller alten Prozesse (weiße Kachel im Raum) vor dem erneuten Öffnen der Anwendung!
