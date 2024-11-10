using AssetManagement.DataAccess;
using AssetManagement.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace AssetManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddDbContext<AssetUnityManagementContext>();
            services.AddScoped<AssetService>();
            services.AddScoped<TagService>();
            services.AddScoped<FileService>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IAssetRepository, AssetRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddTransient<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }

}
