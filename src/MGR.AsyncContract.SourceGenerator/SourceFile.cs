namespace MGR.AsyncContract.SourceGenerator
{
    internal sealed class SourceFile
    {
        public SourceFile(string fileName, string sourceCode)
        {
            FileName = fileName;
            SourceCode = sourceCode;
        }
        public string FileName { get; }
        public string SourceCode { get; }
    }
}
