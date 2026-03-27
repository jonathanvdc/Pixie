using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Pixie.Markup;
using Pixie.Options;
using Pixie.Terminal;
using Pixie.Terminal.Devices;

namespace Pixie.Tests
{
    [TestFixture]
    public class GnuOptionSetParserTests
    {
        private static Option Files = SequenceOption.CreateStringOption(
            OptionForm.Long("files"));

        private static Option Help = FlagOption.CreateFlagOption(
            new OptionForm[]
            {
                OptionForm.Short("h"),
                OptionForm.Long("help")
            });

        private static Option TypedFiles = SequenceOption.CreateStringOption(
            OptionForm.Short("x"));

        private static Option OptimizationLevel = ValueOption.CreateInt32Option(
            OptionForm.Short("O"),
            0);

        private static Option Color = ValueOption.CreateStringOption(
            OptionForm.Long("color"),
            "auto");

        private static Option Verbose = new FlagOption(
            OptionForm.Long("verbose"),
            OptionForm.Long("no-verbose"),
            false);

        [Test]
        public void ParsePositionalArguments()
        {
            var parser = new GnuOptionSetParser(new Option[] { }, Files);

            var fileNames = new string[] { "file1", "file2" };

            var parsedOpts = parser.Parse(fileNames, TestEnvironment.GlobalLog);

            Assert.AreEqual(fileNames, parsedOpts.GetValue<string[]>(Files));
        }

        [Test]
        public void ErrorOnUnexpectedPositionalArgument()
        {
            var parser = new GnuOptionSetParser(new Option[] { });

            var args = new string[] { "file" };

            AssertError(parser, args);
        }

        [Test]
        public void ParseOptionAndPositionalArguments()
        {
            var parser = new GnuOptionSetParser(new Option[] { Help, TypedFiles }, Files);

            var args = new string[] { "file1", "-h", "file2", "-xc++", "file3" };

            var parsedOpts = parser.Parse(args, TestEnvironment.GlobalLog);

            Assert.AreEqual(
                new string[] { "file1", "file2" },
                parsedOpts.GetValue<string[]>(Files));

            Assert.IsTrue(parsedOpts.GetValue<bool>(Help));

            Assert.AreEqual(
                new string[] { "c++", "file3" },
                parsedOpts.GetValue<string[]>(TypedFiles));
        }

        [Test]
        public void ErrorOnMissingIntegerArgument()
        {
            var parser = new GnuOptionSetParser(new Option[] { OptimizationLevel });

            var args = new string[] { "-O" };

            AssertError(parser, args);
        }

        [Test]
        public void EndOfOptionsTreatsFollowingArgumentsAsPositional()
        {
            var parser = new GnuOptionSetParser(new Option[] { Help }, Files);

            var parsedOpts = parser.Parse(
                new[] { "-h", "--", "--not-an-option", "-x" },
                TestEnvironment.GlobalLog);

            Assert.IsTrue(parsedOpts.GetValue<bool>(Help));
            Assert.AreEqual(
                new[] { "--not-an-option", "-x" },
                parsedOpts.GetValue<string[]>(Files));
        }

        [Test]
        public void ParseLongKeyValueOption()
        {
            var parser = new GnuOptionSetParser(new Option[] { Color });

            var parsedOpts = parser.Parse(
                new[] { "--color=always" },
                TestEnvironment.GlobalLog);

            Assert.AreEqual("always", parsedOpts.GetValue<string>(Color));
        }

        [Test]
        public void ParseNegativeFlagFormOverridesPositive()
        {
            var parser = new GnuOptionSetParser(new Option[] { Verbose });

            var parsedOpts = parser.Parse(
                new[] { "--verbose", "--no-verbose" },
                TestEnvironment.GlobalLog);

            Assert.IsFalse(parsedOpts.GetValue<bool>(Verbose));
        }

        [Test]
        public void RepeatedSequenceOptionPreservesEncounterOrder()
        {
            var parser = new GnuOptionSetParser(new Option[] { TypedFiles }, Files);

            var parsedOpts = parser.Parse(
                new[] { "-xc++", "a.cpp", "-xobjc", "b.m" },
                TestEnvironment.GlobalLog);

            CollectionAssert.AreEqual(
                new[] { "c++", "a.cpp", "objc", "b.m" },
                parsedOpts.GetValue<IReadOnlyList<string>>(TypedFiles));
        }

