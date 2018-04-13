using System;
using System.Collections.Generic;
using System.Linq;

namespace Pixie
{
    /// <summary>
    /// A helper class that creates diffs.
    /// </summary>
    public static class Diff
    {
        /// <summary>
        /// Creates a diff from an old sequence and a new sequence.
        /// </summary>
        /// <param name="oldSequence">A 'before' sequence for the diff.</param>
        /// <param name="newSequence">An 'after' sequence of the diff.</param>
        /// <returns>
        /// A diff that, when applied, turns <paramref name="oldSequence"/> into
        /// <paramref name="newSequence"/>.
        /// </returns>
        public static Diff<T> Create<T>(
            IEnumerable<T> oldSequence,
            IEnumerable<T> newSequence)
        {
            return Create<T>(oldSequence, newSequence, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Creates a diff from an old sequence, a new sequence
        /// and an equality comparer.
        /// </summary>
        /// <param name="oldSequence">A 'before' sequence for the diff.</param>
        /// <param name="newSequence">An 'after' sequence of the diff.</param>
        /// <param name="comparer">
        /// The equality comparer to use for comparing elements.
        /// </param>
        /// <returns>
        /// A diff that, when applied, turns <paramref name="oldSequence"/> into
        /// <paramref name="newSequence"/>.
        /// </returns>
        public static Diff<T> Create<T>(
            IEnumerable<T> oldSequence,
            IEnumerable<T> newSequence,
            IEqualityComparer<T> comparer)
        {
            // Algorithm based on the Python version of simplediff by
            // Paul Butler. Original source code at
            // https://github.com/paulgb/simplediff.
            // Comments are mostly preserved from original algorithm.

            // Create a map from old values to their indices.
            var oldIndexMap = new Dictionary<T, List<int>>(comparer);
            int oldSequenceLength = 0;
            {
                foreach (var val in oldSequence)
                {
                    List<int> indexList;
                    if (!oldIndexMap.TryGetValue(val, out indexList))
                    {
                        indexList = new List<int>();
                        oldIndexMap[val] = indexList;
                    }
                    indexList.Add(oldSequenceLength);
                    oldSequenceLength++;
                }
            }

            // Find the largest substring common to old and new.
            // We use a dynamic programming approach here.
            //
            // We iterate over each value in the `new` list, calling the
            // index `inew`. At each iteration, `overlap[i]` is the
            // length of the largest suffix of `old[:i]` equal to a suffix
            // of `new[:inew]` (or unset when `old[i]` != `new[inew]`).
            //
            // At each stage of iteration, the new `overlap` (called
            // `_overlap` until the original `overlap` is no longer needed)
            // is built from the old one.
            //
            // If the length of overlap exceeds the largest substring
            // seen so far (`sub_length`), we update the largest substring
            // to the overlapping strings.

            var overlap = new Dictionary<int, int>();

            // `sub_start_old` is the index of the beginning of the largest overlapping
            // substring in the old list. `sub_start_new` is the index of the beginning
            // of the same substring in the new list. `sub_length` is the length that
            // overlaps in both.
            // These track the largest overlapping substring seen so far, so naturally
            // we start with a 0-length substring.
            var sub_start_old = 0;
            var sub_start_new = 0;
            var sub_length = 0;

            int inew = 0;
            foreach (var val in newSequence)
            {
                var _overlap = new Dictionary<int, int>();
                List<int> oldIndexList;
                if (oldIndexMap.TryGetValue(val, out oldIndexList))
                {
                    for (int i = 0; i < oldIndexList.Count; i++)
                    {
                        int iold = oldIndexList[i];
                        // Now we are considering all values of iold such that
                        // `old[iold] == new[inew]`.
                        _overlap[iold] = (iold == 0 ? 0 : GetOrDefault<int, int>(overlap, iold - 1, 0)) + 1;
                        if (_overlap[iold] > sub_length)
                        {
                            // This is the largest substring seen so far, so store its
                            // indices.
                            sub_length = _overlap[iold];
                            sub_start_old = iold - sub_length + 1;
                            sub_start_new = inew - sub_length + 1;
                        }
                    }
                    overlap = _overlap;
                }

                inew++;
            }
            int newSequenceLength = inew;

            var diffElems = new List<DiffElement<T>>();
            if (sub_length == 0)
            {
                // If no common substring is found, we return an insert and delete...
                if (oldSequenceLength > 0)
                {
                    diffElems.Add(
                        new DiffElement<T>(
                            DiffOperation.Deletion,
                            oldSequence));
                }
                if (newSequenceLength > 0)
                {
                    diffElems.Add(
                        new DiffElement<T>(
                            DiffOperation.Insertion,
                            newSequence));
                }
            }
            else
            {
                // ...otherwise, the common substring is unchanged and we recursively
                // diff the text before and after that substring.
                diffElems.AddRange(
                    Create<T>(
                        oldSequence.Take<T>(sub_start_old),
                        newSequence.Take<T>(sub_start_new),
                        comparer)
                    .Elements);

                diffElems.Add(
                    new DiffElement<T>(
                        DiffOperation.Unchanged,
                        newSequence
                            .Skip<T>(sub_start_new)
                            .Take<T>(sub_length)));

                diffElems.AddRange(
                    Create<T>(
                        oldSequence.Skip<T>(sub_start_old + sub_length),
                        newSequence.Skip<T>(sub_start_new + sub_length),
                        comparer)
                    .Elements);
            }
            return new Diff<T>(diffElems);
        }

        private static V GetOrDefault<K, V>(Dictionary<K, V> dictionary, K key, V defaultValue)
        {
            V result;
            if (dictionary.TryGetValue(key, out result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }
    }

    /// <summary>
    /// Represents the changes required to turn one sequence
    /// into another.
    /// </summary>
    public struct Diff<T> : IEquatable<Diff<T>>
    {
        /// <summary>
        /// Creates a diff from a sequence of diff elements.
        /// </summary>
        /// <param name="elements">The diff elements.</param>
        public Diff(IReadOnlyList<DiffElement<T>> elements)
        {
            this.Elements = elements;
        }

        /// <summary>
        /// Gets the elements in this diff.
        /// </summary>
        /// <returns>The diff elements.</returns>
        public IReadOnlyList<DiffElement<T>> Elements { get; private set; }

        /// <summary>
        /// Recreates the old sequence, that is, the sequence
        /// prior to applying this diff.
        /// </summary>
        /// <returns>The old sequence.</returns>
        public IReadOnlyList<T> OldSequence
        {
            get
            {
                var result = new List<T>();
                int elemCount = Elements.Count;
                for (int i = 0; i < elemCount; i++)
                {
                    var elem = Elements[i];
                    if (elem.Operation != DiffOperation.Insertion)
                    {
                        result.AddRange(elem.Values);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Recreates the new sequence, that is, the sequence
        /// produced by applying this diff.
        /// </summary>
        /// <returns>The new sequence.</returns>
        public IReadOnlyList<T> NewSequence
        {
            get
            {
                var result = new List<T>();
                int elemCount = Elements.Count;
                for (int i = 0; i < elemCount; i++)
                {
                    var elem = Elements[i];
                    if (elem.Operation != DiffOperation.Deletion)
                    {
                        result.AddRange(elem.Values);
                    }
                }
                return result;
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Diff<T> && Equals((Diff<T>)obj);
        }

        /// <summary>
        /// Tests if this diff is the same as another diff.
        /// </summary>
        /// <param name="other">The other diff.</param>
        /// <returns>
        /// <c>true</c> if this diff equals the other diff; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Diff<T> other)
        {
            return Enumerable.SequenceEqual<DiffElement<T>>(Elements, other.Elements);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int elemCount = Elements.Count;
            int hash = 0;
            for (int i = 0; i < elemCount; i++)
            {
                hash = (hash << 1) ^ Elements[i].GetHashCode();
            }
            return hash;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "[" + string.Join<DiffElement<T>>(", ", Elements) + "]";
        }
    }

    /// <summary>
    /// An enumeration of possible operations a diff element
    /// can perform.
    /// </summary>
    public enum DiffOperation
    {
        /// <summary>
        /// The diff element leaves a number of values in place.
        /// </summary>
        Unchanged,

        /// <summary>
        /// The diff inserts a number of new values into the
        /// sequence.
        /// </summary>
        Insertion,

        /// <summary>
        /// The diff deletes a number of old values from the
        /// sequence.
        /// </summary>
        Deletion
    }

    /// <summary>
    /// An element of a diff.
    /// </summary>
    public struct DiffElement<T> : IEquatable<DiffElement<T>>
    {
        /// <summary>
        /// Creates a diff element.
        /// </summary>
        /// <param name="operation">
        /// The operation to perform.
        /// </param>
        /// <param name="values">
        /// The sequence of values to operate on.
        /// </param>
        public DiffElement(
            DiffOperation operation,
            IEnumerable<T> values)
            : this(operation, values.ToArray<T>())
        { }

        /// <summary>
        /// Creates a diff element.
        /// </summary>
        /// <param name="operation">
        /// The operation to perform.
        /// </param>
        /// <param name="values">
        /// The sequence of values to operate on.
        /// </param>
        public DiffElement(
            DiffOperation operation,
            IReadOnlyList<T> values)
        {
            this.Operation = operation;
            this.Values = values;
        }

        /// <summary>
        /// Gets the operation this diff element performs.
        /// </summary>
        /// <returns>The operation it performs.</returns>
        public DiffOperation Operation { get; private set; }

        /// <summary>
        /// Gets the sequence of values this diff element inserts,
        /// deletes or leaves unchanged.
        /// </summary>
        /// <returns>The sequence of values.</returns>
        public IReadOnlyList<T> Values { get; private set; }

        /// <summary>
        /// Tests if this diff elements equals another diff element.
        /// </summary>
        /// <param name="other">A diff element to test for equality.</param>
        /// <returns>
        /// <c>true</c> if this diff element equals the other diff element;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(DiffElement<T> other)
        {
            return Operation == other.Operation
                && Enumerable.SequenceEqual<T>(Values, other.Values);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is DiffElement<T> && Equals((DiffElement<T>)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int valueCount = Values.Count;
            int hash = Operation.GetHashCode();
            for (int i = 0; i < valueCount; i++)
            {
                hash = (hash << 1) ^ Values[i].GetHashCode();
            }
            return hash;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string prefix = Operation == DiffOperation.Insertion
                ? "+"
                : Operation == DiffOperation.Deletion
                    ? "-"
                    : "=";
            return prefix + " " + string.Concat<T>(Values);
        }
    }
}
