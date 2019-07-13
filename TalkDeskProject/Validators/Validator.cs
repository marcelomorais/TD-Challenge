using Microsoft.Extensions.Options;
using System;
using System.Linq;
using TalkDeskProject.Settings;

namespace TalkDeskProject.Validators
{
    public class Validator : IValidator
    {
        private Configuration _configuration;

        public Validator(IOptions<Configuration> configuration)
        {
            _configuration = configuration.Value;
        }

        public bool ValidateFormats(string item)
        {
            var lineValue = item.Split(_configuration.Delimiter);

            if (item.Count(x => x == _configuration.Delimiter) != _configuration.TotalDelimiters || item.Count(x => x == _configuration.PhoneIndicator) != _configuration.QuantityPhoneNumbers)
                return false;

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
