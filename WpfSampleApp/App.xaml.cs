using DEM.Net.Core;
using DEM.Net.Core.Configuration;
using DEM.Net.Extension.VisualTopo;
using DEM.Net.glTF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MonoGameViewer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSampleApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        public App()
        {
            var config = new ConfigurationBuilder()
                       .SetBasePath(AppContext.BaseDirectory)
                       .AddJsonFile("appsettings.json", optional: true)
                       .AddJsonFile("secrets.json", optional: true, reloadOnChange: false)
                       .Build();


            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services, config);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services, IConfigurationRoot config)
        {
            services.AddOptions()
                 .Configure<AppSecrets>(config.GetSection(nameof(AppSecrets)))
                 .Configure<DEMNetOptions>(config.GetSection(nameof(DEMNetOptions)))
                 .AddDemNetCore()
                 .AddDemNetglTF()
                 .AddDemNetVisualTopoExtension()
                 .BuildServiceProvider();


            services.AddDemNetCore();
            services.AddDemNetglTF();
            services.AddDemNetVisualTopoExtension();

            services.AddSingleton<DemNetVisualTopoService>(); 
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainScene>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow.DataContext = serviceProvider.GetService<MainScene>();
            mainWindow.Show();
        }

    }
}
