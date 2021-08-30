using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Text;

namespace MGR.AsyncContract.SourceGenerator
{
    internal sealed class AsyncContractGenerator
    {
        private readonly Compilation _compilation;
        private readonly INamedTypeSymbol _serviceContractAttributeSymbol;
        private readonly INamedTypeSymbol _generatedCodeAttributeSymbol;

        public AsyncContractGenerator(Compilation compilation, INamedTypeSymbol serviceContractAttributeSymbol)
        {
            _compilation = compilation;
            _serviceContractAttributeSymbol = serviceContractAttributeSymbol;
            _generatedCodeAttributeSymbol = compilation.GetTypeByMetadataName(typeof(GeneratedCodeAttribute).FullName)!;
        }
        public static bool TryCreate(Compilation compilation, out AsyncContractGenerator generator)
        {
            var serviceContractAttributeSymbol =
            compilation.GetTypeByMetadataName(Constants.FullyQualifiedServiceContractAttribute);
            if (serviceContractAttributeSymbol == null)
            {
                generator = default!;
                return false;
            }
            generator = new(compilation, serviceContractAttributeSymbol);
            return true;

        }
        public GenerationResult Generate(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            var namespacesToUse = Enumerable.Empty<string>();
            var @namespace = "";
            var attributes = Enumerable.Empty<string>();
            var interfaceName = "";
            var methods = Enumerable.Empty<string>();
            var @interface = @$"{string.Join(Environment.NewLine, namespacesToUse.Select(namespaceToUse => $"using {namespaceToUse};"))}
namespace {@namespace}
{{
    {string.Join(Environment.NewLine, attributes)}
    {interfaceName}
    {{
        {string.Join(Environment.NewLine, methods)}
    }}
}}
";
            return new GenerationResult();
        }
    }
}
