using Kubernetes1.Configuration;

namespace Kubernetes1.Throttling
{
    public class ThrottlingMiddlewareConfiguration : ConfigurationBase
    {
        public int EventsPerSecond { get; set; }

        public int WindowSizeInSeconds { get; set; }
    }
}