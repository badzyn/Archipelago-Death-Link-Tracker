namespace DEATHTRACKERARCHIPELAGO
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        { 

            if (args.Length >= 1 &&
                args[0] == "--update")
            {
                if (args.Length < 3)
                    return;

                string targetExe = args[1];
                string downloadedExe = args[2];

                Task.Run(async () =>
                {
                    await Updater.PerformReplacementAsync(
                        targetExe,
                        downloadedExe);
                }).GetAwaiter().GetResult();

                return;
            }



            if (args.Length >= 1 &&
                args[0] == "--cleanup")
            {
                if (args.Length < 2)
                    return;

                string temporaryFile = args[1];

                
                Thread.Sleep(1000);

                Updater.CleanupTemporaryFile(
                    temporaryFile);

                return;
            }



            ApplicationConfiguration.Initialize();

            Application.Run(new Form1());
        }
    }
}