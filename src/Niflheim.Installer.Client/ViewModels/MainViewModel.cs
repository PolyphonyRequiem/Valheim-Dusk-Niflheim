using Microsoft.Win32;
using Niflheim.Installer.Client.Commands;
using Niflheim.Installer.Client.Configuration;
using Niflheim.Installer.Client.Repositories;
using Niflheim.Installer.Clients;
using Niflheim.Installer.Entities;
using Niflheim.Installer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Niflheim.Installer.Client.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    { 
        private bool steamDetectionCompleted = false;

        private string consoleText = "Welcome to the Niflheim Installer.";
        public string ConsoleText { get => consoleText; set => SetProperty(ref consoleText, value); }

        private string installButtonText = "Install";
        public string InstallButtonText { get => installButtonText; set => SetProperty(ref installButtonText, value); }

        private string valheimPath = "";
        public string ValheimPath
        {
            get => valheimPath;
            set
            {
                if (value != valheimPath)
                {
                    SetProperty(ref valheimPath, value);
                    this.preferences.SetPreference("valheimPath", value);
                    CheckStatus();
                }
            }
        }

        private string niflheimPath = "";
        public string NiflheimPath
        {
            get => niflheimPath;
            set
            {
                if (value != niflheimPath)
                {
                    SetProperty(ref niflheimPath, value);
                    this.preferences.SetPreference("niflheimPath", value);
                    CheckStatus();
                }
            }
        }

        private bool niflheimPathLocked = true;
        private ModpackArchiveDefinition latestModpack;
        private FileInfo preferencesFile = new FileInfo("preferences.niflheimpath.cfg");
        private bool ready;

        public bool NiflheimPathLocked { get => niflheimPathLocked; set => SetProperty(ref niflheimPathLocked, value); }

        public InstallCommand InstallCommand { get; init; }
        public BrowseCommand BrowseValheimCommand { get; init; }
        public BrowseCommand BrowseNiflheimCommand { get; init; }

        private readonly WebModpackRepository webModpackRepository;
        private readonly PreferencesService preferences;
        private readonly DiscoveryClient discoveryClient;
        private readonly AppConfig config;

        public MainViewModel(WebModpackRepository webModpackRepository, PreferencesService preferences, DiscoveryClient discoveryClient, AppConfig config)
        {

            this.webModpackRepository = webModpackRepository;
            this.preferences = preferences;
            this.discoveryClient = discoveryClient;
            this.config = config;

            this.BrowseValheimCommand = new BrowseCommand(() => this.ValheimPath, (path) => this.ValheimPath = path);
            this.BrowseNiflheimCommand = new BrowseCommand(() => this.NiflheimPath, (path) => this.NiflheimPath = path);
            this.InstallCommand = new InstallCommand(new Progress<string>(s => this.ConsoleText += $"\n{s}"));
        }

        public void Loaded()
        {
            this.NiflheimPath = this.preferences.GetPreference("niflheimPath");
            this.steamDetectionCompleted = Boolean.Parse(this.preferences.GetPreference("steamDetectionCompleted"));

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            var launcherVersion = SemanticVersion.Parse($"{version.Major}.{version.Minor}.{version.Revision}");
            var result = this.discoveryClient.CheckForLauncherUpdate(launcherVersion);

            if (result.UpdateRequired)
            {
                var updatePrompt = MessageBox.Show($"An updated launcher, version {result.LatestVersion} is available! Clicking okay will launch your browser to download the latest version.  Once it is downloaded, close this Launcher and extract the files into the same directory your launcher is in!", "Launcher Update", MessageBoxButton.OKCancel);

                if (updatePrompt == MessageBoxResult.OK)
                {
                    Process.Start("cmd", $"/c start {result.UpdateUrl}");
                }
            }

            if (this.steamDetectionCompleted == false)
            {
                this.TryDetectSteam();
                this.steamDetectionCompleted = true;
                this.preferences.SetPreference("steamDetectionCompleted", this.steamDetectionCompleted.ToString());
            }
            this.ValheimPath = this.preferences.GetPreference("valheimPath");

            this.ready = true;
            CheckStatus();
        }

        private void TryDetectSteam()
        {
            this.AppendLog("Attempting to find Valheim...");
            RegistryKey steamRegKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam");
            
            if (steamRegKey == null)
            {
                this.AppendLog("Couldn't find steam.  Ah well.  Please browse to the Valheim install path above.");
            }

            var steamInstallPath = steamRegKey.GetValue("InstallPath") as string;

            if (steamInstallPath == null)
            {
                this.AppendLog("Couldn't find steam.  Ah well.  Please browse to the Valheim install path above.");
                return;
            }

            // find app manifest
            FileInfo libraryFolders = new FileInfo(Path.Combine(steamInstallPath, @"steamapps\libraryfolders.vdf"));

            List<DirectoryInfo> steamLibDirectories = new List<DirectoryInfo>();
            steamLibDirectories.Add(new DirectoryInfo(Path.Combine(steamInstallPath, @"steamapps")));

            if (libraryFolders.Exists)
            {
                var libfoldContent = File.ReadAllText(libraryFolders.FullName);
                var matches = Regex.Matches(libfoldContent, "\"\\d\"\\s+\"(?<libraryPath>.*?)\"", RegexOptions.IgnoreCase);
                if (matches.Count > 0)
                {
                    this.AppendLog($"Found {matches.Count} steam libraries.  Searching them for Valheim manifest.");
                    var auxLibDirs = matches.Select(m => new DirectoryInfo(m.Groups["libraryPath"].Value.Replace("\\\\", "\\")));
                    steamLibDirectories.AddRange(auxLibDirs);
                }
            }

            foreach (var lib in steamLibDirectories)
            {
                this.AppendLog($"Checking {lib.FullName} for Valheim manifest.");
                var candidate = new FileInfo(Path.Combine(lib.FullName, @"appmanifest_892970.acf"));

                if (candidate.Exists)
                {
                    this.AppendLog($"Found!");
                    var manifestContent = File.ReadAllText(candidate.FullName);
                    var installDirMatch = Regex.Match(manifestContent, "\"installdir\"\\s+\"(?<valheimPath>.*?)\"", RegexOptions.IgnoreCase);

                    if (installDirMatch.Success)
                    {
                        var valheimRelativeInstallDir = installDirMatch.Groups["valheimPath"].Value.Replace("\\\\", "\\");
                        var valheimDir = Path.Combine(lib.FullName, "common", valheimRelativeInstallDir);

                        DirectoryInfo valheimDirInfo = new DirectoryInfo(valheimDir);
                        if (valheimDirInfo.Exists)
                        {
                            this.AppendLog($"Discovered Valheim at {valheimDirInfo.FullName}!");
                            this.preferences.SetPreference("valheimPath", valheimDirInfo.FullName);
                        }
                        else
                        {
                            this.AppendLog("Couldn't verify Valheim Directory. Ah well.  Please browse to the Valheim install path above.");
                        }
                    }
                    break;
                }
            }            
        }

        private void CheckStatus()
        {
            if (!this.ready)
            {
                return;
            }
            Task.Run(() =>
            {

                this.AppendLog("Getting the latest version.");
                var modpacks = this.webModpackRepository.GetAllActiveModpacksWithTag(this.config.Tag);
                modpacks.OrderByDescending(_ => SemanticVersion.Parse(_.Version));
                this.latestModpack = modpacks.FirstOrDefault() ?? (ModpackArchiveDefinition.None as ModpackArchiveDefinition);

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
                    if (latestModpack.Version > version)
                    {
                        this.AppendLog($"Found Niflheim, current version {version} is out of date.  Ready to update to version {latestModpack.Version}");
                        this.PrepareUpdate(latestModpack);
                        return;
                    }
                    if (latestModpack.Version < version)
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
