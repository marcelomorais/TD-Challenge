using System.Threading.Tasks;

namespace TalkDeskProject.Interfaces
{
    public interface IEngineService
    {
        Task Initialise();
        Task ProcessLines(string[] lines);
        (string callFrom, decimal totalAmount) CalculateTime(string[] line);
    }
}
