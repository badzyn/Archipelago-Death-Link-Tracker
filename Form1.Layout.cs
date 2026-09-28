namespace DEATHTRACKERARCHIPELAGO;

public partial class Form1
{
    private System.Windows.Forms.Timer? mainAnimation;
    private float mainPhase;
    private ComboBox statisticsSort = null!;
    private bool syncingSort;
    private OverlaySettingsForm? settingsWindow;
    private static readonly Color SurfaceColor = Color.FromArgb(30, 30, 33);
    private static readonly Color ControlColor = Color.FromArgb(42, 42, 46);
    private static readonly Color MutedTextColor = Color.FromArgb(182, 184, 193);

    private void ConfigureLayout()
    {
        SuspendLayout();
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        WindowTheme.Attach(this);
        MinimumSize = new Size(900, 560);
        Controls.Clear();
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 4 };
        root.ColumnStyles.Add(new(SizeType.Percent, 100));
        root.RowStyles.Add(new(SizeType.Absolute, 45));
        root.RowStyles.Add(new(SizeType.Absolute, 94));
        root.RowStyles.Add(new(SizeType.Percent, 100));
        root.RowStyles.Add(new(SizeType.Absolute, 80));
        titleLabel.Dock = DockStyle.Fill;
        root.Controls.Add(titleLabel, 0, 0);
        var cards = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1 };
        foreach (var (card, heading, value) in new[]
        {
            (cardDeaths, lblTotalDeathsTitle, lblTotalDeaths), (cardPlayers, lblPlayersTitle, lblPlayers),
            (cardLastDeath, lblLastDeathTitle, lblLastDeath), (cardStreak, lblStreakTitle, lblStreak)
        })
        {
            cards.ColumnStyles.Add(new(SizeType.Percent, 25));
            card.Dock = DockStyle.Fill; card.Margin = new Padding(4);
            cards.Controls.Add(card);
            var content = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12, 8, 12, 8), ColumnCount = 1, RowCount = 2 };
            content.ColumnStyles.Add(new(SizeType.Percent, 100));
            content.RowStyles.Add(new(SizeType.AutoSize));
            content.RowStyles.Add(new(SizeType.Percent, 100));
            heading.AutoSize = true; heading.Dock = DockStyle.Fill; heading.Margin = new Padding(0, 0, 0, 4);
            heading.ForeColor = MutedTextColor;
            value.AutoSize = false; value.AutoEllipsis = true; value.Dock = DockStyle.Fill;
            value.TextAlign = ContentAlignment.MiddleLeft; value.Margin = Padding.Empty;
            content.Controls.Add(heading, 0, 0); content.Controls.Add(value, 0, 1);
            card.Controls.Add(content);
        }
        root.Controls.Add(cards, 0, 1);
        var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        body.ColumnStyles.Add(new(SizeType.Percent, 55)); body.ColumnStyles.Add(new(SizeType.Percent, 45));
        var stats = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        stats.ColumnStyles.Add(new(SizeType.Percent, 100));
        stats.RowStyles.Add(new(SizeType.Absolute, 38)); stats.RowStyles.Add(new(SizeType.Percent, 100));
        var statsHeader = new FlowLayoutPanel { Dock = DockStyle.Fill };
        lblRankingTitle.Text = "Session statistics"; lblRankingTitle.Margin = new Padding(0, 5, 12, 0);
        statisticsSort = new DarkComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList, Width = 155, FlatStyle = FlatStyle.Flat,
            BackColor = ControlColor, ForeColor = ForeColor, DrawMode = DrawMode.OwnerDrawFixed, ItemHeight = 24
        };
        statisticsSort.DrawItem += (_, e) =>
        {
            bool selected = (e.State & DrawItemState.Selected) != 0;
            using var background = new SolidBrush(selected ? Color.FromArgb(58, 64, 76) : ControlColor);
            e.Graphics.FillRectangle(background, e.Bounds);
            if (e.Index >= 0)
            {
                var bounds = e.Bounds; bounds.Inflate(-8, 0);
                TextRenderer.DrawText(e.Graphics, statisticsSort.GetItemText(statisticsSort.Items[e.Index]), statisticsSort.Font,
                    bounds, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
            e.DrawFocusRectangle();
        };
        statisticsSort.FormattingEnabled = true;
        statisticsSort.Format += (_, e) => e.Value = e.ListItem switch
        {
            StatisticsSort.MostDeaths => "Most deaths", StatisticsSort.LeastDeaths => "Least deaths",
            StatisticsSort.HighestStreak => "Highest streak", _ => "Alphabetical"
        };
        statisticsSort.DataSource = Enum.GetValues<StatisticsSort>(); statisticsSort.SelectedItem = preferences.Overlay.Sort;
        statisticsSort.SelectedValueChanged += (_, _) =>
        {
            if (syncingSort) return;
            preferences.Overlay.Sort = (StatisticsSort)statisticsSort.SelectedItem!;
            UpdateUI(); SaveSettings();
        };
        statsHeader.Controls.Add(lblRankingTitle); statsHeader.Controls.Add(statisticsSort);
        lvRanking.Dock = DockStyle.Fill; lvRanking.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        lvRanking.AllowColumnReorder = false;
        lvRanking.ColumnReordered += (_, e) => e.Cancel = true;
        lvRanking.ColumnWidthChanging += (_, e) =>
        {
            e.Cancel = true;
            e.NewWidth = lvRanking.Columns[e.ColumnIndex].Width;
        };
        lvRanking.Columns.Clear();
        lvRanking.Columns.Add("Player / streak", 180); lvRanking.Columns.Add("Deaths", 60);
        lvRanking.Columns.Add("Current", 65); lvRanking.Columns.Add("Best", 55); lvRanking.Columns.Add("Rank", 50);
        lvRanking.BackColor = SurfaceColor;
        lvRanking.Font = new Font("Segoe UI", 10F);
        components ??= new System.ComponentModel.Container();
        var rowHeight = new ImageList(components) { ImageSize = new Size(1, 32) };
        lvRanking.SmallImageList = rowHeight;
        lvRanking.Resize += (_, _) =>
        {
            // The native list computes scrollbars after Resize. Fit columns once that work finishes.
            if (lvRanking.IsHandleCreated)
                lvRanking.BeginInvoke(() =>
                {
                    FitStatisticsColumns();
                });
        };
        lvRanking.OwnerDraw = true;
        lvRanking.DrawColumnHeader += (_, e) =>
        {
            using var background = new SolidBrush(ControlColor);
            e.Graphics.FillRectangle(background, e.Bounds);
            var bounds = e.Bounds; bounds.Inflate(-8, 0);
            TextRenderer.DrawText(e.Graphics, e.Header?.Text, lvRanking.Font, bounds, MutedTextColor,
                TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis |
                (e.ColumnIndex == 0 ? TextFormatFlags.Left : TextFormatFlags.HorizontalCenter));
        };
        lvRanking.DrawSubItem += DrawStatisticsCell;
        stats.Controls.Add(statsHeader, 0, 0); stats.Controls.Add(lvRanking, 0, 1);
        body.Controls.Add(stats, 0, 0);
        var events = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        events.ColumnStyles.Add(new(SizeType.Percent, 100));
        events.RowStyles.Add(new(SizeType.Absolute, 38)); events.RowStyles.Add(new(SizeType.Percent, 100));
        lblHistoryTitle.Dock = DockStyle.Fill; lbHistory.Dock = DockStyle.Fill;
        lbHistory.BackColor = SurfaceColor; lbHistory.ForeColor = Color.FromArgb(225, 226, 232);
        lbHistory.Font = new Font("Segoe UI", 10F);
        lbHistory.ReadOnly = true; lbHistory.WordWrap = true; lbHistory.ScrollBars = RichTextBoxScrollBars.Vertical;
        events.Controls.Add(lblHistoryTitle, 0, 0); events.Controls.Add(lbHistory, 0, 1);
        body.Controls.Add(events, 1, 0); root.Controls.Add(body, 0, 2);
        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(0, 8, 0, 0), ColumnCount = 1, RowCount = 2 };
        footer.ColumnStyles.Add(new(SizeType.Percent, 100));
        footer.RowStyles.Add(new(SizeType.Percent, 100)); footer.RowStyles.Add(new(SizeType.Absolute, 38));
        lblStatus.AutoSize = false; lblStatus.Dock = DockStyle.Fill; lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        var settingsButton = new Button { Text = "Overlay Settings", Width = 140, Height = 30, FlatStyle = FlatStyle.Flat };
        settingsButton.Click += (_, _) => OpenOverlaySettings();
        var actions = new TableLayoutPanel { Dock = DockStyle.Fill, Margin = Padding.Empty, ColumnCount = 5, RowCount = 1 };
        actions.RowStyles.Add(new(SizeType.Percent, 100));
        actions.ColumnStyles.Add(new(SizeType.Absolute, 120)); actions.ColumnStyles.Add(new(SizeType.Absolute, 150));
        actions.ColumnStyles.Add(new(SizeType.Percent, 60)); actions.ColumnStyles.Add(new(SizeType.Percent, 40));
        actions.ColumnStyles.Add(new(SizeType.Absolute, 120));
        foreach (var button in new[] { btnOverlay, settingsButton, btnConnect })
        {
            button.Dock = DockStyle.Fill; button.Margin = new Padding(0, 0, 8, 0);
            button.FlatStyle = FlatStyle.Flat; button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.FromArgb(65, 67, 75);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, 59, 69);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(66, 73, 87);
            button.BackColor = ControlColor; button.ForeColor = ForeColor;
            button.UseVisualStyleBackColor = false; button.Font = new Font("Segoe UI", 10F);
        }
        btnConnect.Margin = Padding.Empty;
        actions.Controls.Add(btnOverlay, 0, 0); actions.Controls.Add(settingsButton, 1, 0);
        actions.Controls.Add(CreateInputFrame(txtServer), 2, 0); actions.Controls.Add(CreateInputFrame(txtSlot), 3, 0);
        actions.Controls.Add(btnConnect, 4, 0);
        footer.Controls.Add(lblStatus, 0, 0); footer.Controls.Add(actions, 0, 1);
        root.Controls.Add(footer, 0, 3); Controls.Add(root);
        // Dispose the now-empty designer containers after reparenting their children.
        panelTop.Dispose(); panelRanking.Dispose(); panelHistory.Dispose(); panelBottom.Dispose();
        components ??= new System.ComponentModel.Container();
        mainAnimation = new System.Windows.Forms.Timer(components) { Interval = 50 };
        mainAnimation.Tick += (_, _) => { mainPhase += .18f; lblStreak.UpdateEffect(streaks.ActiveCount, mainPhase, preferences.Overlay); };
        VisibleChanged += (_, _) => UpdateMainAnimation();
        Resize += (_, _) => UpdateMainAnimation();
        ResumeLayout(true);
        UpdateUI();
    }

    private void FitStatisticsColumns()
    {
        if (lvRanking.IsDisposed || lvRanking.Columns.Count != 5) return;
        int available = lvRanking.ClientSize.Width;
        int[] minimum = { 120, 60, 65, 55, 50 };
        int extra = Math.Max(0, available - minimum.Sum());
        int assigned = 0;
        for (int i = 0; i < minimum.Length; i++)
        {
            int width = i == minimum.Length - 1 ? Math.Max(minimum[i], available - assigned)
                : minimum[i] + (i == 0 ? extra * 6 / 10 : extra / 10);
            lvRanking.Columns[i].Width = width;
            assigned += width;
        }
    }

    private void OpenOverlaySettings()
    {
        if (settingsWindow != null && !settingsWindow.IsDisposed) { settingsWindow.Activate(); return; }
        settingsWindow = new OverlaySettingsForm(preferences, () =>
        {
            syncingSort = true; statisticsSort.SelectedItem = preferences.Overlay.Sort; syncingSort = false;
            if (overlay != null && !overlay.IsDisposed) overlay.ApplySettings(preferences.Overlay);
            UpdateUI(); SaveSettings();
        });
        settingsWindow.FormClosed += (_, _) =>
        {
            settingsWindow = null;
            if (overlay != null && !overlay.IsDisposed) overlay.SetEditing(false);
        };
        if (overlay != null && !overlay.IsDisposed) overlay.SetEditing(true);
        settingsWindow.Show(this);
    }

    private static Control CreateInputFrame(TextBox input)
    {
        var frame = new Panel { Dock = DockStyle.Fill, BackColor = ControlColor, Margin = new Padding(0, 0, 8, 0) };
        input.BorderStyle = BorderStyle.None; input.BackColor = ControlColor; input.ForeColor = Color.White;
        input.Font = new Font("Segoe UI", 10F);
        frame.Controls.Add(input);
        frame.Layout += (_, _) => input.SetBounds(10, Math.Max(0, (frame.Height - input.PreferredHeight) / 2), Math.Max(1, frame.Width - 20), input.PreferredHeight);
        frame.Paint += (_, e) =>
        {
            using var border = new Pen(Color.FromArgb(65, 67, 75));
            e.Graphics.DrawRectangle(border, 0, 0, frame.Width - 1, frame.Height - 1);
        };
        return frame;
    }

    private void RestoreWindow()
    {
        var saved = new Rectangle(preferences.WindowX, preferences.WindowY, preferences.WindowWidth, preferences.WindowHeight);
        var area = Screen.FromRectangle(saved).WorkingArea;
        Size = new Size(Math.Clamp(preferences.WindowWidth, MinimumSize.Width, Math.Max(MinimumSize.Width, area.Width)),
            Math.Clamp(preferences.WindowHeight, MinimumSize.Height, Math.Max(MinimumSize.Height, area.Height)));
        if (preferences.HasWindowPosition)
        {
            StartPosition = FormStartPosition.Manual;
            Location = new Point(Math.Clamp(saved.X, area.Left, Math.Max(area.Left, area.Right - Width)),
                Math.Clamp(saved.Y, area.Top, Math.Max(area.Top, area.Bottom - Height)));
        }
        if (preferences.Maximized) WindowState = FormWindowState.Maximized;
    }

    private void UpdateMainAnimation()
    {
        lblStreak.UpdateEffect(streaks.ActiveCount, mainPhase, preferences.Overlay);
        if (mainAnimation != null) mainAnimation.Enabled = Visible && WindowState != FormWindowState.Minimized &&
            preferences.Overlay.Animations && preferences.Overlay.FlameEffects && streaks.ActiveCount >= 6;
    }

    private void DrawStatisticsCell(object? sender, DrawListViewSubItemEventArgs e)
    {
        if (e.Item?.Tag is not PlayerStatistics player) return;
        using var background = new SolidBrush(e.ItemIndex % 2 == 0 ? SurfaceColor : Color.FromArgb(35, 35, 39));
        e.Graphics.FillRectangle(background, e.Bounds);
        var bounds = e.Bounds; bounds.Inflate(-8, 0);
        var state = e.Graphics.Save();
        e.Graphics.SetClip(e.Bounds);
        TextRenderer.DrawText(e.Graphics, e.SubItem!.Text, lvRanking.Font, bounds, Color.FromArgb(232, 233, 239),
            TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis |
            (e.ColumnIndex == 0 ? TextFormatFlags.Left : TextFormatFlags.HorizontalCenter));
        e.Graphics.Restore(state);
    }
}
