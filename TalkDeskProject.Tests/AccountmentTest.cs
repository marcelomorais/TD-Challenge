using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TalkDeskProject.Services;
using TalkDeskProject.Settings;

namespace TalkDeskProject.Tests
{
    [TestClass]
    public class AccountmentTest
    {
        private Mock<IOptions<Configuration>> _mockSettings;
        private Accountment _accountment;

        [TestInitialize]
        public void Initialise()
        {
            _mockSettings = new Mock<IOptions<Configuration>>();
            _mockSettings.Setup(x => x.Value).Returns(new Configuration { CostAfterFiveMinutes = 0.02M, CostBeforeFiveMinutes = 0.05M, Delimiter = ';', MoreExpensiveMinutes = 5, PhoneIndicator = '+', QuantityPhoneNumbers = 2, TotalDelimiters = 3 });
            _accountment = new Accountment(_mockSettings.Object);
        }

        [TestMethod]
        public void CalculateAmount_CalcWithExpensiveMinutes_CalculateAmountCorrectly()
        {
            var call = new Dictionary<string, double>() { { "351217235554", 120 } };
            var timeDiff = TimeSpan.FromSeconds(call.First().Value);
            var minutes = Convert.ToDecimal(timeDiff.TotalMinutes);

            var response = _accountment.CalculateAmount(call);

            Assert.IsTrue(response.ContainsKey(call.First().Key));
            Assert.AreEqual(minutes * _mockSettings.Object.Value.CostBeforeFiveMinutes, response.First().Value);
        }

        [TestMethod]
        public void CalculateAmount_CalcWithLessExpensiveMinutes_CalculateAmountCorrectly()
        {
            var call = new Dictionary<string, double>() { { "351217235554", 2039 } };
            var timeDiff = TimeSpan.FromSeconds(call.First().Value);
            var minutes = Convert.ToDecimal(timeDiff.TotalMinutes);

            var response = _accountment.CalculateAmount(call);
            var amountCalc = (minutes - _mockSettings.Object.Value.MoreExpensiveMinutes) *
                _mockSettings.Object.Value.CostAfterFiveMinutes +
                (_mockSettings.Object.Value.CostBeforeFiveMinutes *
                _mockSettings.Object.Value.MoreExpensiveMinutes);

            Assert.IsTrue(response.ContainsKey(call.First().Key));
            Assert.AreEqual(decimal.Round(amountCalc, 2, MidpointRounding.AwayFromZero), response.First().Value);
        }

        [TestMethod]
        public void CalculateTime_CalcTotalTimeSpentOnCall_CalculateTimeCorrectly()
        {
            string line = "15:15:04;15:23:49;+351217538222;+351214434422";
            var arguments = line.Split(';');

            var date1 = DateTime.Parse(arguments[1]);
            var date2 = DateTime.Parse(arguments[0]);
            var calc = date1 - date2;
            var response = _accountment.CalculateTime(arguments);

            Assert.AreEqual("+351217538222", response.callFrom);
            Assert.AreEqual(calc.TotalSeconds, response.totalTime);
        }

        [TestMethod]
        public void CalculateFinalResult_CalcTotalTimeSpentOnCall_CalculateTimeCorrectly()
        {
            string line = "15:15:04;15:23:49;+351217538222;+351214434422";
            var arguments = line.Split(';');

            var date1 = DateTime.Parse(arguments[1]);
            var date2 = DateTime.Parse(arguments[0]);
            var calc = date1 - date2;
            var response = _accountment.CalculateTime(arguments);

            Assert.AreEqual("+351217538222", response.callFrom);
            Assert.AreEqual(calc.TotalSeconds, response.totalTime);
        }
    }
}
