using System.Text.Json.Serialization;

namespace Sniffer.Grep
{
    [JsonSerializable(typeof(GrepOptions))]
    public class GrepOptions
    {
        public static GrepOptions Default()
        {
            return new GrepOptions
            {
            };
        }
    }
}
