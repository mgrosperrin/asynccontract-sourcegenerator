using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class InterfaceConfiguration
    {
        public ImmutableArray<Diagnostic> Diagnostics { get; }
    }
}