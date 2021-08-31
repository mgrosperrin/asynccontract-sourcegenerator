using System;

namespace MGR.AsyncContract.SourceGenerator
{
    internal class EmptyDisposable : IDisposable
    {
        private EmptyDisposable() { }
        public static EmptyDisposable Instance { get; } = new();
        public void Dispose() { }
    }
}
