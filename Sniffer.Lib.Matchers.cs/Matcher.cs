namespace Sniffer.Lib.Matchers.cs
{
    public class MatcherFactory
    {
        public static IMatcher Create(string pattern)
        {
            return RegexMatcher.CreateIfValid(pattern)
                   ?? WildcardMatcher.CreateIfValid(pattern)
                   ?? new ExactMatcher(pattern);
        }


        public static IMatcher Create(IEnumerable<string> patterns)
        {
            return new AggregateMatcher(patterns.Select(Create));
        }
    }
}