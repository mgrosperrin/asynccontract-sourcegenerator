namespace MGR.AsyncContract.SourceGenerator
{
    internal static class Constants
    {
        private const string Namespace = "System.ServiceModel";

        public const string ServiceContract = "ServiceContract";
        public const string FullyQualifiedServiceContract = Namespace + "." + ServiceContract;
        public const string ServiceContractAttribute = ServiceContract + "Attribute";
        public const string FullyQualifiedServiceContractAttribute = Namespace + "." + ServiceContractAttribute;

        public const string OperationContract = "OperationContract";
        public const string FullyQualifiedOperationContract = Namespace + "." + OperationContract;
        public const string OperationContractAttribute = OperationContract + "Attribute";
        public const string FullyQualifiedOperationContractAttribute = Namespace + "." + OperationContractAttribute;
    }
}