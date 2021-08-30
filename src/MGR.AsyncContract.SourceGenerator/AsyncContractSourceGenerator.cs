using Microsoft.CodeAnalysis;

namespace MGR.AsyncContract.SourceGenerator
{
    [Generator]
    public class AsyncContractSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not ServiceContractSyntaxReceiver receiver)
            {
                return;
            }
            if (AsyncContractGenerator.TryCreate(context.Compilation, out var asyncContractGenerator))
            {
                foreach (var interfaceDeclaration in receiver.Targets)
                {
                    var generationResult = asyncContractGenerator.Generate(interfaceDeclaration);
                    if (generationResult.Source is not null)
                    {
                        context.AddSource(generationResult.Source.FileName, generationResult.Source.SourceCode);
                    }
                    foreach (var generationDiagnostics in generationResult.Diagnostics)
                    {
                        context.ReportDiagnostic(generationDiagnostics);
                    }
                }
            }
        }
        public void Initialize(GeneratorInitializationContext context) => context.RegisterForSyntaxNotifications(() => new ServiceContractSyntaxReceiver());
    }
}
