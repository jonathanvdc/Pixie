using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Pixie.Code;
using Pixie.Terminal;
using Pixie.Terminal.Devices;
using Pixie.Markup;
using Pixie.Terminal.Render;
using Pixie.Transforms;

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
            var diagnostic = DiagnosticExtractor.Transform(entry, new Text("program"));
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, terminalWidth, Encoding.ASCII);
            var log = new TerminalLog(terminal).WithRenderers(
                DiagnosticRenderer.Instance,
                new HighlightedSourceRenderer(contextLineCount));
            log.Log(diagnostic.Contents);
            return writer.ToString();
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

            var entry = new LogEntry(
                Severity.Error,
                "hello world",
                new MarkupNode[]
                {
                    new Text("look at this beautiful error message!"),
                    new HighlightedSource(highlightRegion, focusRegion)
                });

            var rendered = RenderDiagnostic(entry, 80, 5);

            StringAssert.Contains("code.cs:3:5: error: hello world:", rendered);
            StringAssert.Contains("look at this beautiful error message!", rendered);
            StringAssert.Contains("3 |     public Program()", rendered);
            StringAssert.Contains("~", rendered);
            StringAssert.Contains("^", rendered);
            StringAssert.Contains("4 |     { }", rendered);
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
                var entry = new LogEntry(
                    Severity.Error,
                    $"generated case {i}",
                    new MarkupNode[]
                    {
                        "generated diagnostic body",
                        new HighlightedSource(highlightRegion, focusRegion)
                    });

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
            var entry = new LogEntry(
                Severity.Error,
                "narrow width",
                "body",
                new HighlightedSource(highlightRegion, focusRegion));

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
            var entry = new LogEntry(
                Severity.Error,
                "multiline",
                "body",
                new HighlightedSource(highlightRegion, focusRegion));

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
            var entry = new LogEntry(
                Severity.Error,
                "tabs",
                "body",
                new HighlightedSource(highlightRegion, focusRegion));

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
            var entry = new LogEntry(
                Severity.Error,
                "whitespace only",
                "body",
                new HighlightedSource(highlightRegion, focusRegion));

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
            var entry = new LogEntry(
                Severity.Error,
                "line end",
                "body",
                new HighlightedSource(highlightRegion, focusRegion));

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
            var entry = new LogEntry(
                Severity.Error,
                "trailing whitespace",
                "body",
                new HighlightedSource(highlightRegion, focusRegion));

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
            var entry = new LogEntry(
                Severity.Error,
                "blank line whitespace",
                "body",
                new HighlightedSource(highlightRegion, focusRegion));

            var rendered = RenderDiagnostic(entry, 80, 1);

            StringAssert.Contains("blank.txt:2:1: error: blank line whitespace:", rendered);
            StringAssert.Contains("2 |", rendered);
            StringAssert.Contains("^", rendered);
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
