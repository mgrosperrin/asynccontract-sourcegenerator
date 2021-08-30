using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Text;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class AttributesGenerator
    {
        private readonly ImmutableArray<AttributeData> _attributesData;

        public AttributesGenerator(ImmutableArray<AttributeData> attributesData)
        {
            _attributesData = attributesData;
        }

        public string Generate()
        {
            var attributeText = new StringBuilder();
            foreach (var attributeData in _attributesData)
            {
                attributeText.Append("[");
                attributeText.Append(attributeData);
                attributeText.AppendLine("]");
            }

            return attributeText.ToString();
        }
    }
}