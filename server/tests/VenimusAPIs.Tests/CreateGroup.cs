using TestStack.BDDfy;
using Xunit;

namespace VenimusAPIs.Tests
{
    [Story(AsA = "SystemAdministrator", IWant = "To be able to create new groups", SoThat = "People can build communities")]
    public class CreateGroup
    {
        [Fact]
        public void Execute()
        {
            this.BDDfy();
        }

        private void GivenIAmASystemAdministrator()
        {
        }

        private void WhenICallTheCreateGroupApi()
        {
        }

        private void ThenANewGroupIsAddedToTheDatabase()
        {
            Assert.False(true);
        }

        private void ThenTheLocationOfTheNewGroupIsReturned()
        {
            Assert.False(true);
        }
    }
}
