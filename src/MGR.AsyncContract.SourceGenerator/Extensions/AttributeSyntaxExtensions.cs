using MGR.AsyncContract.SourceGenerator;

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    internal static class AttributeSyntaxExtensions
    {
        public static bool IsServiceContract(this AttributeSyntax attributeSyntax)
        {
            var attributeName = attributeSyntax.Name.ToString();
            return attributeName == Constants.ServiceContract
                || attributeName == Constants.FullyQualifiedServiceContract
                || attributeName == Constants.FullyQualifiedServiceContractAttribute
                || attributeName == Constants.ServiceContractAttribute;
        }
        public static bool IsOperationContract(this AttributeSyntax attributeSyntax)
        {
            var attributeName = attributeSyntax.Name.ToString();
            return attributeName == Constants.OperationContract
                || attributeName == Constants.FullyQualifiedOperationContract
                || attributeName == Constants.FullyQualifiedOperationContractAttribute
                || attributeName == Constants.OperationContractAttribute;
        }
    }
}