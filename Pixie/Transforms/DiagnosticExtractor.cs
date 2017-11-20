using System;
using System.Collections.Generic;
using Pixie.Markup;

namespace Pixie.Transforms
{
    /// <summary>
    /// A transformation that turns log entries into diagnostics.
    /// </summary>
    public sealed class DiagnosticExtractor
    {
        /// <summary>
        /// Creates a diagnostic extractor.
        /// </summary>
        /// <param name="defaultOrigin">
        /// The default origin to use, for when a log entry
        /// does not specify an origin.
        /// </param>
        /// <param name="defaultKind">
        /// The kind of diagnostic to provide.
        /// </param>
        /// <param name="defaultThemeColor">
        /// The diagnostic theme color.
        /// </param>
        /// <param name="defaultTitle">
        /// The diagnostic title.
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
        /// Transforms a markup tree to include a diagnostic.
        /// </summary>
        /// <param name="tree">The tree to transform.</param>
        /// <returns>A transformed tree.</returns>
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
        /// Transforms a log entry to include a diagnostic.
        /// </summary>
        /// <param name="entry">The log entry to transform.</param>
        /// <param name="defaultOrigin">
        /// The default origin of a diagnostic. This is typically the
        /// name of the application.</param>
        /// <returns>A transformed log entry.</returns>
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