using System.Text.Json;

namespace DEATHTRACKERARCHIPELAGO;

public sealed class OverlaySettingsForm : Form
{
    private readonly TrackerSettings settings;
    private readonly Action changed;
    private readonly PropertyGrid grid = new() { Dock = DockStyle.Fill, HelpVisible = true, ToolbarVisible = false };
    private readonly ComboBox presets = new() { Width = 190, DropDownStyle = ComboBoxStyle.DropDown };
    private readonly ToolTip tips = new();
    private OverlaySettings valid;

    public OverlaySettingsForm(TrackerSettings settings, Action changed)
    {
        this.settings = settings;
        this.changed = changed;
        valid = settings.Overlay.Copy();
        Text = "Overlay Settings — changes apply live";
        WindowTheme.Attach(this);
        Size = new Size(600, 740);
        MinimumSize = new Size(480, 480);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.FromArgb(30, 30, 33);
        ForeColor = Color.White;
        grid.ViewBackColor = BackColor;
        grid.ViewForeColor = ForeColor;
        grid.HelpBackColor = BackColor;
        grid.HelpForeColor = ForeColor;
        grid.SelectedObject = settings.Overlay;
        grid.PropertySort = PropertySort.Categorized;
        grid.PropertyValueChanged += (_, _) =>
        {
            try { settings.Overlay.Validate(); valid = settings.Overlay.Copy(); changed(); }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Invalid setting"); Apply(valid.Copy()); }
        };
        // PropertyGrid provides collapsible categories and descriptions; expose those as hover tips too.
        grid.SelectedGridItemChanged += (_, e) => SetSettingTooltip(grid,
            e.NewSelection?.PropertyDescriptor?.Description ?? "Select a setting to see its description.");
        var bar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 100, Padding = new Padding(8), AutoScroll = true };
        bar.Controls.Add(presets);
        tips.SetToolTip(presets, "Enter a preset name to save, or select a saved layout to load.");
        AddButton(bar, "Save preset", "Save a copy of the current layout under this name.", () =>
        {
            string name = presets.Text.Trim();
            if (name.Length == 0 || name.Length > 80) throw new ArgumentException("Enter a preset name of 1–80 characters.");
            settings.Presets[name] = settings.Overlay.Copy();
            RefreshPresets(); presets.Text = name; changed();
        });
        AddButton(bar, "Load", "Apply the selected saved layout immediately.", () =>
        {
            if (!settings.Presets.TryGetValue(presets.Text, out var preset)) throw new ArgumentException("Select a saved preset first.");
            Apply(preset.Copy());
        });
        AddButton(bar, "Defaults", "Reset the current overlay layout. Saved presets are retained.", () => Apply(new()));
        AddButton(bar, "Export JSON", "Export the current overlay configuration to a JSON file.", () =>
        {
            using var dialog = new SaveFileDialog { Filter = "JSON configuration|*.json", FileName = "overlay.json" };
            if (dialog.ShowDialog(this) == DialogResult.OK)
                File.WriteAllText(dialog.FileName, JsonSerializer.Serialize(settings.Overlay, SettingsStore.JsonOptions));
        });
        AddButton(bar, "Import JSON", "Validate and apply an exported overlay configuration.", () =>
        {
            using var dialog = new OpenFileDialog { Filter = "JSON configuration|*.json" };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            if (new FileInfo(dialog.FileName).Length > 65536) throw new InvalidDataException("Configuration is too large.");
            var imported = JsonSerializer.Deserialize<OverlaySettings>(File.ReadAllText(dialog.FileName), SettingsStore.JsonOptions)
                ?? throw new InvalidDataException("Configuration is empty.");
            imported.Validate(); Apply(imported);
        });
        Controls.Add(grid); Controls.Add(bar);
        RefreshPresets();
    }

    private void Apply(OverlaySettings value)
    {
        value.Validate(); settings.Overlay = value; valid = value.Copy(); grid.SelectedObject = value; changed();
    }
    public void RefreshSettings()
    {
        valid = settings.Overlay.Copy();
        grid.SelectedObject = settings.Overlay;
        grid.Refresh();
    }
    private void RefreshPresets()
    {
        presets.Items.Clear(); presets.Items.AddRange(settings.Presets.Keys.Order().Cast<object>().ToArray());
    }
    private void SetSettingTooltip(Control control, string description)
    {
        tips.SetToolTip(control, description);
        foreach (Control child in control.Controls) SetSettingTooltip(child, description);
    }
    private void AddButton(Control parent, string text, string tip, Action action)
    {
        var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.Flat };
        tips.SetToolTip(button, tip);
        button.Click += (_, _) => { try { action(); } catch (Exception ex) { MessageBox.Show(this, ex.Message, "Overlay settings"); } };
        parent.Controls.Add(button);
    }
    protected override void Dispose(bool disposing) { if (disposing) tips.Dispose(); base.Dispose(disposing); }
}
