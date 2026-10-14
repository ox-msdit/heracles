using System.Net;

namespace Heracles.UnitTests
{

    public class HeraclesGetTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesGetTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task GetWithInvalidCredentialsThrowsHttpRequestException()
        {
            //Act
            Task Result() => Fixture.InvalidHydraClient.Get(Fixture.DummyRecord);

            //Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(Result);
            Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
        }

        [Fact]
        public async Task GetWithNonExistantRecordThrowsHttpRequestException()
        {
            //Act
            Task Result() => Fixture.ValidHydraClient.Get(Fixture.DummyRecord);

            //Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(Result);
            Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        }

        [Fact]
        public async Task GetWithValidRecordNoIdThrowsArgumentNullException()
        {
            //Act
            Task Result() => Fixture.ValidHydraClient.Get(Fixture.DummyRecordNoId);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(Result);
        }

        [Fact]
        public async Task GetWithValidRecordReturnsCopy()
        {
            //Arrange
            List<Heracles.Lib.Record> results = await Fixture.ValidHydraClient.Search("", 1);

            //Act
            Heracles.Lib.Record result = await Fixture.ValidHydraClient.Get(results.First());

            //Assert
            Assert.Equal(results.First().Id, result.Id);
        }

    }

}