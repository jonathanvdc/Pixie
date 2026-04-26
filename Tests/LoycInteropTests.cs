using System;
using System.IO;
using Loyc;
using Loyc.Collections;
using Loyc.Syntax;
using NUnit.Framework;
using Pixie.Code;
using Pixie.Loyc;
using Pixie.Markup;
using Pixie.Terminal;
using Pixie.Terminal.Devices;

namespace Pixie.Tests
{
    [TestFixture]
    public class LoycInteropTests
    {
        public LoycInteropTests()
        {
            this.rng = new Random();
        }

        private Random rng;

        private const string TestSource = "int int x = 10; class A\n{\n \n";

        [Test]
        public void LoycSourceDocumentPositions()
        {
            var file = new SourceFile<ICharSource>(new UString(TestSource), "input.cs");
            var stringDoc = new StringDocument("input.cs", TestSource);
            var loycDoc = file.ToSourceDocument();

            for (int i = 0; i < stringDoc.Length; i++)
            {
                var refPosition = stringDoc.GetPosition(i);
                var loycPosition = loycDoc.GetPosition(i);
                Assert.AreEqual(refPosition.Line, loycPosition.Line);
                Assert.AreEqual(refPosition.Column, loycPosition.Column);
            }
        }

        [Test]
        public void LoycSourceDocumentLineStarts()
        {
            var file = new SourceFile<ICharSource>(new UString(TestSource), "input.cs");
            var stringDoc = new StringDocument("input.cs", TestSource);
            var loycDoc = file.ToSourceDocument();

            Assert.AreEqual(stringDoc.LineCount, file.LineCount);
            Assert.AreEqual(stringDoc.LineCount, loycDoc.LineCount);
            for (int i = 0; i < stringDoc.LineCount; i++)
            {
                Assert.AreEqual(stringDoc.GetLineOffset(i), loycDoc.GetLineOffset(i));
            }
        }
    }
}
