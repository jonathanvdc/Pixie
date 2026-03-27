using System;
using System.Collections.Generic;
using Pixie.Markup;

namespace Pixie.Transforms
{
    /// <summary>
    /// A transformation that upgrades plain log entries into
    /// compiler-style diagnostics.
    /// It looks for a title, text, and highlighted source in the markup tree
    /// and uses that information to construct a diagnostic header and origin.
    /// </summary>
    public sealed class DiagnosticExtractor
    {
        /// <summary>
        /// Creates a diagnostic extractor with defaults to use when the source
        /// markup does not already provide a complete diagnostic.
        /// </summary>
        /// <param name="defaultOrigin">
        /// The fallback origin to use when a log entry does not contain a
        /// source reference.
        /// </param>
        /// <param name="defaultKind">
        /// The fallback diagnostic kind, such as <c>error</c> or
        /// <c>warning</c>.
        /// </param>
        /// <param name="defaultThemeColor">
        /// The fallback theme color to use for the resulting diagnostic.
        /// </param>
        /// <param name="defaultTitle">
        /// The fallback title to use when the log entry does not contain one.
        /// </param>
        public DiagnosticExtractor(
            MarkupNode defaultOrigin,
            string defaultKind,
            Color defaultThemeColor,
            MarkupNode defaultTitle)
        {
            this.DefaultOrigin = defaultOrigin;
            this.DefaultKind = defaultKind;
            this.DefaultThemeColor = defaultThemeColor;
            this.DefaultTitle = defaultTitle;
        }

        /// <summary>
        /// Gets the default origin to use, for when a log entry
        /// does not specify an origin.
        /// </summary>
        /// <returns>The default origin.</returns>
        public MarkupNode DefaultOrigin { get; private set; }

        /// <summary>
        /// Gets the default kind of diagnostic to produce.
        /// </summary>
        /// <returns>The default kind of diagnostic.</returns>
        public string DefaultKind { get; private set; }

        /// <summary>
        /// Gets the default theme color for diagnostics.
        /// </summary>
        /// <returns>The default theme color for diagnostics.</returns>
        public Color DefaultThemeColor { get; private set; }

        /// <summary>
        /// Gets the default title for diagnostics.
        /// </summary>
        /// <returns>The default title for diagnostics.</returns>
        public MarkupNode DefaultTitle { get; private set; }

        /// <summary>
        /// Wraps a markup tree in a <see cref="Diagnostic"/> unless it already
        /// contains one.
        /// </summary>
        /// <param name="tree">The tree to transform.</param>
        /// <returns>A diagnostic node or the original diagnostic markup.</returns>
        public MarkupNode Transform(MarkupNode tree)
        {
            var visitor = new DiagnosticExtractingVisitor(this);
            return visitor.Transform(tree);
        }

        private static Dictionary<Severity, string> defaultKinds =
            new Dictionary<Severity, string>()
        {
            { Severity.Info, "info" },
            { Severity.Message, "message" },
            { Severity.Warning, "warning" },
            { Severity.Error, "error" }
        };

        private static Dictionary<Severity, Color> defaultColors = 
            new Dictionary<Severity, Color>()
        {
            { Severity.Info, Colors.Green },
            { Severity.Message, Colors.Green },
            { Severity.Warning, Colors.Yellow },
            { Severity.Error, Colors.Red }
        };

        /// <summary>
        /// Wraps a log entry in a <see cref="Diagnostic"/> using defaults based
        /// on the entry's severity.
        /// </summary>
        /// <param name="entry">The log entry to transform.</param>
        /// <param name="defaultOrigin">
        /// The fallback diagnostic origin. This is typically the name of the
        /// application.
        /// </param>
        /// <returns>A log entry whose contents are diagnostic markup.</returns>
        public static LogEntry Transform(LogEntry entry, MarkupNode defaultOrigin)
        {
            var extractor = new DiagnosticExtractor(
                defaultOrigin,
                defaultKinds[entry.Severity],
                defaultColors[entry.Severity],
                new Text(""));

            return new LogEntry(entry.Severity, extractor.Transform(entry.Contents));
        }
    }

    /// <summary>
    /// A node visitor that helps turn log entries into diagnostics.
    /// </summary>
    internal sealed class DiagnosticExtractingVisitor : MarkupVisitor
    {
        public DiagnosticExtractingVisitor(DiagnosticExtractor extractor)
        {
            this.extractor = extractor;
        }

        public DiagnosticExtractor extractor;

        private MarkupNode title;

        private MarkupNode origin;

        private bool foundText;

        private bool foundExistingDiagnostic;

        private bool FoundEverything =>
            foundExistingDiagnostic
            || (title != null
                && origin != null);

        /// <inheritdoc/>
        protected override bool IsOfInterest(MarkupNode node)
        {
            return !FoundEverything
                && (node is Title
                    || node is Text
                    || node is HighlightedSource
                    || node is Diagnostic);
        }

        /// <inheritdoc/>
        protected override MarkupNode VisitInteresting(MarkupNode node)
        {
            if (!foundText)
            {
                if (node is Title)
                {
                    title = ((Title)node).Contents;
                    foundText = true;
                    return new Text("");
                }
                else if (node is Text)
                {
                    foundText = true;
                    return node;
                }
                else if (node is Diagnostic)
                {
                    // A top-level diagnostic is just what we're looking for.
                    foundExistingDiagnostic = true;
                    return node;
                }
            }

            if (origin == null && node is HighlightedSource)
            {
                var src = (HighlightedSource)node;
                origin = new SourceReference(src.HighlightedSpan);
                return node;
            }

            return VisitUninteresting(node);
        }

        /// <summary>
        /// Transforms a node to include a diagnostic.
        /// </summary>
        /// <param name="node">The node to transform.</param>
        /// <returns>A transformed node.</returns>
        public MarkupNode Transform(MarkupNode node)
        {
            var visited = Visit(node);
            if (foundExistingDiagnostic)
            {
                return visited;
            }
            else
            {
                return new Diagnostic(
                    origin ?? extractor.DefaultOrigin,
                    extractor.DefaultKind,
                    extractor.DefaultThemeColor,
                    title ?? extractor.DefaultTitle,
                    visited);
            }
        }
    }
}
