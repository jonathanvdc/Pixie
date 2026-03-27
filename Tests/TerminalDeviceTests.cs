using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Pixie.Markup;
using Pixie.Terminal;
using Pixie.Terminal.Devices;

namespace Pixie.Tests
{
    [TestFixture]
    public class TerminalDeviceTests
    {
        private sealed class EncodedStringWriter : StringWriter
        {
            public EncodedStringWriter(Encoding encoding)
            {
                this.encoding = encoding;
            }

            private readonly Encoding encoding;

            public override Encoding Encoding => encoding;
        }

        private static readonly FieldInfo TerminalIdentifierProviderField =
            typeof(TextWriterTerminal).GetField(
                "terminalIdentifierProvider",
                BindingFlags.NonPublic | BindingFlags.Static)!;

        private static readonly FieldInfo WindowsIsOSProviderField =
            typeof(TextWriterTerminal).GetField(
                "windowsIsOSProvider",
                BindingFlags.NonPublic | BindingFlags.Static)!;

        private static readonly FieldInfo ConsoleBufferWidthProviderField =
            typeof(TextWriterTerminal).GetField(
                "consoleBufferWidthProvider",
                BindingFlags.NonPublic | BindingFlags.Static)!;

        private static void WithStaticField(FieldInfo field, object value, System.Action action)
        {
            var oldValue = field.GetValue(null);
            field.SetValue(null, value);
            try
            {
                action();
            }
            finally
            {
                field.SetValue(null, oldValue);
            }
        }

        [Test]
        public void GetFirstRenderableStringSkipsUnrenderableOptions()
        {
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, 80, Encoding.ASCII);

            Assert.AreEqual("fallback", terminal.GetFirstRenderableString("⟨x⟩", "fallback"));
            Assert.IsNull(terminal.GetFirstRenderableString("⟨x⟩"));
        }

        [Test]
        public void OutputTerminalSeparatorsDoNotDuplicateNewlines()
        {
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, 80, Encoding.ASCII);

            terminal.WriteSeparator(1);
            terminal.WriteSeparator(1);
            terminal.Write("x");
            terminal.WriteLine();

