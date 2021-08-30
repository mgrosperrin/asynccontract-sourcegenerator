﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;
using System.ServiceModel;
using Xunit;

namespace MGR.AsyncContract.SourceGenerator.UnitTests
{
    public partial class ServiceContractConfigurationTests
    {
        [Fact]
        public void CreateForStruct()
        {
            var code = @"
using System.ServiceModel;

[ServiceContract(Name = ""Test""), GeneratedCode(""RR"")]
public interface ITestService {}";
            var sourceSyntaxTree = CSharpSyntaxTree.ParseText(code);
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
                .Select(_ => MetadataReference.CreateFromFile(_.Location))
                .Concat(new[] { MetadataReference.CreateFromFile(typeof(ServiceContractAttribute).Assembly.Location) });

            var compilation = CSharpCompilation.Create(
                "generator",
                new[] { sourceSyntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var receiver = new ServiceContractSyntaxReceiver();

            foreach (var node in sourceSyntaxTree.GetRoot().DescendantNodes(descendIntoChildren: _ => true))
            {
                receiver.OnVisitSyntaxNode(node);
            }

            var serviceContractNamedSymbol =
                compilation.GetTypeByMetadataName(Constants.FullyQualifiedServiceContractAttribute);
            var configuration =
                new ServiceContractGenerator(receiver.Targets.First(), compilation, serviceContractNamedSymbol);
            //var information = new ServiceContractConfiguration(null, compilation, null);
            //    StronglyTypedIdInformation.Create(receiver, compilation);
            //var information = GetInformation(code);

            //var kvp = Assert.Single(information.Ids);
            //Assert.Equal("TestId", kvp.Key.Name);
            //Assert.Equal(DefaultAttribute.Converters, kvp.Value.Configuration.Converters);
            //Assert.Equal(DefaultAttribute.BackingType, kvp.Value.Configuration.BackingType);
        }
    }
}