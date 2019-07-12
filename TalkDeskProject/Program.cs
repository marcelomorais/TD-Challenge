using Microsoft.Extensions.DependencyInjection;
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

            var engine = serviceProvider.GetService<IEngineService>();

            engine.Initialise();
        }

    }
}
