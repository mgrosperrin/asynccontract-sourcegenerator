using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class AttributesGenerator
    {
        private readonly Compilation _compilation;
        private readonly INamedTypeSymbol _generatedCodeAttributeSymbol;

        public AttributesGenerator(Compilation compilation, INamedTypeSymbol generatedCodeAttributeSymbol)
        {
            _compilation = compilation;
            _generatedCodeAttributeSymbol = generatedCodeAttributeSymbol;
        }
        public (string SourceCode, IEnumerable<Diagnostic> Diagnostics) Generate(ImmutableArray<AttributeData> attributesData)
        {
            var attributeText = new StringBuilder();
            foreach (var attributeData in attributesData)
            {
                attributeText.Append("[");
                attributeText.Append(attributeData);
                attributeText.AppendLine("]");
            }

            return (attributeText.ToString(), Enumerable.Empty<Diagnostic>());
        }
    }
}