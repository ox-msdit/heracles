using System.Net;

namespace Heracles.UnitTests
{

    public class HeraclesUpdateTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesUpdateTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task UpdateWithInvalidCredentialsThrowsHttpRequestException()
        {
            //Act
            Task Result() => Fixture.InvalidHydraClient.Update(Fixture.DummyRecord);

            //Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(Result);
            Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
        }

        [Fact]
        public async Task UpdateWithNonExistantRecordThrowsHttpRequestException()
        {
            //Act
            Task Result() => Fixture.ValidHydraClient.Update(Fixture.DummyRecord);

            //Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(Result);
            Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        }

        [Fact]
        public async Task UpdateWithValidRecordNoIdThrowsArgumentNullException()
        {
            //Act
            Task Result() => Fixture.ValidHydraClient.Update(Fixture.DummyRecordNoId);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(Result);
        }

    }

}