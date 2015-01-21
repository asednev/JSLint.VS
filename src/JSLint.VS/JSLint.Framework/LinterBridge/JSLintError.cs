
namespace JSLint.Framework.LinterBridge
{
    public sealed class JSLintError
    {
        public string Message { get; internal set; }

        public int Line { get; internal set; }

        public int Column { get; internal set; }

        public string Evidence { get; internal set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}: {2}  {3}", Line, Column, Message, Evidence);
        }
    }
}
