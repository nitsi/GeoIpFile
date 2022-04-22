using Kubernetes1.Configuration;

namespace Kubernetes1.Infra
{
    public class WebApiExceptionFilterConfiguration : ConfigurationBase
    {
        public bool ShowInternalExceptionForDevelopment { get; set; }
    }
}