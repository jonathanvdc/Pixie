using Pixie.Transforms;

namespace Pixie.Tests
{
    public static class TestEnvironment
    {
        public static readonly ILog GlobalLog = new TestLog(
                new[] { Severity.Error },
                Pixie.Terminal.TerminalLog.Acquire())
            .WithDiagnostics("program");
    }
}
