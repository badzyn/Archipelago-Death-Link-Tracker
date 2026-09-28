using System.Runtime.InteropServices;

namespace DEATHTRACKERARCHIPELAGO;

internal static class WindowTheme
{
    public static void Attach(Form window)
    {
        window.HandleCreated += (_, _) => Apply(window);
        window.Shown += (_, _) => Apply(window);
        if (window.IsHandleCreated) Apply(window);
    }

    private static bool Apply(Form window)
    {
        if (SystemInformation.HighContrast) return false;
        int dark = 1;
        int background = ColorTranslator.ToWin32(Color.FromArgb(22, 22, 24));
        int foreground = ColorTranslator.ToWin32(Color.FromArgb(232, 233, 239));
        // Unsupported attributes are ignored by older Windows versions. Keep the native
        // caption so resizing, snapping, system menus and accessibility still work.
        DwmSetWindowAttribute(window.Handle, 20, ref dark, sizeof(int));
        int captionResult = DwmSetWindowAttribute(window.Handle, 35, ref background, sizeof(int));
        int textResult = DwmSetWindowAttribute(window.Handle, 36, ref foreground, sizeof(int));
        return captionResult == 0 && textResult == 0;
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr window, int attribute, ref int value, int size);
}
