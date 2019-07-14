using Microsoft.Extensions.DependencyInjection;
using TalkDeskProject.ConfigurationSettings;
using TalkDeskProject.Interfaces;
using TalkDeskProject.Services;

namespace TalkDeskProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new DependencyInjection()
                .ServiceBuilder();

            var engine = serviceProvider.GetService<IEngine>();

            engine.Initialise(args[0]);
        }

    }
}
