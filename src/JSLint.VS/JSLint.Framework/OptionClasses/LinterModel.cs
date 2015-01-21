namespace JSLint.Framework.OptionClasses
{
    public class LinterModel
    {
        public Linters Type { get; set; }
        public string Name { get; set; }

        public LinterModel(string name, Linters type)
        {
            Name = name;
            Type = type;
        }

        public bool HasQuotMarkOption { get; set; }

        public bool HasMaxComplexityOptions { get; set; }

        public bool HasUnusedVariableOptionBakedIn { get; set; }
    }
}
