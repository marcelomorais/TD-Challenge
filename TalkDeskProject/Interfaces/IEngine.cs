using System.Collections.Generic;
using System.Threading.Tasks;

namespace TalkDeskProject.Interfaces
{
    public interface IEngine
    {
        Task Initialise();
        Task ProcessLines(List<string> lines);
        (string callFrom, decimal totalAmount) CalculateAmount(string[] line);
        (string callFrom, double totalTime) CalculateTime(string[] line);
        string CalculateFinalResult(Dictionary<string, decimal> totalAmountPerNumber, Dictionary<string, double> totalCallTimePerNumber);
    }
}
