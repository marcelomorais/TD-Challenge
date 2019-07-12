using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkDeskProject.Configuration;
using TalkDeskProject.Constants;
using TalkDeskProject.Extensions;
using TalkDeskProject.Interfaces;
using TalkDeskProject.Validators;

namespace TalkDeskProject
{
    public class Engine : IEngine
    {
        private IValidator _validator;
        private ConfigurationSettings _configuration;

        public Engine(IValidator validator, IOptions<ConfigurationSettings> configuration)
        {
            _validator = validator;
            _configuration = configuration.Value;
        }

        public async Task Initialise()
        {
            Console.WriteLine(Message.InsertFile);
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine(Message.EmptyInput);
                await Initialise();
            }

            try
            {
                List<string> alllines = new List<string>();

                File.ReadLines(input, Encoding.UTF8)
                    .AsParallel()
                    .WithDegreeOfParallelism(10)
                    .ForAll(x => alllines.Add(x));

                if (!alllines.Any())
                {
                    Console.WriteLine(Message.EmptyFile);
                    await Initialise();
                }

                Console.WriteLine(Message.Calculating);

                await ProcessLines(alllines);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(Message.FileDoNotExist);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine(Message.DirectoryDoNotExist);
            }
            catch (IOException)
            {
                Console.WriteLine(Message.Error);
            }
            finally
            {
                Console.WriteLine();
                await Initialise();
            }
        }
        public async Task ProcessLines(List<string> lines)
        {
            Dictionary<string, decimal> totalAmountPerNumber = new Dictionary<string, decimal>();
            Dictionary<string, double> totalCallTimePerNumber = new Dictionary<string, double>();

            int processedLines = 0;
            int errorLines = 0;

            foreach (var item in lines)
            {
                if (item.Count(x => x == _configuration.Delimiter) != _configuration.TotalDelimiters || item.Count(x => x == _configuration.PhoneIndicator) != _configuration.QuantityPhoneNumbers)
                {
                    Console.WriteLine($"The line with value \"{item}\" wasn't computed because was bad formatted.\n");
                    errorLines++;
                    continue;
                }
                var lineItems = item.Split(_configuration.Delimiter);

                if (!_validator.ValidateFormats(lineItems))
                {
                    Console.WriteLine($"The line with value \"{item}\" wasn't computed because was bad formatted.\n");
                    errorLines++;
                    continue;
                }

                var calc = CalculateAmount(lineItems);
                var time = CalculateTime(lineItems);

                totalAmountPerNumber.UpdateValue(calc.callFrom, calc.totalAmount);
                totalCallTimePerNumber.UpdateValue(time.callFrom, time.totalTime);
                processedLines++;
            }

            Console.WriteLine($"Total of {processedLines} lines processed with success.\n");
            Console.WriteLine($"Total of {errorLines} lines not processed.\n");

            var finalAnswer = CalculateFinalResult(totalAmountPerNumber, totalCallTimePerNumber);

            Console.WriteLine($"Total: {finalAnswer}\n");
        }
        public (string callFrom, decimal totalAmount) CalculateAmount(string[] line)
        {
            decimal totalAmount = 0;
            DateTime timeStart = DateTime.Parse(line[0]);
            DateTime timeFinish = DateTime.Parse(line[1]);
            string callFrom = line[2];

            var timeDiff = timeFinish - timeStart;
            var minutes = timeDiff.Minutes;
            var seconds = timeDiff.Seconds;

            totalAmount = minutes > _configuration.MoreExpensiveMinutes ? 
                (minutes - _configuration.MoreExpensiveMinutes) * _configuration.CostAfterFiveMinutes + (_configuration.CostBeforeFiveMinutes * _configuration.MoreExpensiveMinutes) : 
                minutes * _configuration.CostBeforeFiveMinutes;

            return (callFrom, totalAmount);
        }
        public (string callFrom, double totalTime) CalculateTime(string[] line)
        {
            DateTime timeStart = DateTime.Parse(line[0]);
            DateTime timeFinish = DateTime.Parse(line[1]);
            string callFrom = line[2];

            var timeDiff = timeFinish - timeStart;

            return (callFrom, timeDiff.TotalSeconds);
        }
        public string CalculateFinalResult(Dictionary<string, decimal> totalAmountPerNumber, Dictionary<string, double> totalCallTimePerNumber)
        {
            var biggerAmountNotCharged = totalAmountPerNumber.Max(x => x.Value);
            var highestCall = totalCallTimePerNumber.Max(x => x.Value);

            var phoneWithBiggerAmount = totalAmountPerNumber.FirstOrDefault(x => x.Value == biggerAmountNotCharged);
            var phoneWithHighestCall = totalCallTimePerNumber.FirstOrDefault(x => x.Value == highestCall);

            totalAmountPerNumber.Remove(phoneWithHighestCall.Key);

            var biggerAmountCharged = totalAmountPerNumber.Max(x => x.Value);

            var sumOfAllAmounts = totalAmountPerNumber.Values.Sum();
            var finalAnswer = sumOfAllAmounts.ToString("N", CultureInfo.InvariantCulture);

            Console.WriteLine($"Phone with bigger amount cost: {phoneWithBiggerAmount.Key} with amount {phoneWithBiggerAmount.Value}.\n");
            Console.WriteLine($"Phone with highest call duration (will not be charged): {phoneWithHighestCall.Key} with {phoneWithHighestCall.Value} seconds.\n");

            return finalAnswer;
        }

    }
}
