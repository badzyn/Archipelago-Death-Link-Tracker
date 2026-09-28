using System.Drawing.Drawing2D;

namespace DEATHTRACKERARCHIPELAGO;

internal static class FlameRenderer
{
    public static void DrawFlame(Graphics graphics, RectangleF bounds, int streak, float phase, bool animated, Color flame, Color ember)
    {
        if (streak < 2 || bounds.Width <= 0 || bounds.Height <= 0) return;
        var state = graphics.Save();
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        float motion = animated && streak >= 6 ? MathF.Sin(phase) : 0;
        float intensity = streak >= 10 ? 1 : streak >= 6 ? .85f : streak >= 3 ? .7f : .5f;
        float scale = streak >= 10 ? 1 : streak >= 6 ? .92f : streak >= 3 ? .8f : .65f;
        float size = Math.Min(bounds.Width, bounds.Height) * scale;
        graphics.TranslateTransform(bounds.X + (bounds.Width - size) / 2, bounds.Y + (bounds.Height - size) / 2);
        graphics.ScaleTransform(size / 40, size / 40);
        using (var haloShape = new GraphicsPath())
        {
            haloShape.AddEllipse(0, 8, 40, 32);
            using var halo = new PathGradientBrush(haloShape)
            {
                CenterColor = Color.FromArgb((int)(55 * intensity), flame), SurroundColors = new[] { Color.Transparent }
            };
            graphics.FillPath(halo, haloShape);
        }
        using var outer = new GraphicsPath();
        outer.MoveToFlame(motion);
        using var fill = new LinearGradientBrush(new PointF(20, 4), new PointF(20, 37),
            Color.FromArgb((int)(210 + 45 * intensity), ember), flame);
        graphics.FillPath(fill, outer);
        using var inner = new GraphicsPath();
        inner.AddBezier(20, 35, 10, 32, 15, 24, 20 + motion, 20);
        inner.AddBezier(20 + motion, 20, 19, 27, 27, 27, 25, 32);
        inner.AddBezier(25, 32, 24, 35, 22, 36, 20, 35);
        using var core = new LinearGradientBrush(new PointF(20, 20), new PointF(20, 36), Color.FromArgb(245, 255, 249, 215), ember);
        graphics.FillPath(core, inner);
        if (streak >= 10)
        {
            for (int i = 0; i < 3; i++)
            {
                float progress = animated ? (phase * .12f + i / 3f) % 1 : (i + 1) / 4f;
                using var spark = new SolidBrush(Color.FromArgb((int)(190 * (1 - progress)), ember));
                graphics.FillEllipse(spark, 7 + i * 12 + MathF.Sin(phase + i) * (animated ? 1.5f : 0), 14 - progress * 12, 1.6f, 2.6f);
            }
        }
        graphics.Restore(state);
    }

    private static void MoveToFlame(this GraphicsPath shape, float motion)
    {
        shape.AddBezier(20, 37, 5, 37, 5, 25, 11, 18);
        shape.AddBezier(11, 18, 10, 24, 14, 25, 15, 23);
        shape.AddBezier(15, 23, 20, 16, 15, 13, 23 + motion * 1.5f, 4);
        shape.AddBezier(23 + motion * 1.5f, 4, 21, 15, 33, 18, 32, 27);
        shape.AddBezier(32, 27, 32, 33, 27, 37, 20, 37);
        shape.CloseFigure();
    }

    // A soft heat accent beneath overlay text, never an outline around the letters.
    public static void DrawAccent(Graphics graphics, RectangleF bounds, int streak, float phase, bool animated, Color color)
    {
        if (streak < 2 || bounds.Width <= 0) return;
        float pulse = animated && streak >= 6 ? .8f + .2f * MathF.Sin(phase) : .85f;
        float height = streak >= 10 ? 10 : streak >= 6 ? 8 : streak >= 3 ? 6 : 4;
        var accent = new RectangleF(bounds.X, bounds.Bottom - height - 2, bounds.Width, height);
        using var gradient = new LinearGradientBrush(accent, Color.Transparent, Color.FromArgb((int)(65 * pulse), color), LinearGradientMode.Vertical);
        graphics.FillRectangle(gradient, accent);
        using var edge = new Pen(Color.FromArgb((int)(150 * pulse), color), 1);
        graphics.DrawLine(edge, accent.Left, accent.Bottom, accent.Right, accent.Bottom);
    }
}
