using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class ServiceContractAttributeConfiguration
    {
        public ImmutableArray<AttributeArgumentSyntax> Arguments { get; }

        public ServiceContractAttributeConfiguration(AttributeData serviceContractAttributeData)
        {
            //ServiceName = serviceContractAttributeData.NamedArguments
        }
    }
}
