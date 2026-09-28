using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace DEATHTRACKERARCHIPELAGO;

public partial class OverlayForm : Form
{
    private OverlaySettings settings = new();
    private List<PlayerStatistics> players = new();
    private DeathRecord? latest;
    private int total;
    private readonly System.Windows.Forms.Timer animation;
    private readonly List<OverlayLine> lines = new();
    private bool dragging;
    private bool editing;
    private Point dragMouse, dragForm;
    private float phase;
    private int scrollOffset;
    private int contentHeight;
    public event Action<Point>? PositionChanged;
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    internal Control? ScreenReference { get; set; }
    private record OverlayText(string Text, string Color = "", int Streak = 0);
    private record OverlayRun(string Text, Color Color, int Streak, RectangleF Bounds);
    private record OverlayLine(string Text, List<OverlayRun> Runs);

    public OverlayForm()
    {
        components = new System.ComponentModel.Container();
        animation = new System.Windows.Forms.Timer(components) { Interval = 33 };
        animation.Tick += (_, _) => { phase += .13f; Render(); };
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        Cursor = Cursors.SizeAll;
        AccessibleName = "DeathLink overlay. Drag to move; mouse wheel scrolls statistics.";
        MouseDown += (_, e) => { if (e.Button == MouseButtons.Left && !settings.LockPosition) { dragging = true; Capture = true; dragMouse = Cursor.Position; dragForm = Location; } };
        MouseMove += (_, _) => { if (dragging) Location = new Point(dragForm.X + Cursor.Position.X - dragMouse.X, dragForm.Y + Cursor.Position.Y - dragMouse.Y); };
        MouseUp += (_, e) =>
        {
            if (!dragging || e.Button != MouseButtons.Left) return;
            dragging = false; Capture = false;
            settings.Position = OverlayPosition.Custom; settings.X = Left; settings.Y = Top;
            PositionChanged?.Invoke(Location);
        };
        MouseCaptureChanged += (_, _) => { if (!Capture) dragging = false; };
        MouseWheel += (_, e) => { scrollOffset = Math.Clamp(scrollOffset - Math.Sign(e.Delta) * 80, 0, Math.Max(0, contentHeight - Height)); Render(); };
        VisibleChanged += (_, _) => { UpdateAnimation(); if (Visible) RefreshLayout(); };
    }

    protected override CreateParams CreateParams
    {
        get { var cp = base.CreateParams; cp.ExStyle |= 0x00080000; return cp; } // WS_EX_LAYERED: per-pixel background alpha
    }

    public void ApplySettings(OverlaySettings value)
    {
        settings = value.Copy();
        Cursor = settings.LockPosition ? Cursors.Default : Cursors.SizeAll;
        if (settings.LockPosition) { dragging = false; Capture = false; }
        scrollOffset = 0;
        if (IsHandleCreated) RefreshLayout();
        UpdateAnimation();
    }

    public void SetEditing(bool value)
    {
        editing = value;
        Render();
    }

    public void UpdateOverlay(int deaths, DeathRecord? lastDeath, List<PlayerStatistics> statistics)
    {
        total = deaths; latest = lastDeath; players = statistics;
        if (IsHandleCreated) RefreshLayout();
        UpdateAnimation();
    }

    private void UpdateAnimation() => animation.Enabled = Visible && settings.Animations && settings.FlameEffects && settings.ShowStreaks &&
        (settings.ShowLatestDeath || settings.ShowStatistics || settings.ShowPlayerDeaths || settings.ShowLeaderboard) && players.Any(p => p.CurrentStreak >= 6);

    private Font MakeFont()
    {
        try { return new Font(settings.FontFamily, settings.FontSize, FontStyle.Bold); }
        catch (ArgumentException) { return new Font("Segoe UI", settings.FontSize, FontStyle.Bold); }
    }

