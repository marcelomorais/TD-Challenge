using System;

namespace TalkDeskProject.Validators
{
    public class Validator : IValidator
    {
        public bool ValidateFormats(string[] lineValue)
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
