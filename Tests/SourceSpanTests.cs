using NUnit.Framework;
using Pixie.Code;

namespace Pixie.Tests
{
    [TestFixture]
    public class SourceSpanTests
    {
        [Test]
        public void DefaultSpanIsUnknown()
        {
            var span = SourceSpan.Unknown;

            Assert.IsFalse(span.IsKnown);
            Assert.AreEqual(string.Empty, span.Text);
            Assert.IsNull(span.Resolve());
            Assert.AreEqual(0, span.Position.Line);
            Assert.AreEqual(0, span.Position.Column);
        }

        [Test]
        public void MergeReturnsKnownSpanWhenOtherSpanIsUnknown()
        {
            var doc = new StringDocument("input.txt", "abcdef");
            var known = new SourceSpan(doc, 2, 2);

            Assert.AreEqual(known.Start, SourceSpan.Merge(SourceSpan.Unknown, known).Start);
            Assert.AreEqual(known.Start, SourceSpan.Merge(known, SourceSpan.Unknown).Start);
        }

        [Test]
        public void MergeCoversBothSpansInSameDocument()
        {
            var doc = new StringDocument("input.txt", "abcdef");

            var merged = SourceSpan.Merge(
                new SourceSpan(doc, 4, 1),
                new SourceSpan(doc, 1, 2));

            Assert.AreEqual(1, merged.Start);
            Assert.AreEqual(4, merged.Length);
            Assert.AreEqual(5, merged.End);
        }

        [Test]
        public void MergeKeepsFirstSpanForDifferentDocuments()
        {
            var first = new SourceSpan(new StringDocument("a.txt", "abc"), 1, 1);
            var second = new SourceSpan(new StringDocument("b.txt", "abc"), 2, 1);

            var merged = SourceSpan.Merge(first, second);

            Assert.AreEqual(first.Document, merged.Document);
            Assert.AreEqual(first.Start, merged.Start);
            Assert.AreEqual(first.Length, merged.Length);
        }
    }
}
