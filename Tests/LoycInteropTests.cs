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
        public void LoycSourceDocumentGrid()
        {
            var file = new SourceFile<ICharSource>(new UString(TestSource), "input.cs");
            var stringDoc = new StringDocument("input.cs", TestSource);
            var loycDoc = file.ToSourceDocument();

            for (int i = 0; i < stringDoc.Length; i++)
            {
                var refGridPos = stringDoc.GetGridPosition(i);
                var loycGridPos = loycDoc.GetGridPosition(i);
                Assert.AreEqual(refGridPos, loycGridPos);
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
