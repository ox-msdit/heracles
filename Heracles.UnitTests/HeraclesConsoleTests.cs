using System.Net;

namespace Heracles.UnitTests
{

    public class HeraclesConsoleTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesConsoleTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task LoopJsonRecordsWithRecordAndSafeModeAndInvalidClientReturnsHttpRequestExeption()
        {
            //Arrange
            string json = $"[{Lib.RecordHelpers.RecordToJson(Fixture.DummyRecord)}]";

            //Act
            Task Result() => Console.Helpers.LoopJsonRecords(json, Fixture.InvalidHydraClient.Get, true);

            //Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(Result);
            Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
        }

        [Fact]
        public async Task LoopJsonRecordsWithRecordListAndSafeModeAndInvalidClientThrowsInvalidDataException()
        {
            //Arrange
            string json = Lib.RecordHelpers.RecordListToJson(Fixture.DummyRecordList);

            //Act
            Task Result() => Console.Helpers.LoopJsonRecords(json, Fixture.InvalidHydraClient.Get, true);

            //Assert
            await Assert.ThrowsAsync<InvalidDataException>(Result);
        }

    }

    [Collection("Do Not Run In Parallel")]
    public class HeraclesConsoleSerialTests : IClassFixture<HeraclesFixture>
    {

        public HeraclesFixture Fixture { get; }

        public HeraclesConsoleSerialTests(HeraclesFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task LoopJsonRecordsWithRecordListAndNoSafeModeAndInvalidClientReturnsHttpRequestExeption()
        {
            //Arrange
            string json = Lib.RecordHelpers.RecordListToJson(Fixture.DummyRecordList);
            Environment.SetEnvironmentVariable("HYDRA_UNSAFE", "true");

            //Act
            Task Result() => Console.Helpers.LoopJsonRecords(json, Fixture.InvalidHydraClient.Get, true);

            //Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(Result);
            Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
            Environment.SetEnvironmentVariable("HYDRA_UNSAFE", null);

        }

    }
}