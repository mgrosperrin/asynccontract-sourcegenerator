using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;
using System.ServiceModel;
using Xunit;

namespace MGR.AsyncContract.SourceGenerator.UnitTests
{
    public partial class InterfaceGeneratorTests
    {
        public class Generate
        {
            [Theory]
            [MemberData(nameof(Data.FindAttributedServiceContracts), MemberType = typeof(Data))]
            public void Should_Return_Namespaced_Interface_Name_With_Async(string interfaceCode)
            {
                var expected = @"namespace Test
{
    public interface IServiceAsync
    {
    }
}
";
                var interfaceCodeWithNamespace = $@"namespace Test
{{
    {interfaceCode}
}}
";
                var sourceSyntaxTree = CSharpSyntaxTree.ParseText(interfaceCodeWithNamespace);
                var references = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
                    .Select(_ => MetadataReference.CreateFromFile(_.Location))
                    .Concat(new[] { MetadataReference.CreateFromFile(typeof(ServiceContractAttribute).Assembly.Location) });

                var compilation = CSharpCompilation.Create(
                    "generator",
                    new[] { sourceSyntaxTree },
                    references,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                var sut = new InterfaceGenerator(compilation);
                var receiver = new ServiceContractSyntaxReceiver();

                foreach (var node in sourceSyntaxTree.GetRoot().DescendantNodes(descendIntoChildren: _ => true))
                {
                    receiver.OnVisitSyntaxNode(node);
                }
                var interfaceDeclaration = receiver.Targets.First();

                var actual = sut.Generate(interfaceDeclaration);

                Assert.NotNull(actual);
                Assert.Equal("IServiceAsync.g.cs", actual.TargetName);
                Assert.Equal(expected, actual.SourceCode);
            }
            [Theory]
            [MemberData(nameof(Data.FindAttributedServiceContracts), MemberType = typeof(Data))]
            public void Should_Return_Interface_Name_With_Async(string interfaceCode)
            {
                var expected = @"public interface IServiceAsync
{
}
";
                var sourceSyntaxTree = CSharpSyntaxTree.ParseText(interfaceCode);
                var references = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
                    .Select(_ => MetadataReference.CreateFromFile(_.Location))
                    .Concat(new[] { MetadataReference.CreateFromFile(typeof(ServiceContractAttribute).Assembly.Location) });

                var compilation = CSharpCompilation.Create(
                    "generator",
                    new[] { sourceSyntaxTree },
                    references,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                var sut = new InterfaceGenerator(compilation);
                var receiver = new ServiceContractSyntaxReceiver();

                foreach (var node in sourceSyntaxTree.GetRoot().DescendantNodes(descendIntoChildren: _ => true))
                {
                    receiver.OnVisitSyntaxNode(node);
                }
                var interfaceDeclaration = receiver.Targets.First();

                var actual = sut.Generate(interfaceDeclaration);

                Assert.NotNull(actual);
                Assert.Equal("IServiceAsync.g.cs", actual.TargetName);
                Assert.Equal(expected, actual.SourceCode);
            }
            public static class Data
            {
                public static TheoryData<string> FindAttributedServiceContracts { get; } = new()
                {
                    { @" [ServiceContract] public interface IService { }" },
                    { @" [System.ServiceModel.ServiceContract] public interface IService { }" },
                    { @" [ServiceContractAttribute] public interface IService { }" },
                    { @" [System.ServiceModel.ServiceContractAttribute] public interface IService { }" },
                    { @" [ServiceContract(Name = ""TestService"")] public interface IService { }" },
                    { @" [System.ServiceModel.ServiceContract(Name = ""TestService"")] public interface IService { }" },
                    { @" [ServiceContractAttribute(Name = ""TestService"")] public interface IService { }" },
                    { @" [System.ServiceModel.ServiceContractAttribute(Name = ""TestService"")] public interface IService { }" },
                };
            }
        }
    }
}
