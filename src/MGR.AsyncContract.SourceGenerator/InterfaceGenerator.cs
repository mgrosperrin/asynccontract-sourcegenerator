using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class InterfaceGenerator
    {
        private readonly Compilation _compilation;
        private readonly AttributeGenerator _attributeGenerator;

        public InterfaceGenerator(Compilation compilation)
        {
            _compilation = compilation;
            _attributeGenerator = new();
        }

        internal GenerationResult Generate(InterfaceDeclarationSyntax originalInterfaceDeclaration)
        {
            var model = _compilation.GetSemanticModel(originalInterfaceDeclaration.SyntaxTree);
            var interfaceSource = model.GetDeclaredSymbol(originalInterfaceDeclaration) as ITypeSymbol;
            if (interfaceSource is null)
            {
                return new GenerationResult(Guid.NewGuid().ToString() + ".g.cs", string.Empty);
            }
            var originalInterfaceName = interfaceSource.Name;
            var sourceCodeBuilder = GenerateNamespace(originalInterfaceDeclaration, interfaceSource);
            var namespaceSymbol = interfaceSource.ContainingNamespace;
            var namespacePrefix = namespaceSymbol.IsGlobalNamespace ? "" : namespaceSymbol.ToDisplayString() + ".";
            return new GenerationResult(namespacePrefix + originalInterfaceName + "Async.g.cs", sourceCodeBuilder.Build());
        }

        private CodeBuilder GenerateNamespace(InterfaceDeclarationSyntax originalInterfaceDeclaration, ITypeSymbol interfaceTypeSymbol)
        {
            var sourceCodeBuilder = new CodeBuilder();
            using var namespaceBlock = sourceCodeBuilder.StartNamespace(interfaceTypeSymbol.ContainingNamespace);
            GenerateAttributes(sourceCodeBuilder, interfaceTypeSymbol, originalInterfaceDeclaration);
            return GenerateTypeDeclaration(sourceCodeBuilder, originalInterfaceDeclaration, interfaceTypeSymbol);
        }

        private void GenerateAttributes(CodeBuilder sourceCodeBuilder, ITypeSymbol interfaceTypeSymbol, InterfaceDeclarationSyntax originalInterfaceDeclaration)
        {
            sourceCodeBuilder.AppendLine($@"[System.CodeDom.Compiler.GeneratedCode(""AsyncContractSourceGenerator"", ""{ typeof(AsyncContractSourceGenerator).Assembly.GetName().Version }"")]");
            var attributesData = interfaceTypeSymbol.GetAttributes();
            var attrData = attributesData.First();
            var allAttributes = originalInterfaceDeclaration.AttributeLists.SelectMany(attributes => attributes.Attributes);
            foreach (var attributeData in attributesData)
            {
                sourceCodeBuilder.AppendLine(_attributeGenerator.Generate(attributeData));
            }
        }

        private CodeBuilder GenerateTypeDeclaration(CodeBuilder sourceCodeBuilder, InterfaceDeclarationSyntax originalInterfaceDeclaration, ITypeSymbol interfaceTypeSymbol)
        {
            using var typeDeclarationBlock = sourceCodeBuilder.StartBlock(@$"public interface {interfaceTypeSymbol.Name}Async");
            return sourceCodeBuilder;
        }
    }
}
