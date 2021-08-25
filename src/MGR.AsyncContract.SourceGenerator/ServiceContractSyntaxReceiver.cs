using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class ServiceContractSyntaxReceiver : ISyntaxReceiver
    {
        public List<(SyntaxNode Origin, InterfaceDeclarationSyntax Declaration)> Targets { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InterfaceDeclarationSyntax interfaceDeclarationSyntax)
            {
                if (interfaceDeclarationSyntax.AttributeLists.Count > 0
                    && interfaceDeclarationSyntax.AttributeLists
                        .SelectMany(attrList => attrList.Attributes)
                        .Select(attr => attr.Name.ToString())
                        .Any(attrName =>
                            attrName == Constants.ServiceContract
                            || attrName == Constants.FullyQualifiedServiceContract
                            || attrName == Constants.FullyQualifiedServiceContractAttribute
                            || attrName == Constants.ServiceContractAttribute
                            ))
                {
                    Targets.Add((syntaxNode, interfaceDeclarationSyntax));
                }
            }
        }
    }
}