            Assert.AreEqual("\nx\n", writer.ToString());
        }

        [Test]
        public void TextWriterTerminalCanWriteCharactersAndText()
        {
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, 80, Encoding.ASCII);

            terminal.Write('a');
            terminal.Write("bc");

            Assert.AreEqual("abc", writer.ToString());
        }

        [Test]
        public void TextWriterTerminalReportsRenderableAsciiStrings()
        {
            var writer = new StringWriter();
            var terminal = new TextWriterTerminal(writer, 80, Encoding.ASCII);

            Assert.IsTrue(terminal.CanRender("plain ascii"));
            Assert.IsFalse(terminal.CanRender("snowman \u2603"));
        }

        [Test]
        public void LayoutTerminalCanWordWrapAcrossSpaces()
        {
            var writer = new StringWriter();
            var inner = new TextWriterTerminal(writer, 80, Encoding.ASCII);
            var terminal = new LayoutTerminal(inner, Alignment.Left, WrappingStrategy.Word, "", 5);

            terminal.Write("hello world");
            terminal.Flush();

            Assert.AreEqual("hello\nworld", writer.ToString().Trim());
        }

        [Test]
        public void LayoutTerminalCanApplyLeftPaddingAndAlignment()
        {
            var writer = new StringWriter();
            var inner = new TextWriterTerminal(writer, 80, Encoding.ASCII);
            var terminal = LayoutTerminal.AddLeftPadding(LayoutTerminal.Align(inner, Alignment.Right), "> ");

            terminal.Write("x");
            terminal.Flush();

            StringAssert.EndsWith("> x", writer.ToString());
        }

        [Test]
        public void RedirectedConsoleStreamUsesProvidedWidthAndStyle()
        {
            var writer = new EncodedStringWriter(Encoding.ASCII);
            var method = typeof(TextWriterTerminal).GetMethod(
                "FromConsoleStream",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method);

            var terminalObj = method!.Invoke(
                null,
                new object[] { writer, true, NoStyleManager.Instance, 42 });
            Assert.IsInstanceOf<TextWriterTerminal>(terminalObj);
            var terminal = (TextWriterTerminal)terminalObj!;

            Assert.AreEqual(42, terminal.Width);
            Assert.AreSame(NoStyleManager.Instance, terminal.Style);
            Assert.IsFalse(terminal.CanRender("\u2603"));
        }

        [Test]
        public void RedirectedConsoleStreamFallsBackToDefaultWidth()
        {
            var writer = new EncodedStringWriter(Encoding.ASCII);
            var method = typeof(TextWriterTerminal).GetMethod(
                "FromConsoleStream",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method);

            var terminalObj = method!.Invoke(
                null,
                new object?[] { writer, true, null, null });

            Assert.IsInstanceOf<TextWriterTerminal>(terminalObj);
            Assert.AreEqual(80, ((TextWriterTerminal)terminalObj!).Width);
        }

        [Test]
        public void TerminalIdentifierHelperRecognizesAnsiNames()
        {
            var method = typeof(TextWriterTerminal).GetMethod(
                "IsAnsiTerminalIdentifier",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method);

            Assert.IsTrue((bool)method!.Invoke(null, new object[] { "xterm-256color" })!);
            Assert.IsTrue((bool)method.Invoke(null, new object[] { "linux" })!);
            Assert.IsTrue((bool)method.Invoke(null, new object[] { "vt100" })!);
            Assert.IsFalse((bool)method.Invoke(null, new object[] { "dumb" })!);
        }

        [Test]
        public void NonRedirectedAnsiTerminalUsesAnsiStyleManager()
        {
            var writer = new EncodedStringWriter(Encoding.UTF8);
            var method = typeof(TextWriterTerminal).GetMethod(
                "FromConsoleStream",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method);

            WithStaticField(
                TerminalIdentifierProviderField,
                (System.Func<string>)(() => "xterm-256color"),
                () => WithStaticField(
                    ConsoleBufferWidthProviderField,
                    (System.Func<int>)(() => 123),
                    () =>
                    {
                        var terminalObj = method!.Invoke(
                            null,
                            new object?[] { writer, false, null, null });

                        Assert.IsInstanceOf<TextWriterTerminal>(terminalObj);
                        var terminal = (TextWriterTerminal)terminalObj!;
                        Assert.AreEqual(123, terminal.Width);
                        Assert.IsInstanceOf<AnsiStyleManager>(terminal.Style);
                    }));
        }

        [Test]
        public void NonRedirectedFallbackUsesConsoleStyleManagerAndSafeEncodingOnWindows()
        {
            var writer = new EncodedStringWriter(Encoding.UTF8);
            var method = typeof(TextWriterTerminal).GetMethod(
                "FromConsoleStream",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method);

            WithStaticField(
                TerminalIdentifierProviderField,
                (System.Func<string>)(() => "dumb"),
                () => WithStaticField(
                    WindowsIsOSProviderField,
                    (System.Func<bool>)(() => true),
                    () => WithStaticField(
                        ConsoleBufferWidthProviderField,
                        (System.Func<int>)(() => 77),
                        () =>
                        {
                            var terminalObj = method!.Invoke(
                                null,
                                new object?[] { writer, false, null, null });

                            Assert.IsInstanceOf<TextWriterTerminal>(terminalObj);
                            var terminal = (TextWriterTerminal)terminalObj!;
                            Assert.AreEqual(77, terminal.Width);
                            Assert.IsInstanceOf<ConsoleStyleManager>(terminal.Style);
                            Assert.IsFalse(terminal.CanRender("\u2603"));
                        })));
        }

        [Test]
        public void NonRedirectedFallbackKeepsWriterEncodingOffWindows()
        {
            var writer = new EncodedStringWriter(Encoding.UTF8);
            var method = typeof(TextWriterTerminal).GetMethod(
                "FromConsoleStream",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method);

            WithStaticField(
                TerminalIdentifierProviderField,
                (System.Func<string>)(() => "dumb"),
                () => WithStaticField(
                    WindowsIsOSProviderField,
                    (System.Func<bool>)(() => false),
                    () => WithStaticField(
                        ConsoleBufferWidthProviderField,
                        (System.Func<int>)(() => 66),
                        () =>
                        {
                            var terminalObj = method!.Invoke(
                                null,
                                new object?[] { writer, false, null, null });

                            Assert.IsInstanceOf<TextWriterTerminal>(terminalObj);
                            var terminal = (TextWriterTerminal)terminalObj!;
                            Assert.IsInstanceOf<ConsoleStyleManager>(terminal.Style);
                            Assert.IsTrue(terminal.CanRender("\u2603"));
                        })));
        }

        [Test]
        public void TerminalWidthFallsBackToDefaultWhenProviderReturnsZero()
        {
            var method = typeof(TextWriterTerminal).GetMethod(
                "GetTerminalWidthImpl",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method);

            WithStaticField(
                ConsoleBufferWidthProviderField,
                (System.Func<int>)(() => 0),
                () =>
                {
                    var width = (int)method!.Invoke(null, null)!;
                    Assert.AreEqual(80, width);
                });
        }

        [Test]
        public void TerminalWidthFallsBackToDefaultWhenProviderThrows()
        {
            var method = typeof(TextWriterTerminal).GetMethod(
                "GetTerminalWidthImpl",
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method);

            WithStaticField(
                ConsoleBufferWidthProviderField,
                (System.Func<int>)(() => throw new IOException("boom")),
                () =>
                {
                    var width = (int)method!.Invoke(null, null)!;
                    Assert.AreEqual(80, width);
                });
        }

        [Test]
        public void AnsiStyleManagerWritesControlSequencesForPushAndPop()
        {
            var writer = new StringWriter();
            var style = new AnsiStyleManager(writer, Colors.White, Colors.Black);

            style.PushForegroundColor(Colors.Red);
            style.PushDecoration(TextDecoration.Bold, DecorationSpan.UnifyDecorations);
            style.PopStyle();
            style.PopStyle();

            StringAssert.Contains("\u001b[", writer.ToString());
            StringAssert.Contains("31", writer.ToString());
            StringAssert.Contains("1", writer.ToString());
        }

        [Test]
        public void NoStyleManagerAcceptsAllOperationsAsNoOps()
        {
            var style = NoStyleManager.Instance;

            Assert.DoesNotThrow(() =>
            {
                style.PushForegroundColor(Colors.Red);
                style.PushBackgroundColor(Colors.Blue);
                style.PushDecoration(TextDecoration.Underline, DecorationSpan.UnifyDecorations);
                style.PopStyle();
            });
        }
    }
}
