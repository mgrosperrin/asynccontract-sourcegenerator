using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Text;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class InterfaceGenerator
    {
        private readonly Compilation _compilation;

        public InterfaceGenerator(Compilation compilation)
        {
            _compilation = compilation;
        }

        internal GenerationResult Generate(InterfaceDeclarationSyntax originalInterfaceDeclaration)
        {
            var model = _compilation.GetSemanticModel(originalInterfaceDeclaration.SyntaxTree);
            var source = model.GetDeclaredSymbol(originalInterfaceDeclaration) as ITypeSymbol;
            if (source is null)
            {
                return new GenerationResult(Guid.NewGuid().ToString() + ".g.cs", string.Empty);
            }
            var originalInterfaceName = source.Name;
            var sourceCodeBuilder = GenerateNamespace(originalInterfaceDeclaration, source);
            return new GenerationResult(originalInterfaceName + "Async.g.cs", sourceCodeBuilder.Build()) ;
        }

        private CodeBuilder GenerateNamespace(InterfaceDeclarationSyntax originalInterfaceDeclaration, ITypeSymbol interfaceTypeSymbol)
        {
            var sourceCodeBuilder = new CodeBuilder();
            using var namespaceBlock = sourceCodeBuilder.StartNamespace(interfaceTypeSymbol.ContainingNamespace);
            return GenerateTypeDeclaration(sourceCodeBuilder, originalInterfaceDeclaration, interfaceTypeSymbol);
        }

        private CodeBuilder GenerateTypeDeclaration(CodeBuilder sourceCodeBuilder, InterfaceDeclarationSyntax originalInterfaceDeclaration, ITypeSymbol interfaceTypeSymbol)
        {
            using var typeDeclarationBlock = sourceCodeBuilder.StartBlock(@$"public interface {interfaceTypeSymbol.Name}Async");
            return sourceCodeBuilder;
        }
    }
}
