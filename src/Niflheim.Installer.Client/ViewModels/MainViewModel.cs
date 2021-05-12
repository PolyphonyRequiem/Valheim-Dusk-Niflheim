using System.Runtime.CompilerServices;
using System.ComponentModel;
using Niflheim.Installer.Client.Commands;
using System;
using Niflheim.Installer.Client.Repositories;
using System.Windows;
using Niflheim.Installer.Client.Configuration;
using System.Linq;
using Niflheim.Installer.Entities;
using System.IO;
using Niflheim.Installer.Services;
using System.Threading.Tasks;

namespace Niflheim.Installer.Client.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string consoleText = "Welcome to the Niflheim Installer.";
        public string ConsoleText { get => consoleText; set => SetProperty(ref consoleText, value); }

        private string installButtonText = "Install";
        public string InstallButtonText { get => installButtonText; set => SetProperty(ref installButtonText, value); }

        private string valheimPath = @"C:\Program Files (x86)\Steam\steamapps\common\Valheim";
        public string ValheimPath { get => valheimPath; set => SetProperty(ref valheimPath, value); }

        private string niflheimPath = @"C:\Program Files (x86)\Steam\steamapps\common\Niflheim";
        public string NiflheimPath
        {
            get => niflheimPath;
            set
            {
                if (value != niflheimPath)
                {
                    SetProperty(ref niflheimPath, value);
                    CheckStatus();
                }
            }
        }

        private bool niflheimPathLocked = true;
        private ModpackArchiveDefinition latestModpack;

        public bool NiflheimPathLocked { get => niflheimPathLocked; set => SetProperty(ref niflheimPathLocked, value); }

        public InstallCommand InstallCommand { get; init; }
        public BrowseCommand BrowseValheimCommand { get; init; }
        public BrowseCommand BrowseNiflheimCommand { get; init; }

        private readonly WebModpackRepository webModpackRepository;
        private readonly AppConfig config;

        public MainViewModel(WebModpackRepository webModpackRepository, AppConfig config)
        {

            this.webModpackRepository = webModpackRepository;
            this.config = config;

            this.BrowseValheimCommand = new BrowseCommand(() => this.ValheimPath, (path) => this.ValheimPath = path);
            this.BrowseNiflheimCommand = new BrowseCommand(() => this.NiflheimPath, (path) => this.NiflheimPath = path);
            this.InstallCommand = new InstallCommand(new Progress<string>(s => this.ConsoleText += $"\n{s}"), this.config.ServerUrl);
        }

        public void Loaded()
        {
            Task.Run(() =>
            {
                this.AppendLog("Getting the latest version.");
                var modpacks = this.webModpackRepository.GetAllActiveModpacksWithTag(this.config.Tag);
                modpacks.OrderByDescending(_ => SemanticVersion.Parse(_.Version));
                this.latestModpack = modpacks.FirstOrDefault() ?? (ModpackArchiveDefinition.None as ModpackArchiveDefinition);


                this.CheckStatus();
            });
        }

        private void CheckStatus()
        {
            Task.Run(() =>
            {
                if (latestModpack == ModpackArchiveDefinition.None)
                {
                    MessageBox.Show("Unable to find the latest version of Niflheim.  This may be a transient error.  You may attempt to connect play still, but if you're unable to connect to the server please report this error in the discord server's bug");
                    this.PrepareLaunch();
                }

                DirectoryInfo valheimDirectory = new DirectoryInfo(this.ValheimPath);
                DirectoryInfo niflheimDirectory = new DirectoryInfo(this.NiflheimPath);

                var installer = new InstallerService(valheimDirectory, niflheimDirectory);
                if (niflheimDirectory.Exists)
                {
                    var version = installer.GetModVersion();

                    if (version == SemanticVersion.Parse("0.0.0"))
                    {
                        this.AppendLog($"Found Niflheim path but unable to determine version.  Preparing for reinstall. Using version {latestModpack.Version}");
                        this.PrepareInstall(latestModpack);
                        return;
                    }
                    if (version == latestModpack.Version)
                    {
                        this.AppendLog($"Found Niflheim, current version {version} is latest version.");
                        this.PrepareLaunch();
                        return;
                    }
                    if (latestModpack.Version < version)
                    {
                        this.AppendLog($"Found Niflheim, current version {version} is out of date.  Ready to update to version {latestModpack.Version}");
                        this.PrepareUpdate(latestModpack);
                        return;
                    }
                    if (latestModpack.Version > version)
                    {
                        this.AppendLog($"Found Niflheim, current version {version} is AHEAD of latest ({latestModpack.Version}).  Uh oh!  Preparing for reinstall.");
                        this.PrepareInstall(latestModpack);
                        return;
                    }
                }
                else
                {
                    this.AppendLog($"No previous known Niflheim install version.  Preparing for first install.  Using version {latestModpack.Version}");
                    this.PrepareInstall(latestModpack);
                }
            });
        }

        private void PrepareLaunch()
        {
            this.InstallCommand.SetLaunch(this.NiflheimPath);
        }

        private void PrepareInstall(ModpackArchiveDefinition modpack)
        {
            this.InstallCommand.SetInstall(this.ValheimPath, this.NiflheimPath, modpack);
        }

        private void PrepareUpdate(ModpackArchiveDefinition modpack)
        {
            this.InstallCommand.SetUpdate(this.NiflheimPath, modpack);
        }

        public void AppendLog(string s)
        {
            this.ConsoleText += $"\n{s}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }
    }
}
