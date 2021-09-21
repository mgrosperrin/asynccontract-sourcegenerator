using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class InterfaceGenerator
    {
        private readonly Compilation _compilation;
        private readonly AttributeGenerator _attributeGenerator;
        private readonly MethodGenerator _methodGenerator;
        private readonly INamedTypeSymbol _serviceContractSymbol;

        public InterfaceGenerator(Compilation compilation)
        {
            _compilation = compilation;
            _attributeGenerator = new(compilation);
            _methodGenerator = new(compilation, _attributeGenerator);
            _serviceContractSymbol = compilation.GetTypeByMetadataName(Constants.FullyQualifiedServiceContractAttribute)
    ?? throw new InvalidOperationException("Unable to find the attribute System.ServiceModel.ServiceContractAttribute");
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
            var sourceCodeBuilder = GenerateNamespace(model, originalInterfaceDeclaration, interfaceSource);
            var namespaceSymbol = interfaceSource.ContainingNamespace;
            var namespacePrefix = namespaceSymbol.IsGlobalNamespace ? "" : namespaceSymbol.ToDisplayString() + ".";
            return new GenerationResult(namespacePrefix + originalInterfaceName + "Async.g.cs", sourceCodeBuilder.Build());
        }

        private CodeBuilder GenerateNamespace(SemanticModel semanticModel, InterfaceDeclarationSyntax originalInterfaceDeclaration, ITypeSymbol interfaceTypeSymbol)
        {
            var sourceCodeBuilder = new CodeBuilder();
            using var namespaceBlock = sourceCodeBuilder.StartNamespace(interfaceTypeSymbol.ContainingNamespace);
            var interfaceAttributes = interfaceTypeSymbol.GetAttributes();
            GenerateAttributes(sourceCodeBuilder, interfaceAttributes);
            var serviceContractAttribute = interfaceAttributes.Single(attr => _serviceContractSymbol.Equals(attr.AttributeClass, SymbolEqualityComparer.Default));
            var serviceContractInformation = ServiceContractInformation.Parse(serviceContractAttribute, interfaceTypeSymbol.Name);
            return GenerateTypeDeclaration(sourceCodeBuilder, semanticModel, originalInterfaceDeclaration, interfaceTypeSymbol, serviceContractInformation);
        }

        private void GenerateAttributes(CodeBuilder sourceCodeBuilder, IEnumerable<AttributeData> interfaceAttributes)
        {
            sourceCodeBuilder.AppendLine($@"[System.CodeDom.Compiler.GeneratedCode(""AsyncContractSourceGenerator"", ""{ typeof(AsyncContractSourceGenerator).Assembly.GetName().Version }"")]");
            foreach (var attributeData in interfaceAttributes)
            {
                sourceCodeBuilder.AppendLine(_attributeGenerator.GenerateAttribute(attributeData));
            }
        }

        private CodeBuilder GenerateTypeDeclaration(CodeBuilder sourceCodeBuilder, SemanticModel semanticModel, InterfaceDeclarationSyntax originalInterfaceDeclaration, ITypeSymbol interfaceTypeSymbol, ServiceContractInformation serviceContractInformation)
        {
            using var typeDeclarationBlock = sourceCodeBuilder.StartBlock(@$"public interface {interfaceTypeSymbol.Name}Async");
            foreach (var methodDeclaration in originalInterfaceDeclaration.Members.OfType<MethodDeclarationSyntax>())
            {
                sourceCodeBuilder.AppendLine(_methodGenerator.Generate(semanticModel, methodDeclaration, serviceContractInformation));
            }
            return sourceCodeBuilder;
        }
    }
}
