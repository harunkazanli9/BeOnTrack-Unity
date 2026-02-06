# BeOnTrack - Unity Projekt

## Schnellstart

1. **Unity Hub** oeffnen (Unity 2022.3 LTS oder neuer)
2. **Add** > **Add project from disk** > diesen Ordner waehlen
3. Neue Scene erstellen oder die Default Scene oeffnen
4. Leeres GameObject erstellen > **Add Component** > `SceneSetup`
5. **Play** druecken — fertig!

Das `SceneSetup`-Script erstellt automatisch alle Systeme:
- Pfad-System mit farbigem Gradient-Weg
- 2D Avatar mit Bounce-Animation
- Meilenstein-Flaggen mit Konfetti-Effekten
- Touch/Scroll Timeline-Kamera
- UI mit Workout-Button und Screenshot-Funktion
- Parallax-Hintergrund und schwebende Partikel

## Projektstruktur

```
Assets/
  Scripts/
    Core/          GameManager.cs, SceneSetup.cs
    Avatar/        AvatarController.cs
    Path/          PathSystem.cs
    Milestones/    MilestoneManager.cs
    Camera/        JourneyTimeline.cs
    UI/            GameUI.cs, ScreenshotManager.cs
    Data/          WorkoutData.cs, WorkoutService.cs
    VFX/           BackgroundParallax.cs, FloatingParticles.cs
```

## Features

- **Avatar-Weg**: Jedes Workout bewegt den Avatar auf einem kurvigen Pfad
- **Meilensteine**: Flaggen bei 1, 5, 10, 25, 50, 75, 100, 150, 200, 365 Workouts
- **Konfetti**: Partikeleffekte bei erreichten Meilensteinen
- **Scroll-Timeline**: Touch/Drag um durch die Journey zu scrollen
- **Pinch-to-Zoom**: Rein-/Rauszoomen
- **Screenshot**: Bilder fuer Social Media mit BeOnTrack-Overlay
- **Parallax-Hintergrund**: Mehrere Ebenen fuer Tiefeneffekt
- **Schwebende Partikel**: Bunte Lichtpunkte fuer Atmosphaere

## Bedienung

| Aktion | Mobile | Desktop |
|--------|--------|---------|
| Scrollen | Finger ziehen | Maus ziehen |
| Zoomen | Pinch | Mausrad |
| Workout hinzufuegen | + WORKOUT Button | + WORKOUT Button |
| Zum Avatar springen | Pin-Button | Pin-Button |
| Screenshot | Kamera-Button | Kamera-Button |

## Anpassungen

### Avatar aendern
In `AvatarController.cs` die `CreateDefaultAvatar()` Methode anpassen
oder ein eigenes Sprite als `avatarSprite` zuweisen.

### Farben aendern
In `SceneSetup.cs`:
- `pathSystem.pathColorStart` — Pfad-Startfarbe
- `pathSystem.pathColorEnd` — Pfad-Endfarbe
- Kamera-Hintergrund: `Camera.main.backgroundColor`

### Meilensteine aendern
In `MilestoneManager.cs` die `milestones`-Liste bearbeiten.

### Eigene Sprites verwenden
Sprites in `Assets/Sprites/` ablegen und per Inspector zuweisen.
