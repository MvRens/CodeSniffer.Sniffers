using System.Text.RegularExpressions;

namespace Sniffer.Lib.Matchers.cs
{
    internal class WildcardMatcher : IMatcher
    {
        private readonly Regex regex;


        public static IMatcher? CreateIfValid(string pattern)
        {
            return pattern.Contains('*') || pattern.Contains('?')
                ? new WildcardMatcher(pattern)
                : null;
        }


        public WildcardMatcher(string pattern)
        {
            regex = new Regex("^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*") + "$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }


        public bool Matches(string value)
        {
            return regex.IsMatch(value);
        }
    }
}
