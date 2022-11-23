namespace Sniffer.Lib.Matchers.cs
{
    internal class ExactMatcher : IMatcher
    {
        private readonly string value;


        public ExactMatcher(string value)
        {
            this.value = value;
        }


        public bool Matches(string value)
        {
            return value.Equals(this.value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
