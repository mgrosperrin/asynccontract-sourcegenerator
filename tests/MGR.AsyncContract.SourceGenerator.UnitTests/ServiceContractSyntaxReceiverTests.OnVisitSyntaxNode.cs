using Microsoft.CodeAnalysis.CSharp;
using System.Threading.Tasks;
using Xunit;

namespace MGR.AsyncContract.SourceGenerator.UnitTests
{
    public partial class ServiceContractSyntaxReceiverTests
    {
        public class OnVisitSyntaxNode
        {
            [Theory]
            [MemberData(nameof(Data.FindAttributedServiceContracts), MemberType = typeof(Data))]
            public async Task Should_Find_ServiceContract_With_All_Attributes_Syntaxes(string code)
            {
                var rootSyntaxNode = await SyntaxFactory.ParseSyntaxTree(code)
                           .GetRootAsync()
                           .ConfigureAwait(false);

                var receiver = new ServiceContractSyntaxReceiver();
                foreach (var node in rootSyntaxNode.DescendantNodes(descendIntoChildren: _ => true))
                {
                    receiver.OnVisitSyntaxNode(node);
                }

                _ = Assert.Single(receiver.Targets);
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
                    { @" [System.ServiceModel.ServiceContractAttribute(Name = ""TestService"")] public interface IService { }" }
                };
            }
        }
    }
}
