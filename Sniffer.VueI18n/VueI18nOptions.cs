using System.Text.Json.Serialization;

namespace Sniffer.VueI18n
{
    [JsonSerializable(typeof(VueI18NOptions))]
    public class VueI18NOptions
    {
        public static VueI18NOptions Default()
        {
            return new VueI18NOptions
            {
            };
        }
    }
}
