using NUnit.Framework;
using Pixie.Code;

namespace Pixie.Tests
{
    [TestFixture]
    public class SourceRegionTests
    {
        [Test]
        public void UnionWithRegionCombinesOffsetsAcrossBothRegions()
        {
            var doc = new StringDocument("input.txt", "abcdef");
            var left = new SourceRegion(new SourceSpan(doc, 1, 2));
            var right = new SourceRegion(new SourceSpan(doc, 4, 1));

            var union = left.Union(right);

            Assert.AreEqual(1, union.StartOffset);
            Assert.AreEqual(4, union.Length);
            Assert.IsTrue(union.Contains(1));
            Assert.IsTrue(union.Contains(2));
            Assert.IsTrue(union.Contains(4));
            Assert.IsFalse(union.Contains(3));
        }

        [Test]
        public void UnionWithSpanDoesNotMutateOriginalRegion()
        {
            var doc = new StringDocument("input.txt", "abcdef");
            var region = new SourceRegion(new SourceSpan(doc, 1, 1));

            var union = region.Union(new SourceSpan(doc, 4, 1));

            Assert.IsFalse(region.Contains(4));
            Assert.IsTrue(union.Contains(1));
            Assert.IsTrue(union.Contains(4));
        }

        [Test]
        public void ExcludeCharactersCanTrimWhitespaceFromRegion()
        {
            var doc = new StringDocument("input.txt", "  abc  ");
            var region = new SourceRegion(new SourceSpan(doc, 0, doc.Length));

            var trimmed = region.ExcludeCharacters(char.IsWhiteSpace);

            Assert.AreEqual(2, trimmed.StartOffset);
            Assert.AreEqual(3, trimmed.Length);
            Assert.AreEqual("abc", doc.GetText(trimmed.StartOffset, trimmed.Length));
        }
    }
}
