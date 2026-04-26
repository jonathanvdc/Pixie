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
            AssertLine(0, 0, 0, doc);
            AssertNoLine(1, doc);
        }

        [Test]
        public void LineOffsetsHandleTrailingNewlines()
        {
            var doc = new StringDocument("lines.txt", "a\nb\n");

            Assert.AreEqual(3, doc.LineCount);
            AssertLine(0, 0, 1, doc);
            AssertLine(1, 2, 1, doc);
            AssertLine(2, 4, 0, doc);
            AssertNoLine(3, doc);
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
        public void TryGetLineRejectsInvalidLineIndices()
        {
            var doc = new StringDocument("letters.txt", "abc");

            AssertNoLine(-10, doc);
            AssertNoLine(10, doc);
        }

        [Test]
        public void ConsecutiveEmptyLinesEachGetDistinctLineOffsets()
        {
            var doc = new StringDocument("lines.txt", "a\n\n\nb");

            Assert.AreEqual(4, doc.LineCount);
            AssertLine(0, 0, 1, doc);
            AssertLine(1, 2, 0, doc);
            AssertLine(2, 3, 0, doc);
            AssertLine(3, 4, 1, doc);
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

        private static void AssertPosition(int line, int column, LineAndColumnPosition position)
        {
            Assert.AreEqual(line, position.Line);
            Assert.AreEqual(column, position.Column);
        }

        private static void AssertLine(
            int index,
            int start,
            int length,
            OriginalSourceDocument document)
        {
            SourceLine line;
            Assert.IsTrue(document.TryGetLine(index, out line));
            Assert.AreEqual(index, line.Index);
            Assert.AreEqual(start, line.Start);
            Assert.AreEqual(length, line.Length);
        }

        private static void AssertNoLine(
            int index,
            OriginalSourceDocument document)
        {
            SourceLine line;
            Assert.IsFalse(document.TryGetLine(index, out line));
        }
    }
}
