namespace Heracles.UnitTests
{

    public class HeraclesAddTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesAddTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task AddWithInvalidCredentialsThrowsHttpRequestException()
        {
            //Act
            Task result() => Fixture.InvalidHydraClient.Add(Fixture.DummyRecordNoId);

            //Assert
            await Assert.ThrowsAsync<HttpRequestException>(result);
        }

    }

}