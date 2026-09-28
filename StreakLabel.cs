namespace DEATHTRACKERARCHIPELAGO;

internal sealed class StreakLabel : Label
{
    private int streak;
    private float phase;
    private OverlaySettings settings = new();

    public StreakLabel() => DoubleBuffered = true;

    public void UpdateEffect(int currentStreak, float animationPhase, OverlaySettings preferences)
    {
        streak = currentStreak; phase = animationPhase; settings = preferences;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var textBounds = ClientRectangle;
        if (settings.FlameEffects && streak >= 2)
        {
            int iconWidth = Math.Min(38, Height);
            FlameRenderer.DrawFlame(e.Graphics, new RectangleF(0, 0, iconWidth, Height), streak, phase, settings.Animations,
                ColorTranslator.FromHtml(streak >= 3 ? settings.StrongFlameColor : settings.SmallFlameColor),
                ColorTranslator.FromHtml(settings.ParticleColor));
            textBounds.X += iconWidth + 6;
            textBounds.Width = Math.Max(0, textBounds.Width - iconWidth - 6);
        }
        TextRenderer.DrawText(e.Graphics, Text, Font, textBounds, ForeColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
    }
}
