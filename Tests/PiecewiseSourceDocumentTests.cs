using System.IO;
using NUnit.Framework;
using Pixie.Code;

namespace Pixie.Tests
{
    [TestFixture]
    public class PiecewiseSourceDocumentTests
    {
        [Test]
        public void BuilderAssemblesSourceAndLiteralPieces()
        {
            var original = new StringDocument("input.txt", "alpha beta gamma");

            var document = PiecewiseSourceDocument.Create("expanded.txt")
                .AddSource(new SourceSpan(original, 0, 5))
                .AddText(" + ")
                .AddSource(new SourceSpan(original, 11, 5))
                .Build();

            Assert.AreEqual("expanded.txt", document.Identifier);
            Assert.AreEqual("alpha + gamma", document.GetText(0, document.Length));
        }

        [Test]
        public void OpenReadsAcrossPieceBoundariesFromOffset()
        {
            var original = new StringDocument("input.txt", "abcdef");
            var document = PiecewiseSourceDocument.Create("expanded.txt")
                .AddSource(new SourceSpan(original, 0, 3))
                .AddText("::")
                .AddSource(new SourceSpan(original, 3, 3))
                .Build();

            using (var reader = document.Open(2))
            {
                Assert.AreEqual("c::def", reader.ReadToEnd());
            }
        }

        [Test]
        public void LineOffsetsAreComputedAcrossPieces()
        {
            var original = new StringDocument("input.txt", "one\ntwo");
            var document = PiecewiseSourceDocument.Create("expanded.txt")
                .AddSource(new SourceSpan(original, 0, 4))
                .AddText("middle\n")
                .AddSource(new SourceSpan(original, 4, 3))
                .Build();

            Assert.AreEqual(3, document.LineCount);
            Assert.AreEqual(0, document.GetLineOffset(0));
            Assert.AreEqual(4, document.GetLineOffset(1));
            Assert.AreEqual(11, document.GetLineOffset(2));
        }

        [Test]
        public void PositionResolvesThroughSourceBackedPiece()
        {
            var original = new StringDocument("input.txt", "first\nsecond");
            var document = PiecewiseSourceDocument.Create("expanded.txt")
                .AddText("// generated\n")
                .AddSource(new SourceSpan(original, 6, 6))
                .Build();

            var position = document.GetPosition("// generated\n".Length);

            Assert.AreEqual("input.txt", position.Identifier);
            Assert.AreEqual(2, position.Line);
            Assert.AreEqual(1, position.Column);
        }

        [Test]
        public void GeneratedTextCanResolveToAnchor()
        {
            var original = new StringDocument("input.txt", "alpha beta");
            var anchor = new SourceSpan(original, 6, 4);
            var document = PiecewiseSourceDocument.Create("expanded.txt")
                .AddText("generated", anchor)
                .Build();

            var resolved = document.ResolveSpan(4, 3);

            Assert.AreEqual(original, resolved.PrimarySpan.Document);
            Assert.AreEqual(anchor.Start, resolved.PrimarySpan.Start);
            Assert.AreEqual(0, resolved.PrimarySpan.Length);
        }

        [Test]
        public void SpanCrossingPiecesReturnsAllKnownOrigins()
        {
            var original = new StringDocument("input.txt", "abcdef");
            var document = PiecewiseSourceDocument.Create("expanded.txt")
                .AddSource(new SourceSpan(original, 0, 2))
                .AddText("??")
                .AddSource(new SourceSpan(original, 4, 2))
                .Build();

            var resolved = document.ResolveSpan(1, 5);

            Assert.AreEqual(2, resolved.OriginSpans.Count);
            Assert.AreEqual(1, resolved.OriginSpans[0].Start);
            Assert.AreEqual(1, resolved.OriginSpans[0].Length);
            Assert.AreEqual(4, resolved.OriginSpans[1].Start);
            Assert.AreEqual(2, resolved.OriginSpans[1].Length);
        }
    }
}
