using Newtonsoft.Json;

namespace TalkDeskProject.Configuration
{
    [JsonObject("Config")]
    public class ConfigurationSettings
    {
        public char Delimiter { get; set; }
        public char PhoneIndicator { get; set; }
        public int TotalDelimiters { get; set; }
        public int QuantityPhoneNumbers { get; set; }
        public int MoreExpensiveMinutes { get; set; }
        public decimal CostBeforeFiveMinutes { get; set; }
        public decimal CostAfterFiveMinutes { get; set; }
    }
}
