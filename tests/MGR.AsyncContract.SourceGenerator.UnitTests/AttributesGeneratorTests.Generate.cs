using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace MGR.AsyncContract.SourceGenerator.UnitTests
{
    public partial class AttributesGeneratorTests
    {
        public class Generate
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
                    .Concat(new[]
                        { MetadataReference.CreateFromFile(typeof(ServiceContractAttribute).Assembly.Location) });

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
                Assert.NotNull(serviceContractNamedSymbol);
                var configuration =
                    new ServiceContractConfiguration(receiver.Targets.First(), compilation, serviceContractNamedSymbol);
                var attributesGenerator = configuration.AttributesGenerator;

                var actual = attributesGenerator.Generate();
                Assert.Equal(@"[System.ServiceModel.ServiceContractAttribute(Name = ""Test"")]
[GeneratedCode(""RR"")]", actual);
            }
        }
    }
}
