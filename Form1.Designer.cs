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
            lblStatus = new Label();
            txtServer = new TextBox();
            txtSlot = new TextBox();
            btnConnect = new Button();

            SuspendLayout();

            // 
            // FORM
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(22, 22, 24);
            ClientSize = new Size(980, 620);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Death Tracker";

            // 
            // panelTop
            // 
            panelTop.BackColor = Color.FromArgb(30, 30, 33);
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 130;

            // 
            // titleLabel
            // 
            titleLabel.Text = "💀  Death Tracker";
            titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Location = new Point(20, 15);
            titleLabel.AutoSize = true;

            panelTop.Controls.Add(titleLabel);

            // 
            // cardDeaths
            // 
            cardDeaths.BackColor = Color.FromArgb(42, 42, 46);
            cardDeaths.Location = new Point(20, 55);
            cardDeaths.Size = new Size(180, 60);

            lblTotalDeathsTitle.Text = "TOTAL DEATHS";
            lblTotalDeathsTitle.ForeColor = Color.Gray;
            lblTotalDeathsTitle.Location = new Point(10, 8);
            lblTotalDeathsTitle.AutoSize = true;

            lblTotalDeaths.Text = "0";
            lblTotalDeaths.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTotalDeaths.ForeColor = Color.FromArgb(255, 80, 80);
            lblTotalDeaths.Location = new Point(10, 25);
            lblTotalDeaths.AutoSize = true;

            cardDeaths.Controls.Add(lblTotalDeathsTitle);
            cardDeaths.Controls.Add(lblTotalDeaths);

            // 
            // cardPlayers
            // 
            cardPlayers.BackColor = Color.FromArgb(42, 42, 46);
            cardPlayers.Location = new Point(220, 55);
            cardPlayers.Size = new Size(180, 60);

            lblPlayersTitle.Text = "PLAYERS";
            lblPlayersTitle.ForeColor = Color.Gray;
            lblPlayersTitle.Location = new Point(10, 8);
            lblPlayersTitle.AutoSize = true;

            lblPlayers.Text = "0";
            lblPlayers.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblPlayers.ForeColor = Color.DeepSkyBlue;
            lblPlayers.Location = new Point(10, 25);
            lblPlayers.AutoSize = true;

            cardPlayers.Controls.Add(lblPlayersTitle);
            cardPlayers.Controls.Add(lblPlayers);

            // 
            // cardLastDeath
            // 
            cardLastDeath.BackColor = Color.FromArgb(42, 42, 46);
            cardLastDeath.Location = new Point(420, 55);
            cardLastDeath.Size = new Size(250, 60);

            lblLastDeathTitle.Text = "LAST DEATH";
            lblLastDeathTitle.ForeColor = Color.Gray;
            lblLastDeathTitle.Location = new Point(10, 8);
            lblLastDeathTitle.AutoSize = true;

            lblLastDeath.Text = "-";
            lblLastDeath.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblLastDeath.ForeColor = Color.White;
            lblLastDeath.Location = new Point(10, 28);
            lblLastDeath.AutoSize = true;

            cardLastDeath.Controls.Add(lblLastDeathTitle);
            cardLastDeath.Controls.Add(lblLastDeath);

            panelTop.Controls.Add(cardDeaths);
            panelTop.Controls.Add(cardPlayers);
            panelTop.Controls.Add(cardLastDeath);

            // 
            // panelRanking
            // 
            panelRanking.BackColor = Color.FromArgb(30, 30, 33);
            panelRanking.Location = new Point(20, 150);
            panelRanking.Size = new Size(280, 390);

            // 
            // lblRankingTitle
            // 
            lblRankingTitle.Text = "🏆 Ranking";
            lblRankingTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblRankingTitle.ForeColor = Color.White;
            lblRankingTitle.Location = new Point(12, 12);
            lblRankingTitle.AutoSize = true;

            //
            // btnOverlay
            //
            btnOverlay = new Button();

            btnOverlay.Location = new Point(300, 15);
            btnOverlay.Size = new Size(110, 28);
            btnOverlay.Text = "Overlay";

            btnOverlay.BackColor = Color.FromArgb(60, 60, 65);
            btnOverlay.ForeColor = Color.White;

            btnOverlay.FlatStyle = FlatStyle.Flat;
            btnOverlay.FlatAppearance.BorderSize = 0;

            panelBottom.Controls.Add(btnOverlay);

            // 
            // lblRankingNick
            // 
            lblRankingNick.Text = "Nick";
            lblRankingNick.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblRankingNick.ForeColor = Color.Gray;
            lblRankingNick.Location = new Point(14, 42);
            lblRankingNick.AutoSize = true;

            // 
            // lblRankingDeaths
            // 
            lblRankingDeaths.Text = "Deaths";
            lblRankingDeaths.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblRankingDeaths.ForeColor = Color.Gray;
            lblRankingDeaths.Location = new Point(207, 42);
            lblRankingDeaths.AutoSize = true;

            // 
            // lvRanking
            // 
            lvRanking.Location = new Point(12, 62);
            lvRanking.Size = new Size(255, 310);

            lvRanking.BackColor = Color.FromArgb(22, 22, 24);
            lvRanking.ForeColor = Color.White;

            lvRanking.BorderStyle = BorderStyle.None;

            lvRanking.FullRowSelect = true;
            lvRanking.GridLines = false;

        
            lvRanking.HeaderStyle = ColumnHeaderStyle.None;

          
            lvRanking.MultiSelect = false;

            
            lvRanking.HideSelection = true;


            lvRanking.View = View.Details;

            columnPlayer.Text = "";
            columnPlayer.Width = 190;

            columnDeaths.Text = "";
            columnDeaths.Width = 65;

            lvRanking.Columns.AddRange(
                new ColumnHeader[]
                {
                    columnPlayer,
                    columnDeaths
                });

            panelRanking.Controls.Add(lblRankingTitle);
            panelRanking.Controls.Add(lblRankingNick);
            panelRanking.Controls.Add(lblRankingDeaths);
            panelRanking.Controls.Add(lvRanking);

            // 
            // panelHistory
            // 
            panelHistory.BackColor = Color.FromArgb(30, 30, 33);
            panelHistory.Location = new Point(320, 150);
            panelHistory.Size = new Size(640, 390);

            // 
            // lblHistoryTitle
            // 
            lblHistoryTitle.Text = "📜 Death History";
            lblHistoryTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblHistoryTitle.ForeColor = Color.White;
            lblHistoryTitle.Location = new Point(12, 12);
            lblHistoryTitle.AutoSize = true;

            // 
            // lbHistory
            // 
            lbHistory.Location = new Point(12, 42);
            lbHistory.Size = new Size(615, 330);

            lbHistory.BackColor = Color.FromArgb(22, 22, 24);
            lbHistory.ForeColor = Color.White;

            lbHistory.BorderStyle = BorderStyle.None;
            lbHistory.Font = new Font("Consolas", 10F);

            // Brak zaznaczania elementów.
            lbHistory.SelectionMode = SelectionMode.None;

            panelHistory.Controls.Add(lblHistoryTitle);
            panelHistory.Controls.Add(lbHistory);

            // 
            // panelBottom
            // 
            panelBottom.BackColor = Color.FromArgb(28, 28, 30);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Height = 60;

            // 
            // lblStatus
            // 
            lblStatus.Text = "Disconnected";
            lblStatus.ForeColor = Color.OrangeRed;
            lblStatus.Location = new Point(20, 20);
            lblStatus.AutoSize = true;

            // 
            // txtServer
            // 
            txtServer.Location = new Point(430, 17);
            txtServer.Size = new Size(210, 23);

            txtServer.BackColor = Color.FromArgb(40, 40, 42);
            txtServer.ForeColor = Color.White;

            txtServer.BorderStyle = BorderStyle.FixedSingle;
            txtServer.PlaceholderText = "server:port";

            // 
            // txtSlot
            // 
            txtSlot.Location = new Point(650, 17);
            txtSlot.Size = new Size(150, 23);

            txtSlot.BackColor = Color.FromArgb(40, 40, 42);
            txtSlot.ForeColor = Color.White;

            txtSlot.BorderStyle = BorderStyle.FixedSingle;
            txtSlot.PlaceholderText = "Slot name";

            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(815, 15);
            btnConnect.Size = new Size(120, 28);

            btnConnect.Text = "Connect";

            btnConnect.BackColor = Color.FromArgb(0, 120, 215);
            btnConnect.ForeColor = Color.White;

            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.FlatAppearance.BorderSize = 0;

            // 
            // panelBottom controls
            // 
            panelBottom.Controls.Add(lblStatus);
            panelBottom.Controls.Add(txtServer);
            panelBottom.Controls.Add(txtSlot);
            panelBottom.Controls.Add(btnConnect);

            // 
            // FORM controls
            // 
            Controls.Add(panelTop);
            Controls.Add(panelRanking);
            Controls.Add(panelHistory);
            Controls.Add(panelBottom);

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

        private Label lblTotalDeathsTitle;
        private Label lblPlayersTitle;
        private Label lblLastDeathTitle;

        private Label lblTotalDeaths;
        private Label lblPlayers;
        private Label lblLastDeath;

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