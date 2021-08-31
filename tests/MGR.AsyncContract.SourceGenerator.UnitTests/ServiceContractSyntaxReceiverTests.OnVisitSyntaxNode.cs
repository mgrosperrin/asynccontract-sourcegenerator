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
            [Fact]
            public async Task Should_Not_Find_ServiceContract_If_Attribute_Is_Missing()
            {
                var rootSyntaxNode = await SyntaxFactory.ParseSyntaxTree(" public interface IService { }")
                           .GetRootAsync()
                           .ConfigureAwait(false);

                var receiver = new ServiceContractSyntaxReceiver();
                foreach (var node in rootSyntaxNode.DescendantNodes(descendIntoChildren: _ => true))
                {
                    receiver.OnVisitSyntaxNode(node);
                }

                Assert.Empty(receiver.Targets);
            }
            [Theory]
            [MemberData(nameof(Data.DontFindAttributedServiceContracts), MemberType = typeof(Data))]
            public async Task Should_Not_Find_ServiceContract_If_Type_Is_Not_Interface(string code)
            {
                var rootSyntaxNode = await SyntaxFactory.ParseSyntaxTree(code)
                           .GetRootAsync()
                           .ConfigureAwait(false);

                var receiver = new ServiceContractSyntaxReceiver();
                foreach (var node in rootSyntaxNode.DescendantNodes(descendIntoChildren: _ => true))
                {
                    receiver.OnVisitSyntaxNode(node);
                }

                Assert.Empty(receiver.Targets);
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
                public static TheoryData<string> DontFindAttributedServiceContracts { get; } = new()
                {
                    { @" [ServiceContract] public class Service { }" },
                    { @" [System.ServiceModel.ServiceContract] public class Service { }" },
                    { @" [ServiceContractAttribute] public class Service { }" },
                    { @" [System.ServiceModel.ServiceContractAttribute] public class Service { }" },
                    { @" [ServiceContract(Name = ""TestService"")] public class Service { }" },
                    { @" [System.ServiceModel.ServiceContract(Name = ""TestService"")] public class Service { }" },
                    { @" [ServiceContractAttribute(Name = ""TestService"")] public class Service { }" },
                    { @" [System.ServiceModel.ServiceContractAttribute(Name = ""TestService"")] public class Service { }" },
                    { @" [ServiceContract] public struct Service { }" },
                    { @" [System.ServiceModel.ServiceContract] public struct Service { }" },
                    { @" [ServiceContractAttribute] public struct Service { }" },
                    { @" [System.ServiceModel.ServiceContractAttribute] public struct Service { }" },
                    { @" [ServiceContract(Name = ""TestService"")] public struct Service { }" },
                    { @" [System.ServiceModel.ServiceContract(Name = ""TestService"")] public struct Service { }" },
                    { @" [ServiceContractAttribute(Name = ""TestService"")] public struct Service { }" },
                    { @" [System.ServiceModel.ServiceContractAttribute(Name = ""TestService"")] public struct Service { }" },
                };
            }
        }
    }
}