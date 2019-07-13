using System.Collections.Generic;

namespace TalkDeskProject.Interfaces
{
    public interface IAccountment
    {
        (string callFrom, decimal totalAmount) CalculateAmount(string[] line);
        (string callFrom, double totalTime) CalculateTime(string[] line);
        (KeyValuePair<string, decimal> phoneWithBiggerAmount, KeyValuePair<string, double> phoneWithHighestCall, string finalAnswer) CalculateFinalResult(Dictionary<string, decimal> totalAmountPerNumber, Dictionary<string, double> totalCallTimePerNumber);
    }
}
