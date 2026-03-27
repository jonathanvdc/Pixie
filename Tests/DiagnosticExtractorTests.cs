using NUnit.Framework;
using Pixie.Code;
using Pixie.Markup;
using Pixie.Transforms;

namespace Pixie.Tests
{
    [TestFixture]
    public class DiagnosticExtractorTests
    {
        [Test]
        public void TransformWrapsPlainTextInDiagnosticWithDefaults()
        {
            var extractor = new DiagnosticExtractor("pixie", "warning", Colors.Yellow, "fallback title");

            var result = (Diagnostic)extractor.Transform("plain message");

            Assert.AreEqual("warning", result.Kind);
            Assert.AreEqual(Colors.Yellow.ToString(), result.ThemeColor.ToString());
            Assert.AreEqual("pixie", ((Text)result.Origin).Contents);
            Assert.AreEqual("fallback title", ((Text)result.Title).Contents);
            Assert.AreEqual("plain message", ((Text)result.Message).Contents);
        }

        [Test]
        public void TransformUsesLeadingTitleAsDiagnosticTitle()
        {
            var extractor = new DiagnosticExtractor("pixie", "error", Colors.Red, "default title");
            var tree = new Sequence(new Title("bad input"), "details");

            var result = (Diagnostic)extractor.Transform(tree);

            Assert.AreEqual("bad input", ((Text)result.Title).Contents);
            Assert.IsTrue(Text.IsEmpty(result.Message is Sequence seq ? seq.Contents[0] : result.Message));
        }

        [Test]
        public void TransformUsesHighlightedSourceAsOrigin()
        {
            var doc = new StringDocument("input.cs", "abc");
            var span = new SourceSpan(doc, 1, 1);
            var source = new HighlightedSource(new SourceRegion(span));
            var extractor = new DiagnosticExtractor("pixie", "error", Colors.Red, "default title");

            var result = (Diagnostic)extractor.Transform(new Sequence(source, "oops"));

            Assert.IsInstanceOf<SourceReference>(result.Origin);
            Assert.AreEqual("input.cs:1:2", RenderTests.Render(result.Origin).Trim());
        }

        [Test]
        public void TransformLeavesExistingDiagnosticUntouched()
        {
            var existing = new Diagnostic("origin", "warning", Colors.Yellow, "title", "message");
            var extractor = new DiagnosticExtractor("pixie", "error", Colors.Red, "default title");

            var result = extractor.Transform(existing);

            Assert.AreSame(existing, result);
        }

        [Test]
        public void TransformLogEntryMapsSeverityToDiagnosticDefaults()
        {
            var entry = new LogEntry(Severity.Error, "broken");

            var result = DiagnosticExtractor.Transform(entry, "pixie");

            Assert.AreEqual(Severity.Error, result.Severity);
            var diagnostic = (Diagnostic)result.Contents;
            Assert.AreEqual("error", diagnostic.Kind);
            Assert.AreEqual(Colors.Red.ToString(), diagnostic.ThemeColor.ToString());
            Assert.AreEqual("pixie", ((Text)diagnostic.Origin).Contents);
        }
    }
}
