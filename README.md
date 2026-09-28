# Archipelago DeathLink Tracker

Windows desktop tracker for Archipelago DeathLink events. Requires Windows and .NET 10 Desktop Runtime (or a self-contained published build).

## Overlay and statistics

Click **Overlay** to show or hide the independent, always-on-top overlay. Drag it to move it; dragging switches its position to Custom. If a large player list exceeds the monitor height, scroll over the overlay with the mouse wheel.

Overlay Settings stays open while you drag the overlay. During editing, a thick border and a faint temporary background provide a large grab area even at 0% background opacity. Both disappear when settings closes, without changing saved appearance settings. Enable **Window & Layout → Lock Overlay Position** to block mouse dragging; position and X/Y settings remain available for precise placement. The lock is saved with settings and presets.

Open **Overlay Settings** for live customization. Expand/collapse the five categories; select a setting for its description and hover for a tooltip. Changes are saved immediately. Colors use `#RRGGBB`; background opacity affects only the background, keeping text readable. Set individual colors for headings, the session total, player names, the latest player, death counts, current/best streaks, ranks and timestamps. An empty individual text color inherits **Text Color**. Flame colors, ember particles and the overlay scrollbar also have their own color settings. All colors are included in presets and JSON exports.

The latest death shows the player and optional timestamp; causes remain in the main app history. Player rows have no initial-letter badges or “Players” heading, and totals appear as plain numbers beside each name. Compact mode combines player statistics onto one line when space allows. Statistics columns are locked against manual dragging and spread across the panel as the main window resizes.

The main statistics selector and overlay statistics share a sort preference. The optional leaderboard always sorts by most deaths. Tied death totals share a competition rank (1, 1, 3).

Streaks count uninterrupted deaths by the same player. A player with no deaths has streak 0; an interrupted player returns to 1. The active player starts at 1 and increments on each consecutive death. Session bests remain even after interruption or after events leave the 100-entry history. The top Streak card shows a small flame at x2, a larger orange/red flame at x3–5, smooth flickering at x6–9, and a full flame with rising embers at x10+. Statistics rows remain plain. The overlay uses subtle heat accents beneath names instead of glowing letter outlines. Disabling animations keeps the effect still; disabling flame effects removes it. Timers stop when hidden or no animated streak is present.

## Presets and persistence

Enter a name and click **Save preset** to save a layout, or select a name and click **Load**. Saving an existing name replaces that preset. **Defaults** resets the current layout and keeps saved presets. **Export JSON** and **Import JSON** exchange the current overlay configuration; imports validate supported ranges before applying anything.

Preferences, presets, and the main window's normal size, position, and maximized state are stored in `%LOCALAPPDATA%\ArchipelagoDeathLinkTracker\settings.json`. Existing `settings.txt` connection details and overlay coordinates are migrated from the application's prior settings location. Off-screen positions are clamped to an available monitor. The main window has a 900 × 560 minimum size.

Death statistics continue to use the existing `DeathTracker_State` server key, with additional optional streak metadata. Old state without metadata reconstructs streaks from the available history; historical bests outside that history cannot be recovered. Older tracker clients can still read the shared totals/history, but writing state from an older client drops streak metadata. Connecting to a different server or room seed clears the previous session before loading the new room's shared state.

## Build and checks

```powershell
dotnet build
dotnet run --project tests/Tracker.Tests.csproj
```

The dependency-free regression runner checks streaks, legacy state/settings, presets, JSON validation, packet handling, responsive layouts, overlay alpha, visibility, and animation behavior. It renders previews into the test build output and does not connect to an Archipelago server or modify user preferences.
