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

        [Test]
        public void ParsePositionalArguments()
        {
            var parser = new GnuOptionSetParser(new Option[] { }, Files);

            var fileNames = new string[] { "file1", "file2" };

            var parsedOpts = parser.Parse(fileNames, Program.GlobalLog);

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

            var parsedOpts = parser.Parse(args, Program.GlobalLog);

            Assert.AreEqual(
                new string[] { "file1", "file2" },
                parsedOpts.GetValue<string[]>(Files));

            Assert.IsTrue(parsedOpts.GetValue<bool>(Help));

            Assert.AreEqual(
                new string[] { "c++", "file3" },
                parsedOpts.GetValue<string[]>(TypedFiles));
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
