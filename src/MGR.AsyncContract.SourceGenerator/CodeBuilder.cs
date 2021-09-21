using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class CodeBuilder
    {
        private static string[] NewLineSeparators = new[] { Environment.NewLine };
        private readonly StringBuilder _codeBuilder = new();
        private int _currentIndentation = 0;

        public CodeBuilder IncreaseIndentation()
        {
            _currentIndentation++;
            return this;
        }
        public CodeBuilder DecreaseIndentation()
        {
            _currentIndentation--;
            return this;
        }
        public IDisposable StartNamespace(INamespaceSymbol namespaceSymbol)
        {
            if (namespaceSymbol.IsGlobalNamespace)
            {
                return EmptyDisposable.Instance;
            }
            return StartBlock($"namespace {namespaceSymbol.ToDisplayString()}");
        }

        public CodeBlock StartBlock(string line)
        {
            AppendLine(line);
            AppendLine("{");
            IncreaseIndentation();
            return new(this);
        }
        public CodeBuilder AppendLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return this;
            }
            var indentation = string.Empty;
            if (_currentIndentation > 0)
            {
                indentation = new string(' ', _currentIndentation * 4);
            }
            foreach (var singleLine in line.Split(NewLineSeparators, StringSplitOptions.None))
            {
                _codeBuilder.Append(indentation)
                    .AppendLine(singleLine);
            }
            return this;
        }

        public string Build() => _codeBuilder.ToString();
    }
}
