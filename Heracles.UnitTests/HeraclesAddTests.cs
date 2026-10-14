using System.Net;
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
            Task Result() => Fixture.InvalidHydraClient.Add(Fixture.DummyRecordNoId);

            //Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(Result);
            Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
        }

    }

}