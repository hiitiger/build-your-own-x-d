using MCompiler.CodeAnalysis.Text;
using Xunit;

namespace MCompiler.Tests.CodeAnalysis.text
{
    public class SourceTextTests
    {
        [Theory]
        [InlineData(".", 1)]
        [InlineData(".\r\n", 2)]
        [InlineData(".\r\n\r\n", 3)]
        public void SourceText_IncludesLastLine(string text, int expectedLineCount)
        {
            var sourceText = SourceText.From(text);
            var lineCount = sourceText.Lines.Length;

            Assert.Equal(lineCount, expectedLineCount);
        }
    }
}
