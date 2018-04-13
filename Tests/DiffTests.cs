using NUnit.Framework;

namespace Pixie.Tests
{
    [TestFixture]
    public class DiffTests
    {
        // Unit tests adapted from Python simplediff unit tests.
        // Original source code at
        // https://github.com/paulgb/simplediff.

        [Test]
        public void DiffWords1()
        {
            // "old": ["The", "quick", "brown", "fox"],
            // "new": ["The", "slow", "green", "turtle"],
            // "diff": [("=", ["The"]),
            //          ("-", ["quick", "brown", "fox"]),
            //          ("+", ["slow", "green", "turtle"])]

            var oldWords = new[] { "The", "quick", "brown", "fox" };
            var newWords = new[] { "The", "slow", "green", "turtle" };
            var expectedDiff = new Diff<string>(
                new DiffElement<string>[]
                {
                    new DiffElement<string>(DiffOperation.Unchanged, new[] { "The" }),
                    new DiffElement<string>(DiffOperation.Deletion, new[] { "quick", "brown", "fox" }),
                    new DiffElement<string>(DiffOperation.Insertion, new[] { "slow", "green", "turtle" })
                });

            Assert.AreEqual(expectedDiff, Diff.Create<string>(oldWords, newWords));
        }

        [Test]
        public void DiffWords2()
        {
            // "old": ["jumps", "over", "the", "lazy", "dog"],
            // "new": ["walks", "around", "the", "orange", "cat"],
            // "diff": [("-", ["jumps", "over"]),
            //          ("+", ["walks", "around"]),
            //          ("=", ["the"]),
            //          ("-", ["lazy", "dog"]),
            //          ("+", ["orange", "cat"])]

            var oldWords = new[] { "jumps", "over", "the", "lazy", "dog" };
            var newWords = new[] { "walks", "around", "the", "orange", "cat" };
            var expectedDiff = new Diff<string>(
                new DiffElement<string>[]
                {
                    new DiffElement<string>(DiffOperation.Deletion, new[] { "jumps", "over" }),
                    new DiffElement<string>(DiffOperation.Insertion, new[] { "walks", "around" }),
                    new DiffElement<string>(DiffOperation.Unchanged, new[] { "the" }),
                    new DiffElement<string>(DiffOperation.Deletion, new[] { "lazy", "dog" }),
                    new DiffElement<string>(DiffOperation.Insertion, new[] { "orange", "cat" })
                });

            Assert.AreEqual(expectedDiff, Diff.Create<string>(oldWords, newWords));
        }

        [Test]
        public void DiffChars1()
        {
            // "old": "The quick brown fox.",
            // "new": "The kuick brown fix.",
            // "diff": [("=", "The "),
            //          ("-", "q"),
            //          ("+", "k"),
            //          ("=", "uick brown f"),
            //          ("-", "o"),
            //          ("+", "i"),
            //          ("=", "x.")]

            var oldStr = "The quick brown fox.";
            var newStr = "The kuick brown fix.";
            var expectedDiff = new Diff<char>(
                new DiffElement<char>[]
                {
                    new DiffElement<char>(DiffOperation.Unchanged, "The "),
                    new DiffElement<char>(DiffOperation.Deletion, "q"),
                    new DiffElement<char>(DiffOperation.Insertion, "k"),
                    new DiffElement<char>(DiffOperation.Unchanged, "uick brown f"),
                    new DiffElement<char>(DiffOperation.Deletion, "o"),
                    new DiffElement<char>(DiffOperation.Insertion, "i"),
                    new DiffElement<char>(DiffOperation.Unchanged, "x.")
                });

            Assert.AreEqual(expectedDiff, Diff.Create<char>(oldStr, newStr));
        }

        [Test]
        public void DiffChars2()
        {
            // Regression test.

            // "old": "The quick brown fox.",
            // "new": "The kuick brown fix.",
            // "diff": [("=", "The "),
            //          ("-", "q"),
            //          ("+", "k"),
            //          ("=", "uick brown f"),
            //          ("-", "o"),
            //          ("+", "i"),
            //          ("=", "x.")]

            var oldStr = "The quick brown fox.";
            var newStr = "The kuick brown fix.";
            var expectedDiff = new Diff<char>(
                new DiffElement<char>[]
                {
                    new DiffElement<char>(DiffOperation.Unchanged, "The "),
                    new DiffElement<char>(DiffOperation.Deletion, "q"),
                    new DiffElement<char>(DiffOperation.Insertion, "k"),
                    new DiffElement<char>(DiffOperation.Unchanged, "uick brown f"),
                    new DiffElement<char>(DiffOperation.Deletion, "o"),
                    new DiffElement<char>(DiffOperation.Insertion, "i"),
                    new DiffElement<char>(DiffOperation.Unchanged, "x.")
                });

            System.Console.WriteLine(Diff.Create<char>("-fsynxax only", "-fsyntax only"));
            System.Console.WriteLine(Diff.Create<char>(oldStr, newStr));
            Assert.AreEqual(expectedDiff, Diff.Create<char>(oldStr, newStr));
        }
    }
}
