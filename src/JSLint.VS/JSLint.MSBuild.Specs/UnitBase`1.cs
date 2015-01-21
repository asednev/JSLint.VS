namespace JSLint.MSBuild.Specs
{
    public abstract class UnitBase<T> : UnitBase
        where T : class
    {
        public UnitBase()
        {
            this.Instance = this.Construct();
        }

        public T Instance { get; protected set; }

        public virtual T Construct()
        {
            return this.AutoMocker.Create<T>();
        }
    }
}