    private void RefreshLayout()
    {
        using var bitmap = new Bitmap(1, 1);
        using var graphics = Graphics.FromImage(bitmap);
        using var font = MakeFont();
        var area = settings.Position == OverlayPosition.Custom
            ? Screen.FromPoint(new Point(settings.X, settings.Y)).WorkingArea
            : Screen.FromControl(ScreenReference ?? this).WorkingArea;
        int width = Math.Min(settings.Width, area.Width);
        int padding = Math.Min(settings.Padding, width / 4);
        float y = padding;
        lines.Clear();
        using var format = (StringFormat)StringFormat.GenericTypographic.Clone();
        format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
        float lineHeight = MathF.Ceiling(font.GetHeight(graphics)) + 4;
        float rightEdge = width - padding;
        void Add(params OverlayText[] spans)
        {
            var runs = new List<OverlayRun>();
            float x = padding;
            float Measure(string text) => graphics.MeasureString(text, font, PointF.Empty, format).Width;
            void Append(string text, OverlayText span)
            {
                float measured = Measure(text);
                var color = ColorTranslator.FromHtml(span.Color == "" ? settings.TextColor : span.Color);
                var bounds = new RectangleF(x, y, measured, lineHeight);
                if (runs.LastOrDefault() is { } previous && previous.Bounds.Y == y && previous.Color == color && previous.Streak == span.Streak)
                    runs[^1] = previous with { Text = previous.Text + text, Bounds = new RectangleF(previous.Bounds.X, y, bounds.Right - previous.Bounds.X, lineHeight) };
                else runs.Add(new(text, color, span.Streak, bounds));
                x += measured;
            }
            foreach (var span in spans)
            foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(span.Text, @"\n|[^\S\n]+|[^\s]+"))
            {
                string token = match.Value;
                if (token == "\n") { x = padding; y += lineHeight; continue; }
                float measured = Measure(token);
                if (x > padding && x + measured > rightEdge)
                {
                    x = padding; y += lineHeight;
                    if (string.IsNullOrWhiteSpace(token)) continue;
                }
                if (measured <= rightEdge - padding) Append(token, span);
                else
                {
                    // Even an unbroken long nickname must fit; keep Unicode text elements intact.
                    var elements = System.Globalization.StringInfo.GetTextElementEnumerator(token);
                    while (elements.MoveNext())
                    {
                        string element = elements.GetTextElement();
                        if (x > padding && x + Measure(element) > rightEdge) { x = padding; y += lineHeight; }
                        Append(element, span);
                    }
                }
            }
            lines.Add(new(string.Concat(spans.Select(span => span.Text)), runs));
            y += lineHeight + settings.Spacing;
        }
        Add(new("Deaths ", settings.HeadingColor), new(total.ToString(), settings.TotalColor));
        List<OverlayText> PlayerRow(PlayerStatistics player)
        {
            var spans = new List<OverlayText> { new(player.Name, settings.PlayerColor, player.CurrentStreak) };
            if (settings.ShowStreaks) spans.Add(new($" x{player.CurrentStreak}", settings.StreakColor));
            if (settings.ShowPlayerDeaths) spans.Add(new($" · {player.Deaths}", settings.DeathCountColor));
            return spans;
        }
        if (settings.ShowLatestDeath)
        {
            if (latest == null) Add(new OverlayText("Waiting for a death"));
            else
            {
                var player = players.FirstOrDefault(p => p.Name == latest.Player);
                var spans = new List<OverlayText> { new(latest.Player, settings.LatestPlayerColor, player?.CurrentStreak ?? 0) };
                if (settings.ShowTimestamps) spans.Add(new($"  {latest.Time:HH:mm:ss}", settings.TimestampColor));
                Add(spans.ToArray());
            }
        }
        if (settings.ShowStatistics || settings.ShowPlayerDeaths || settings.ShowStreaks)
        {
            if (settings.ShowStatistics) Add(new OverlayText("Session statistics", settings.HeadingColor));
            foreach (var player in players)
            {
                var spans = PlayerRow(player);
                if (settings.ShowStatistics)
                {
                    spans.Add(new(settings.Compact ? " · " : "\n"));
                    spans.Add(new($"Rank #{player.Rank}", settings.RankColor));
                    spans.Add(new($" · Best x{player.HighestStreak}", settings.BestStreakColor));
                }
                Add(spans.ToArray());
            }
        }
        if (settings.ShowLeaderboard)
        {
            Add(new OverlayText("Death leaderboard", settings.HeadingColor));
            foreach (var player in players.OrderByDescending(p => p.Deaths).ThenBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
            {
                var spans = PlayerRow(player);
                spans.Insert(0, new($"#{player.Rank}  ", settings.RankColor));
                Add(spans.ToArray());
            }
        }
        contentHeight = (int)Math.Ceiling(y + padding);
        Size = new Size(width, Math.Min(contentHeight, area.Height));
        scrollOffset = Math.Clamp(scrollOffset, 0, Math.Max(0, contentHeight - Height));
        int right = area.Right - Width - 20, bottom = area.Bottom - Height - 20;
        Point location = settings.Position switch
        {
            OverlayPosition.TopRight => new(right, area.Top + 20),
            OverlayPosition.BottomLeft => new(area.Left + 20, bottom),
            OverlayPosition.BottomRight => new(right, bottom),
            OverlayPosition.Custom => new(settings.X, settings.Y),
            _ => new(area.Left + 20, area.Top + 20)
        };
        var target = settings.Position == OverlayPosition.Custom ? Screen.FromPoint(location).WorkingArea : area;
        if (!dragging)
            Location = new Point(Math.Clamp(location.X, target.Left, Math.Max(target.Left, target.Right - Width)),
                Math.Clamp(location.Y, target.Top, Math.Max(target.Top, target.Bottom - Height)));
        Render();
    }

    private void Render()
    {
        if (!IsHandleCreated || !Visible || Width <= 0 || Height <= 0) return;
        using var bitmap = RenderBitmap();
        Present(bitmap);
    }

