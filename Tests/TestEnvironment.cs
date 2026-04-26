namespace Pixie.Tests
{
    public static class TestEnvironment
    {
        public static readonly ILog GlobalLog = new ThrowingLog(
                new[] { Severity.Error },
                Pixie.Terminal.TerminalLog.Acquire());
    }
}
