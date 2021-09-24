using Microsoft.CodeAnalysis;

namespace MGR.AsyncContract.SourceGenerator
{
    [Generator]
    public class AsyncContractSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) => context.RegisterForSyntaxNotifications(() => new ServiceContractSyntaxReceiver());
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not ServiceContractSyntaxReceiver receiver)
            {
                return;
            }
            var interfaceGenerator = new InterfaceGenerator(context.Compilation);
            foreach (var originalInterfaceDeclaration in receiver.Targets)
            {
                var generationResult = interfaceGenerator.Generate(originalInterfaceDeclaration);

                context.AddSource(generationResult.TargetName, generationResult.SourceCode);
            }
        }
    }
}
