using System;

namespace MGR.AsyncContract.SourceGenerator
{
    internal struct CodeBlock : IDisposable
    {
        private readonly CodeBuilder _codeBuilder;

        public CodeBlock(CodeBuilder codeBuilder)
        {
            _codeBuilder = codeBuilder;
        }

        public void Dispose()
        {
            _codeBuilder.DecreaseIndentation();
            _codeBuilder.AppendLine("}");
        }
    }
}
