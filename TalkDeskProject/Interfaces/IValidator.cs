namespace TalkDeskProject.Validators
{
    public interface IValidator
    {
        bool ValidateFormats(string[] lineValue);
    }
}
