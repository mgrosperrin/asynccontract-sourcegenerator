using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.ServiceModel;
using Xunit;

namespace MGR.AsyncContract.SourceGenerator.UnitTests
{
    public partial class MethodGeneratorTests
    {
        public class Generate
        {
            private const string MethodName = "GetValue";
            [Fact]
            public void Should_Not_Generate_Method_Without_OperationContract()
            {
                var interfaceCode = $@"using System.ServiceModel;
[ServiceContract]
public interface ITest
{{
    int { MethodName }();
}}
";
                var expected = "";
                 var (sut, semanticModel, methodDeclaration) = CreateMethodGeneratorAndMethodDeclarationFromSourceCode(interfaceCode);

                var actual = sut.Generate(semanticModel, methodDeclaration, new ServiceContractInformation("http://tempuri.org", "ITest"));

                Assert.Equal(expected, actual);
            }
            [Fact]
            public void Should_Generate_Method_With_OperationContract_And_Void_NoParameter()
            {
                var interfaceCode = $@"using System.ServiceModel;
[ServiceContract]
public interface ITest
{{
    [OperationContract()]
    void { MethodName }();
}}
";
                var expected = $@"[System.ServiceModel.OperationContractAttribute(Action = ""http://tempuri.org/ITest/{ MethodName }"")]
System.Threading.Tasks.Task { MethodName }Async();";
                var (sut, semanticModel, methodDeclaration) = CreateMethodGeneratorAndMethodDeclarationFromSourceCode(interfaceCode);

                var actual = sut.Generate(semanticModel, methodDeclaration, new ServiceContractInformation("http://tempuri.org", "ITest"));

                Assert.Equal(expected, actual);
            }
            [Fact]
            public void Should_Generate_Method_With_OperationContract_And_Void_Parameter()
            {
                var interfaceCode = $@"using System.ServiceModel;
[ServiceContract]
public interface ITest
{{
    [OperationContract()]
    void { MethodName }(int param1);
}}
";
                var expected = $@"[System.ServiceModel.OperationContractAttribute(Action = ""http://tempuri.org/ITest/{ MethodName }"")]
System.Threading.Tasks.Task { MethodName }Async(int param1);";
                var (sut, semanticModel, methodDeclaration) = CreateMethodGeneratorAndMethodDeclarationFromSourceCode(interfaceCode);

                var actual = sut.Generate(semanticModel, methodDeclaration, new ServiceContractInformation("http://tempuri.org", "ITest"));

                Assert.Equal(expected, actual);
            }
            [Fact]
            public void Should_Generate_Method_With_OperationContract_And_Void_Multiple_Parameters()
            {
                var interfaceCode = $@"using System.ServiceModel;
using System.Collections.Generic;
[ServiceContract]
public interface ITest
{{
    [OperationContract()]
    void { MethodName }(int param1, List<string> param2);
}}
";
                var expected = $@"[System.ServiceModel.OperationContractAttribute(Action = ""http://tempuri.org/ITest/{ MethodName }"")]
System.Threading.Tasks.Task { MethodName }Async(int param1, System.Collections.Generic.List<string> param2);";
                var (sut, semanticModel, methodDeclaration) = CreateMethodGeneratorAndMethodDeclarationFromSourceCode(interfaceCode);

                var actual = sut.Generate(semanticModel, methodDeclaration, new ServiceContractInformation("http://tempuri.org", "ITest"));

                Assert.Equal(expected, actual);
            }
            [Fact]
            public void Should_Generate_Method_With_OperationContract_And_ReturnType_NoParameter()
            {
                var interfaceCode = $@"using System.ServiceModel;
[ServiceContract]
public interface ITest
{{
    [OperationContract()]
    int { MethodName }();
}}
";
                var expected = $@"[System.ServiceModel.OperationContractAttribute(Action = ""http://tempuri.org/ITest/{ MethodName }"")]
System.Threading.Tasks.Task<int> { MethodName }Async();";
                var (sut, semanticModel, methodDeclaration) = CreateMethodGeneratorAndMethodDeclarationFromSourceCode(interfaceCode);

                var actual = sut.Generate(semanticModel, methodDeclaration, new ServiceContractInformation("http://tempuri.org", "ITest"));

                Assert.Equal(expected, actual);
            }
            [Fact]
            public void Should_Generate_Method_With_OperationContract_And_ReturnType_Parameter()
            {
                var interfaceCode = $@"using System.ServiceModel;
[ServiceContract]
public interface ITest
{{
    [OperationContract()]
    int { MethodName }(int param1);
}}
";
                var expected = $@"[System.ServiceModel.OperationContractAttribute(Action = ""http://tempuri.org/ITest/{ MethodName }"")]
System.Threading.Tasks.Task<int> { MethodName }Async(int param1);";
                var (sut, semanticModel, methodDeclaration) = CreateMethodGeneratorAndMethodDeclarationFromSourceCode(interfaceCode);

                var actual = sut.Generate(semanticModel, methodDeclaration, new ServiceContractInformation("http://tempuri.org", "ITest"));

                Assert.Equal(expected, actual);
            }
            [Fact]
            public void Should_Generate_Method_With_OperationContract_And_ReturnType_Multiple_Parameters()
            {
                var interfaceCode = $@"using System.ServiceModel;
using System.Collections.Generic
using System.IO;
[ServiceContract]
public interface ITest
{{
    [OperationContract()]
    int { MethodName }(int param1, List<Stream> param2);
}}
";
                var expected = $@"[System.ServiceModel.OperationContractAttribute(Action = ""http://tempuri.org/ITest/{ MethodName }"")]
System.Threading.Tasks.Task<int> { MethodName }Async(int param1, System.Collections.Generic.List<System.IO.Stream> param2);";
                var (sut, semanticModel, methodDeclaration) = CreateMethodGeneratorAndMethodDeclarationFromSourceCode(interfaceCode);

                var actual = sut.Generate(semanticModel, methodDeclaration, new ServiceContractInformation("http://tempuri.org", "ITest"));

                Assert.Equal(expected, actual);
            }

            private (MethodGenerator, SemanticModel, MethodDeclarationSyntax) CreateMethodGeneratorAndMethodDeclarationFromSourceCode(string sourceCode)
            {
                var (compilation, sourceSyntaxTree) = CreateCompilationAndSyntaxTreeFromSourceCode(sourceCode);
                var receiver = new ServiceContractSyntaxReceiver();
                foreach (var node in sourceSyntaxTree.GetRoot().DescendantNodes(descendIntoChildren: _ => true))
                {
                    receiver.OnVisitSyntaxNode(node);
                }
                var interfaceDeclaration = receiver.Targets.First();
                var methodDeclaration = interfaceDeclaration.Members.OfType<MethodDeclarationSyntax>()
                    .First(method => method.Identifier.ValueText == MethodName);
                var methodGenerator = new MethodGenerator(compilation, new(compilation));

                var semanticModel = compilation.GetSemanticModel(interfaceDeclaration.SyntaxTree);

                return (methodGenerator, semanticModel, methodDeclaration);
            }
            private (Compilation, SyntaxTree) CreateCompilationAndSyntaxTreeFromSourceCode(string sourceCode)
            {
                var sourceSyntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
                var references = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
                    .Select(_ => MetadataReference.CreateFromFile(_.Location))
                    .Concat(new[] { MetadataReference.CreateFromFile(typeof(ServiceContractAttribute).Assembly.Location) });

                var compilation = CSharpCompilation.Create(
                    "generator",
                    new[] { sourceSyntaxTree },
                    references,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
                return (compilation, sourceSyntaxTree);
            }
        }
    }
}
