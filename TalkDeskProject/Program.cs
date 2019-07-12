using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            var serviceProvider = new ServiceCollection()
            .AddSingleton<IEngineService, EngineService>()
            .AddSingleton<IValidators, Validator>()
            .BuildServiceProvider();

            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json",
                     optional: true,
                     reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            var mySettingsConfig = new ConfigurationSettings();
            //configuration.GetSection("Config").Bind(mySettingsConfig);

            //IConfigurationRoot configuration = builder.Build();


            var engine = serviceProvider.GetService<IEngineService>();

            engine.Initialise();
        }

    }
}
