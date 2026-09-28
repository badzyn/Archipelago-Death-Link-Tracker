namespace DEATHTRACKERARCHIPELAGO;

// Keep native keyboard/dropdown behavior while painting the remaining light chrome.
internal sealed class DarkComboBox : ComboBox
{
    protected override void WndProc(ref Message message)
    {
        base.WndProc(ref message);
        bool printing = message.Msg is 0x0317 or 0x0318; // WM_PRINT / WM_PRINTCLIENT
        if (message.Msg != 0x000F && !printing) return; // WM_PAINT
        using var graphics = printing ? Graphics.FromHdc(message.WParam) : CreateGraphics();
        using var background = new SolidBrush(BackColor);
        using var border = new Pen(Color.FromArgb(65, 67, 75));
        using var arrow = new SolidBrush(Enabled ? ForeColor : Color.Gray);
        int buttonWidth = SystemInformation.VerticalScrollBarWidth;
        graphics.FillRectangle(background, Width - buttonWidth - 1, 1, buttonWidth, Height - 2);
        graphics.DrawRectangle(border, 0, 0, Width - 1, Height - 1);
        int x = Width - buttonWidth / 2 - 1, y = Height / 2;
        graphics.FillPolygon(arrow, new[] { new Point(x - 4, y - 2), new Point(x + 4, y - 2), new Point(x, y + 2) });
    }
}
