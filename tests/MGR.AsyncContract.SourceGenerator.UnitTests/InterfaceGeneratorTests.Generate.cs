using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.CodeDom.Compiler;
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
            [GeneratedCode("", "")]
            [MemberData(nameof(Data.FindAttributedServiceContractsWithName), MemberType = typeof(Data))]
            [MemberData(nameof(Data.FindAttributedServiceContractsWithoutName), MemberType = typeof(Data))]
            [MemberData(nameof(Data.FindAttributedServiceContractsWithNameAndSessionMode), MemberType = typeof(Data))]
            public void Should_Return_Namespaced_Interface_Name_With_Async(string interfaceCode, string orginalExpected)
            {
                var expected = @"namespace Test
{
    "
    + string.Join(Environment.NewLine + @"    ", orginalExpected.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
    + @"
}
";
                var interfaceCodeWithNamespace = $@"namespace Test
{{
    {interfaceCode}
}}
";
                var (sut, interfaceDeclaration) = CreateInterfaceGeneratorAndInterfaceDeclarationFromSourceCode(interfaceCodeWithNamespace);

                var actual = sut.Generate(interfaceDeclaration);

                Assert.NotNull(actual);
                Assert.Equal("Test.IServiceAsync.g.cs", actual.TargetName);
                Assert.Equal(expected, actual.SourceCode);
            }
            [Theory]
            [MemberData(nameof(Data.FindAttributedServiceContractsWithName), MemberType = typeof(Data))]
            [MemberData(nameof(Data.FindAttributedServiceContractsWithoutName), MemberType = typeof(Data))]
            [MemberData(nameof(Data.FindAttributedServiceContractsWithNameAndSessionMode), MemberType = typeof(Data))]
            public void Should_Return_Interface_Name_With_Async(string interfaceCode, string expected)
            {
                var (sut, interfaceDeclaration) = CreateInterfaceGeneratorAndInterfaceDeclarationFromSourceCode(interfaceCode);

                var actual = sut.Generate(interfaceDeclaration);

                Assert.NotNull(actual);
                Assert.Equal("IServiceAsync.g.cs", actual.TargetName);
                Assert.Equal(expected, actual.SourceCode);
            }

            private (InterfaceGenerator, InterfaceDeclarationSyntax) CreateInterfaceGeneratorAndInterfaceDeclarationFromSourceCode(string sourceCode)
            {
                var (compilation, sourceSyntaxTree) = CreateCompilationAndSyntaxTreeFromSourceCode(sourceCode);
                var receiver = new ServiceContractSyntaxReceiver();
                foreach (var node in sourceSyntaxTree.GetRoot().DescendantNodes(descendIntoChildren: _ => true))
                {
                    receiver.OnVisitSyntaxNode(node);
                }
                var interfaceDeclaration = receiver.Targets.First();
                var interfaceGenerator = new InterfaceGenerator(compilation);
                return (interfaceGenerator, interfaceDeclaration);
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

            public static class Data
            {
                private const string InterfaceDeclaration = @"public interface IService
{
}";
                private const string AsyncInterfaceDeclaration = @"public interface IServiceAsync
{
}";
                private static readonly string ExpectedFindAttributedServiceContractsWithName = @"[System.CodeDom.Compiler.GeneratedCode(""AsyncContractSourceGenerator"", """ + typeof(AsyncContractSourceGenerator).Assembly.GetName().Version + @""")]
[System.ServiceModel.ServiceContractAttribute(Name = ""TestService"")]
" + AsyncInterfaceDeclaration + @"
";
                private static readonly string ExpectedFindAttributedServiceContractsWithNameAndSessionMode = @"[System.CodeDom.Compiler.GeneratedCode(""AsyncContractSourceGenerator"", """ + typeof(AsyncContractSourceGenerator).Assembly.GetName().Version + @""")]
[System.ServiceModel.ServiceContractAttribute(Name = ""TestService"", SessionMode = (System.ServiceModel.SessionMode)1)]
" + AsyncInterfaceDeclaration + @"
";
                private static readonly string ExpectedFindAttributedServiceContractsWithoutName = @"[System.CodeDom.Compiler.GeneratedCode(""AsyncContractSourceGenerator"", """ + typeof(AsyncContractSourceGenerator).Assembly.GetName().Version + @""")]
[System.ServiceModel.ServiceContractAttribute()]
" + AsyncInterfaceDeclaration + @"
";

                public static TheoryData<string, string> FindAttributedServiceContractsWithName { get; } = new()
                {
                    { @"using System.ServiceModel;
[ServiceContract(Name = ""TestService"")]
" + InterfaceDeclaration + @"
", ExpectedFindAttributedServiceContractsWithName },
                    { @"[System.ServiceModel.ServiceContract(Name = ""TestService"")]
" + InterfaceDeclaration + @"
", ExpectedFindAttributedServiceContractsWithName },
                    { @"using System.ServiceModel;
[ServiceContractAttribute(Name = ""TestService"")]
" + InterfaceDeclaration + @"
", ExpectedFindAttributedServiceContractsWithName },
                    { @"[System.ServiceModel.ServiceContractAttribute(Name = ""TestService"")]
" + InterfaceDeclaration + @"
", ExpectedFindAttributedServiceContractsWithName }
                };
                public static TheoryData<string, string> FindAttributedServiceContractsWithNameAndSessionMode { get; } = new()
                {
                    { @"using System.ServiceModel;
[ServiceContract(Name = ""TestService"", SessionMode = SessionMode.Required)]
" + InterfaceDeclaration + @"
", ExpectedFindAttributedServiceContractsWithNameAndSessionMode },
                    { @"[System.ServiceModel.ServiceContract(Name = ""TestService"", SessionMode = System.ServiceModel.SessionMode.Required)]
" + InterfaceDeclaration + @"
", ExpectedFindAttributedServiceContractsWithNameAndSessionMode },
                    { @"using System.ServiceModel;
[ServiceContractAttribute(Name = ""TestService"", SessionMode = (SessionMode)1)]
" + InterfaceDeclaration + @"
", ExpectedFindAttributedServiceContractsWithNameAndSessionMode },
                    { @"[System.ServiceModel.ServiceContractAttribute(Name = ""TestService"", SessionMode = (System.ServiceModel.SessionMode)1)]
" + InterfaceDeclaration + @"
", ExpectedFindAttributedServiceContractsWithNameAndSessionMode }
                };
                public static TheoryData<string, string> FindAttributedServiceContractsWithoutName { get; } = new()
                {
                    { @"using System.ServiceModel;
[ServiceContract]
" + InterfaceDeclaration + @"
", ExpectedFindAttributedServiceContractsWithoutName },
                    { @"[System.ServiceModel.ServiceContract]
" + InterfaceDeclaration + @"
", ExpectedFindAttributedServiceContractsWithoutName },
                    { @"using System.ServiceModel;
[ServiceContractAttribute]
" + InterfaceDeclaration + @"
", ExpectedFindAttributedServiceContractsWithoutName },
                    { @"[System.ServiceModel.ServiceContractAttribute]
" + InterfaceDeclaration + @"
", ExpectedFindAttributedServiceContractsWithoutName }
                };
            }
        }
    }
}
