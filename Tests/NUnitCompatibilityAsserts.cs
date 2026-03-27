using System.Collections;

namespace NUnit.Framework
{
    internal static class StringAssert
    {
        public static void Contains(string expected, string actual)
        {
            Assert.That(actual, Does.Contain(expected));
        }

        public static void StartsWith(string expected, string actual)
        {
            Assert.That(actual, Does.StartWith(expected));
        }

        public static void EndsWith(string expected, string actual)
        {
            Assert.That(actual, Does.EndWith(expected));
        }
    }

    internal static class CollectionAssert
    {
        public static void AreEqual(IEnumerable expected, IEnumerable actual)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
