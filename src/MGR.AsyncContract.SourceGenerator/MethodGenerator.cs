using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
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
                return string.Empty;
            }
            var hasOperationContract = false;
            var methodBuilder = new StringBuilder();
            foreach (var attribute in methodSymbol.GetAttributes())
            {
                if (_attributeGenerator.IsOperationContractAttribute(attribute))
                {
                    hasOperationContract = true;
                    methodBuilder.AppendLine(_attributeGenerator.GenerateOperationContractAttribute(attribute, serviceContractInformation, methodSymbol.Name));
                }
                else
                {
                    methodBuilder.AppendLine(_attributeGenerator.GenerateAttribute(attribute));

                }
            }
            if (!hasOperationContract)
            {
                return string.Empty;
            }
            methodBuilder.AppendFormat("System.Threading.Tasks.Task{0} {1}Async({2});",
                methodSymbol.ReturnsVoid ? "" : $"<{ methodSymbol.ReturnType.ToDisplayString() }>",
                methodSymbol.Name,
                string.Join(", ", methodSymbol.Parameters.Select(parameter => parameter.ToDisplayString() + " " + parameter.Name)));
            return methodBuilder.ToString();
        }
    }
}
