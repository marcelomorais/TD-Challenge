using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using TalkDeskProject.Interfaces;
using TalkDeskProject.Services;
using TalkDeskProject.Settings;
using TalkDeskProject.Validators;

namespace TalkDeskProject.ConfigurationSettings
{
    public class DependencyInjection
    {
        public ServiceProvider ServiceBuilder()
        {
            var configuration = ConfigurationBuilder();

            var serviceProvider = new ServiceCollection()
          .AddOptions()
          .Configure<Configuration>(options => configuration.GetSection("Config").Bind(options))
          .AddSingleton<IEngine, Engine>()
          .AddSingleton<IConsoleWrapper, ConsoleWrapper>()
          .AddSingleton<IFileWrapper, FileWrapper>()
          .AddSingleton<IAccountment, Accountment>()
          .AddSingleton<IValidator, Validator>()
          .BuildServiceProvider();

            return serviceProvider;
        }

        public IConfigurationRoot ConfigurationBuilder()
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("config.json",
                       optional: false,
                       reloadOnChange: true);


            IConfigurationRoot configuration = builder.Build();

            return configuration;
        }
    }
}
