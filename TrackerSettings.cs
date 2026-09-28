using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DEATHTRACKERARCHIPELAGO;

public enum OverlayPosition { TopLeft, TopRight, BottomLeft, BottomRight, Custom }

public sealed class OverlaySettings
{
    [Category("Overlay Content"), DisplayName("Show Latest Death"), Description("Display the latest player. The death cause is only shown in the main app history.")]
    public bool ShowLatestDeath { get; set; } = true;
    [Category("Overlay Content"), DisplayName("Show Player Deaths"), Description("Display total deaths beside each player.")]
    public bool ShowPlayerDeaths { get; set; } = true;
    [Category("Overlay Content"), DisplayName("Show Streaks"), Description("Display the current streak next to each nickname.")]
    public bool ShowStreaks { get; set; } = true;
    [Category("Overlay Content"), DisplayName("Show Leaderboard"), Description("Display a separate leaderboard ordered by total deaths.")]
    public bool ShowLeaderboard { get; set; } = false;
    // Retained only to read older presets. Initial-letter badges are no longer rendered.
    [Browsable(false)]
    public bool ShowIcons { get; set; } = false;
    [Category("Overlay Content"), DisplayName("Show Timestamps"), Description("Include the local time of the latest death.")]
    public bool ShowTimestamps { get; set; } = true;
    [Category("Death Streak Effects"), DisplayName("Flame Effects"), Description("Add increasingly intense orange and red nickname glows at x2, x3, x6 and x10.")]
    public bool FlameEffects { get; set; } = true;
    [Category("Death Streak Effects"), DisplayName("Animations"), Description("Animate streaks of x6 or more. Disable to use static glows and reduce motion.")]
    public bool Animations { get; set; } = true;
    [Category("Overlay Appearance"), DisplayName("Font Size"), Description("Text size in points (8–48).")]
    public float FontSize { get; set; } = 14;
    [Category("Overlay Appearance"), DisplayName("Font Family"), Description("Installed font family name. Unavailable fonts fall back to Segoe UI.")]
    public string FontFamily { get; set; } = "Segoe UI";
    [Category("Overlay Appearance"), DisplayName("Text Color"), Description("Text color as #RRGGBB.")]
    public string TextColor { get; set; } = "#FFFFFF";
    [Category("Overlay Appearance"), DisplayName("Heading Color"), Description("Section heading color as #RRGGBB. Leave empty to use Text Color.")]
    public string HeadingColor { get; set; } = "";
    [Category("Overlay Appearance"), DisplayName("Total Color"), Description("Session total color as #RRGGBB. Leave empty to use Text Color.")]
    public string TotalColor { get; set; } = "";
    [Category("Overlay Appearance"), DisplayName("Player Color"), Description("Player nickname color as #RRGGBB. Leave empty to use Text Color.")]
    public string PlayerColor { get; set; } = "";
    [Category("Overlay Appearance"), DisplayName("Latest Player Color"), Description("Latest death nickname color as #RRGGBB. Leave empty to use Text Color.")]
    public string LatestPlayerColor { get; set; } = "";
    [Category("Overlay Appearance"), DisplayName("Death Count Color"), Description("Per-player total color as #RRGGBB. Leave empty to use Text Color.")]
    public string DeathCountColor { get; set; } = "";
    [Category("Overlay Appearance"), DisplayName("Streak Color"), Description("Current streak color as #RRGGBB. Leave empty to use Text Color.")]
    public string StreakColor { get; set; } = "";
    [Category("Overlay Appearance"), DisplayName("Best Streak Color"), Description("Session best streak color as #RRGGBB. Leave empty to use Text Color.")]
    public string BestStreakColor { get; set; } = "";
    [Category("Overlay Appearance"), DisplayName("Rank Color"), Description("Session and leaderboard rank color as #RRGGBB. Leave empty to use Text Color.")]
    public string RankColor { get; set; } = "";
    [Category("Overlay Appearance"), DisplayName("Timestamp Color"), Description("Latest death time color as #RRGGBB. Leave empty to use Text Color.")]
    public string TimestampColor { get; set; } = "";
    [Category("Overlay Appearance"), DisplayName("Scrollbar Color"), Description("Overlay scrollbar thumb color as #RRGGBB.")]
    public string ScrollbarColor { get; set; } = "#C0C0C0";
    [Category("Death Streak Effects"), DisplayName("Small Flame Color"), Description("Glow color for x2 as #RRGGBB.")]
    public string SmallFlameColor { get; set; } = "#FF8C00";
    [Category("Death Streak Effects"), DisplayName("Strong Flame Color"), Description("Glow color for x3 and above as #RRGGBB.")]
    public string StrongFlameColor { get; set; } = "#FF4500";
    [Category("Death Streak Effects"), DisplayName("Particle Color"), Description("Ember color for x10 and above as #RRGGBB.")]
    public string ParticleColor { get; set; } = "#FFD700";
    [Category("Overlay Appearance"), DisplayName("Background Color"), Description("Background color as #RRGGBB.")]
    public string BackgroundColor { get; set; } = "#1E1E21";
    [Category("Overlay Appearance"), DisplayName("Background Opacity"), Description("Background opacity from 0 (transparent) to 100. Text stays opaque.")]
    public int BackgroundOpacity { get; set; } = 80;
    [Category("Overlay Appearance"), DisplayName("Border Radius"), Description("Rounded background corner radius in pixels (0–80).")]
    public int BorderRadius { get; set; } = 16;
    [Category("Overlay Appearance"), DisplayName("Padding"), Description("Space inside the overlay edges in pixels (4–80).")]
    public int Padding { get; set; } = 16;
    [Category("Overlay Appearance"), DisplayName("Spacing"), Description("Vertical gap between lines in pixels (0–40).")]
    public int Spacing { get; set; } = 6;
    [Category("Window & Layout"), DisplayName("Width"), Description("Overlay width in pixels (240–1600). Long text wraps automatically.")]
    public int Width { get; set; } = 420;
    [Category("Window & Layout"), DisplayName("Position"), Description("Anchor to the main app's monitor or use custom desktop coordinates. Dragging selects Custom.")]
    public OverlayPosition Position { get; set; } = OverlayPosition.TopLeft;
    [Category("Window & Layout"), DisplayName("X"), Description("Custom desktop X coordinate; negative values support additional monitors.")]
    public int X { get; set; } = 20;
    [Category("Window & Layout"), DisplayName("Y"), Description("Custom desktop Y coordinate; negative values support additional monitors.")]
    public int Y { get; set; } = 20;
    [Category("Window & Layout"), DisplayName("Lock Overlay Position"), Description("Prevent mouse dragging, including while Overlay Settings is open. Position and X/Y settings can still move the overlay.")]
    public bool LockPosition { get; set; }
    [Category("Window & Layout"), DisplayName("Compact"), Description("Keep player statistics on one line when space allows.")]
    public bool Compact { get; set; } = false;
    [Category("Statistics"), DisplayName("Show Statistics"), Description("Show player statistics, session rank and highest streak in the overlay.")]
    public bool ShowStatistics { get; set; } = true;
    [Category("Statistics"), DisplayName("Sort"), Description("Sort the main statistics panel and overlay statistics. Leaderboard always uses most deaths.")]
    public StatisticsSort Sort { get; set; }

