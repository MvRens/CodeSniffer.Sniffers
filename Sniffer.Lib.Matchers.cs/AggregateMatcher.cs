namespace Sniffer.Lib.Matchers.cs
{
    internal class AggregateMatcher : IMatcher
    {
        private readonly IMatcher[] matchers;


        public AggregateMatcher(IEnumerable<IMatcher> matchers)
        {
            this.matchers = matchers.ToArray();
        }


        public bool Matches(string value)
        {
            return matchers.Any(m => m.Matches(value));
        }
    }
}
