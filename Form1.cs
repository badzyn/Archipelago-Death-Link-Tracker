using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DEATHTRACKERARCHIPELAGO
{



    public partial class Form1 : Form
    {
        private ClientWebSocket? socket;

        private readonly CancellationTokenSource shutdown =
            new CancellationTokenSource();

        private readonly string settingsFile =
            Path.Combine(Application.LocalUserAppDataPath, "settings.txt");

        private readonly List<DeathRecord> history = new();
        private readonly Dictionary<string, int> deathCounts = new();
        private readonly HashSet<string> deathLinkPlayers = new();
        private readonly Dictionary<int, string> connectedPlayers = new();

        private string streakPlayer = "";
        private int streakCounter = 0;

        private OverlayForm? overlay;
        private bool overlayVisible = false;

        //private const string DeathTrackerKey = "DeathTracker_TotalDeaths";

        private int totalDeaths = 0;


        public Form1()
        {
            InitializeComponent();

            Version? version = Assembly
                .GetExecutingAssembly()
                .GetName()
                .Version;

            Text = $"Archipelago DeathLink Tracker - v{version?.ToString(3)}";

            btnConnect.Click += BtnConnect_Click;
            btnOverlay.Click += BtnOverlay_Click;

            lvRanking.ItemSelectionChanged += (sender, e) =>
            {
                if (e.IsSelected)
                    e.Item.Selected = false;
            };

            LoadSettings();
            Shown += Form1_Shown;
        }

        private void LoadSettings()
        {
            try
            {
                if (!File.Exists(settingsFile))
                    return;

                string[] lines = File.ReadAllLines(settingsFile);

                if (lines.Length >= 1 &&
                    !string.IsNullOrWhiteSpace(lines[0]))
                {
                    txtServer.Text = lines[0].Trim();
                }

                if (lines.Length >= 2 &&
                    !string.IsNullOrWhiteSpace(lines[1]))
                {
                    txtSlot.Text = lines[1].Trim();
                }
            }
            catch
            {
                
            }
        }

        private void SaveSettings()
        {
            try
            {
                string server = txtServer.Text.Trim();
                string slot = txtSlot.Text.Trim();

                int overlayX = 20;
                int overlayY = 20;

                if (File.Exists(settingsFile))
                {
                    string[] lines = File.ReadAllLines(settingsFile);

                    if (lines.Length >= 3)
                        int.TryParse(lines[2], out overlayX);

                    if (lines.Length >= 4)
                        int.TryParse(lines[3], out overlayY);
                }

                if (overlay != null)
                {
                    overlayX = overlay.Left;
                    overlayY = overlay.Top;
                }

                Directory.CreateDirectory(
                    Application.LocalUserAppDataPath);

                File.WriteAllLines(
                    settingsFile,
                    new[]
                    {
                server,
                slot,
                overlayX.ToString(),
                overlayY.ToString()
                    });
            }
            catch
            {
            }
        }

        private async void Form1_Shown(object? sender, EventArgs e)
        {
            Shown -= Form1_Shown;
            await CheckForUpdates();
        }

        private async Task CheckForUpdates()
        {
            GithubRelease? release =
                await Updater.CheckForUpdateAsync();

            if (release == null)
                return;

            Version version =
                Updater.ParseVersion(
                    release.TagName);

            DialogResult result = MessageBox.Show(
                $"New version available!\n\n" +
                $"Current version: {Updater.CurrentVersion}\n" +
                $"New version: {version}\n\n" +
                $"Do you want to download and install it now?",
                "Update available",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result != DialogResult.Yes)
                return;

            await DownloadAndInstallUpdate(release);
        }

        private async Task DownloadAndInstallUpdate(
    GithubRelease release)
        {
            string temporaryExe = Path.Combine(
                Path.GetTempPath(),
                $"ArchipelagoDeathLinkTracker_update_{Guid.NewGuid():N}.exe");

            try
            {
                Cursor = Cursors.WaitCursor;

                Text = "Archipelago Death Link Tracker - Updating...";

                Progress<int> progress = new Progress<int>(
                    percentage =>
                    {
                        Text =
                            $"Archipelago Death Link Tracker - " +
                            $"Downloading update... {percentage}%";
                    });

                await Updater.DownloadUpdateAsync(
                    release,
                    temporaryExe,
                    progress);

                string? currentExe =
                    Environment.ProcessPath;

                if (string.IsNullOrWhiteSpace(currentExe))
                    throw new Exception(
                        "Cannot determine the path of the current application.");

                DialogResult result = MessageBox.Show(
                    $"Update {release.TagName} has been downloaded.\n\n" +
                    "The program will now be restarted.",
                    "Update ready",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                if (result == DialogResult.OK)
                {
                    Updater.StartReplacement(
                        temporaryExe);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error duirng update\n\n" +
                    ex.Message,
                    "Update error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                try
                {
                    if (File.Exists(temporaryExe))
                        File.Delete(temporaryExe);
                }
                catch
                {
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private void BtnOverlay_Click(object? sender, EventArgs e)
        {
            if (!overlayVisible)
            {
                overlay ??= new OverlayForm();
                overlay.Show();
                overlayVisible = true;
                btnOverlay.Text = "Hide Overlay";

                string lastPlayer = history.Count > 0 ? history[0].Player : "N/A";
                overlay.UpdateOverlay(totalDeaths, lastPlayer);
            }
            else
            {
                overlay?.Hide();

                overlayVisible = false;
                btnOverlay.Text = "Overlay";
            }
        }

        private async void BtnConnect_Click(object? sender, EventArgs e)
        {
            if (socket != null &&
                socket.State == WebSocketState.Open)
            {
                await DisconnectFromArchipelago();
            }
            else
            {
                await ConnectToArchipelago();
            }
        }

        private async Task ConnectToArchipelago()
        {
            string serverAddress = txtServer.Text.Trim();
            SaveSettings();

            if (string.IsNullOrWhiteSpace(serverAddress))
            {
                MessageBox.Show("Podaj adres serwera.");
                return;
            }

            if (!serverAddress.Contains("://"))
            {
                serverAddress = "ws://" + serverAddress;
            }

            try
            {
                socket?.Dispose();
                socket = new ClientWebSocket();

                lblStatus.Text = "Connecting...";
                lblStatus.ForeColor = Color.Gold;
                btnConnect.Text = "Connecting...";

                try
                {
                    await socket.ConnectAsync(
                        new Uri(serverAddress),
                        CancellationToken.None);
                }
                catch (WebSocketException)
                {
                    socket.Dispose();

                    string secureAddress;

                    if (serverAddress.StartsWith("ws://"))
                    {
                        secureAddress =
                            "wss://" + serverAddress.Substring(5);
                    }
                    else
                    {
                        throw;
                    }

                    socket = new ClientWebSocket();

                    await socket.ConnectAsync(
                        new Uri(secureAddress),
                        CancellationToken.None);
                }

                lblStatus.Text = "Connected to server";
                btnConnect.Text = "Disconnect";

                _ = ReceiveMessages();
            }
            catch (Exception ex)
            {
                socket?.Dispose();
                socket = null;

                lblStatus.Text = "Connection failed";
                lblStatus.ForeColor = Color.OrangeRed;
                btnConnect.Text = "Connect";

                MessageBox.Show(
                    $"Nie udało się połączyć:\n\n{ex}",
                    "Death Tracker");
            }
        }

        private async Task ReceiveMessages()
        {
            if (socket == null)
                return;

            byte[] buffer = new byte[64 * 1024];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    using MemoryStream messageStream = new MemoryStream();

                    WebSocketReceiveResult result;

                    do
                    {
                        result = await socket.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            shutdown.Token);

                        if (result.MessageType ==
                            WebSocketMessageType.Close)
                        {
                            return;
                        }

                        messageStream.Write(
                            buffer,
                            0,
                            result.Count);

                    } while (!result.EndOfMessage);

                    string json = Encoding.UTF8.GetString(
                        messageStream.ToArray());

                    ProcessServerMessage(json);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Receive error: {ex.Message}");

                if (IsHandleCreated)
                {
                    BeginInvoke(() =>
                    {
                        lblStatus.Text = "Connection lost";
                        btnConnect.Text = "Connect";
                    });
                }
            }
        }

        private void ProcessServerMessage(string json)
        {
            try
            {
                using JsonDocument document =
                    JsonDocument.Parse(json);

                JsonElement root = document.RootElement;

                if (root.ValueKind != JsonValueKind.Array)
                    return;

                foreach (JsonElement packet in root.EnumerateArray())
                {
                    if (!packet.TryGetProperty(
                            "cmd",
                            out JsonElement cmdElement))
                        continue;

                    string? command =
                        cmdElement.GetString();

                    Console.WriteLine(
                        $"AP -> {command}");

                    if (command == "RoomInfo")
                    {
                        HandleRoomInfo(packet);
                    }
                    else if (command == "Connected")
                    {
                        HandleConnected(packet);
                    }
                    else if (command == "ConnectionRefused")
                    {
                        HandleConnectionRefused(packet);
                    }
                    else if (command == "Bounced")
                    {
                        HandleBounced(packet);
                    }
                    else if (command == "RoomUpdate")
                    {
                        HandleRoomUpdate(packet);
                    }
                    else if (command == "Retrieved")
                    {
                        HandleRetrieved(packet);
                    }
                    else if (command == "SetReply")
                    {
                        HandleSetReply(packet);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Invalid AP packet: {ex.Message}");
            }
        }
        private void HandleSetReply(JsonElement packet)
        {
            if (!packet.TryGetProperty("key", out JsonElement key))
                return;

            if (key.GetString() != "DeathTracker_State")
                return;

            if (!packet.TryGetProperty("value", out JsonElement value))
                return;

            ApplyDeathTrackerState(value);
        }

        private void ApplyDeathTrackerState(JsonElement value)
        {
            if (value.ValueKind != JsonValueKind.String)
                return;

            string? stateJson = value.GetString();

            if (string.IsNullOrWhiteSpace(stateJson))
                return;

            try
            {
                DeathTrackerState? state =
                    JsonSerializer.Deserialize<DeathTrackerState>(stateJson);

                if (state == null)
                    return;

                totalDeaths = state.TotalDeaths;

                deathCounts.Clear();

                foreach (var pair in state.DeathCounts)
                {
                    deathCounts[pair.Key] = pair.Value;
                }

                history.Clear();
                history.AddRange(state.History);

                deathLinkPlayers.Clear();

                foreach (var pair in deathCounts)
                {
                    deathLinkPlayers.Add(pair.Key);
                }

                if (InvokeRequired)
                {
                    BeginInvoke(UpdateUI);
                }
                else
                {
                    UpdateUI();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"DeathTracker state error: {ex.Message}");
            }
        }

        private void HandleRoomUpdate(JsonElement packet)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => HandleRoomUpdate(packet));
                return;
            }

            //dolaczajacy gracze

            if (packet.TryGetProperty("checked_players", out JsonElement joined))
            {
                foreach (JsonElement slot in joined.EnumerateArray())
                {
                    int id = slot.GetInt32();

                    if (!connectedPlayers.ContainsKey(id)) connectedPlayers[id] = $"Player {id}";
                }
            }

            //wtchodzacy gracze

            if (packet.TryGetProperty("permissions", out _))
            {

            }

            UpdateUI();
        }

        private async Task SubscribeDeathTrackerData()
        {
            if (socket == null ||
                socket.State != WebSocketState.Open)
                return;

            var packet = new
            {
                cmd = "SetNotify",
                keys = new[]
                {
            "DeathTracker_State"
        }
            };

            string json = JsonSerializer.Serialize(new[] { packet });

            byte[] bytes = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        private async Task RequestDeathTrackerData()
        {
            if (socket == null ||
                socket.State != WebSocketState.Open)
                return;

            var packet = new
            {
                cmd = "Get",
                keys = new[]
                {
            "DeathTracker_State"
        }
            };

            string json = JsonSerializer.Serialize(new[] { packet });

            byte[] bytes = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        private async Task SaveDeathTrackerData()
        {
            if (socket == null ||
                socket.State != WebSocketState.Open)
                return;

            var state = new DeathTrackerState
            {
                TotalDeaths = totalDeaths,
                DeathCounts = new Dictionary<string, int>(deathCounts),
                History = new List<DeathRecord>(history)
            };

            string stateJson = JsonSerializer.Serialize(state);

            var packet = new
            {
                cmd = "Set",
                key = "DeathTracker_State",
                default_value = "",
                want_reply = false,
                operations = new[]
                {
            new
            {
                operation = "replace",
                value = stateJson
            }
        }
            };

            string json = JsonSerializer.Serialize(
                new[] { packet });

            byte[] bytes = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        private void HandleRetrieved(JsonElement packet)
        {
            if (!packet.TryGetProperty("keys", out JsonElement keys))
                return;

            if (!keys.TryGetProperty(
                    "DeathTracker_State",
                    out JsonElement value))
                return;

            ApplyDeathTrackerState(value);
        }

        private void HandleRoomInfo(JsonElement packet)
        {
            Console.WriteLine("Received RoomInfo.");

            SendConnect();
        }

        private void SendConnect()
        {
            if (socket == null ||
                socket.State != WebSocketState.Open)
                return;

            string slotName = txtSlot.Text.Trim();

            var connectPacket = new
            {
                cmd = "Connect",
                password = (string?)null,
                name = slotName,

                version = new
                {
                    major = 0,
                    minor = 6,
                    build = 6,
                    @class = "Version"
                },

                tags = new[]
                {
            "Tracker",
            "DeathLink"
            
        },

                items_handling = 0,
                uuid = Guid.NewGuid().ToString(),

                game = (string?)null,

                slot_data = true
            };

            string json = JsonSerializer.Serialize(
                new[] { connectPacket });

            byte[] bytes = Encoding.UTF8.GetBytes(json);

            _ = socket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            Console.WriteLine("AP <- Tracker Connect");
        }

        private void HandleConnected(JsonElement packet)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => HandleConnected(packet));
                return;
            }

            lblStatus.Text = "Connected to Archipelago";
            lblStatus.ForeColor = Color.LightGreen;

            connectedPlayers.Clear();

            if (packet.TryGetProperty("players", out JsonElement players))
            {
                foreach (JsonElement player in players.EnumerateArray())
                {
                    int slot = player.GetProperty("slot").GetInt32();

                    string name =
                        player.TryGetProperty("alias", out var alias)
                            ? alias.GetString() ?? ""
                            : player.GetProperty("name").GetString() ?? "";

                    connectedPlayers[slot] = name;
                    
                    
                    if (!deathCounts.ContainsKey(name))
                        deathCounts[name] = 0;
                }
            }

            UpdateUI();
            _ = RequestDeathTrackerData();
            _ = SubscribeDeathTrackerData();
        }

        private void HandleConnectionRefused(
            JsonElement packet)
        {
            string reason = "Unknown reason";

            if (packet.TryGetProperty(
                    "errors",
                    out JsonElement errors))
            {
                reason = errors.ToString();
            }

            if (InvokeRequired)
            {
                BeginInvoke(() =>
                {
                    lblStatus.Text = "Connection refused";
                    lblStatus.ForeColor = Color.OrangeRed;
                    btnConnect.Text = "Connect";

                    MessageBox.Show(
                        reason,
                        "Archipelago");
                });

                return;
            }

            lblStatus.Text = "Connection refused";
            btnConnect.Text = "Connect";
        }

        private void HandleBounced(JsonElement packet)
        {
            if (!packet.TryGetProperty("tags", out var tags))
                return;

            bool deathLink = false;

            foreach (var tag in tags.EnumerateArray())
            {
                if (tag.ValueKind == JsonValueKind.String &&
                    tag.GetString() == "DeathLink")
                {
                    deathLink = true;
                    break;
                }
            }

            if (!deathLink)
                return;

            if (!packet.TryGetProperty("data", out var data) ||
                data.ValueKind != JsonValueKind.Object)
                return;

            string player = "Unknown";

            if (data.TryGetProperty("source", out var sourceElement) &&
                sourceElement.ValueKind == JsonValueKind.String)
            {
                player = sourceElement.GetString() ?? "Unknown";
            }

            string cause = "Died.";

            if (data.TryGetProperty("cause", out var causeElement) &&
                causeElement.ValueKind == JsonValueKind.String)
            {
                string? receivedCause = causeElement.GetString();

                if (!string.IsNullOrWhiteSpace(receivedCause))
                    cause = receivedCause;
            }

            totalDeaths++;

            deathLinkPlayers.Add(player);

            if (!deathCounts.ContainsKey(player))
                deathCounts[player] = 0;

            deathCounts[player]++;

            history.Insert(0, new DeathRecord
            {
                Player = player,
                Cause = cause,
                Time = DateTime.Now
            });

            if (history.Count > 100)
                history.RemoveAt(100);

            BeginInvoke(UpdateUI);

            _ = SaveDeathTrackerData();
        }

        private void UpdateUI()
        {
            lblTotalDeaths.Text = totalDeaths.ToString();

            
            lblPlayers.Text = connectedPlayers.Count.ToString();

            if (history.Count > 0)
                lblLastDeath.Text =
                    $"{history[0].Player} ({history[0].Time:HH:mm:ss})";
            else
                lblLastDeath.Text = "-";

            
            lbHistory.Items.Clear();
            foreach (var death in history)
            {
                lbHistory.Items.Add(
                    $"[{death.Time:dd.MM.yyyy HH:mm:ss}] {death.Player} — {death.Cause}");
            }

            
            lvRanking.Items.Clear();

            foreach (var pair in deathCounts.OrderByDescending(x => x.Value))
            {
                ListViewItem item = new(pair.Key);
                item.SubItems.Add(pair.Value.ToString());

                if (lvRanking.Items.Count == 0)
                    item.ForeColor = Color.Gold;
                else if (lvRanking.Items.Count == 1)
                    item.ForeColor = Color.Silver;
                else if (lvRanking.Items.Count == 2)
                    item.ForeColor = Color.Peru;

                lvRanking.Items.Add(item);
            }

            if (overlayVisible && overlay != null)
            {
                string lastPlayer = history.Count > 0 ? history[0].Player : "N/A";
                overlay.UpdateOverlay(totalDeaths, lastPlayer);
            }
        }

        private async Task DisconnectFromArchipelago()
        {
            try
            {
                if (socket != null &&
                    socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "User disconnected",
                        CancellationToken.None);
                }

                socket?.Dispose();
                socket = null;

                lblStatus.Text = "Disconnected";
                lblStatus.ForeColor = Color.OrangeRed;
                btnConnect.Text = "Connect";
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Disconnect error: {ex.Message}");

                socket?.Dispose();
                socket = null;

                lblStatus.Text = "Disconnected";
                btnConnect.Text = "Connect";
            }
        }

        protected override void OnFormClosing(
            FormClosingEventArgs e)
        {
            shutdown.Cancel();

            socket?.Dispose();

            base.OnFormClosing(e);

            overlay?.Close();
        }
    }

    public class DeathRecord
    {
        public string Player { get; set; } = "";
        public string Cause { get; set; } = "";
        public DateTime Time { get; set; }
    }

    public class DeathTrackerState
    {
        public int TotalDeaths { get; set; }
        public List<DeathRecord> History { get; set; } = new();
        public Dictionary<string, int> DeathCounts { get; set; } = new();
    }
}