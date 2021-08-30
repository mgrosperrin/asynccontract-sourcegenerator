using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace MGR.AsyncContract.SourceGenerator
{
    internal sealed class GenerationResult
    {
        public GenerationResult()
        {
            Diagnostics = new List<Diagnostic>();
        }
        public SourceFile? Source { get; internal set; }
        public IEnumerable<Diagnostic> Diagnostics { get; internal set; }
    }
}
