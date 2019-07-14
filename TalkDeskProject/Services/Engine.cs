using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalkDeskProject.Constants;
using TalkDeskProject.Extensions;
using TalkDeskProject.Interfaces;
using TalkDeskProject.Settings;
using TalkDeskProject.Validators;

namespace TalkDeskProject.Services
{
    public class Engine : AbstractEngine
    {
        private IValidator _validator;
        private IAccountment _accountment;
        private IConsoleWrapper _console;
        private IFileWrapper _fileWrapper;
        private Configuration _configuration;

        public Engine(IConsoleWrapper console,
            IFileWrapper fileWrapper,
            IAccountment accountment,
            IValidator validator,
            IOptions<Configuration> configuration) : base(console,
             fileWrapper)
        {
            _validator = validator;
            _configuration = configuration.Value;
            _accountment = accountment;
            _console = console;
            _fileWrapper = fileWrapper;
        }

        public async Task Initialise(string input = null)
        {
            await base.Initialise(input);
        }

        public override async Task ProcessLines(List<string> lines)
        {
            Dictionary<string, double> totalCallTimePerNumber = new Dictionary<string, double>();

            int processedLines = 0;
            int errorLines = 0;

            foreach (var item in lines)
            {
                if (!_validator.ValidateFormats(item))
                {
                    _console.WriteLine(Message.BadFormat.Replace("{item}", item));
                    errorLines++;
                    continue;
                }

                var lineItems = item.Split(_configuration.Delimiter);

                var time = _accountment.CalculateTime(lineItems);

                totalCallTimePerNumber.UpdateValue(time.callFrom, time.totalTime);
                processedLines++;
            }

            var totalAmountPerNumber = _accountment.CalculateAmount(totalCallTimePerNumber);

            _console.WriteLine($"Total of {processedLines} lines processed with success.\n");
            _console.WriteLine($"Total of {errorLines} lines not processed.\n");

            if (processedLines > 0)
            {
                var finalResult = _accountment.CalculateFinalResult(totalAmountPerNumber, totalCallTimePerNumber);

                _console.WriteLine($"Phone with bigger amount cost: {finalResult.phoneWithBiggerAmount.Key} with amount {finalResult.phoneWithBiggerAmount.Value}.\n");
                _console.WriteLine($"Phone with highest call duration (will not be charged): {finalResult.phoneWithHighestCall.Key} with {finalResult.phoneWithHighestCall.Value} seconds.\n");

                _console.WriteLine($"Total: {finalResult.finalAnswer}\n");
            }

        }
    }
}
