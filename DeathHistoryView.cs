using System.Runtime.InteropServices;
using System.Text;

namespace DEATHTRACKERARCHIPELAGO;

// Retain native wrapping and scrolling without treating the event log as an editor.
internal sealed class DeathHistoryView : RichTextBox
{
    public DeathHistoryView()
    {
        ReadOnly = true;
        TabStop = false;
        DetectUrls = false;
        Cursor = Cursors.Arrow;
        SetStyle(ControlStyles.Selectable, false);
    }

    public void SetEntries(IEnumerable<DeathRecord> entries)
    {
        var rtf = new StringBuilder(@"{\rtf1\ansi\uc1{\fonttbl{\f0 ");
        AppendEscaped(rtf, Font.Name);
        rtf.Append(@";}}{\colortbl ;\red").Append(ForeColor.R)
            .Append(@"\green").Append(ForeColor.G).Append(@"\blue").Append(ForeColor.B)
            .Append(@";}\f0\cf1\fs").Append((int)Math.Round(Font.SizeInPoints * 2)).Append(' ');
        bool first = true;
        foreach (var entry in entries)
        {
            if (!first) rtf.Append(@"\par\par ");
            first = false;
            rtf.Append(@"\b ");
            AppendEscaped(rtf, $"[{entry.Time:dd.MM.yyyy HH:mm:ss}] {entry.Player}");
            rtf.Append(@"\b0 ");
            AppendEscaped(rtf, $" — {entry.Cause}");
        }
        rtf.Append('}');
        Rtf = rtf.ToString();
        Select(0, 0);
    }

    private static void AppendEscaped(StringBuilder target, string text)
    {
        // Names and causes are server-provided text, never RTF markup.
        foreach (char character in text.Replace("\r\n", "\n").Replace('\r', '\n'))
        {
            if (character is '\\' or '{' or '}') target.Append('\\').Append(character);
            else if (character == '\n') target.Append(@"\line ");
            else if (character == '\t') target.Append(@"\tab ");
            else if (character > 127) target.Append(@"\u").Append(unchecked((short)character)).Append('?');
            else target.Append(character);
        }
    }

    protected override void WndProc(ref Message message)
    {
        switch (message.Msg)
        {
            case 0x0021: // WM_MOUSEACTIVATE: scrolling does not need keyboard focus.
                message.Result = (IntPtr)3; // MA_NOACTIVATE
                return;
            case 0x0020 when ((long)message.LParam & 0xffff) == 1: // WM_SETCURSOR / HTCLIENT
                Cursor.Current = Cursors.Arrow;
                message.Result = (IntPtr)1;
                return;
            case 0x0201: // Client-area button messages only; native scrollbar messages pass through.
            case 0x0202:
            case 0x0203:
            case 0x0204:
            case 0x0205:
            case 0x0206:
            case 0x0207:
            case 0x0208:
            case 0x0209:
            case 0x007B: // WM_CONTEXTMENU
                return;
        }
        base.WndProc(ref message);
        if (message.Msg == 0x0007) HideCaret(Handle); // Defensive: even programmatic focus has no caret.
    }

    [DllImport("user32.dll")]
    private static extern bool HideCaret(IntPtr window);
}
