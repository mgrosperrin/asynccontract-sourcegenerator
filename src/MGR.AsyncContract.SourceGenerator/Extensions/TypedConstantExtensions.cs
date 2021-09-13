namespace Microsoft.CodeAnalysis
{
    internal static class TypedConstantExtensions
    {
        public static string GetValue(this TypedConstant source)
        {
            if (source.IsNull)
            {
                return "null";
            }
            switch (source.Kind)
            {
                case TypedConstantKind.Primitive:
                    return ConvertFromPrimitive(source);
                case TypedConstantKind.Enum:
                    return ConvertFromEnum(source);
                case TypedConstantKind.Error:
                case TypedConstantKind.Type:
                case TypedConstantKind.Array:
                default:
                    return source.Value!.ToString();
            }
        }

        private static string ConvertFromEnum(TypedConstant source) => "(" + source.Type!.ContainingNamespace.GetNamespaceAsPrefix() + source.Type.Name + ")" + source.Value;

        private static string ConvertFromPrimitive(TypedConstant source)
        {
            switch (source.Type!.SpecialType)
            {
                case SpecialType.System_String:
                    return "\"" + source.Value + "\"";
                default:
                    return source.Value!.ToString();
            }
        }
    }
}
