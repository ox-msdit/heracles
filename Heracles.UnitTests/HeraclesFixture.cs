using Heracles.Lib;

namespace Heracles.UnitTests
{
    public class HeraclesFixture
    {
        public readonly string? Cihost;
        public HydraClient ValidHydraClient { get; }
        public HydraClient InvalidHydraClient { get; }
        public Lib.Record DummyRecord = new()
        {
            Hostname = "notarealhostname",
            Content = "test",
            Type = "TXT",
            Id = Guid.NewGuid()
        };
        public Lib.Record DummyRecordNoId = new()
        {
            Hostname = "notarealhostname",
            Content = "test",
            Type = "TXT"
        };
        public List<Lib.Record> DummyRecordList = 
        [
            new () {Hostname = "foo", Content = "foo", Type = "TXT", Id = Guid.NewGuid()},
            new () {Hostname = "bar", Content = "bar", Type = "TXT", Id = Guid.NewGuid()},
        ];

        public HeraclesFixture()
        {
            var credentials = Environment.GetEnvironmentVariable("HYDRA_TOKEN");
            ArgumentNullException.ThrowIfNull(credentials);
            var uri = Environment.GetEnvironmentVariable("HYDRA_URI");
            ArgumentNullException.ThrowIfNull(uri);
            Cihost = Environment.GetEnvironmentVariable("HYDRA_CI_HOST");
            ArgumentNullException.ThrowIfNull(Cihost);
            ValidHydraClient = new(uri, credentials);
            InvalidHydraClient = new(uri, "notvalidcredentials");
            // Pace integration tests to accommodate Hydra API connection limitations.
            Thread.Sleep(Random.Shared.Next(0,5_000));
        }

    }

}