        [Test]
        public void UnknownOptionLogsSuggestedAlternative()
        {
            var parser = new GnuOptionSetParser(new Option[] { Help });
            var log = new RecordingLog();

            parser.Parse(new[] { "--hep" }, log);

            Assert.AreEqual(1, log.RecordedEntries.Count);
            Assert.AreEqual(Severity.Error, log.RecordedEntries[0].Severity);
            StringAssert.Contains("did you mean", Render(log.RecordedEntries[0].Contents));
            StringAssert.Contains("--help", Render(log.RecordedEntries[0].Contents));
        }

        [Test]
        public void UnexpectedFlagArgumentLogsUsage()
        {
            var parser = new GnuOptionSetParser(new Option[] { Help });
            var log = new RecordingLog();

            parser.Parse(new[] { "--help=yes" }, log);

            Assert.AreEqual(1, log.RecordedEntries.Count);
            StringAssert.Contains("unexpected argument", Render(log.RecordedEntries[0].Contents));
            StringAssert.Contains("usage: ", Render(log.RecordedEntries[0].Contents));
            StringAssert.Contains("--help", Render(log.RecordedEntries[0].Contents));
        }

        [Test]
        public void InvalidIntegerArgumentLogsSpecificMessage()
        {
            var parser = new GnuOptionSetParser(new Option[] { OptimizationLevel });
            var log = new RecordingLog();

            parser.Parse(new[] { "-Onope" }, log);

            Assert.AreEqual(1, log.RecordedEntries.Count);
            StringAssert.Contains("should be an integer", Render(log.RecordedEntries[0].Contents));
            StringAssert.Contains("-O", Render(log.RecordedEntries[0].Contents));
            StringAssert.Contains("nope", Render(log.RecordedEntries[0].Contents));
        }

        [Test]
        public void TryGetValueReturnsDefaultWhenOptionIsMissing()
        {
            var parser = new GnuOptionSetParser(new Option[] { Color });
            var parsedOpts = parser.Parse(new string[] { }, TestEnvironment.GlobalLog);

            string value;
            var found = parsedOpts.TryGetValue<string>(Color, out value);

            Assert.IsFalse(found);
            Assert.AreEqual("auto", value);
            Assert.IsFalse(parsedOpts.ContainsOption(Color));
        }

        [Test]
        public void ParseOptionFormFromCommandLineSpelling()
        {
            Assert.AreEqual(OptionForm.Short("h"), OptionForm.Parse("-h"));
            Assert.AreEqual(OptionForm.Long("help"), OptionForm.Parse("--help"));
        }

        [Test]
        public void HighLevelFlagBuilderCreatesAliasForms()
        {
            var help = Option.Flag("-h", "--help");
            var parser = new GnuOptionSetParser(new Option[] { help });

            var parsedOpts = parser.Parse(new[] { "--help" }, TestEnvironment.GlobalLog);

            Assert.IsTrue(parsedOpts.GetValue<bool>(help));
        }

        [Test]
        public void HighLevelValueBuilderParsesTypedValue()
        {
            var optimization = Option
                .Int32WithDefault(0, "-O", "--optimize")
                .WithDescription("Pick an optimization level.")
                .WithParameter("level");

            var parser = new GnuOptionSetParser(new Option[] { optimization });
            var parsedOpts = parser.Parse(new[] { "--optimize=3" }, TestEnvironment.GlobalLog);

            Assert.AreEqual(3, parsedOpts.GetValue<int>(optimization));
            StringAssert.Contains("level", Render(new OptionHelp(optimization, GnuOptionPrinter.Instance)));
        }

        [Test]
        public void HighLevelSequenceBuilderUsesSymbolicParameterHelper()
        {
            var files = Option
                .StringSequence("--file")
                .WithDescription("Consume files as input.")
                .WithParameter("path");

            var parser = new GnuOptionSetParser(new Option[] { files });
            var parsedOpts = parser.Parse(new[] { "--file", "a.txt", "b.txt" }, TestEnvironment.GlobalLog);

            CollectionAssert.AreEqual(
                new[] { "a.txt", "b.txt" },
                parsedOpts.GetValue<IReadOnlyList<string>>(files));
            StringAssert.Contains("path", Render(new OptionHelp(files, GnuOptionPrinter.Instance)));
        }

        [Test]
        public void ConvenienceStringBuilderUsesEmptyStringDefault()
        {
            var color = Option.String("--color");
            var parser = new GnuOptionSetParser(new Option[] { color });
            var parsedOpts = parser.Parse(new string[] { }, TestEnvironment.GlobalLog);

            Assert.AreEqual("", parsedOpts.GetValue<string>(color));
        }

