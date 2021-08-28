using Microsoft.CodeAnalysis;

namespace MGR.AsyncContract.SourceGenerator
{
    [Generator]
    public class AsyncContractSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            //context.AddSource();
        }
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ServiceContractSyntaxReceiver());
        }
    }
}
