using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DEATHTRACKERARCHIPELAGO
{
    public partial class OverlayForm : Form
    {
        private bool dragging;
        private Point dragStartMouse;
        private Point dragStartForm;

        private readonly string settingsFile =
            Path.Combine(
                Application.LocalUserAppDataPath,
                "settings.txt");

        public OverlayForm()
        {
            InitializeComponent();

            lblDeaths.TabStop = false;

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;

            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;

            Width = 180;
            Height = 90;

            StartPosition = FormStartPosition.Manual;

            LoadPosition();

            
            lblDeaths.TabStop = false;
            lblDeaths.Enabled = true;

           
            MouseDown += Overlay_MouseDown;
            MouseMove += Overlay_MouseMove;
            MouseUp += Overlay_MouseUp;

           
            lblDeaths.MouseDown += Overlay_MouseDown;
            lblDeaths.MouseMove += Overlay_MouseMove;
            lblDeaths.MouseUp += Overlay_MouseUp;

            lblLastDeath.MouseDown += Overlay_MouseDown;
            lblLastDeath.MouseMove += Overlay_MouseMove;
            lblLastDeath.MouseUp += Overlay_MouseUp;

            lblLastDeath.Cursor = Cursors.Hand;
            lblDeaths.Cursor = Cursors.Hand;
        }

        public void UpdateOverlay(int deaths, string lastPlayer)
        {
            lblDeaths.Text = $"💀 {deaths}";
            if (string.IsNullOrWhiteSpace(lastPlayer) ||
                lastPlayer == "N/A")
            {
                lblLastDeath.Text = "Last: N/A";
            }
            else
            {
                lblLastDeath.Text = $"Last: {lastPlayer}";
            }
        }

        private void Overlay_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            dragging = true;

            dragStartMouse = Cursor.Position;

            dragStartForm = Location;

            Cursor = Cursors.Hand;
            lblDeaths.Cursor = Cursors.Hand;
        }

        private void Overlay_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!dragging)
                return;

            Point currentMouse = Cursor.Position;

            int deltaX =
                currentMouse.X - dragStartMouse.X;

            int deltaY =
                currentMouse.Y - dragStartMouse.Y;

            Location = new Point(
                dragStartForm.X + deltaX,
                dragStartForm.Y + deltaY);
        }

        private void Overlay_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            dragging = false;

            Cursor = Cursors.Default;

            SavePosition();
        }

        private void SavePosition()
        {
            try
            {
                Directory.CreateDirectory(
                    Application.LocalUserAppDataPath);

                string server = "";
                string slot = "";

                if (File.Exists(settingsFile))
                {
                    string[] lines =
                        File.ReadAllLines(settingsFile);

                    if (lines.Length >= 1)
                        server = lines[0];

                    if (lines.Length >= 2)
                        slot = lines[1];
                }

                File.WriteAllLines(
                    settingsFile,
                    new[]
                    {
                        server,
                        slot,
                        Left.ToString(),
                        Top.ToString()
                    });
            }
            catch
            {
            }
        }

        private void LoadPosition()
        {
            try
            {
                if (!File.Exists(settingsFile))
                {
                    Location = new Point(20, 20);
                    return;
                }

                string[] lines =
                    File.ReadAllLines(settingsFile);

                if (lines.Length >= 4 &&
                    int.TryParse(lines[2], out int x) &&
                    int.TryParse(lines[3], out int y))
                {
                    Location = new Point(x, y);
                }
                else
                {
                    Location = new Point(20, 20);
                }
            }
            catch
            {
                Location = new Point(20, 20);
            }
        }

        protected override void OnFormClosing(
            FormClosingEventArgs e)
        {
            SavePosition();
            base.OnFormClosing(e);
        }
    }
}