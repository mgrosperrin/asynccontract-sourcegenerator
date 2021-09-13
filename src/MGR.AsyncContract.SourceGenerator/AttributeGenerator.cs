using Microsoft.CodeAnalysis;
using System.Linq;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class AttributeGenerator
    {
        public string Generate(AttributeData attributeData)
        {
            var attributeClass = attributeData.AttributeClass;
            if (attributeClass is null)
            {
                return string.Empty;
            }
            var fullName = attributeClass.ContainingNamespace.GetNamespaceAsPrefix() + attributeClass.Name;
            var namedParameters = string.Join(", ",
                attributeData.NamedArguments.Select(namedArgument => namedArgument.Key + " = " + namedArgument.Value.GetValue())
                );
            var constructorParameters = namedParameters;

            return $"[{fullName}({constructorParameters})]";
        }
    }
}
