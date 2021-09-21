using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class AttributeGenerator
    {
        private readonly INamedTypeSymbol _operationContractSymbol;
        public AttributeGenerator(Compilation compilation)
        {
            _operationContractSymbol = compilation.GetTypeByMetadataName(Constants.FullyQualifiedOperationContractAttribute)
    ?? throw new InvalidOperationException("Unable to find the attribute System.ServiceModel.OperationContractAttribute");
        }
        public string GenerateAttribute(AttributeData attributeData) => Generate(attributeData, NoOp);

        public string GenerateOperationContractAttribute(AttributeData attributeData, ServiceContractInformation serviceContractInformation, string methodName)
        {
            var attributeClass = attributeData.AttributeClass;
            if (attributeClass is null)
            {
                return string.Empty;
            }
            Func<ImmutableArray<KeyValuePair<string, TypedConstant>>, IEnumerable<KeyValuePair<string, string>>> transformOperation = IsOperationContractAttribute(attributeData) ? arguments => TransformNamedAttributes(arguments, serviceContractInformation, methodName) : NoOp;

            return Generate(attributeData, transformOperation);
        }
        private string Generate(AttributeData attributeData, Func<ImmutableArray<KeyValuePair<string, TypedConstant>>, IEnumerable<KeyValuePair<string, string>>> transformOperation)
        {
            var attributeClass = attributeData.AttributeClass;
            if (attributeClass is null)
            {
                return string.Empty;
            }
            var namedArguments = transformOperation(attributeData.NamedArguments);
            var fullName = attributeClass.ContainingNamespace.GetNamespaceAsPrefix() + attributeClass.Name;
            var namedParameters = string.Join(", ",
                namedArguments.Select(namedArgument => namedArgument.Key + " = " + namedArgument.Value)
                );
            var constructorParameters = namedParameters;

            return $"[{fullName}({constructorParameters})]";
        }
        private static IEnumerable<KeyValuePair<string, string>> NoOp(ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments) => namedArguments.Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value.GetValue()));

        private IEnumerable<KeyValuePair<string, string>> TransformNamedAttributes(ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments, ServiceContractInformation serviceContractInformation, string methodName)
        {
            var addActionArgument = true;
            foreach (var argument in namedArguments)
            {
                if (argument.Key == "Action")
                {
                    addActionArgument = false;
                }
                yield return new KeyValuePair<string, string>(argument.Key, argument.Value.GetValue());
            }
            if (addActionArgument)
            {
                yield return new KeyValuePair<string, string>("Action", $@"""{serviceContractInformation.RootNamespace}/{serviceContractInformation.ServiceName}/{methodName}""");
            }
        }
        public bool IsOperationContractAttribute(AttributeData attribute) => _operationContractSymbol.Equals(attribute.AttributeClass, SymbolEqualityComparer.Default);
    }
}