    public OverlaySettings Copy() => (OverlaySettings)MemberwiseClone();
    public void Validate()
    {
        if (!float.IsFinite(FontSize) || FontSize < 8 || FontSize > 48 || Width < 240 || Width > 1600 ||
            Padding < 4 || Padding > 80 || Spacing < 0 || Spacing > 40 || BorderRadius < 0 || BorderRadius > 80 ||
            BackgroundOpacity < 0 || BackgroundOpacity > 100 || !Enum.IsDefined(Position) || !Enum.IsDefined(Sort) ||
            string.IsNullOrWhiteSpace(FontFamily) || FontFamily.Length > 100 ||
            new[] { TextColor, BackgroundColor, ScrollbarColor, SmallFlameColor, StrongFlameColor, ParticleColor }.Any(color => !ValidColor(color)) ||
            new[] { HeadingColor, TotalColor, PlayerColor, LatestPlayerColor, DeathCountColor, StreakColor, BestStreakColor, RankColor, TimestampColor }
                .Any(color => color != "" && !ValidColor(color)))
            throw new ArgumentException("Invalid overlay settings. Check the ranges and #RRGGBB colors in the setting descriptions.");
    }
    private static bool ValidColor(string? value) => value != null && System.Text.RegularExpressions.Regex.IsMatch(value, "^#[0-9a-fA-F]{6}$");
}

public sealed class TrackerSettings
{
    public int Version { get; set; } = 1;
    public string Server { get; set; } = "";
    public string Slot { get; set; } = "";
    public OverlaySettings Overlay { get; set; } = new();
    public Dictionary<string, OverlaySettings> Presets { get; set; } = new();
    public int WindowX { get; set; }
    public int WindowY { get; set; }
    public int WindowWidth { get; set; } = 1100;
    public int WindowHeight { get; set; } = 720;
    public bool HasWindowPosition { get; set; }
    public bool Maximized { get; set; }
}

public sealed class SettingsStore
{
    public static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true, Converters = { new JsonStringEnumConverter() } };
    public string FilePath { get; }
    public SettingsStore(string? path = null) => FilePath = path ?? Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ArchipelagoDeathLinkTracker", "settings.json");

    public TrackerSettings Load(string legacyPath)
    {
        if (File.Exists(FilePath))
        {
            var settings = JsonSerializer.Deserialize<TrackerSettings>(File.ReadAllText(FilePath), JsonOptions)
                ?? throw new InvalidDataException("Settings file is empty.");
            if (settings.Version != 1 || settings.Overlay == null || settings.Presets == null)
                throw new InvalidDataException("Unsupported settings format.");
            settings.Overlay.Validate();
            foreach (var preset in settings.Presets.Values) (preset ?? throw new InvalidDataException("Invalid preset.")).Validate();
            return settings;
        }
        var migrated = new TrackerSettings();
        if (File.Exists(legacyPath))
        {
            var lines = File.ReadAllLines(legacyPath);
            if (lines.Length > 0) migrated.Server = lines[0];
            if (lines.Length > 1) migrated.Slot = lines[1];
            if (lines.Length > 3 && int.TryParse(lines[2], out int x) && int.TryParse(lines[3], out int y))
            { migrated.Overlay.Position = OverlayPosition.Custom; migrated.Overlay.X = x; migrated.Overlay.Y = y; }
        }
        return migrated;
    }
    public void Save(TrackerSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        string temporary = FilePath + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(settings, JsonOptions));
        File.Move(temporary, FilePath, true);
    }
}
