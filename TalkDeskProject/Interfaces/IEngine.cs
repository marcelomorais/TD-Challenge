using System.Collections.Generic;
using System.Threading.Tasks;

namespace TalkDeskProject.Interfaces
{
    public interface IEngine
    {
        Task Initialise();
        Task ProcessLines(List<string> lines);
    }
}
