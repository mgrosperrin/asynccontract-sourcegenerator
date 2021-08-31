using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

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
            return new GenerationResult(originalInterfaceName + "Async.g.cs", @$"public interface {originalInterfaceName}Async
{{
}}");
        }
    }
}
