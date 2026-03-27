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
