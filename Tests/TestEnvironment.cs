using Pixie.Transforms;

namespace Pixie.Tests
{
    public static class TestEnvironment
    {
        public static readonly ILog GlobalLog = new TransformLog(
            new TestLog(
                new[] { Severity.Error },
                Pixie.Terminal.TerminalLog.Acquire()),
            MakeDiagnostic);

        private static LogEntry MakeDiagnostic(LogEntry entry)
        {
            return DiagnosticExtractor.Transform(entry, "program");
        }
    }
}
