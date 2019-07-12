using System.Collections.Generic;

namespace TalkDeskProject.Extensions
{
    public static class DictionaryExtension
    {
        public static void UpdateAmount(this Dictionary<string, decimal> actualDictionary, string key, decimal value)
        {
            if (actualDictionary.ContainsKey(key))
            {
                var newValue = actualDictionary.GetValueOrDefault(key) + value;

                actualDictionary[key] = newValue;
            }
            else
            {
                actualDictionary.Add(key, value);
            }
        }
    }
}
