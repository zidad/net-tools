using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Net.System;
using Net.Text;

namespace Net.Collections
{
    public static class EnumerableExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> toAdd)
        {
            toAdd.ForEach(collection.Add);
        }

        public static IEnumerable<TTarget> SafeCast<TTarget>(this IEnumerable source)
        {
            return source == null ? null : source.Cast<TTarget>();
        }

        /// <summary>
        /// performs a foreach on the IEnumerable<typeparamref name="T"/> and executes the action.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
        {
            foreach (var obj in e)
                action(obj);
        }

        /// <summary>
        /// recursively returns all items in a tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">source</param>
        /// <param name="descendBySelector">descend by</param>
        /// <returns></returns>
        public static IEnumerable<T> Descendants<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> descendBySelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (descendBySelector == null) throw new ArgumentNullException("descendBySelector");

            foreach (var item in source)
            {
                yield return item;
                foreach (T childItem in Descendants(descendBySelector(item), descendBySelector))
                    yield return childItem;
            }
        }

        public static IEnumerable<T> SkipEmpty<T>(this IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            return items.Where(item => !ReferenceEquals(item, null));
        }

        /// <summary>
        /// Concatenates a sequence of items to a string.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the input sequence.</typeparam>
        /// <param name="items">The input sequence.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The concatenated string.</returns>
        public static string ConcatToString<T>(this IEnumerable<T> items, string separator)
        {
            return String.Join(
                separator,
                items.
                    ToList().
                    ConvertAll(item => item.To<string>()).
                    ToArray());
        }


        /// <summary>
        /// Concatenates a sequence of items to a string.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the input sequence.</typeparam>
        /// <param name="items">The input sequence.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="value">The value.</param>
        /// <returns>The concatenated string.</returns>
        public static string ConcatToString<T>(this IEnumerable<T> items, Func<T, object> value, string separator)
        {
            return String.Join(
                separator,
                items.
                    ToList().
                    ConvertAll(item => value.Invoke(item).To<string>()).
                    ToArray());
        }

        /// <summary>
        /// Concatenates a sequence of items to a string.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the input sequence.</typeparam>
        /// <param name="items">The input sequence.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="formattedItem">The formatted item string.</param>
        /// <param name="args">A lamdba expression that returns an object array containing zero or more objects to format.</param>
        /// <returns>The concatenated string.</returns>
        public static string ConcatToString<T>(this IEnumerable<T> items, string formattedItem, Func<T, object[]> args, string separator)
        {
            return String.Join(
                separator,
                items.
                    ToList().
                    ConvertAll(item => formattedItem.FormatWith(args.Invoke(item))).
                    ToArray());
        }

        /// <summary>
        /// Concatenates a sequence of items to a string.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the input sequence.</typeparam>
        /// <param name="items">The input sequence.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="formattedItemString">The formatted item string.</param>
        /// <param name="arg">A lamdba expression that returns an object containing the object to format.</param>
        /// <returns>The concatenated string.</returns>
        public static string ConcatToString<T>(this IEnumerable<T> items, string formattedItemString, Func<T, object> arg, string separator)
        {
            return String.Join(
                separator,
                items.
                    ToList().
                    ConvertAll(item => formattedItemString.FormatWith(arg.Invoke(item))).
                    ToArray());
        }


        /// <summary>
        /// loops through an IEnumerable and performs an action on each item, passes the index of the item to the action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">source</param>
        /// <param name="action">The action.</param>
        static public void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var i = 0;
            foreach (var value in source)
            {
                action(value, i);
                i++;
            }
        }

        // Stolen from http://www.make-awesome.com/2010/08/batch-or-partition-a-collection-with-linq/
        static public IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize) 
        {
            var batch = new List<T>(batchSize);

            foreach (T item in source)
            {
                batch.Add(item);
                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<T>(batchSize);
                }
            }

            if (batch.Count > 0)
                yield return batch;
        }

        // ReSharper disable UnusedParameter.Global 
        // value 'o' is not used, but is required to invoke this method as an extension method
        public static PropertyEqualityComparer<TObject, TProperty> CompareProperty<TObject, TProperty>(this TObject o, Func<TObject, TProperty> getter)
            // ReSharper restore UnusedParameter.Global
        {
            if (getter == null) throw new ArgumentNullException("getter");
            return new PropertyEqualityComparer<TObject, TProperty>(getter);
        }

        public static IEnumerable<T1> Intersect<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, bool> predicate)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (predicate == null) throw new ArgumentNullException("predicate");
            return first.Where(variable => second.Any(arg => predicate(variable, arg)));
        }

        public static IEnumerable<TObject> Intersect<TObject, TProperty>(this IEnumerable<TObject> first, IEnumerable<TObject> second, Func<TObject, TProperty> getter)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (getter == null) throw new ArgumentNullException("getter");
            return first.Where(variable => second.Any(arg => Equals(getter(variable), getter(arg))));
        }

        public static IEnumerable<T> Distinct<T, TProperty>(this IEnumerable<T> e, Func<T, TProperty> getter)
        {
            if (e == null) throw new ArgumentNullException("e");
            if (getter == null) throw new ArgumentNullException("getter");
            return e.Distinct(new PropertyEqualityComparer<T, TProperty>(getter));
        }

        public static IEnumerable<T> Except<T, TProperty>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TProperty> getter)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (getter == null) throw new ArgumentNullException("getter");
            return first.Except(second, new PropertyEqualityComparer<T, TProperty>(getter));
        }

        public static IEnumerable<T1> Except<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, bool> predicate)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (predicate == null) throw new ArgumentNullException("predicate");
            return first.Where(variable => !second.Any(arg => predicate(variable, arg)));
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> first, T second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            return first.Except(new[] { second });
        }

        public static IEnumerable<T> Except<T, TProperty>(this IEnumerable<T> first, T second, Func<T, TProperty> getter)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (getter == null) throw new ArgumentNullException("getter");
            return first.Except(new[] { second }, new PropertyEqualityComparer<T, TProperty>(getter));
        }

        public static IEnumerable<T1> Except<T1, T2>(this IEnumerable<T1> first, T2 second, Func<T1, T2, bool> predicate)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (predicate == null) throw new ArgumentNullException("predicate");
            return first.Except(new[] { second }, predicate);
        }

        public static IEnumerable<T> Union<T, TProperty>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TProperty> getter)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (getter == null) throw new ArgumentNullException("getter");
            return first.Union(second, new PropertyEqualityComparer<T, TProperty>(getter));
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, T second)
        {
            if (ReferenceEquals(first, null)) throw new ArgumentNullException("first");
            if (ReferenceEquals(second, null)) throw new ArgumentNullException("second");
            return first.Concat(new[] { second });
        }

        public static IEnumerable<T> ConcatIfNotNull<T>(this IEnumerable<T> first, T second)
        {
            first = first ?? Enumerable.Empty<T>();
            if (!ReferenceEquals(second, null))
                first = first.Concat(second);
            return first;
        }

        public static IEnumerable<T> Insert<T>(this IEnumerable<T> second, T first)
        {
            if (ReferenceEquals(first, null)) throw new ArgumentNullException("first");
            if (ReferenceEquals(second, null)) throw new ArgumentNullException("second");
            return new[] { first }.Concat(second);
        }


        /// <summary>
        /// Returns the index of the first occurence of an item in a IEnumerable, based on a property of T
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="o">The object to match.</param>
        /// <param name="getter">The property getter.</param>
        /// <returns></returns>
        public static int IndexOf<T, TP>(this IEnumerable<T> source, T o, Func<T, TP> getter)
            where TP : struct
        {
            if (source == null) throw new ArgumentNullException("source");
            if (getter == null) throw new ArgumentNullException("getter");
            return source.IndexOf(o, new PropertyEqualityComparer<T, TP>(getter));
        }

        /// <summary>
        /// Returns the index of the first occurence of an item in a IEnumerable, based on a custom IEqualityComparer
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="o">The object to match.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> source, T o, IEqualityComparer<T> comparer)
        {
            int i = 0;
            foreach (var enumerable in source)
            {
                if (comparer.Equals(enumerable, o))
                    return i;
                i++;
            }
            return -1;
        }


        /// <summary>
        /// Returns the index of the first occurence of an item in a IEnumerable, based on a custom IEqualityComparer
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="o">The object to match.</param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> source, T o)
        {
            int i = 0;
            foreach (var enumerable in source)
            {
                if (Equals(enumerable, o))
                    return i;
                i++;
            }
            return -1;
        }

        public static T SingleOrThrow<T>(this IEnumerable<T> source, string failureMessage)
            where T : class
        {
            T single = source.SingleOrDefault();

            if (single == null)
                throw new NotSupportedException(failureMessage + ". The query should have resulted in 1 element due to the usage of Single(), though resulted in 0 elements being returned.");

            return single;
        }

        public static T SingleOrThrow<T>(this IEnumerable<T> source, Func<T, bool> predicate, string failureMessage)
            where T : class
        {
            T single = source.SingleOrDefault(predicate);

            if (single == null)
                throw new NotSupportedException(failureMessage + ". The query should have resulted in 1 element due to the usage of Single(), though resulted in 0 elements being returned.");

            return single;
        }

        public static T FirstOrThrow<T>(this IEnumerable<T> source, Func<T, bool> predicate, string failureMessage)
            where T : class
        {
            T first = source.FirstOrDefault(predicate);

            if (first == null)
                throw new NotSupportedException(failureMessage + ". The query should have resulted in at least 1 element due to the usage of First(), though resulted in 0 elements being returned.");

            return first;
        }

        public static T FirstOrThrow<T>(this IEnumerable<T> source, string failureMessage)
            where T : class
        {
            T first = source.FirstOrDefault();

            if (first == null)
                throw new NotSupportedException(failureMessage + ". The query should have resulted in at least 1 element due to the usage of First(), though resulted in 0 elements being returned.");

            return first;
        }

        /// <summary>
        /// This will recurse through a tree of items and return it as a flat IEnumerable[Recursive[T]]
        /// bug: there's an issue with this method, the 'NextSibling' will only be set after the next item has been enumerated.
        /// Make sure to enumerate through the entire list (call something like 'ToList()') if you require the 'NextSibling' property to be set.
        /// </summary>
        public static IEnumerable<Recursive<T>> SelectRecursive<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (childSelector == null) throw new ArgumentNullException("childSelector");
            return SelectRecursive(items, childSelector, null, 0);
        }

        /// <summary>
        /// See limitations of SelectRecursive[T]()
        /// </summary>
        static IEnumerable<Recursive<T>> SelectRecursive<T>(IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector, Recursive<T> parent, int depth)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (childSelector == null) throw new ArgumentNullException("childSelector");

            int indexInParent = 0;
            Recursive<T> previous = null;

            foreach (var item in items)
            {
                var newItem = new Recursive<T>
                {
                    Item = item,
                    Parent = parent,
                    PreviousSibling = previous,
                    Depth = depth,
                    IndexInParent = indexInParent,
                };

                //hack:we probably should read one item in advance, modifying an item that has already been returned is evil
                if (previous != null)
                    previous.NextSibling = newItem;

                yield return newItem;

                foreach (var childItem in SelectRecursive(childSelector(item), childSelector, newItem, depth + 1))
                    yield return childItem;

                previous = newItem;
                indexInParent++;
            }
        }


        public class Recursive<T>
        {
            public int Depth { get; set; }
            public int IndexInParent { get; set; }
            public T Item { get; set; }
            public Recursive<T> Parent { get; set; }
            public Recursive<T> PreviousSibling { get; set; }
            public Recursive<T> NextSibling { get; set; }
        }

    }
}