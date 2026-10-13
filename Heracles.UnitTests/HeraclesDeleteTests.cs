namespace Heracles.UnitTests
{

    public class HeraclesDeleteTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesDeleteTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task DeleteWithInvalidCredentialsThrowsHttpRequestException()
        {
            //Act
            Task result() => Fixture.InvalidHydraClient.Delete(Fixture.DummyRecord);

            //Assert
            await Assert.ThrowsAsync<HttpRequestException>(result);
        }

        [Fact]
        public async Task DeleteWithNonExistantRecordThrowsHttpRequestException()
        {
            //Act
            Task result() => Fixture.ValidHydraClient.Delete(Fixture.DummyRecord);

            //Assert
            await Assert.ThrowsAsync<HttpRequestException>(result);
        }

        [Fact]
        public async Task DeleteWithValidRecordNoIdThrowsArgumentNullException()
        {
            //Act
            Task result() => Fixture.ValidHydraClient.Delete(Fixture.DummyRecordNoId);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(result);
        }

    }

}