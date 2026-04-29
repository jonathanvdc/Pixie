using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Pixie.Code;
using Pixie.Terminal;
using Pixie.Terminal.Devices;
using Pixie.Markup;

namespace Pixie.Tests
{
    [TestFixture]
    public class CaretDiagnosticTests
    {
        private const string SourceCode = @"public static class Program
{
    public Program()
    { }
}";

        private static string RenderDiagnostic(
            LogEntry entry,
            int terminalWidth,
            int contextLineCount)
        {
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, terminalWidth, Encoding.ASCII);
            var log = new TerminalLog(terminal);
            log.Log(entry);
            return writer.ToString();
        }

        private static string RenderEntry(
            LogEntry entry,
            int terminalWidth,
            int contextLineCount)
        {
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, terminalWidth, Encoding.ASCII);
            var log = new TerminalLog(terminal);
            log.Log(entry);
            return writer.ToString();
        }

        private static LogEntry ErrorDiagnostic(
            string title,
            SourceRegion highlightRegion,
            SourceRegion focusRegion,
            params Inline[] contents)
        {
            var highlightedSource = new HighlightedSource(highlightRegion, focusRegion);
            return new LogEntry(
                Severity.Error,
                Diagnostic.FromSeverity(
                    Severity.Error,
                    new SourceReference(highlightedSource.HighlightedSpan),
                    title,
                    new Sequence(contents),
                    highlightedSource));
        }

        [Test]
        public void CaretDiagnosticRendersHeaderMessageAndCaretSnippet()
        {
            var doc = new StringDocument("code.cs", SourceCode);
            var ctorStartOffset = SourceCode.IndexOf("public Program()", StringComparison.InvariantCulture);
            var ctorNameOffset = SourceCode.IndexOf("Program()", StringComparison.InvariantCulture);

            var highlightRegion = new SourceRegion(
                    new SourceSpan(doc, ctorStartOffset, "public Program()".Length))
                .ExcludeCharacters(char.IsWhiteSpace);

            var focusRegion = new SourceRegion(
                new SourceSpan(doc, ctorNameOffset, "Program".Length));

            var entry = ErrorDiagnostic(
                "hello world",
                highlightRegion,
                focusRegion,
                new Text("look at this beautiful error message!"));

            var rendered = RenderDiagnostic(entry, 80, 5);

            StringAssert.Contains("code.cs:3:5: error: hello world:", rendered);
            StringAssert.Contains("look at this beautiful error message!", rendered);
            StringAssert.Contains("3 |     public Program()", rendered);
            StringAssert.Contains("~", rendered);
            StringAssert.Contains("^", rendered);
            StringAssert.Contains("4 |     { }", rendered);
        }

        [Test]
        public void CaretDiagnosticStylesFocusTextAndSquiggleWithDiagnosticSeverity()
        {
            var source = "warning here";
            var doc = new StringDocument("warning.txt", source);
            var focusRegion = new SourceRegion(new SourceSpan(doc, 0, 7));
            var highlightedSource = new HighlightedSource(focusRegion);
            var entry = new LogEntry(
                Severity.Warning,
                Diagnostic.FromSeverity(
                    Severity.Warning,
                    new SourceReference(highlightedSource.HighlightedSpan),
                    "watch it",
                    "body",
                    highlightedSource));

            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(
                writer,
                80,
                new AnsiStyleManager(writer),
                Encoding.UTF8);
            new TerminalLog(terminal).Log(entry);

            var lines = writer.ToString().Split('\n');
            string sourceLine = lines.First(line => line.Contains("33m") && line.Contains("1m") && !line.Contains("^"));
            string squiggleLine = lines.First(line => line.Contains("^"));

            StringAssert.Contains("33m", sourceLine);
            StringAssert.Contains("1m", sourceLine);
            StringAssert.Contains("33m", squiggleLine);
        }

        [Test]
        public void RawHighlightedSourceLogEntryDoesNotRenderDocumentIdentifierHeader()
        {
            const string source = "<{%>";
            var doc = new StringDocument("code.eol", source);
            var errOffset = source.IndexOf("%", StringComparison.InvariantCulture);
            var highlightRegion = new SourceRegion(new SourceSpan(doc, errOffset, 1));
            var entry = new LogEntry(
                Severity.Error,
                new Paragraph("Expected ending curly"),
                new HighlightedSource(highlightRegion));

            var rendered = RenderEntry(entry, 80, 1);

            Assert.IsFalse(rendered.Contains("code.eol"));
            StringAssert.Contains("Expected ending curly", rendered);
            StringAssert.Contains("1 | <{%>", rendered);
            StringAssert.Contains("^", rendered);
        }

        [Test]
        public void ExplicitDiagnosticAddsDocumentIdentifierHeaderForHighlightedSource()
        {
            const string source = "<{%>";
            var doc = new StringDocument("code.eol", source);
            var errOffset = source.IndexOf("%", StringComparison.InvariantCulture);
            var highlightRegion = new SourceRegion(new SourceSpan(doc, errOffset, 1));
            var entry = ErrorDiagnostic(
                "Expected ending curly",
                highlightRegion,
                highlightRegion);

            var rendered = RenderDiagnostic(entry, 80, 1);

            StringAssert.Contains("code.eol:1:3: error: Expected ending curly:", rendered);
            StringAssert.Contains("1 | <{%>", rendered);
            StringAssert.Contains("^", rendered);
        }

        [Test]
        public void GeneratedCaretDiagnosticsRemainStableAcrossManyInputs()
        {
            var rng = new Random(12345);

            for (int i = 0; i < 200; i++)
            {
                var source = GenerateSource(rng);
                var doc = new StringDocument($"generated-{i}.txt", source);
                var start = rng.Next(doc.Length);
                var maxLength = doc.Length - start;
                var highlightLength = rng.Next(1, maxLength + 1);
                var highlightRegion = new SourceRegion(new SourceSpan(doc, start, highlightLength));

                var nonWhitespaceOffsets = new List<int>();
                for (int offset = highlightRegion.StartOffset; offset < highlightRegion.EndOffset; offset++)
                {
                    if (!char.IsWhiteSpace(source[offset]))
                    {
                        nonWhitespaceOffsets.Add(offset);
                    }
                }

                if (nonWhitespaceOffsets.Count > 0)
                {
                    var firstOffset = nonWhitespaceOffsets[0];
                    var lastOffset = nonWhitespaceOffsets[nonWhitespaceOffsets.Count - 1];
                    highlightRegion = new SourceRegion(
                        new SourceSpan(doc, firstOffset, lastOffset - firstOffset + 1))
                        .ExcludeCharacters(char.IsWhiteSpace);
                }

                var expectCaret = nonWhitespaceOffsets.Count > 0;
                var focusStart = expectCaret
                    ? nonWhitespaceOffsets[rng.Next(nonWhitespaceOffsets.Count)]
                    : highlightRegion.StartOffset;
                var focusLength = expectCaret
                    ? Math.Max(1, Math.Min(highlightRegion.EndOffset - focusStart, rng.Next(1, 6)))
                    : 1;

                if (focusStart + focusLength > doc.Length)
                {
                    focusLength = doc.Length - focusStart;
                }
                if (focusStart + focusLength > highlightRegion.EndOffset)
                {
                    focusLength = highlightRegion.EndOffset - focusStart;
                }

                var focusRegion = new SourceRegion(new SourceSpan(doc, focusStart, Math.Max(1, focusLength)));
                var entry = ErrorDiagnostic(
                    $"generated case {i}",
                    highlightRegion,
                    focusRegion,
                    "generated diagnostic body");

                var width = rng.Next(18, 50);
                var context = rng.Next(0, 4);

                Assert.DoesNotThrow(() =>
                {
                    var rendered = RenderDiagnostic(entry, width, context);

                    StringAssert.Contains($"generated-{i}.txt:", rendered);
                    StringAssert.Contains("error: generated case", rendered);
                    StringAssert.Contains("generated diagnostic body", rendered);
                    StringAssert.Contains("|", rendered);
                    Assert.IsFalse(rendered.Contains("\t"));
                    if (expectCaret)
                    {
                        StringAssert.Contains("^", rendered);
                    }
                }, $"Generated caret diagnostic failed for case {i}.");
            }
        }

        [Test]
        public void CaretDiagnosticWrapsCleanlyOnVeryNarrowTerminal()
        {
            var source = "alpha beta gamma delta";
            var doc = new StringDocument("narrow.txt", source);
            var highlightRegion = new SourceRegion(new SourceSpan(doc, 6, 10));
            var focusRegion = new SourceRegion(new SourceSpan(doc, 11, 5));
            var entry = ErrorDiagnostic(
                "narrow width",
                highlightRegion,
                focusRegion,
                "body");

            var rendered = RenderDiagnostic(entry, 18, 0);

            StringAssert.Contains("narrow.txt:1:7: error: narrow width:", rendered);
            StringAssert.Contains("^", rendered);
            Assert.Greater(rendered.Split('\n').Length, 4);
        }

        [Test]
        public void CaretDiagnosticSupportsHighlightsThatSpanMultipleLines()
        {
            var source = "first line\nsecond target line\nthird line";
            var doc = new StringDocument("multi.txt", source);
            var highlightStart = source.IndexOf("line\nsecond", StringComparison.InvariantCulture);
            var focusStart = source.IndexOf("target", StringComparison.InvariantCulture);
            var highlightRegion = new SourceRegion(new SourceSpan(doc, highlightStart, "line\nsecond target".Length));
            var focusRegion = new SourceRegion(new SourceSpan(doc, focusStart, "target".Length));
            var entry = ErrorDiagnostic(
                "multiline",
                highlightRegion,
                focusRegion,
                "body");

            var rendered = RenderDiagnostic(entry, 80, 2);

            StringAssert.Contains("1 | first line", rendered);
            StringAssert.Contains("2 | second target line", rendered);
            StringAssert.Contains("^", rendered);
            StringAssert.Contains("~", rendered);
        }

        [Test]
        public void CaretDiagnosticReplacesTabsAndTrimsTrailingWhitespace()
        {
            var source = "\tvalue\t=\t42   \n\treturn value;\t  ";
            var doc = new StringDocument("tabs.txt", source);
            var focusStart = source.IndexOf("42", StringComparison.InvariantCulture);
            var highlightRegion = new SourceRegion(new SourceSpan(doc, source.IndexOf("value", StringComparison.InvariantCulture), "value\t=\t42".Length));
            var focusRegion = new SourceRegion(new SourceSpan(doc, focusStart, 2));
            var entry = ErrorDiagnostic(
                "tabs",
                highlightRegion,
                focusRegion,
                "body");

            var rendered = RenderDiagnostic(entry, 80, 1);

            Assert.IsFalse(rendered.Contains("\t"));
            StringAssert.Contains("42", rendered);
            StringAssert.Contains("^", rendered);
            Assert.IsFalse(rendered.Contains("42   "));
        }

        [Test]
        public void CaretDiagnosticDoesNotRequireVisibleCaretForWhitespaceOnlyHighlight()
        {
            var source = "left   right";
            var doc = new StringDocument("space.txt", source);
            var highlightRegion = new SourceRegion(new SourceSpan(doc, 4, 3));
            var focusRegion = new SourceRegion(new SourceSpan(doc, 4, 1));
            var entry = ErrorDiagnostic(
                "whitespace only",
                highlightRegion,
                focusRegion,
                "body");

            var rendered = RenderDiagnostic(entry, 80, 0);

            StringAssert.Contains("space.txt:1:5: error: whitespace only:", rendered);
            StringAssert.Contains("body", rendered);
            StringAssert.Contains("^", rendered);
        }

        [Test]
        public void CaretDiagnosticHandlesFocusNearEndOfLine()
        {
            var source = "0123456789 end";
            var doc = new StringDocument("end.txt", source);
            var focusStart = source.IndexOf("end", StringComparison.InvariantCulture);
            var highlightRegion = new SourceRegion(new SourceSpan(doc, focusStart - 2, 5));
            var focusRegion = new SourceRegion(new SourceSpan(doc, focusStart + 2, 1));
            var entry = ErrorDiagnostic(
                "line end",
                highlightRegion,
                focusRegion,
                "body");

            var rendered = RenderDiagnostic(entry, 80, 0);

            StringAssert.Contains("1 | 0123456789 end", rendered);
            StringAssert.Contains("^", rendered);
            StringAssert.Contains("~", rendered);
        }

        [Test]
        public void CaretDiagnosticShowsCaretForTrailingWhitespaceFocus()
        {
            var source = "value   ";
            var doc = new StringDocument("trail.txt", source);
            var highlightRegion = new SourceRegion(new SourceSpan(doc, 5, 3));
            var focusRegion = new SourceRegion(new SourceSpan(doc, 7, 1));
            var entry = ErrorDiagnostic(
                "trailing whitespace",
                highlightRegion,
                focusRegion,
                "body");

            var rendered = RenderDiagnostic(entry, 80, 0);

            StringAssert.Contains("trail.txt:1:6: error: trailing whitespace:", rendered);
            StringAssert.Contains("^", rendered);
        }

        [Test]
        public void CaretDiagnosticShowsCaretForWhitespaceOnlyLineFocus()
        {
            var source = "first\n   \nthird";
            var doc = new StringDocument("blank.txt", source);
            var whitespaceLineStart = source.IndexOf("   ", StringComparison.InvariantCulture);
            var highlightRegion = new SourceRegion(new SourceSpan(doc, whitespaceLineStart, 3));
            var focusRegion = new SourceRegion(new SourceSpan(doc, whitespaceLineStart + 1, 1));
            var entry = ErrorDiagnostic(
                "blank line whitespace",
                highlightRegion,
                focusRegion,
                "body");

            var rendered = RenderDiagnostic(entry, 80, 1);

            StringAssert.Contains("blank.txt:2:1: error: blank line whitespace:", rendered);
            StringAssert.Contains("2 |", rendered);
            StringAssert.Contains("^", rendered);
        }

        [Test]
        public void PiecewiseCaretDiagnosticWithGeneratedPrefixRendersOriginalSource()
        {
            var source = "before\nerror here\nafter";
            var original = new StringDocument("input.txt", source);
            var prefix = "// generated header\n";
            var document = PiecewiseSourceDocument.Create("expanded.txt")
                .AddText(prefix)
                .AddSource(new SourceSpan(original, 0, original.Length))
                .Build();
            var errorOffset = prefix.Length + source.IndexOf("error", StringComparison.InvariantCulture);
            var highlightRegion = new SourceRegion(new SourceSpan(document, errorOffset, "error".Length));

            var rendered = RenderDiagnostic(
                ErrorDiagnostic("piecewise", highlightRegion, highlightRegion, "body"),
                80,
                1);

            StringAssert.Contains("input.txt:2:1: error: piecewise:", rendered);
            StringAssert.Contains("1 | before", rendered);
            StringAssert.Contains("2 | error here", rendered);
            StringAssert.Contains("3 | after", rendered);
            Assert.IsFalse(rendered.Contains("generated header"));
            StringAssert.Contains("^", rendered);
        }

        [Test]
        public void PiecewiseCaretDiagnosticWithSourcePiecesSeparatedByGeneratedTextUsesOriginalLines()
        {
            var source = "alpha\nbeta target\ngamma";
            var original = new StringDocument("input.txt", source);
            var firstLength = source.IndexOf("target", StringComparison.InvariantCulture);
            var document = PiecewiseSourceDocument.Create("expanded.txt")
                .AddSource(new SourceSpan(original, 0, firstLength))
                .AddText("/* generated */")
                .AddSource(new SourceSpan(original, firstLength, "target".Length))
                .Build();
            var targetOffset = firstLength + "/* generated */".Length;
            var highlightRegion = new SourceRegion(new SourceSpan(document, targetOffset, "target".Length));

            var rendered = RenderDiagnostic(
                ErrorDiagnostic("split source", highlightRegion, highlightRegion, "body"),
                80,
                1);

            StringAssert.Contains("input.txt:2:6: error: split source:", rendered);
            StringAssert.Contains("2 | beta target", rendered);
            Assert.IsFalse(rendered.Contains("generated"));
            StringAssert.Contains("^", rendered);
        }

        [Test]
        public void PiecewiseCaretDiagnosticForAnchoredGeneratedTextPointsAtAnchor()
        {
            var source = "alpha\nbeta\ngamma";
            var original = new StringDocument("input.txt", source);
            var anchorStart = source.IndexOf("beta", StringComparison.InvariantCulture);
            var anchor = new SourceSpan(original, anchorStart, "beta".Length);
            var document = PiecewiseSourceDocument.Create("expanded.txt")
                .AddText("generated-token", anchor)
                .Build();
            var highlightRegion = new SourceRegion(new SourceSpan(document, 0, "generated".Length));

            var rendered = RenderDiagnostic(
                ErrorDiagnostic("anchored generated", highlightRegion, highlightRegion, "body"),
                80,
                1);

            StringAssert.Contains("input.txt:2:1: error: anchored generated:", rendered);
            StringAssert.Contains("2 | beta", rendered);
            Assert.IsFalse(rendered.Contains("generated-token"));
        }

        [Test]
        public void PiecewiseCaretDiagnosticCrossingGeneratedTextHighlightsKnownOriginalPieces()
        {
            var source = "abc def";
            var original = new StringDocument("input.txt", source);
            var document = PiecewiseSourceDocument.Create("expanded.txt")
                .AddSource(new SourceSpan(original, 0, 3))
                .AddText(" <generated> ")
                .AddSource(new SourceSpan(original, 4, 3))
                .Build();
            var highlightRegion = new SourceRegion(new SourceSpan(document, 1, document.Length - 1));
            var focusStart = document.Length - 3;
            var focusRegion = new SourceRegion(new SourceSpan(document, focusStart, 3));

            var rendered = RenderDiagnostic(
                ErrorDiagnostic("cross piece", highlightRegion, focusRegion, "body"),
                80,
                0);

            StringAssert.Contains("input.txt:1:2: error: cross piece:", rendered);
            StringAssert.Contains("1 | abc def", rendered);
            Assert.IsFalse(rendered.Contains("generated"));
            StringAssert.Contains("^", rendered);
            StringAssert.Contains("~", rendered);
        }

        private static string GenerateSource(Random rng)
        {
            var lineCount = rng.Next(1, 8);
            var builder = new StringBuilder();
            const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_ ();,\t    ";

            for (int i = 0; i < lineCount; i++)
            {
                var lineLength = rng.Next(0, 40);
                for (int j = 0; j < lineLength; j++)
                {
                    builder.Append(alphabet[rng.Next(alphabet.Length)]);
                }

                if (i + 1 < lineCount)
                {
                    builder.Append('\n');
                }
            }

            if (builder.Length == 0)
            {
                builder.Append('x');
            }

            return builder.ToString();
        }
    }
}
