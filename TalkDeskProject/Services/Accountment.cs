﻿using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TalkDeskProject.Interfaces;
using TalkDeskProject.Settings;

namespace TalkDeskProject.Services
{
    public class Accountment : IAccountment
    {
        private Configuration _configuration;

        public Accountment(IOptions<Configuration> configuration)
        {
            _configuration = configuration.Value;
        }

        public Dictionary<string, decimal> CalculateAmount(Dictionary<string, double> calls)
        {
            Dictionary<string, decimal> valuePerPhone = new Dictionary<string, decimal>();

            foreach (var item in calls)
            {
                var phone = item.Key;
                var timeDiff = TimeSpan.FromSeconds(item.Value);
                var minutes = Convert.ToDecimal(timeDiff.TotalMinutes);

                var amount = minutes > _configuration.MoreExpensiveMinutes ?
                (minutes - _configuration.MoreExpensiveMinutes) * _configuration.CostAfterFiveMinutes + (_configuration.CostBeforeFiveMinutes * _configuration.MoreExpensiveMinutes) :
                minutes * _configuration.CostBeforeFiveMinutes;

                valuePerPhone.Add(phone, decimal.Round(amount, 2, MidpointRounding.AwayFromZero));
            }
            
            return valuePerPhone;
        }
        public (string callFrom, double totalTime) CalculateTime(string[] line)
        {
            DateTime timeStart = DateTime.Parse(line[0]);
            DateTime timeFinish = DateTime.Parse(line[1]);
            string callFrom = line[2];

            var timeDiff = timeFinish - timeStart;

            return (callFrom, timeDiff.TotalSeconds);
        }
        public virtual (KeyValuePair<string, decimal> phoneWithBiggerAmount, KeyValuePair<string, double> phoneWithHighestCall, string finalAnswer) CalculateFinalResult(Dictionary<string, decimal> totalAmountPerNumber, Dictionary<string, double> totalCallTimePerNumber)
        {
            var biggerAmount = totalAmountPerNumber.Max(x => x.Value);
            var highestCall = totalCallTimePerNumber.Max(x => x.Value);

            var phoneWithBiggerAmount = totalAmountPerNumber.FirstOrDefault(x => x.Value == biggerAmount);
            var phoneWithHighestCall = totalCallTimePerNumber.FirstOrDefault(x => x.Value == highestCall);

            totalAmountPerNumber.Remove(phoneWithHighestCall.Key);

            var biggerAmountCharged = totalAmountPerNumber.Max(x => x.Value);

            var sumOfAllAmounts = totalAmountPerNumber.Values.Sum();
            var finalAnswer = sumOfAllAmounts.ToString("N", CultureInfo.InvariantCulture);

            return (phoneWithBiggerAmount, phoneWithHighestCall, finalAnswer);
        }
    }
}
