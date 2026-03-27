using System.Collections.Generic;
using NUnit.Framework;
using Pixie.Markup;
using Pixie.Options;

namespace Pixie.Tests
{
    [TestFixture]
    public class OptionFormattingTests
    {
        [Test]
        public void GnuOptionPrinterUsesConcatenatedSyntaxForShortSingleParameter()
        {
            var rendered = RenderTests.Render(
                GnuOptionPrinter.Instance.Print(
                    OptionForm.Short("O"),
                    new OptionParameter[] { new SymbolicOptionParameter("level") }));

            Assert.AreEqual("-O<level>", rendered.Trim());
        }

        [Test]
        public void GnuOptionPrinterUsesEqualsSyntaxForLongSingleParameter()
        {
            var rendered = RenderTests.Render(
                GnuOptionPrinter.Instance.Print(
                    OptionForm.Long("color"),
                    new OptionParameter[] { new SymbolicOptionParameter("when") }));

            Assert.AreEqual("--color=<when>", rendered.Trim());
        }

        [Test]
        public void GnuOptionPrinterUsesGeneralSyntaxForVarargs()
        {
            var rendered = RenderTests.Render(
                GnuOptionPrinter.Instance.Print(
                    OptionForm.Short("x"),
                    new OptionParameter[] { new SymbolicOptionParameter("file", true) }));

            Assert.AreEqual("-x <file...>", rendered.Trim());
        }

        [Test]
        public void OptionDocsReturnsSharedParametersForEachForm()
        {
            var forms = new[] { OptionForm.Short("I"), OptionForm.Long("include") };
            var parameters = new OptionParameter[] { new SymbolicOptionParameter("dir") };
            var docs = new OptionDocs("Paths", "Include path", forms, parameters);

            CollectionAssert.AreEqual(parameters, docs.GetParameters(forms[0]));
            CollectionAssert.AreEqual(parameters, docs.GetParameters(forms[1]));
        }

        [Test]
        public void OptionDocsReturnsEmptyParametersForUnknownForm()
        {
            var docs = new OptionDocs(
                "Paths",
                "Include path",
                new Dictionary<OptionForm, IReadOnlyList<OptionParameter>>());

            Assert.AreEqual(0, docs.GetParameters(OptionForm.Long("missing")).Count);
        }

        [Test]
        public void OptionFormEqualityAndPrintingDependOnKindAndName()
        {
            var shortHelp = OptionForm.Short("h");
            var sameShortHelp = OptionForm.Short("h");
            var longHelp = OptionForm.Long("h");

            Assert.IsTrue(shortHelp == sameShortHelp);
            Assert.IsTrue(shortHelp.Equals(sameShortHelp));
            Assert.IsFalse(shortHelp == longHelp);
            Assert.AreEqual("-h", shortHelp.ToString());
            Assert.AreEqual("--h", longHelp.ToString());
        }
    }
}
