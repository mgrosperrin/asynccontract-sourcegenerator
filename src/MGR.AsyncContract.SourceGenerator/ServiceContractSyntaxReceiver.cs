using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace MGR.AsyncContract.SourceGenerator
{
    internal sealed class ServiceContractSyntaxReceiver : ISyntaxReceiver
    {
        public List<InterfaceDeclarationSyntax> Targets { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InterfaceDeclarationSyntax interfaceDeclarationSyntax)
            {
                if (interfaceDeclarationSyntax.AttributeLists.Count > 0
                    && interfaceDeclarationSyntax.AttributeLists
                        .SelectMany(attrList => attrList.Attributes)
                        .Any(attr => attr.IsServiceContract()))
                {
                    Targets.Add(interfaceDeclarationSyntax);
                }
            }
        }
    }
}