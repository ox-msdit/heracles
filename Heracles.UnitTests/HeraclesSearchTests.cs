using System.Net;

namespace Heracles.UnitTests
{

    public class HeraclesSearchTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesSearchTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task SearchWithInvalidCredentialsThrowsHttpRequestException()
        {
            //Act
            Task Result() => Fixture.InvalidHydraClient.Search("_acme-challenge");

            //Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(Result);
            Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
        }

        [Fact]
        public async Task SearchWithValidSubstringReturnsValidResult()
        {
            //Act
            List<Heracles.Lib.Record> results = await Fixture.ValidHydraClient.Search("_acme-challenge");
            
            //Assert
            Assert.NotNull(results);
            Assert.NotEmpty(results);
        }

        [Fact]
        public async Task SearchWithInvalidSubstringReturnsEmptyResult()
        {
            //Act
            List<Heracles.Lib.Record> results = await Fixture.ValidHydraClient.Search("willnevermatch");

            //Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task SearchWithValidLimitReturnsValidResult(int x)
        {
            //Act
            List<Heracles.Lib.Record> results = await Fixture.ValidHydraClient.Search("_acme-challenge", x);

            //Assert
            Assert.NotNull(results);
            Assert.Equal(x, results.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(500001)]
        public async Task SearchWithInvalidLimitThrowsArgumentOutOfRangeExceptionException(int x)
        {
            //Act
            Task Result() => Fixture.InvalidHydraClient.Search("_acme-challenge", x);

            //Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(Result);
        }

    }

}