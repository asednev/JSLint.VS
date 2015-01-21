namespace JSLint.MSBuild.Specs
{
    using System;
    using Autofac.Extras.Moq;
    using Xunit;

    [Trait("Category", "Unit")]
    public abstract class UnitBase : IDisposable
    {
        public UnitBase()
        {
            this.AutoMocker = AutoMock.GetLoose();
        }

        public AutoMock AutoMocker { get; private set; }

        public virtual void Dispose()
        {
        }
    }
}
