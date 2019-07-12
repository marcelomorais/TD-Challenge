using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalkDeskProject.Configuration;
using TalkDeskProject.Interfaces;
using TalkDeskProject.Validators;

namespace TalkDeskProject.Tests
{
    [TestClass]
    public class EngineTest
    {
        private Mock<IEngine> _mockEngine;
        private Mock<IValidator> _mockValidator;
        private Mock<IOptions<ConfigurationSettings>> _mockSettings;
        private Engine _engine;
        private List<string> lines;
        
        [TestInitialize]
        public void Initialise()
        {
            _mockEngine = new Mock<IEngine>();
            _mockValidator = new Mock<IValidator>();
            _mockSettings = new Mock<IOptions<ConfigurationSettings>>();
            _mockSettings.Setup(x => x.Value).Returns(new ConfigurationSettings { CostAfterFiveMinutes = 0.02M, CostBeforeFiveMinutes = 0.05M, Delimiter = ';', MoreExpensiveMinutes = 5, PhoneIndicator = '+', QuantityPhoneNumbers = 2, TotalDelimiters = 3 });

            _engine = new Engine(_mockValidator.Object, _mockSettings.Object);

            lines = new List<string>
            {
            "09:11:30;09:15:22;+351914374373;+351215355312",
            "15:20:04;15:23:49;+351217538222;+351214434422",
            "16:43:02;16:50:20;+351217235554;+351329932233"
            };
        }

        [TestMethod]
        public async Task ProcessLines_ValidLines_ProcessSuccessfully()
        {
            _mockEngine.Setup(x=> x.CalculateAmount(It.IsAny<string[]>())).Returns(("", 0));
            _mockEngine.Setup(x=> x.CalculateTime(It.IsAny<string[]>())).Returns(("", 0));
            _mockEngine.Setup(x=> x.CalculateFinalResult(It.IsAny<Dictionary<string, decimal>>(), It.IsAny<Dictionary<string, double>>())).Returns("");
            _mockValidator.Setup(x => x.ValidateFormats(It.IsAny<string[]>())).Returns(true);

            await _engine.ProcessLines(lines);
        }

    }
}
