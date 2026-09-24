using System.Diagnostics;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace DEATHTRACKERARCHIPELAGO
{
    public static class Updater
    {
        private const string Repository =
            "badzyn/Archipelago-Death-Link-Tracker";

        private const string GitHubApi =
            $"https://api.github.com/repos/{Repository}/releases/latest";

        private static readonly HttpClient client = new HttpClient();

        static Updater()
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Archipelago-Death-Link-Tracker");
        }

        public static Version CurrentVersion
        {
            get
            {
                Version? version =
                    Assembly.GetExecutingAssembly().GetName().Version;

                return version ?? new Version(1, 0, 0);
            }
        }

        public static async Task<GithubRelease?> CheckForUpdateAsync()
        {
            try
            {
                GithubRelease? release =
                    await client.GetFromJsonAsync<GithubRelease>(GitHubApi);

                if (release == null)
                    return null;

                Version remoteVersion = ParseVersion(release.TagName);

                if (remoteVersion > CurrentVersion)
                    return release;

                return null;
            }
            catch
            {

                return null;
            }
        }

        public static Version ParseVersion(string tag)
        {
            string version = tag.Trim();

            if (version.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                version = version.Substring(1);

       
            if (Version.TryParse(version, out Version? result))
                return result;

            return new Version(0, 0, 0);
        }

        public static async Task DownloadUpdateAsync(
            GithubRelease release,
            string destination,
            IProgress<int>? progress = null)
        {
            GithubAsset? exeAsset = release.Assets.FirstOrDefault(
                asset =>
                    asset.Name.EndsWith(
                        ".exe",
                        StringComparison.OrdinalIgnoreCase));

            if (exeAsset == null)
                throw new Exception(
                    "GitHub Release nie zawiera pliku EXE.");

            using HttpResponseMessage response =
                await client.GetAsync(
                    exeAsset.DownloadUrl,
                    HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            long? totalBytes =
                response.Content.Headers.ContentLength;

            await using Stream input =
                await response.Content.ReadAsStreamAsync();

            await using FileStream output =
                new FileStream(
                    destination,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None);

            byte[] buffer = new byte[81920];

            long downloaded = 0;

            while (true)
            {
                int read = await input.ReadAsync(buffer);

                if (read == 0)
                    break;

                await output.WriteAsync(
                    buffer.AsMemory(0, read));

                downloaded += read;

                if (totalBytes.HasValue &&
                    totalBytes.Value > 0)
                {
                    int percentage =
                        (int)(downloaded * 100 / totalBytes.Value);

                    progress?.Report(percentage);
                }
            }
        }

        public static void StartReplacement(
            string downloadedExe)
        {
            string? currentExe =
                Environment.ProcessPath;

            if (string.IsNullOrWhiteSpace(currentExe))
                throw new Exception(
                    "Nie można określić ścieżki aplikacji.");

            Process.Start(new ProcessStartInfo
            {
                FileName = downloadedExe,
                Arguments =
                    $"--update \"{currentExe}\" \"{downloadedExe}\"",
                UseShellExecute = true
            });

            Application.Exit();
        }

        public static async Task PerformReplacementAsync(
            string targetExe,
            string downloadedExe)
        {

            for (int i = 0; i < 50; i++)
            {
                try
                {
                    File.Copy(
                        downloadedExe,
                        targetExe,
                        true);

                    break;
                }
                catch (IOException)
                {
                    await Task.Delay(200);
                }
                catch (UnauthorizedAccessException)
                {
                    await Task.Delay(200);
                }
            }

            if (!File.Exists(targetExe))
                throw new Exception(
                    "Nie udało się podmienić aplikacji.");

            Process.Start(new ProcessStartInfo
            {
                FileName = targetExe,
                Arguments =
                    $"--cleanup \"{downloadedExe}\"",
                UseShellExecute = true
            });
        }

        public static void CleanupTemporaryFile(
            string temporaryFile)
        {
            try
            {
                if (File.Exists(temporaryFile))
                    File.Delete(temporaryFile);
            }
            catch
            {

            }
        }
    }

    public class GithubRelease
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("body")]
        public string Body { get; set; } = "";

        [JsonPropertyName("assets")]
        public List<GithubAsset> Assets { get; set; } = new();
    }

    public class GithubAsset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("browser_download_url")]
        public string DownloadUrl { get; set; } = "";
    }
}