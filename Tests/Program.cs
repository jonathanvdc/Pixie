using System;
using NUnitLite;
using Pixie.Transforms;

namespace Pixie.Tests
{
    public static class Program
    {
        public static ILog GlobalLog = new TransformLog(
            new TestLog(
                new Severity[] { Severity.Error },
                Pixie.Terminal.TerminalLog.Acquire()),
            MakeDiagnostic);

        private static LogEntry MakeDiagnostic(LogEntry entry)
        {
            return DiagnosticExtractor.Transform(entry, "program");
        }

        public static int Main(string[] args)
        {
            return new AutoRun().Execute(args);
        }
    }
}
