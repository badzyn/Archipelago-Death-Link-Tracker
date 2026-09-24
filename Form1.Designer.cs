namespace DEATHTRACKERARCHIPELAGO
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panelTop = new Panel();
            titleLabel = new Label();
            cardDeaths = new Panel();
            lblTotalDeathsTitle = new Label();
            lblTotalDeaths = new Label();
            cardPlayers = new Panel();
            lblPlayersTitle = new Label();
            lblPlayers = new Label();
            cardLastDeath = new Panel();
            lblLastDeathTitle = new Label();
            lblLastDeath = new Label();
            cardStreak = new Panel();
            lblStreakTitle = new Label();
            lblStreak = new Label();
            panelRanking = new Panel();
            lblRankingTitle = new Label();
            lblRankingNick = new Label();
            lblRankingDeaths = new Label();
            lvRanking = new ListView();
            columnPlayer = new ColumnHeader();
            columnDeaths = new ColumnHeader();
            panelHistory = new Panel();
            lblHistoryTitle = new Label();
            lbHistory = new ListBox();
            panelBottom = new Panel();
            btnOverlay = new Button();
            lblStatus = new Label();
            txtServer = new TextBox();
            txtSlot = new TextBox();
            btnConnect = new Button();
            panelTop.SuspendLayout();
            cardDeaths.SuspendLayout();
            cardPlayers.SuspendLayout();
            cardLastDeath.SuspendLayout();
            cardStreak.SuspendLayout();
            panelRanking.SuspendLayout();
            panelHistory.SuspendLayout();
            panelBottom.SuspendLayout();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.BackColor = Color.FromArgb(30, 30, 33);
            panelTop.Controls.Add(titleLabel);
            panelTop.Controls.Add(cardDeaths);
            panelTop.Controls.Add(cardPlayers);
            panelTop.Controls.Add(cardLastDeath);
            panelTop.Controls.Add(cardStreak);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(980, 130);
            panelTop.TabIndex = 0;
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(20, 15);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(219, 32);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "💀  Death Tracker";
            // 
            // cardDeaths
            // 
            cardDeaths.BackColor = Color.FromArgb(42, 42, 46);
            cardDeaths.Controls.Add(lblTotalDeathsTitle);
            cardDeaths.Controls.Add(lblTotalDeaths);
            cardDeaths.Location = new Point(20, 55);
            cardDeaths.Name = "cardDeaths";
            cardDeaths.Size = new Size(180, 60);
            cardDeaths.TabIndex = 1;
            // 
            // lblTotalDeathsTitle
            // 
            lblTotalDeathsTitle.AutoSize = true;
            lblTotalDeathsTitle.ForeColor = Color.Gray;
            lblTotalDeathsTitle.Location = new Point(10, 8);
            lblTotalDeathsTitle.Name = "lblTotalDeathsTitle";
            lblTotalDeathsTitle.Size = new Size(87, 15);
            lblTotalDeathsTitle.TabIndex = 0;
            lblTotalDeathsTitle.Text = "TOTAL DEATHS";
            // 
            // lblTotalDeaths
            // 
            lblTotalDeaths.AutoSize = true;
            lblTotalDeaths.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTotalDeaths.ForeColor = Color.FromArgb(255, 80, 80);
            lblTotalDeaths.Location = new Point(10, 25);
            lblTotalDeaths.Name = "lblTotalDeaths";
            lblTotalDeaths.Size = new Size(28, 32);
            lblTotalDeaths.TabIndex = 1;
            lblTotalDeaths.Text = "0";
            // 
            // cardPlayers
            // 
            cardPlayers.BackColor = Color.FromArgb(42, 42, 46);
            cardPlayers.Controls.Add(lblPlayersTitle);
            cardPlayers.Controls.Add(lblPlayers);
            cardPlayers.Location = new Point(220, 55);
            cardPlayers.Name = "cardPlayers";
            cardPlayers.Size = new Size(180, 60);
            cardPlayers.TabIndex = 2;
            // 
            // lblPlayersTitle
            // 
            lblPlayersTitle.AutoSize = true;
            lblPlayersTitle.ForeColor = Color.Gray;
            lblPlayersTitle.Location = new Point(10, 8);
            lblPlayersTitle.Name = "lblPlayersTitle";
            lblPlayersTitle.Size = new Size(53, 15);
            lblPlayersTitle.TabIndex = 0;
            lblPlayersTitle.Text = "PLAYERS";
            // 
            // lblPlayers
            // 
            lblPlayers.AutoSize = true;
            lblPlayers.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblPlayers.ForeColor = Color.DeepSkyBlue;
            lblPlayers.Location = new Point(10, 25);
            lblPlayers.Name = "lblPlayers";
            lblPlayers.Size = new Size(28, 32);
            lblPlayers.TabIndex = 1;
            lblPlayers.Text = "0";
            // 
            // cardLastDeath
            // 
            cardLastDeath.BackColor = Color.FromArgb(42, 42, 46);
            cardLastDeath.Controls.Add(lblLastDeathTitle);
            cardLastDeath.Controls.Add(lblLastDeath);
            cardLastDeath.Location = new Point(420, 55);
            cardLastDeath.Name = "cardLastDeath";
            cardLastDeath.Size = new Size(250, 60);
            cardLastDeath.TabIndex = 3;
            // 
            // lblLastDeathTitle
            // 
            lblLastDeathTitle.AutoSize = true;
            lblLastDeathTitle.ForeColor = Color.Gray;
            lblLastDeathTitle.Location = new Point(10, 8);
            lblLastDeathTitle.Name = "lblLastDeathTitle";
            lblLastDeathTitle.Size = new Size(74, 15);
            lblLastDeathTitle.TabIndex = 0;
            lblLastDeathTitle.Text = "LAST DEATH";
            // 
            // lblLastDeath
            // 
            lblLastDeath.AutoSize = true;
            lblLastDeath.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblLastDeath.ForeColor = Color.White;
            lblLastDeath.Location = new Point(10, 28);
            lblLastDeath.Name = "lblLastDeath";
            lblLastDeath.Size = new Size(15, 20);
            lblLastDeath.TabIndex = 1;
            lblLastDeath.Text = "-";
            // 
            // cardStreak
            // 
            cardStreak.BackColor = Color.FromArgb(42, 42, 46);
            cardStreak.Controls.Add(lblStreakTitle);
            cardStreak.Controls.Add(lblStreak);
            cardStreak.Location = new Point(690, 55);
            cardStreak.Name = "cardStreak";
            cardStreak.Size = new Size(180, 60);
            cardStreak.TabIndex = 4;
            // 
            // lblStreakTitle
            // 
            lblStreakTitle.AutoSize = true;
            lblStreakTitle.ForeColor = Color.Gray;
            lblStreakTitle.Location = new Point(10, 8);
            lblStreakTitle.Name = "lblStreakTitle";
            lblStreakTitle.Size = new Size(39, 15);
            lblStreakTitle.TabIndex = 0;
            lblStreakTitle.Text = "Streak";
            // 
            // lblStreak
            // 
            lblStreak.AutoSize = true;
            lblStreak.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblStreak.ForeColor = Color.White;
            lblStreak.Location = new Point(10, 28);
            lblStreak.Name = "lblStreak";
            lblStreak.Size = new Size(15, 20);
            lblStreak.TabIndex = 1;
            lblStreak.Text = "-";
            // 
            // panelRanking
            // 
            panelRanking.BackColor = Color.FromArgb(30, 30, 33);
            panelRanking.Controls.Add(lblRankingTitle);
            panelRanking.Controls.Add(lblRankingNick);
            panelRanking.Controls.Add(lblRankingDeaths);
            panelRanking.Controls.Add(lvRanking);
            panelRanking.Location = new Point(20, 150);
            panelRanking.Name = "panelRanking";
            panelRanking.Size = new Size(280, 390);
            panelRanking.TabIndex = 1;
            // 
            // lblRankingTitle
            // 
            lblRankingTitle.AutoSize = true;
            lblRankingTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblRankingTitle.ForeColor = Color.White;
            lblRankingTitle.Location = new Point(12, 12);
            lblRankingTitle.Name = "lblRankingTitle";
            lblRankingTitle.Size = new Size(92, 20);
            lblRankingTitle.TabIndex = 0;
            lblRankingTitle.Text = "🏆 Ranking";
            // 
            // lblRankingNick
            // 
            lblRankingNick.AutoSize = true;
            lblRankingNick.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblRankingNick.ForeColor = Color.Gray;
            lblRankingNick.Location = new Point(14, 42);
            lblRankingNick.Name = "lblRankingNick";
            lblRankingNick.Size = new Size(32, 15);
            lblRankingNick.TabIndex = 1;
            lblRankingNick.Text = "Nick";
            // 
            // lblRankingDeaths
            // 
            lblRankingDeaths.AutoSize = true;
            lblRankingDeaths.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblRankingDeaths.ForeColor = Color.Gray;
            lblRankingDeaths.Location = new Point(207, 42);
            lblRankingDeaths.Name = "lblRankingDeaths";
            lblRankingDeaths.Size = new Size(46, 15);
            lblRankingDeaths.TabIndex = 2;
            lblRankingDeaths.Text = "Deaths";
            // 
            // lvRanking
            // 
            lvRanking.BackColor = Color.FromArgb(22, 22, 24);
            lvRanking.BorderStyle = BorderStyle.None;
            lvRanking.Columns.AddRange(new ColumnHeader[] { columnPlayer, columnDeaths });
            lvRanking.ForeColor = Color.White;
            lvRanking.FullRowSelect = true;
            lvRanking.HeaderStyle = ColumnHeaderStyle.None;
            lvRanking.HideSelection = true;
            lvRanking.Location = new Point(12, 62);
            lvRanking.MultiSelect = false;
            lvRanking.Name = "lvRanking";
            lvRanking.Size = new Size(255, 310);
            lvRanking.TabIndex = 3;
            lvRanking.UseCompatibleStateImageBehavior = false;
            lvRanking.View = View.Details;
            // 
            // columnPlayer
            // 
            columnPlayer.Text = "";
            columnPlayer.Width = 190;
            // 
            // columnDeaths
            // 
            columnDeaths.Text = "";
            columnDeaths.Width = 65;
            // 
            // panelHistory
            // 
            panelHistory.BackColor = Color.FromArgb(30, 30, 33);
            panelHistory.Controls.Add(lblHistoryTitle);
            panelHistory.Controls.Add(lbHistory);
            panelHistory.Location = new Point(320, 150);
            panelHistory.Name = "panelHistory";
            panelHistory.Size = new Size(640, 390);
            panelHistory.TabIndex = 2;
            // 
            // lblHistoryTitle
            // 
            lblHistoryTitle.AutoSize = true;
            lblHistoryTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblHistoryTitle.ForeColor = Color.White;
            lblHistoryTitle.Location = new Point(12, 12);
            lblHistoryTitle.Name = "lblHistoryTitle";
            lblHistoryTitle.Size = new Size(133, 20);
            lblHistoryTitle.TabIndex = 0;
            lblHistoryTitle.Text = "📜 Death History";
            // 
            // lbHistory
            // 
            lbHistory.BackColor = Color.FromArgb(22, 22, 24);
            lbHistory.BorderStyle = BorderStyle.None;
            lbHistory.Font = new Font("Consolas", 10F);
            lbHistory.ForeColor = Color.White;
            lbHistory.Location = new Point(12, 42);
            lbHistory.Name = "lbHistory";
            lbHistory.SelectionMode = SelectionMode.None;
            lbHistory.Size = new Size(615, 330);
            lbHistory.TabIndex = 1;
            // 
            // panelBottom
            // 
            panelBottom.BackColor = Color.FromArgb(28, 28, 30);
            panelBottom.Controls.Add(btnOverlay);
            panelBottom.Controls.Add(lblStatus);
            panelBottom.Controls.Add(txtServer);
            panelBottom.Controls.Add(txtSlot);
            panelBottom.Controls.Add(btnConnect);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 560);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(980, 60);
            panelBottom.TabIndex = 3;
            // 
            // btnOverlay
            // 
            btnOverlay.BackColor = Color.FromArgb(60, 60, 65);
            btnOverlay.FlatAppearance.BorderSize = 0;
            btnOverlay.FlatStyle = FlatStyle.Flat;
            btnOverlay.ForeColor = Color.White;
            btnOverlay.Location = new Point(300, 15);
            btnOverlay.Name = "btnOverlay";
            btnOverlay.Size = new Size(110, 28);
            btnOverlay.TabIndex = 0;
            btnOverlay.Text = "Overlay";
            btnOverlay.UseVisualStyleBackColor = false;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.OrangeRed;
            lblStatus.Location = new Point(20, 20);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(79, 15);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "Disconnected";
            // 
            // txtServer
            // 
            txtServer.BackColor = Color.FromArgb(40, 40, 42);
            txtServer.BorderStyle = BorderStyle.FixedSingle;
            txtServer.ForeColor = Color.White;
            txtServer.Location = new Point(430, 17);
            txtServer.Name = "txtServer";
            txtServer.PlaceholderText = "server:port";
            txtServer.Size = new Size(210, 23);
            txtServer.TabIndex = 2;
            // 
            // txtSlot
            // 
            txtSlot.BackColor = Color.FromArgb(40, 40, 42);
            txtSlot.BorderStyle = BorderStyle.FixedSingle;
            txtSlot.ForeColor = Color.White;
            txtSlot.Location = new Point(650, 17);
            txtSlot.Name = "txtSlot";
            txtSlot.PlaceholderText = "Slot name";
            txtSlot.Size = new Size(150, 23);
            txtSlot.TabIndex = 3;
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.FromArgb(0, 120, 215);
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.ForeColor = Color.White;
            btnConnect.Location = new Point(815, 15);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(120, 28);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(22, 22, 24);
            ClientSize = new Size(980, 620);
            Controls.Add(panelTop);
            Controls.Add(panelRanking);
            Controls.Add(panelHistory);
            Controls.Add(panelBottom);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Death Tracker";
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            cardDeaths.ResumeLayout(false);
            cardDeaths.PerformLayout();
            cardPlayers.ResumeLayout(false);
            cardPlayers.PerformLayout();
            cardLastDeath.ResumeLayout(false);
            cardLastDeath.PerformLayout();
            cardStreak.ResumeLayout(false);
            cardStreak.PerformLayout();
            panelRanking.ResumeLayout(false);
            panelRanking.PerformLayout();
            panelHistory.ResumeLayout(false);
            panelHistory.PerformLayout();
            panelBottom.ResumeLayout(false);
            panelBottom.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTop;
        private Panel panelRanking;
        private Panel panelHistory;
        private Panel panelBottom;

        private Panel cardDeaths;
        private Panel cardPlayers;
        private Panel cardLastDeath;

        private Label titleLabel;
        private Label lblStreakTitle;
        private Panel cardStreak;

        private Label lblTotalDeathsTitle;
        private Label lblPlayersTitle;
        private Label lblLastDeathTitle;

        private Label lblTotalDeaths;
        private Label lblPlayers;
        private Label lblLastDeath;

        private Label lblStreak;

        private Label lblRankingTitle;
        private Label lblRankingNick;
        private Label lblRankingDeaths;

        private Label lblHistoryTitle;
        private Label lblStatus;

        private TextBox txtServer;
        private TextBox txtSlot;
        private Button btnConnect;

        private ListView lvRanking;
        private ColumnHeader columnPlayer;
        private ColumnHeader columnDeaths;

        private ListBox lbHistory;
        private Button btnOverlay;
    }
}