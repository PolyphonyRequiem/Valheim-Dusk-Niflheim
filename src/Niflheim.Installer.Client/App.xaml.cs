﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Niflheim.Installer.Client.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;

namespace Niflheim.Installer.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost host;

        private TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(5);

        public App()
        {
            host = new HostBuilder()
                       .ConfigureAppConfiguration(ConfigureApplication)
                       .ConfigureServices(ConfigureServices)
                       .ConfigureLogging(ConfigureLogging)
                       .Build();
        }

        private void ConfigureApplication(HostBuilderContext context, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddJsonFile(".\\appsettings.json", optional: false, reloadOnChange: false);
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<MainWindow>()
                             .AddSingleton<MainViewModel>();
        }

        private void ConfigureLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddDebug();
        }

        private async void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            await host.StartAsync();

            this.MainWindow = host.Services.GetRequiredService<MainWindow>();
            this.MainWindow.Show();
        }

        private async void OnApplicationExit(object sender, ExitEventArgs e)
        {
            await host.StopAsync(ShutdownTimeout);
            Debug.WriteLine("Host stopped.");
            Process.GetCurrentProcess().Kill();
        }
    }
}
