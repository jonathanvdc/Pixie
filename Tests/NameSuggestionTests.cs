using NUnit.Framework;

namespace Pixie.Tests
{
    [TestFixture]
    public class NameSuggestionTests
    {
        [Test]
        public void SuggestMostSimilarNames()
        {
            var validNames = new[] { "-x", "--help", "c++" };
            Assert.AreEqual("-x", NameSuggestion.SuggestName("x", validNames));
            Assert.AreEqual("--help", NameSuggestion.SuggestName("-help", validNames));
            Assert.AreEqual("c++", NameSuggestion.SuggestName("d++", validNames));
        }
    }
}
