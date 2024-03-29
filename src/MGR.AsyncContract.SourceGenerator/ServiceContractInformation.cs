﻿using Microsoft.CodeAnalysis;

namespace MGR.AsyncContract.SourceGenerator
{
    public class ServiceContractInformation
    {
        public ServiceContractInformation(string rootNamespace, string serviceName)
        {
            RootNamespace = rootNamespace;
            ServiceName = serviceName;
        }

        public string RootNamespace { get; }
        public string ServiceName { get; }

        public string GenerateActionValueForMethod(string methodName) => $@"{RootNamespace}/{ServiceName}/{methodName}";

        public static ServiceContractInformation Parse(AttributeData serviceContractAttribute, string interfaceName)
        {
            string rootNamespace = "http://tempuri.org";
            string serviceName = interfaceName;
            foreach (var argument in serviceContractAttribute.NamedArguments)
            {
                if (argument.Key == "Namespace")
                {
                    rootNamespace = argument.Value.GetValue().Trim('"');
                }
                if (argument.Key == "Name")
                {
                    serviceName = argument.Value.GetValue().Trim('"');
                }
            }
            return new ServiceContractInformation(rootNamespace, serviceName);
        }
    }
}
