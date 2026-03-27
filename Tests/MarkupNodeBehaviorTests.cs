using System;
using NUnit.Framework;
using Pixie.Code;
using Pixie.Markup;

namespace Pixie.Tests
{
    [TestFixture]
    public class MarkupNodeBehaviorTests
    {
        [Test]
        public void DegradableTextMapUsesMappedFallback()
        {
            var degradable = new DegradableText("main", "fallback");

            var mapped = (DegradableText)degradable.Map(_ => (MarkupNode)"mapped");

            Assert.AreEqual("mapped", ((Text)mapped.Fallback).Contents);
        }

        [Test]
        public void TextIsEmptyRecognizesSimpleEmptyCases()
        {
            Assert.IsTrue(Text.IsEmpty(new Text("")));
            Assert.IsTrue(Text.IsEmpty(new Sequence(new MarkupNode[] { "" })));
            Assert.IsTrue(Text.IsEmpty(new Sequence(Array.Empty<MarkupNode>())));
            Assert.IsFalse(Text.IsEmpty(new Sequence(new MarkupNode[] { "", "x" })));
            Assert.IsFalse(Text.IsEmpty(new Title("")));
        }

        [Test]
        public void HighlightedSourceRejectsMismatchedDocuments()
        {
            var doc1 = new StringDocument("a.cs", "abc");
            var doc2 = new StringDocument("b.cs", "abc");
            var region1 = new SourceRegion(new SourceSpan(doc1, 0, 1));
            var region2 = new SourceRegion(new SourceSpan(doc2, 0, 1));

            Assert.Throws<ArgumentException>(() => new HighlightedSource(region1, region2));
        }

        [Test]
        public void QuotationEvenHelpersOnlyQuoteOddIndexedArguments()
        {
            var quoted = Quotation.QuoteEven("a", "b", "c", "d");
            var boldQuoted = Quotation.QuoteEvenInBold("a", "b", "c", "d");

            Assert.AreEqual("a'b'c'd'", RenderTests.Render(quoted).Trim());
            Assert.AreEqual("a'b'c'd'", RenderTests.Render(boldQuoted).Trim());
        }

        [Test]
        public void DegradableTextFallsBackWhenEncodingCannotRender()
        {
            var rendered = RenderTests.Render(
                new DegradableText("⟨ok⟩", "<ok>"));

            Assert.AreEqual("<ok>", rendered.Trim());
        }
    }
}
