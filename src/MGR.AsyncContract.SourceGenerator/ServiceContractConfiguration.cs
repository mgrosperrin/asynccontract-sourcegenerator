using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class ServiceContractConfiguration
    {
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ServiceContractAttributeConfiguration ServiceContractAttribute { get; }

        public ImmutableArray<AttributeConfiguration> OthersAttributes { get; }
        public InterfaceConfiguration Interface { get; }
        public ServiceContractConfiguration(InterfaceDeclarationSyntax interfaceDeclarationSyntax, Compilation compilation, INamedTypeSymbol serviceContractAttributeSymbol)
        {
            var model = compilation.GetSemanticModel(interfaceDeclarationSyntax.SyntaxTree);
            var source = ModelExtensions.GetDeclaredSymbol(model, interfaceDeclarationSyntax);

            if (source is not ITypeSymbol interfaceSymbol)
            {
                throw new ArgumentException();
            }
            var allAttributes = source.GetAttributes();

            var serviceContractAttribute = allAttributes.SingleOrDefault(
                x => x.AttributeClass!.Equals(serviceContractAttributeSymbol, SymbolEqualityComparer.Default));

            if (serviceContractAttribute is null)
            {
                throw new ArgumentException();
            }

            ServiceContractAttribute = new ServiceContractAttributeConfiguration(serviceContractAttribute);
            OthersAttributes = ImmutableArray.Create<AttributeConfiguration>();
            Interface = new InterfaceConfiguration();
        }
    }
}
