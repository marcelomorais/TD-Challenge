﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TalkDeskProject.Constants;
using TalkDeskProject.Extension;
namespace TalkDeskProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialise().Wait();
        }

        static async Task Initialise()
        {
            Console.WriteLine(Message.InsertFile);
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine(Message.EmptyInput);
                Console.WriteLine(Message.InsertFile);
                return;
            }

            try
            {
                var lines = await File.ReadAllLinesAsync(input);

                if (!lines.Any())
                {
                    Console.WriteLine(Message.EmptyFile);
                    await Initialise();
                }

                Console.WriteLine(Message.Calculating);

                await ProcessLines(lines);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("That file does not exist!");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Directory does not exist!");
            }
            catch (IOException)
            {
                Console.WriteLine("Oops! Something Wrong happened!");
            }
            finally
            {
                Console.WriteLine();
                await Initialise();
            }
        }

        private static async Task ProcessLines(string[] lines)
        {
            Dictionary<string, decimal> finalResult = new Dictionary<string, decimal>();
            int processedLines = 0;
            int errorLines = 0;

            foreach (var item in lines)
            {
                if (item.Count(x => x == ';') != 3 || item.Count(x => x == '+') != 2)
                {
                    Console.WriteLine($"The line with value \"{item}\" wasn't computed because was bad formatted.\n");
                    errorLines++;
                    continue;
                }
                var lineItems = item.Split(";");

                if (!ValidateFormats(lineItems))
                {
                    Console.WriteLine($"The line with value \"{item}\" wasn't computed because was bad formatted.\n");
                    errorLines++;
                    continue;
                }

                var calc = CalculateTime(lineItems);

                finalResult.UpdateAmount(calc.callFrom, calc.totalAmount);

                processedLines++;
            }

            Console.WriteLine($"Total of {processedLines} lines processed with success.\n");
            Console.WriteLine($"Total of {errorLines} lines not processed.\n");

            var biggerAmount = finalResult.Max(x => x.Value);
            var phoneWithBiggerAmount = finalResult.FirstOrDefault(x => x.Value == biggerAmount);
            var sumOfAllAmounts = finalResult.Values.Sum();
            var finalAnswer = (sumOfAllAmounts - biggerAmount).ToString("N", CultureInfo.InvariantCulture);

            Console.WriteLine($"Phone with bigger amount cost: {phoneWithBiggerAmount.Key} with amount {phoneWithBiggerAmount.Value}.\n");
            Console.WriteLine($"Total: {finalAnswer}\n");
        }

        private static (string callFrom, decimal totalAmount) CalculateTime(string[] line)
        {
            decimal totalAmount = 0;
            DateTime timeStart = DateTime.Parse(line[0]);
            DateTime timeFinish = DateTime.Parse(line[1]);
            string callFrom = line[2];

            var timeDiff = timeFinish - timeStart;
            var minutes = timeDiff.Minutes;
            var seconds = timeDiff.Seconds;

            totalAmount = minutes > 5 ? (minutes - 5) * 0.2M + 0.25M : minutes * 0.5M;

            return (callFrom, totalAmount);
        }

        private static bool ValidateFormats(string[] lineValue)
        {
            if (!DateTime.TryParse(lineValue[0], out DateTime timeOfStart))
                return false;

            if (!DateTime.TryParse(lineValue[1], out DateTime timeOfFinish))
                return false;

            if (timeOfStart > timeOfFinish)
                return false;

            if (lineValue[2] == lineValue[3])
                return false;

            return true;
        }
    }
}