        [Test]
        public void GenericValueBuilderParsesCustomType()
        {
            var mode = Option
                .Value(
                    (OptionForm form, string argument, ILog log) =>
                        argument == null ? 0 : argument.Length,
                    0,
                    "--mode")
                .WithParameter("name");

            var parser = new GnuOptionSetParser(new Option[] { mode });
            var parsedOpts = parser.Parse(new[] { "--mode=fast" }, TestEnvironment.GlobalLog);

            Assert.AreEqual(4, parsedOpts.GetValue<int>(mode));
        }

        [Test]
        public void GenericSequenceBuilderParsesCustomType()
        {
            var lengths = Option.Sequence(
                (OptionForm form, string argument, ILog log) => argument.Length,
                "--name-length");

            var parser = new GnuOptionSetParser(new Option[] { lengths });
            var parsedOpts = parser.Parse(
                new[] { "--name-length", "Ada", "Grace" },
                TestEnvironment.GlobalLog);

            CollectionAssert.AreEqual(
                new[] { 3, 5 },
                parsedOpts.GetValue<IReadOnlyList<int>>(lengths));
        }

        [Test]
        public void CommandLineFacadeParsesOptionsAndPositionals()
        {
            var help = Option.Flag("-h", "--help");
            var files = Option.StringSequence("--files")
                .WithParameter("file");
            var commandLine = new CommandLine(
                new Option[] { help, files },
                files);

            var result = commandLine.Parse(new[] { "a.txt", "-h", "b.txt" });

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.GetValue<bool>(help));
            CollectionAssert.AreEqual(
                new[] { "a.txt", "b.txt" },
                result.GetValue<IReadOnlyList<string>>(files));
        }

        [Test]
        public void CommandLineFacadeCapturesErrors()
        {
            var help = Option.Flag("--help");
            var commandLine = new CommandLine(help);

            var result = commandLine.Parse(new[] { "--unknown" });

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Contains(Severity.Error));
            Assert.AreEqual(1, result.Diagnostics.Count);
        }

        [Test]
        public void CommandLineFacadeForwardsDiagnosticsToProvidedLog()
        {
            var help = Option.Flag("--help");
            var forwardingLog = new RecordingLog();
            var commandLine = new CommandLine(help);

            var result = commandLine.Parse(new[] { "--unknown" }, forwardingLog);

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(1, forwardingLog.RecordedEntries.Count);
            Assert.AreEqual(1, result.Diagnostics.Count);
        }

        [Test]
        public void CommandLineCanPrintGeneratedHelpAndRecommendExit()
        {
            var files = Option.StringSequence("--files").WithParameter("file");
            var commandLine = new CommandLine(files)
                .WithHelp("Example program.", "example [files-or-options]");
            var log = new RecordingLog();

            var result = commandLine.Parse(new[] { "--help" }, log);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.WasHandled);
            Assert.AreEqual(0, result.ExitCode);
            Assert.IsTrue(result.WasHelpRequested);
            StringAssert.Contains("Description", Render(log.RecordedEntries[0].Contents));
            StringAssert.Contains("example [files-or-options]", Render(log.RecordedEntries[0].Contents));
        }

        [Test]
        public void CommandLineCanPrintVersionAndRecommendExit()
        {
            var commandLine = new CommandLine()
                .WithVersion("example 1.2.3");
            var log = new RecordingLog();

            var result = commandLine.Parse(new[] { "--version" }, log);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.WasHandled);
            Assert.AreEqual(0, result.ExitCode);
            Assert.IsTrue(result.WasVersionRequested);
            StringAssert.Contains("example 1.2.3", Render(log.RecordedEntries[0].Contents));
        }

        [Test]
        public void ParseErrorsRecommendNonZeroExitCode()
        {
            var commandLine = new CommandLine();

            var result = commandLine.Parse(new[] { "--unknown" });

            Assert.IsFalse(result.IsSuccess);
            Assert.IsFalse(result.WasHandled);
            Assert.AreEqual(1, result.ExitCode);
        }

        private static string Render(MarkupNode node)
        {
            return RenderTests.Render(node).Trim();
        }

        private void AssertError(OptionSetParser parser, IReadOnlyList<string> args)
        {
            Assert.Throws<PixieException>(new ParseAndDiscardClosure(parser, args).Parse);
        }
    }

    internal sealed class ParseAndDiscardClosure
    {
        public ParseAndDiscardClosure(OptionSetParser parser, IReadOnlyList<string> args)
        {
            this.parser = parser;
            this.args = args;
        }

        private OptionSetParser parser;
        private IReadOnlyList<string> args;

        public void Parse()
        {
            parser.Parse(
                args,
                new TestLog(
                    new Severity[] { Severity.Error },
                    NullLog.Instance));
        }
    }
}
