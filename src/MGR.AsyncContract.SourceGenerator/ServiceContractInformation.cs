using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class ServiceContractInformation
    {
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ServiceContractConfiguration Configuration { get; }

        public ServiceContractInformation() { }
    }
}
