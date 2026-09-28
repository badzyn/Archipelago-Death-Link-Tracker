using System.Drawing.Drawing2D;

namespace DEATHTRACKERARCHIPELAGO;

internal static class FlameRenderer
{
    public static void DrawGlow(Graphics graphics, Font font, string text, RectangleF bounds, int streak, float phase, bool animated, Color? color = null, StringFormat? format = null)
    {
        float pulse = animated && streak >= 6 ? (MathF.Sin(phase) + 1) / 2 : .5f;
        int radius = streak >= 10 ? 7 : streak >= 6 ? 5 : streak >= 3 ? 3 : 2;
        using var outline = new GraphicsPath();
        outline.AddString(text.Split('\n')[0], font.FontFamily, (int)font.Style,
            font.SizeInPoints * graphics.DpiY / 72, bounds, format ?? StringFormat.GenericDefault);
        var smoothing = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        for (int spread = radius; spread >= 1; spread--)
        {
            using var halo = new Pen(Color.FromArgb((int)(12 + 12 * pulse), color ?? (streak >= 3 ? Color.OrangeRed : Color.DarkOrange)), spread * 2)
            { LineJoin = LineJoin.Round };
            graphics.DrawPath(halo, outline);
        }
        graphics.SmoothingMode = smoothing;
    }
}
