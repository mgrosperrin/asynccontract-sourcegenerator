using Microsoft.CodeAnalysis;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class ServiceContractAttributeConfiguration
    {
        public string ServiceName { get; }
        public string Namespace { get; }
        public string SessionMode { get; }

        public ServiceContractAttributeConfiguration(AttributeData serviceContractAttributeData)
        {
            //ServiceName = serviceContractAttributeData.NamedArguments
        }
    }
}
