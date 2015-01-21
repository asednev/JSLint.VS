namespace JSLint.MSBuild.Specs.Helpers
{
    using System.Text.RegularExpressions;
    using Xunit.Sdk;

    public static class Expect
    {
        public static void Matches(string pattern, string actual)
        {
            Matches(new Regex(pattern), actual);
        }

        public static void Matches(string pattern, RegexOptions options, string actual)
        {
            Matches(new Regex(pattern, options), actual);
        }

        public static void Matches(Regex pattern, string actual)
        {
            if (!pattern.IsMatch(actual))
            {
                throw new AssertException("Expected \"" + actual + "\" to match pattern \"" + pattern.ToString() + "\"");
            }
        }
    }
}
