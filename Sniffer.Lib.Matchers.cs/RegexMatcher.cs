using System.Text.RegularExpressions;

namespace Sniffer.Lib.Matchers.cs
{
    internal class RegexMatcher : IMatcher
    {
        private readonly Regex regex;


        public static IMatcher? CreateIfValid(string pattern)
        {
            return pattern.StartsWith("/") && pattern.EndsWith("/")
                ? new RegexMatcher(pattern)
                : null;
        }


        public RegexMatcher(string pattern)
        {
            regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }


        public bool Matches(string value)
        {
            return regex.IsMatch(value);
        }
    }
}
