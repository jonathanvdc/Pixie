using System.IO;
using NUnit.Framework;
using Pixie.Code;

namespace Pixie.Tests
{
    [TestFixture]
    public class SourceDocumentViewTests
    {
        [Test]
        public void OriginalSourceDocumentResolvesSpansToItself()
        {
            var doc = new StringDocument("input.txt", "first\nsecond");

            var resolved = doc.ResolveSpan(6, 6);

            Assert.AreEqual(doc, resolved.PrimarySpan.Document);
            Assert.AreEqual(6, resolved.PrimarySpan.Start);
            Assert.AreEqual(6, resolved.PrimarySpan.Length);
            Assert.AreEqual(1, resolved.OriginSpans.Count);
        }

        [Test]
        public void OriginalSourceDocumentUsesOneBasedDiagnosticPositions()
        {
            var doc = new StringDocument("input.txt", "first\nsecond");

            var position = doc.GetPosition(6);

            Assert.AreEqual("input.txt", position.Identifier);
            Assert.AreEqual(2, position.Line);
            Assert.AreEqual(1, position.Column);
        }

        [Test]
        public void SourceDocumentViewPositionsResolveThroughOriginalSource()
        {
            var original = new StringDocument("input.txt", "before target after");
            var view = new FixedSourceDocumentView("target", original, 7);

            var position = view.GetPosition(0);

            Assert.AreEqual("input.txt", position.Identifier);
            Assert.AreEqual(1, position.Line);
            Assert.AreEqual(8, position.Column);
        }

        private sealed class FixedSourceDocumentView : DerivedSourceDocument
        {
            private readonly string text;
            private readonly OriginalSourceDocument originalDocument;
            private readonly int originalStart;

            public FixedSourceDocumentView(
                string text,
                OriginalSourceDocument originalDocument,
                int originalStart)
            {
                this.text = text;
                this.originalDocument = originalDocument;
                this.originalStart = originalStart;
            }

            public override string Identifier => "generated";

            public override int Length => text.Length;

            public override TextReader Open(int offset)
            {
                return new StringReader(GetText(offset, Length - offset));
            }

            public override string GetText(int offset, int length)
            {
                return text.Substring(offset, length);
            }

            public override ResolvedSourceSpan ResolveSpan(int start, int length)
            {
                return originalDocument.ResolveSpan(originalStart + start, length);
            }
        }
    }
}