    private Bitmap RenderBitmap()
    {
        var bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
        using (var graphics = Graphics.FromImage(bitmap))
        using (var font = MakeFont())
        using (var background = new SolidBrush(Color.FromArgb(settings.BackgroundOpacity * 255 / 100, ColorTranslator.FromHtml(settings.BackgroundColor))))
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using var shape = RoundedRectangle(new RectangleF(0, 0, Width - 1, Height - 1), settings.BorderRadius);
            graphics.FillPath(background, shape);
            if (editing && !settings.LockPosition)
            {
                // Layered windows ignore alpha-zero pixels for hit testing. A temporary
                // tint makes the entire edit surface draggable without changing saved opacity.
                using var editSurface = new SolidBrush(Color.FromArgb(24, ColorTranslator.FromHtml(settings.BackgroundColor)));
                graphics.FillPath(editSurface, shape);
            }
            graphics.SetClip(shape);
            using var format = (StringFormat)StringFormat.GenericTypographic.Clone();
            format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            foreach (var line in lines)
            foreach (var run in line.Runs)
            {
                var bounds = run.Bounds; bounds.Y -= scrollOffset;
                if (bounds.Bottom < 0 || bounds.Top > Height) continue;
                int streak = settings.FlameEffects && settings.ShowStreaks ? run.Streak : 0;
                if (streak >= 2)
                {
                    FlameRenderer.DrawAccent(graphics, bounds, streak, phase, settings.Animations,
                        ColorTranslator.FromHtml(streak >= 3 ? settings.StrongFlameColor : settings.SmallFlameColor));
                    if (streak >= 10 && settings.Animations)
                    {
                        using var ember = new SolidBrush(Color.FromArgb(180, ColorTranslator.FromHtml(settings.ParticleColor)));
                        for (int i = 0; i < 6; i++)
                        {
                            float progress = (phase * .3f + i / 6f) % 1;
                            graphics.FillEllipse(ember, bounds.X + (i * 41 % Math.Max(1, (int)bounds.Width)), bounds.Y + 12 - progress * 18, 2, 3);
                        }
                    }
                }
                using var foreground = new SolidBrush(run.Color);
                graphics.DrawString(run.Text, font, foreground, bounds.Location, format);
            }
            if (contentHeight > Height)
            {
                using var track = new SolidBrush(Color.FromArgb(180, ColorTranslator.FromHtml(settings.ScrollbarColor)));
                float thumb = Math.Max(20, Height * (float)Height / contentHeight);
                graphics.FillRectangle(track, Width - 5, (Height - thumb) * scrollOffset / Math.Max(1, contentHeight - Height), 3, thumb);
            }
            if (editing)
            {
                using var border = new Pen(Color.FromArgb(settings.LockPosition ? 100 : 230, ColorTranslator.FromHtml(settings.ScrollbarColor)), 8);
                using var outline = RoundedRectangle(new RectangleF(4, 4, Width - 9, Height - 9), settings.BorderRadius);
                graphics.DrawPath(border, outline);
            }
        }
        return bitmap;
    }

    private static GraphicsPath RoundedRectangle(RectangleF rect, float radius)
    {
        var path = new GraphicsPath();
        float d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
        if (d <= 0) path.AddRectangle(rect);
        else
        {
            path.AddArc(rect.X, rect.Y, d, d, 180, 90); path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90); path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90); path.CloseFigure();
        }
        return path;
    }

    private void Present(Bitmap bitmap)
    {
        IntPtr screen = GetDC(IntPtr.Zero), memory = CreateCompatibleDC(screen);
        IntPtr image = bitmap.GetHbitmap(Color.FromArgb(0)), previous = SelectObject(memory, image);
        try
        {
            var destination = Location; var size = Size; var source = Point.Empty;
            var blend = new BlendFunction { SourceConstantAlpha = 255, AlphaFormat = 1 };
            if (!UpdateLayeredWindow(Handle, screen, ref destination, ref size, memory, ref source, 0, ref blend, 2))
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }
        finally { SelectObject(memory, previous); DeleteObject(image); DeleteDC(memory); ReleaseDC(IntPtr.Zero, screen); }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct BlendFunction { public byte BlendOp, BlendFlags, SourceConstantAlpha, AlphaFormat; }
    [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr window);
    [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr window, IntPtr dc);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleDC(IntPtr dc);
    [DllImport("gdi32.dll")] private static extern bool DeleteDC(IntPtr dc);
    [DllImport("gdi32.dll")] private static extern IntPtr SelectObject(IntPtr dc, IntPtr obj);
    [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr obj);
    [DllImport("user32.dll", SetLastError = true)] private static extern bool UpdateLayeredWindow(IntPtr window, IntPtr dc, ref Point destination, ref Size size, IntPtr sourceDc, ref Point source, int key, ref BlendFunction blend, int flags);
}
