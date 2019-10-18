using Xunit;

namespace VenimusAPIs.Tests.Infrastucture
{
    public abstract class BaseTest : IClassFixture<Fixture>
    {
        protected Fixture Fixture { get; }

        public BaseTest(Fixture fixture)
        {
            Fixture = fixture;
        }
    }
}
