namespace MGR.AsyncContract.SourceGenerator
{
    internal class GenerationResult
    {
        public GenerationResult(string targetName, string sourceCode)
        {
            SourceCode = sourceCode;
            TargetName = targetName;
        }
        public string SourceCode { get; }
        public string TargetName { get; }
    }
}