namespace DEATHTRACKERARCHIPELAGO
{
    partial class OverlayForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblDeaths;
        private Label lblLastDeath;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblDeaths = new Label();
            lblLastDeath = new Label();

            SuspendLayout();

            BackColor = Color.Magenta;
            ClientSize = new Size(180, 90);
            TransparencyKey = Color.Magenta;

            lblDeaths.AutoSize = true;
            lblDeaths.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblDeaths.ForeColor = Color.FromArgb(255, 80, 80);
            lblDeaths.BackColor = Color.Transparent;
            lblDeaths.Location = new Point(10, 4);
            lblDeaths.Text = "💀 0";

            lblLastDeath.AutoSize = true;
            lblLastDeath.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblLastDeath.ForeColor = Color.White;
            lblLastDeath.BackColor = Color.Transparent;
            lblLastDeath.Location = new Point(12, 48);
            lblLastDeath.Text = "Ostatnia: -";
            lblLastDeath.TabStop = false;
            lblLastDeath.Cursor = Cursors.Hand;

            Controls.Add(lblDeaths);
            Controls.Add(lblLastDeath);

            ResumeLayout(false);
            PerformLayout();
        }
    }
}