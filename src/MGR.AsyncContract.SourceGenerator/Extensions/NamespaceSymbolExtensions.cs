namespace Microsoft.CodeAnalysis
{
    internal static class NamespaceSymbolExtensions
    {
        public static string GetNamespaceAsPrefix(this INamespaceSymbol source)
        {
            if (source.IsGlobalNamespace)
            {
                return string.Empty;
            }
            return source.ToDisplayString() + ".";
        }
    }
}
