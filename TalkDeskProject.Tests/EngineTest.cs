using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TalkDeskProject.Constants;
using TalkDeskProject.Interfaces;
using TalkDeskProject.Services;
using TalkDeskProject.Settings;
using TalkDeskProject.Validators;

namespace TalkDeskProject.Tests
{
    [TestClass]
    public class EngineTest
    {
        private Mock<IEngine> _mockEngine;
        private Mock<IValidator> _mockValidator;
        private Mock<IAccountment> _mockAccountment;
        private Mock<IConsoleWrapper> _mockConsole;
        private Mock<IFileWrapper> _mockFileWrapper;
        private Mock<IOptions<Configuration>> _mockSettings;
        private Mock<Engine> _engine;
        private List<string> lines;
        private List<string> badFormattedlines;

        [TestInitialize]
        public void Initialise()
        {
            _mockEngine = new Mock<IEngine>();
            _mockConsole = new Mock<IConsoleWrapper>();
            _mockFileWrapper = new Mock<IFileWrapper>();
            _mockAccountment = new Mock<IAccountment>();
            _mockValidator = new Mock<IValidator>();
            _mockSettings = new Mock<IOptions<Configuration>>();
            _mockSettings.Setup(x => x.Value).Returns(new Configuration { CostAfterFiveMinutes = 0.02M, CostBeforeFiveMinutes = 0.05M, Delimiter = ';', MoreExpensiveMinutes = 5, PhoneIndicator = '+', QuantityPhoneNumbers = 2, TotalDelimiters = 3 });
            _mockConsole.Setup(x => x.WriteLine(It.IsAny<string>()));
            _mockConsole.Setup(x => x.ReadLine()).Returns(It.IsAny<string>());

            _engine = new Mock<Engine>(_mockConsole.Object, _mockFileWrapper.Object, _mockAccountment.Object, _mockValidator.Object, _mockSettings.Object) { CallBase = true };

            lines = new List<string>
            {
            "09:11:30;09:15:22;+351914374373;+351215355312",
            "15:20:04;15:23:49;+351217538222;+351214434422",
            "16:43:02;16:50:20;+351217235554;+351329932233"
            };

            badFormattedlines = new List<string>
            {
            "09:11:30;09:15:22;+351914374373;+351215355312;test",
            "15:20:04;15:23:49;351217538222;+351214434422;",
            "16:43:02;16:50:20;+351217235554;+351217235554;"
            };
        }

        [TestMethod]
        public async Task ProcessLines_ValidLines_AllMethodsAreBeingCalled()
        {
            _mockAccountment.Setup(x => x.CalculateAmount(It.IsAny<Dictionary<string, double>>())).Returns(It.IsAny<Dictionary<string, decimal>>());
            _mockAccountment.Setup(x => x.CalculateTime(It.IsAny<string[]>())).Returns((string.Empty, default(double)));
            _mockAccountment.Setup(x => x.CalculateFinalResult(It.IsAny<Dictionary<string, decimal>>(), It.IsAny<Dictionary<string, double>>()));
            _mockValidator.Setup(x => x.ValidateFormats(It.IsAny<string>())).Returns(true);

            await _engine.Object.ProcessLines(lines);

            _mockValidator.Verify(x => x.ValidateFormats(It.IsAny<string>()), Times.Exactly(lines.Count));
            _mockAccountment.Verify(x => x.CalculateAmount(It.IsAny<Dictionary<string, double>>()), Times.Once);
            _mockAccountment.Verify(x => x.CalculateTime(It.IsAny<string[]>()), Times.Exactly(lines.Count));
            _mockAccountment.Verify(x => x.CalculateFinalResult(It.IsAny<Dictionary<string, decimal>>(), It.IsAny<Dictionary<string, double>>()), Times.Once);
            _mockConsole.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeast(5));
        }


        [TestMethod]
        public async Task ProcessLines_FileWithBadFormattedLines_ValidadeAndCalculateFinalResultAreCalled()
        {
            await _engine.Object.ProcessLines(badFormattedlines);

            _mockValidator.Verify(x => x.ValidateFormats(It.IsAny<string>()), Times.Exactly(badFormattedlines.Count));
            _mockAccountment.Verify(x => x.CalculateAmount(It.IsAny<Dictionary<string, double>>()), Times.Once);
            _mockAccountment.Verify(x => x.CalculateTime(It.IsAny<string[]>()), Times.Never);
            _mockAccountment.Verify(x => x.CalculateFinalResult(It.IsAny<Dictionary<string, decimal>>(), It.IsAny<Dictionary<string, double>>()), Times.Never);
            _mockConsole.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeast(4));
            _mockConsole.Verify(x => x.WriteLine("Total of 3 lines not processed.\n"), Times.Once);
        }

        [TestMethod]
        public async Task Initialise_EmptyFile_EscapeKeyPressed()
        {
            _mockConsole.Setup(x => x.ReadLine()).Returns(string.Empty);
            _mockConsole.Setup(x => x.ReadKey()).Returns(new ConsoleKeyInfo(It.IsAny<char>(), ConsoleKey.Escape, false, false, false));

            await _engine.Object.Initialise();

            _mockConsole.Verify(x => x.WriteLine(Message.InsertFile), Times.Once);
            _mockConsole.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Exactly(3));
        }

        [TestMethod]
        public async Task Initialise_ValidInput_ProcessFile()
        {
            _engine = new Mock<Engine>(_mockConsole.Object, _mockFileWrapper.Object, _mockAccountment.Object, _mockValidator.Object, _mockSettings.Object);
            _mockConsole.Setup(x => x.ReadLine()).Returns("something");
            _mockConsole.Setup(x => x.ReadKey()).Returns(new ConsoleKeyInfo(It.IsAny<char>(), ConsoleKey.Escape, false, false, false));
            _mockFileWrapper.Setup(x => x.ReadFileParallel(It.IsAny<string>(), It.IsAny<Encoding>(), It.IsAny<int>())).Returns(lines);
            _engine.Setup(x => x.ProcessLines(lines));

            await _engine.Object.Initialise();

            _mockConsole.Verify(x => x.WriteLine(Message.InsertFile), Times.Once);
            _mockConsole.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Exactly(3));
        }

    }
}
