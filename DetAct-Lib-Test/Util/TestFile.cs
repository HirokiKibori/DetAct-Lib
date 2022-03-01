using System;
using System.IO;

namespace DetAct.Test.Util
{
    public class TestFile : IDisposable
    {
        private static readonly string TEST_FILE_PATH = @"Files\StackExample.btml";

        public string Content { get; private set; }

        public TestFile()
        {
            Content = File.ReadAllText(path: TEST_FILE_PATH);
        }

        public void Dispose()
        {
            Content = null;
            GC.SuppressFinalize(this);
        }
    }
}
