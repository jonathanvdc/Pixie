using Loyc;
using Loyc.Syntax;
using Pixie.Code;
using Pixie.Markup;
using LoycSeverity = Loyc.Severity;

namespace Pixie.Loyc
{
    /// <summary>
    /// A Loyc message sink that redirects messages to a Pixie log.
    /// </summary>
    public sealed class PixieMessageSink : IMessageSink
    {
        /// <summary>
        /// Creates a message sink that redirects messages to a Pixie log.
        /// </summary>
        /// <param name="log">The log to redirect messages to.</param>
        public PixieMessageSink(ILog log)
            : this(log, new SourceDocumentCache())
        { }

        /// <summary>
        /// Creates a message sink that redirects messages to a Pixie log.
        /// </summary>
        /// <param name="log">The log to redirect messages to.</param>
        /// <param name="documentCache">
        /// The source document cache to use for translating
        /// <see cref="global::Loyc.Syntax.SourcePos"/> values
        /// to caret diagnostics.
        /// </param>
        public PixieMessageSink(ILog log, SourceDocumentCache documentCache)
        {
            this.Log = log;
            this.DocumentCache = documentCache;
        }

        /// <summary>
        /// Gets the log to which messages should be redirected.
        /// </summary>
        /// <returns>The target log.</returns>
        public ILog Log { get; private set; }

        /// <summary>
        /// Gets the source document cache for this message sink.
        /// </summary>
        /// <value>The source document cache.</value>
        public SourceDocumentCache DocumentCache { get; private set; }

        /// <inheritdoc/>
        public bool IsEnabled(LoycSeverity level)
        {
            return true;
        }

        /// <inheritdoc/>
        public void Write(
            LoycSeverity level,
            object context,
            [Localizable] string format)
        {
            WriteImpl(level, context, format.Localized());
        }

        /// <inheritdoc/>
        public void Write(
            LoycSeverity level,
            object context,
            [Localizable] string format,
            object arg0,
            object arg1)
        {
            WriteImpl(level, context, format.Localized(arg0, arg1));
        }

        /// <inheritdoc/>
        public void Write(
            LoycSeverity level,
            object context,
            [Localizable] string format,
            params object[] args)
        {
            WriteImpl(level, context, format.Localized(args));
        }

        private void WriteImpl(LoycSeverity level, object context, string message)
        {
            Log.Log(
                new LogEntry(
                    ToPixieSeverity(level),
                    new MarkupNode[]
                    {
                        message,
                        FormatContext(context)
                    }));
        }

        /// <summary>
        /// Takes a Loyc context object and tries to translate
        /// it to a Pixie markup node.
        /// </summary>
        /// <param name="context">The context node to translate.</param>
        /// <returns>A Pixie markup node.</returns>
        private MarkupNode FormatContext(object context)
        {
            if (context is SourceRange)
            {
                return new HighlightedSource(((SourceRange)context).ToSourceRegion());
            }
            else if (context is LNode)
            {
                return FormatContext(((LNode)context).Range);
            }
            else if (context is IHasLocation)
            {
                return FormatContext(((IHasLocation)context).Location);
            }
            else if (context is SourcePos)
            {
                // Source positions are actually pretty tough to create
                // caret diagnostics for. We'll just try to index the source
                // document cache.
                var pos = (SourcePos)context;
                SourceRegion region;
                if (TryIndexSourceDocumentCache(pos, out region))
                {
                    return new HighlightedSource(region);
                }
                else
                {
                    // TODO: maybe handle this case more gracefully.
                    // It'd be nice if the source location actually
                    // appeared at the top of a diagnostic.
                    //
                    // OTOH, we're not even use if the message we're
                    // generating is a diagnostic.
                    return "At " + pos.FileName + ":" + pos.Line +
                        ":" + pos.PosInLine + ".";
                }
            }
            else if (context == null)
            {
                return "";
            }
            else
            {
                // Add the context information as an additional paragraph.
                return new Paragraph(
                    new MarkupNode[]
                    {
                        DecorationSpan.MakeBold(new ColorSpan("context:", Colors.Gray)),
                        " ",
                        context.ToString()
                    });
            }
        }

        private bool TryIndexSourceDocumentCache(SourcePos pos, out SourceRegion result)
        {
            SourceDocument document;
            if (DocumentCache.TryGetDocument(pos.FileName, out document))
            {
                int lineOffset = document.GetLineOffset(pos.Line - 1);
                int offset = lineOffset + pos.PosInLine - 1;
                result = new SourceRegion(new SourceSpan(document, offset, 1));
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Translates a Loyc Severity level to a Pixie severity level.
        /// </summary>
        /// <param name="severity">The severity level to translate.</param>
        /// <returns>An equivalent Pixie severity level.</returns>
        public static Severity ToPixieSeverity(LoycSeverity severity)
        {
            if (severity >= LoycSeverity.ErrorDetail)
                return Severity.Error;
            else if (severity >= LoycSeverity.WarningDetail)
                return Severity.Warning;
            else if (severity >= LoycSeverity.InfoDetail)
                return Severity.Message;
            else
                return Severity.Info;
        }
    }
}