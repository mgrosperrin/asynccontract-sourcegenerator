using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class MethodGenerator
    {
        private readonly Compilation _compilation;
        private readonly AttributeGenerator _attributeGenerator;

        public MethodGenerator(Compilation compilation, AttributeGenerator attributeGenerator)
        {
            _compilation = compilation;
            _attributeGenerator = attributeGenerator;
        }
        public string Generate(SemanticModel semanticModel, MethodDeclarationSyntax methodDeclaration, ServiceContractInformation serviceContractInformation)
        {
            if (semanticModel.GetDeclaredSymbol(methodDeclaration) is not IMethodSymbol methodSymbol)
            {
                return "";
            }
            var methodBuilder = new StringBuilder();
            foreach (var attribute in methodSymbol.GetAttributes())
            {
                methodBuilder.AppendLine(_attributeGenerator.GenerateMethodAttribute(attribute, serviceContractInformation, methodSymbol.Name));
            }

            return methodBuilder.ToString();
        }
    }
}
