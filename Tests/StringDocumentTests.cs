using System.IO;
using NUnit.Framework;
using Pixie.Code;

namespace Pixie.Tests
{
    [TestFixture]
    public class StringDocumentTests
    {
        [Test]
        public void EmptyDocumentHasSingleEmptyLine()
        {
            var doc = new StringDocument("empty.txt", "");

            Assert.AreEqual(0, doc.Length);
            Assert.AreEqual(1, doc.LineCount);
            Assert.AreEqual(0, doc.GetLineOffset(0));
            Assert.AreEqual(0, doc.GetLineOffset(1));
        }

        [Test]
        public void LineOffsetsHandleTrailingNewlines()
        {
            var doc = new StringDocument("lines.txt", "a\nb\n");

            Assert.AreEqual(3, doc.LineCount);
            Assert.AreEqual(0, doc.GetLineOffset(0));
            Assert.AreEqual(2, doc.GetLineOffset(1));
            Assert.AreEqual(4, doc.GetLineOffset(2));
            Assert.AreEqual(doc.Length, doc.GetLineOffset(3));
        }

        [Test]
        public void PositionsHandleCrLfSeparatedLines()
        {
            var doc = new StringDocument("windows.txt", "ab\r\ncd");

            AssertPosition(1, 1, doc.GetPosition(0));
            AssertPosition(1, 3, doc.GetPosition(2));
            AssertPosition(1, 4, doc.GetPosition(3));
            AssertPosition(2, 1, doc.GetPosition(4));
            AssertPosition(2, 2, doc.GetPosition(5));
        }

        [Test]
        public void OpenCanStartReadingFromNonZeroOffset()
        {
            var doc = new StringDocument("letters.txt", "abcdef");

            using (var reader = doc.Open(2))
            {
                Assert.AreEqual("cdef", reader.ReadToEnd());
            }
        }

        [Test]
        public void GetTextReadsTheRequestedSpan()
        {
            var doc = new StringDocument("letters.txt", "abcdef");

            Assert.AreEqual("bcd", doc.GetText(1, 3));
        }

        [Test]
        public void LineOffsetIsClampedToValidBounds()
        {
            var doc = new StringDocument("letters.txt", "abc");

            Assert.AreEqual(0, doc.GetLineOffset(-10));
            Assert.AreEqual(doc.Length, doc.GetLineOffset(10));
        }

        [Test]
        public void ConsecutiveEmptyLinesEachGetDistinctLineOffsets()
        {
            var doc = new StringDocument("lines.txt", "a\n\n\nb");

            Assert.AreEqual(4, doc.LineCount);
            Assert.AreEqual(0, doc.GetLineOffset(0));
            Assert.AreEqual(2, doc.GetLineOffset(1));
            Assert.AreEqual(3, doc.GetLineOffset(2));
            Assert.AreEqual(4, doc.GetLineOffset(3));
        }

        [Test]
        public void PositionAtNewlineCharacterBelongsToPreviousLine()
        {
            var doc = new StringDocument("lines.txt", "ab\ncd");

            AssertPosition(1, 3, doc.GetPosition(2));
            AssertPosition(2, 1, doc.GetPosition(3));
        }

        [Test]
        public void PositionAtDocumentEndUsesLastLine()
        {
            var doc = new StringDocument("lines.txt", "ab\ncd");

            AssertPosition(2, 3, doc.GetPosition(doc.Length));
        }

        [Test]
        public void OpenAtDocumentEndYieldsEmptyReader()
        {
            var doc = new StringDocument("letters.txt", "abcdef");

            using (var reader = doc.Open(doc.Length))
            {
                Assert.AreEqual(string.Empty, reader.ReadToEnd());
            }
        }

        [Test]
        public void OpenWithLargeOffsetConsumesMultipleBufferIterations()
        {
            var contents = new string('a', 1500) + "tail";
            var doc = new StringDocument("big.txt", contents);

            using (var reader = doc.Open(1500))
            {
                Assert.AreEqual("tail", reader.ReadToEnd());
            }
        }

        private static void AssertPosition(int line, int column, SourcePosition position)
        {
            Assert.AreEqual(line, position.Line);
            Assert.AreEqual(column, position.Column);
        }
    }
}
