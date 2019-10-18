using Xunit;

namespace VenimusAPIs.Tests.Infrastucture
{
    public abstract class BaseTest : IClassFixture<Fixture>
    {
        protected Fixture Fixture { get; }

        protected Data Data { get; }

        public BaseTest(Fixture fixture)
        {
            Fixture = fixture;
            Data = new Data();
        }
    }
}
