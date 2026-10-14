using System.Net;

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
            Task Result() => Fixture.InvalidHydraClient.Delete(Fixture.DummyRecord);

            //Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(Result);
            Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
        }

        [Fact]
        public async Task DeleteWithNonExistantRecordThrowsHttpRequestException()
        {
            //Act
            Task Result() => Fixture.ValidHydraClient.Delete(Fixture.DummyRecord);

            //Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(Result);
            Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        }

        [Fact]
        public async Task DeleteWithValidRecordNoIdThrowsArgumentNullException()
        {
            //Act
            Task Result() => Fixture.ValidHydraClient.Delete(Fixture.DummyRecordNoId);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(Result);
        }

    }

}