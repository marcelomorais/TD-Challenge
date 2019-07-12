using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;
using TalkDeskProject.Configuration;
using TalkDeskProject.Interfaces;
using TalkDeskProject.Validators;
namespace TalkDeskProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json",
                        optional: false,
                        reloadOnChange: true);


            IConfigurationRoot configuration = builder.Build();
            var mySettingsConfig = new ConfigurationSettings();

            var serviceProvider = new ServiceCollection()
            .AddOptions()
            .Configure<ConfigurationSettings>(options => configuration.GetSection("Config").Bind(options))
            .AddSingleton<IEngine, Engine>()
            .AddSingleton<IValidator, Validator>()
            .BuildServiceProvider();

            var engine = serviceProvider.GetService<IEngine>();

            engine.Initialise();
        }

    }
}
