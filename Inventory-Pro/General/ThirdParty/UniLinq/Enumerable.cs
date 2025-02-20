//
// Enumerable.cs
//
// Authors:
//  Marek Safar (marek.safar@gmail.com)
//  Antonello Provenzano  <antonello@deveel.com>
//  Alejandro Serrano "Serras" (trupill@yahoo.es)
//  Jb Evain (jbevain@novell.com)
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace Devdog.General.ThirdParty.UniLinq
{
	public static class Enumerable
	{
		enum Fallback
		{
			Default,
			Throw
		}

		#region Aggregate

		public static TSource Aggregate<TSource> (this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
		{
			Check.SourceAndFunc (source, func);

			// custom foreach so that we can efficiently throw an exception
			// if zero elements and treat the first element differently
			using (var enumerator = source.GetEnumerator ()) {
				if (!enumerator.MoveNext ())
					throw EmptySequence ();

				TSource folded = enumerator.Current;
				while (enumerator.MoveNext ())
					folded = func (folded, enumerator.Current);
				return folded;
			}
		}

		public static TAccumulate Aggregate<TSource, TAccumulate> (this IEnumerable<TSource> source,
			TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
		{
			Check.SourceAndFunc (source, func);

			TAccumulate folded = seed;
			foreach (TSource element in source)
				folded = func (folded, element);

			return folded;
		}

		public static TResult Aggregate<TSource, TAccumulate, TResult> (this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
		{
			Check.SourceAndFunc (source, func);
			if (resultSelector == null)
				throw new ArgumentNullException ("resultSelector");

			var result = seed;
			foreach (var e in source)
				result = func (result, e);

			return resultSelector (result);
		}

		#endregion

		#region All

		public static bool All<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			foreach (var element in source)
				if (!predicate (element))
					return false;

			return true;
		}

		#endregion

		#region Any

		public static bool Any<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			var collection = source as ICollection<TSource>;
			if (collection != null)
				return collection.Count > 0;

			using (var enumerator = source.GetEnumerator ())
				return enumerator.MoveNext ();
		}

		public static bool Any<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			foreach (TSource element in source)
				if (predicate (element))
					return true;

			return false;
		}

		#endregion

		#region AsEnumerable

		public static IEnumerable<TSource> AsEnumerable<TSource> (this IEnumerable<TSource> source)
		{
			return source;
		}

		#endregion

		#region Average

		public static double Average (this IEnumerable<int> source)
		{
			Check.Source (source);

			long total = 0;
			int count = 0;
			foreach (var element in source) {
				total = checked (total + element);
				count++;
			}
			if (count == 0)
				throw EmptySequence ();
			return total / (double)count;
		}

		public static double Average (this IEnumerable<long> source)
		{
			Check.Source (source);

			long total = 0;
			long count = 0;
			foreach (var element in source) {
				total += element;
				count++;
			}
			if (count == 0)
				throw EmptySequence ();
			return total / (double)count;
		}

		public static double Average (this IEnumerable<double> source)
		{
			Check.Source (source);

			double total = 0;
			long count = 0;
			foreach (var element in source) {
				total += element;
				count++;
			}
			if (count == 0)
				throw EmptySequence ();
			return total / count;
		}

		public static float Average (this IEnumerable<float> source)
		{
			Check.Source (source);

			float total = 0;
			long count = 0;
			foreach (var element in source) {
				total += element;
				count++;
			}
			if (count == 0)
				throw EmptySequence ();
			return total / count;
		}

		public static decimal Average (this IEnumerable<decimal> source)
		{
			Check.Source (source);

			decimal total = 0;
			long count = 0;
			foreach (var element in source) {
				total += element;
				count++;
			}
			if (count == 0)
				throw EmptySequence ();
			return total / count;
		}

		static TResult? AverageNullable<TElement, TAggregate, TResult> (this IEnumerable<TElement?> source,
			Func<TAggregate, TElement, TAggregate> func, Func<TAggregate, long, TResult> result)
			where TElement : struct
			where TAggregate : struct
			where TResult : struct
		{
			Check.Source (source);

			var total = default (TAggregate);
			long counter = 0;
			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				total = func (total, element.Value);
				counter++;
			}

			if (counter == 0)
				return null;

			return new TResult? (result (total, counter));
		}

		public static double? Average (this IEnumerable<int?> source)
		{
			Check.Source (source);

			long total = 0;
			long counter = 0;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				total = total + element.Value;
				counter++;
			}

			if (counter == 0)
				return null;

			return new double? (total / (double)counter);
		}

		public static double? Average (this IEnumerable<long?> source)
		{
			Check.Source (source);

			long total = 0;
			long counter = 0;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				total = checked (total + element.Value);
				counter++;
			}

			if (counter == 0)
				return null;

			return new double? (total / (double)counter);

		}

		public static double? Average (this IEnumerable<double?> source)
		{
			Check.Source (source);

			double total = 0;
			long counter = 0;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				total = total + element.Value;
				counter++;
			}

			if (counter == 0)
				return null;

			return new double? (total / counter);

		}

		public static decimal? Average (this IEnumerable<decimal?> source)
		{
			Check.Source (source);

			decimal total = 0;
			long counter = 0;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				total = total + element.Value;
				counter++;
			}

			if (counter == 0)
				return null;

			return new decimal? (total / counter);

		}

		public static float? Average (this IEnumerable<float?> source)
		{
			Check.Source (source);

			float total = 0;
			long counter = 0;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				total = total + element.Value;
				counter++;
			}

			if (counter == 0)
				return null;

			return new float? (total / counter);

		}

		public static double Average<TSource> (this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			Check.SourceAndSelector (source, selector);

			long total = 0;
			long count = 0;
			foreach (var element in source) {
				total += selector (element);
				count++;
			}
			if (count == 0)
				throw EmptySequence ();
			return total / (double)count;
		}

		public static double? Average<TSource> (this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			Check.SourceAndSelector (source, selector);

			long total = 0;
			long counter = 0;

			foreach (var element in source) {
				var value = selector (element);
				if (!value.HasValue)
					continue;

				total = total + value.Value;
				counter++;
			}

			if (counter == 0)
				return null;

			return new double? (total / (double)counter);
		}

		public static double Average<TSource> (this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			Check.SourceAndSelector (source, selector);

			long total = 0;
			long count = 0;
			foreach (var element in source) {
				total = checked (total + selector (element));
				count++;
			}
			if (count == 0)
				throw EmptySequence ();
			return total / (double)count;

		}

		public static double? Average<TSource> (this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			Check.SourceAndSelector (source, selector);

			long total = 0;
			long counter = 0;

			foreach (var element in source) {
				var value = selector (element);
				if (!value.HasValue)
					continue;

				total = checked (total + value.Value);
				counter++;
			}

			if (counter == 0)
				return null;

			return new double? (total / (double)counter);
		}

		public static double Average<TSource> (this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			Check.SourceAndSelector (source, selector);

			double total = 0;
			long count = 0;
			foreach (var element in source) {
				total += selector (element);
				count++;
			}
			if (count == 0)
				throw EmptySequence ();
			return total / count;

		}

		public static double? Average<TSource> (this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			Check.SourceAndSelector (source, selector);

			double total = 0;
			long counter = 0;

			foreach (var element in source) {
				var value = selector (element);
				if (!value.HasValue)
					continue;

				total = total + value.Value;
				counter++;
			}

			if (counter == 0)
				return null;

			return new double? (total / counter);

		}

		public static float Average<TSource> (this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			Check.SourceAndSelector (source, selector);

			float total = 0;
			long count = 0;
			foreach (var element in source) {
				total += selector (element);
				count++;
			}
			if (count == 0)
				throw EmptySequence ();
			return total / count;
		}

		public static float? Average<TSource> (this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			Check.SourceAndSelector (source, selector);

			float total = 0;
			long counter = 0;

			foreach (var element in source) {
				var value = selector (element);
				if (!value.HasValue)
					continue;

				total = total + value.Value;
				counter++;
			}

			if (counter == 0)
				return null;

			return new float? (total / counter);
		}

		public static decimal Average<TSource> (this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			Check.SourceAndSelector (source, selector);

			decimal total = 0;
			long count = 0;
			foreach (var element in source) {
				total += selector (element);
				count++;
			}
			if (count == 0)
				throw EmptySequence ();
			return total / count;
		}

		public static decimal? Average<TSource> (this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			Check.SourceAndSelector (source, selector);

			decimal total = 0;
			long counter = 0;

			foreach (var element in source) {
				var value = selector (element);
				if (!value.HasValue)
					continue;

				total = total + value.Value;
				counter++;
			}

			if (counter == 0)
				return null;

			return new decimal? (total / counter);
		}

		#endregion

		#region Cast

		public static IEnumerable<TResult> Cast<TResult> (this IEnumerable source)
		{
			Check.Source (source);

			var actual = source as IEnumerable<TResult>;
			if (actual != null)
				return actual;

			return CreateCastIterator<TResult> (source);
		}

		static IEnumerable<TResult> CreateCastIterator<TResult> (IEnumerable source)
		{
			foreach (TResult element in source)
				yield return element;
		}

		#endregion

		#region Concat

		public static IEnumerable<TSource> Concat<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			Check.FirstAndSecond (first, second);

			return CreateConcatIterator (first, second);
		}

		static IEnumerable<TSource> CreateConcatIterator<TSource> (IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			foreach (TSource element in first)
				yield return element;
			foreach (TSource element in second)
				yield return element;
		}

		#endregion

		#region Contains

		public static bool Contains<TSource> (this IEnumerable<TSource> source, TSource value)
		{
			var collection = source as ICollection<TSource>;
			if (collection != null)
				return collection.Contains (value);

			return Contains<TSource> (source, value, null);
		}

		public static bool Contains<TSource> (this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			Check.Source (source);

			if (comparer == null)
				comparer = EqualityComparer<TSource>.Default;

			foreach (var element in source)
				if (comparer.Equals (element, value))
					return true;

			return false;
		}
		#endregion

		#region Count

		public static int Count<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			var collection = source as ICollection<TSource>;
			if (collection != null)
				return collection.Count;

			int counter = 0;
			using (var enumerator = source.GetEnumerator ())
				while (enumerator.MoveNext ())
					checked {
						counter++;
					}

			return counter;
		}

		public static int Count<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndSelector (source, predicate);

			int counter = 0;
			foreach (var element in source)
				if (predicate (element))
					checked {
						counter++;
					}

			return counter;
		}

		#endregion

		#region DefaultIfEmpty

		public static IEnumerable<TSource> DefaultIfEmpty<TSource> (this IEnumerable<TSource> source)
		{
			return DefaultIfEmpty (source, default (TSource));
		}

		public static IEnumerable<TSource> DefaultIfEmpty<TSource> (this IEnumerable<TSource> source, TSource defaultValue)
		{
			Check.Source (source);

			return CreateDefaultIfEmptyIterator (source, defaultValue);
		}

		static IEnumerable<TSource> CreateDefaultIfEmptyIterator<TSource> (IEnumerable<TSource> source, TSource defaultValue)
		{
			bool empty = true;
			foreach (TSource item in source) {
				empty = false;
				yield return item;
			}

			if (empty)
				yield return defaultValue;
		}

		#endregion

		#region Distinct

		public static IEnumerable<TSource> Distinct<TSource> (this IEnumerable<TSource> source)
		{
			return Distinct<TSource> (source, null);
		}

		public static IEnumerable<TSource> Distinct<TSource> (this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			Check.Source (source);

			if (comparer == null)
				comparer = EqualityComparer<TSource>.Default;

			return CreateDistinctIterator (source, comparer);
		}

		static IEnumerable<TSource> CreateDistinctIterator<TSource> (IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			var items = new HashSet<TSource> (comparer);
			foreach (var element in source) {
				if (! items.Contains (element)) {
					items.Add (element);
					yield return element;
				}
			}
		}

		#endregion

		#region ElementAt

		static TSource ElementAt<TSource> (this IEnumerable<TSource> source, int index, Fallback fallback)
		{
			long counter = 0L;

			foreach (var element in source) {
				if (index == counter++)
					return element;
			}

			if (fallback == Fallback.Throw)
				throw new ArgumentOutOfRangeException ();

			return default (TSource);
		}

		public static TSource ElementAt<TSource> (this IEnumerable<TSource> source, int index)
		{
			Check.Source (source);

			if (index < 0)
				throw new ArgumentOutOfRangeException ();

			var list = source as IList<TSource>;
			if (list != null)
				return list [index];

			return source.ElementAt (index, Fallback.Throw);
		}

		#endregion

		#region ElementAtOrDefault

		public static TSource ElementAtOrDefault<TSource> (this IEnumerable<TSource> source, int index)
		{
			Check.Source (source);

			if (index < 0)
				return default (TSource);

			var list = source as IList<TSource>;
			if (list != null)
				return index < list.Count ? list [index] : default (TSource);

			return source.ElementAt (index, Fallback.Default);
		}

		#endregion

		#region Empty

		public static IEnumerable<TResult> Empty<TResult> ()
		{
			return new TResult[0];
		}

		#endregion

		#region Except

		public static IEnumerable<TSource> Except<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return Except (first, second, null);
		}

		public static IEnumerable<TSource> Except<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Check.FirstAndSecond (first, second);

			if (comparer == null)
				comparer = EqualityComparer<TSource>.Default;

			return CreateExceptIterator (first, second, comparer);
		}

		static IEnumerable<TSource> CreateExceptIterator<TSource> (IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			var items = new HashSet<TSource> (second, comparer);
			foreach (var element in first) {
				if (items.Add (element))
					yield return element;
			}
		}

		#endregion

		#region First

		static TSource First<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate, Fallback fallback)
		{
			foreach (var element in source)
				if (predicate (element))
					return element;

			if (fallback == Fallback.Throw)
				throw NoMatchingElement ();

			return default (TSource);
		}

		public static TSource First<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			var list = source as IList<TSource>;
			if (list != null) {
				if (list.Count != 0)
					return list [0];
			} else {
				using (var enumerator = source.GetEnumerator ()) {
					if (enumerator.MoveNext ())
						return enumerator.Current;
				}
			}

			throw EmptySequence ();
		}

		public static TSource First<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			return source.First (predicate, Fallback.Throw);
		}

		#endregion

		#region FirstOrDefault

		public static TSource FirstOrDefault<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			// inline the code to reduce dependency o generic causing AOT errors on device (e.g. bug #3285)
			foreach (var element in source)
				return element;

			return default (TSource);
		}

		public static TSource FirstOrDefault<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			return source.First (predicate, Fallback.Default);
		}

		#endregion

		#region GroupBy

		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector)
		{
			return GroupBy<TSource, TKey> (source, keySelector, null);
		}

		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector (source, keySelector);

			return CreateGroupByIterator (source, keySelector, comparer);
		}

		static IEnumerable<IGrouping<TKey, TSource>> CreateGroupByIterator<TSource, TKey> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			var groups = new Dictionary<TKey, List<TSource>> (comparer);
			var nullList = new List<TSource> ();
			int counter = 0;
			int nullCounter = -1;

			foreach (TSource element in source) {
				TKey key = keySelector (element);
				if (key == null) {
					nullList.Add (element);
					if (nullCounter == -1) {
						nullCounter = counter;
						counter++;
					}
				} else {
					List<TSource> group;
					if (!groups.TryGetValue (key, out group)) {
						group = new List<TSource> ();
						groups.Add (key, group);
						counter++;
					}
					group.Add (element);
				}
			}

			counter = 0;
			foreach (var group in groups) {
				if (counter == nullCounter) {
					yield return new Grouping<TKey, TSource> (default (TKey), nullList);
					counter++;
				}

				yield return new Grouping<TKey, TSource> (group.Key, group.Value);
				counter++;
			}

			if (counter == nullCounter) {
				yield return new Grouping<TKey, TSource> (default (TKey), nullList);
				counter++;
			}
		}

		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return GroupBy<TSource, TKey, TElement> (source, keySelector, elementSelector, null);
		}

		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeyElementSelectors (source, keySelector, elementSelector);

			return CreateGroupByIterator (source, keySelector, elementSelector, comparer);
		}

		static IEnumerable<IGrouping<TKey, TElement>> CreateGroupByIterator<TSource, TKey, TElement> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			var groups = new Dictionary<TKey, List<TElement>> (comparer);
			var nullList = new List<TElement> ();
			int counter = 0;
			int nullCounter = -1;

			foreach (TSource item in source) {
				TKey key = keySelector (item);
				TElement element = elementSelector (item);
				if (key == null) {
					nullList.Add (element);
					if (nullCounter == -1) {
						nullCounter = counter;
						counter++;
					}
				} else {
					List<TElement> group;
					if (!groups.TryGetValue (key, out group)) {
						group = new List<TElement> ();
						groups.Add (key, group);
						counter++;
					}
					group.Add (element);
				}
			}

			counter = 0;
			foreach (var group in groups) {
				if (counter == nullCounter) {
					yield return new Grouping<TKey, TElement> (default (TKey), nullList);
					counter++;
				}

				yield return new Grouping<TKey, TElement> (group.Key, group.Value);
				counter++;
			}

			if (counter == nullCounter) {
				yield return new Grouping<TKey, TElement> (default (TKey), nullList);
				counter++;
			}
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector,
			Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		{
			return GroupBy (source, keySelector, elementSelector, resultSelector, null);
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector,
			Func<TKey, IEnumerable<TElement>, TResult> resultSelector,
			IEqualityComparer<TKey> comparer)
		{
			Check.GroupBySelectors (source, keySelector, elementSelector, resultSelector);

			return CreateGroupByIterator (source, keySelector, elementSelector, resultSelector, comparer);
		}

		static IEnumerable<TResult> CreateGroupByIterator<TSource, TKey, TElement, TResult> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector,
			Func<TKey, IEnumerable<TElement>, TResult> resultSelector,
			IEqualityComparer<TKey> comparer)
		{
			IEnumerable<IGrouping<TKey, TElement>> groups = GroupBy<TSource, TKey, TElement> (
				source, keySelector, elementSelector, comparer);

			foreach (IGrouping<TKey, TElement> group in groups)
				yield return resultSelector (group.Key, group);
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
		{
			return GroupBy (source, keySelector, resultSelector, null);
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			Func<TKey, IEnumerable<TSource>, TResult> resultSelector,
			IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeyResultSelectors (source, keySelector, resultSelector);

			return CreateGroupByIterator (source, keySelector, resultSelector, comparer);
		}

		static IEnumerable<TResult> CreateGroupByIterator<TSource, TKey, TResult> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			Func<TKey, IEnumerable<TSource>, TResult> resultSelector,
			IEqualityComparer<TKey> comparer)
		{
			IEnumerable<IGrouping<TKey,TSource>> groups = GroupBy<TSource, TKey> (source, keySelector, comparer);

			foreach (IGrouping<TKey, TSource> group in groups)
				yield return resultSelector (group.Key, group);
		}

		#endregion

		# region GroupJoin

		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult> (this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector,
			Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
		{
			return GroupJoin (outer, inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}

		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult> (this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector,
			Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector,
			IEqualityComparer<TKey> comparer)
		{
			Check.JoinSelectors (outer, inner, outerKeySelector, innerKeySelector, resultSelector);

			if (comparer == null)
				comparer = EqualityComparer<TKey>.Default;

			return CreateGroupJoinIterator (outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		static IEnumerable<TResult> CreateGroupJoinIterator<TOuter, TInner, TKey, TResult> (this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector,
			Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector,
			IEqualityComparer<TKey> comparer)
		{
			ILookup<TKey, TInner> innerKeys = ToLookup<TInner, TKey> (inner, innerKeySelector, comparer);

			foreach (TOuter element in outer) {
				TKey outerKey = outerKeySelector (element);
				if (outerKey != null && innerKeys.Contains (outerKey))
					yield return resultSelector (element, innerKeys [outerKey]);
				else
					yield return resultSelector (element, Empty<TInner> ());
			}
		}

		#endregion

		#region Intersect

		public static IEnumerable<TSource> Intersect<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return Intersect (first, second, null);
		}

		public static IEnumerable<TSource> Intersect<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Check.FirstAndSecond (first, second);

			if (comparer == null)
				comparer = EqualityComparer<TSource>.Default;

			return CreateIntersectIterator (first, second, comparer);
		}

		static IEnumerable<TSource> CreateIntersectIterator<TSource> (IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			var items = new HashSet<TSource> (second, comparer);
			foreach (TSource element in first) {
				if (items.Remove (element))
					yield return element;
			}
		}

		#endregion

		# region Join

		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult> (this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector,
			Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Check.JoinSelectors (outer, inner, outerKeySelector, innerKeySelector, resultSelector);

			if (comparer == null)
				comparer = EqualityComparer<TKey>.Default;

			return CreateJoinIterator (outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		static IEnumerable<TResult> CreateJoinIterator<TOuter, TInner, TKey, TResult> (this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector,
			Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			ILookup<TKey, TInner> innerKeys = ToLookup<TInner, TKey> (inner, innerKeySelector, comparer);

			foreach (TOuter element in outer) {
				TKey outerKey = outerKeySelector (element);
				if (outerKey != null && innerKeys.Contains (outerKey)) {
					foreach (TInner innerElement in innerKeys [outerKey])
						yield return resultSelector (element, innerElement);
				}
			}
		}

		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult> (this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector,
			Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
		{
			return outer.Join (inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}

		#endregion

		#region Last

		static TSource Last<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate, Fallback fallback)
		{
			var empty = true;
			var item = default (TSource);

			foreach (var element in source) {
				if (!predicate (element))
					continue;

				item = element;
				empty = false;
			}

			if (!empty)
				return item;

			if (fallback == Fallback.Throw)
				throw NoMatchingElement ();

			return item;
		}

		public static TSource Last<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			var collection = source as ICollection<TSource>;
			if (collection != null && collection.Count == 0)
				throw EmptySequence ();

			var list = source as IList<TSource>;
			if (list != null)
				return list [list.Count - 1];

			var empty = true;
			var item = default (TSource);

			foreach (var element in source) {
				item = element;
				empty = false;
			}

			if (!empty)
				return item;

			throw EmptySequence ();
		}

		public static TSource Last<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			return source.Last (predicate, Fallback.Throw);
		}

		#endregion

		#region LastOrDefault

		public static TSource LastOrDefault<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			var list = source as IList<TSource>;
			if (list != null)
				return list.Count > 0 ? list [list.Count - 1] : default (TSource);

			var empty = true;
			var item = default (TSource);

			foreach (var element in source) {
				item = element;
				empty = false;
			}

			if (!empty)
				return item;

			return item;
		}

		public static TSource LastOrDefault<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			return source.Last (predicate, Fallback.Default);
		}

		#endregion

		#region LongCount

		public static long LongCount<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			var array = source as TSource[];
			if (array != null)
				return array.LongLength;

			long counter = 0;
			using (var enumerator = source.GetEnumerator ())
				while (enumerator.MoveNext ())
					counter++;

			return counter;
		}

		public static long LongCount<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndSelector (source, predicate);

			long counter = 0;
			foreach (TSource element in source)
				if (predicate (element))
					counter++;

			return counter;
		}

		#endregion

		#region Max

		public static int Max (this IEnumerable<int> source)
		{
			Check.Source (source);

			bool empty = true;
			var max = int.MinValue;
			foreach (var element in source) {
				max = Math.Max (element, max);
				empty = false;
			}
			if (empty)
				throw EmptySequence ();
			return max;
		}

		public static long Max (this IEnumerable<long> source)
		{
			Check.Source (source);

			bool empty = true;
			var max = long.MinValue;
			foreach (var element in source) {
				max = Math.Max (element, max);
				empty = false;
			}
			if (empty)
				throw EmptySequence ();
			return max;
		}

		public static double Max (this IEnumerable<double> source)
		{
			Check.Source (source);

			bool empty = true;
			var max = double.MinValue;
			foreach (var element in source) {
				max = Math.Max (element, max);
				empty = false;
			}
			if (empty)
				throw EmptySequence ();
			return max;
		}

		public static float Max (this IEnumerable<float> source)
		{
			Check.Source (source);

			bool empty = true;
			var max = float.MinValue;
			foreach (var element in source) {
				max = Math.Max (element, max);
				empty = false;
			}
			if (empty)
				throw EmptySequence ();
			return max;
		}

		public static decimal Max (this IEnumerable<decimal> source)
		{
			Check.Source (source);

			bool empty = true;
			var max = decimal.MinValue;
			foreach (var element in source) {
				max = Math.Max (element, max);
				empty = false;
			}
			if (empty)
				throw EmptySequence ();
			return max;
		}

		public static int? Max (this IEnumerable<int?> source)
		{
			Check.Source (source);

			bool empty = true;
			var max = int.MinValue;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				max = Math.Max (element.Value, max);
				empty = false;
			}

			if (empty)
				return null;

			return max;
		}

		public static long? Max (this IEnumerable<long?> source)
		{
			Check.Source (source);

			bool empty = true;
			var max = long.MinValue;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				max = Math.Max (element.Value, max);
				empty = false;
			}

			if (empty)
				return null;

			return max;
		}

		public static double? Max (this IEnumerable<double?> source)
		{
			Check.Source (source);

			bool empty = true;
			var max = double.MinValue;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				max = Math.Max (element.Value, max);
				empty = false;
			}

			if (empty)
				return null;

			return max;
		}

		public static float? Max (this IEnumerable<float?> source)
		{
			Check.Source (source);

			bool empty = true;
			var max = float.MinValue;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				max = Math.Max (element.Value, max);
				empty = false;
			}

			if (empty)
				return null;

			return max;
		}

		public static decimal? Max (this IEnumerable<decimal?> source)
		{
			Check.Source (source);

			bool empty = true;
			var max = decimal.MinValue;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				max = Math.Max (element.Value, max);
				empty = false;
			}

			if (empty)
				return null;

			return max;
		}

		public static TSource Max<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			var comparer = Comparer<TSource>.Default;

			TSource max = default (TSource);

			if (default (TSource) == null) {
				foreach (var element in source) {
					if (element == null)
						continue;

					if (max == null || comparer.Compare (element, max) > 0)
						max = element;
				}
			} else {
				bool empty = true;
				foreach (var element in source) {
					if (empty) {
						max = element;
						empty = false;
						continue;
					}
					if (comparer.Compare (element, max) > 0)
						max = element;
				}
				if (empty)
					throw EmptySequence ();
			}
			return max;
		}

		public static int Max<TSource> (this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			var max = int.MinValue;
			foreach (var element in source) {
				max = Math.Max (selector (element), max);
				empty = false;
			}
			if (empty)
				throw NoMatchingElement ();
			return max;
		}

		public static long Max<TSource> (this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			var max = long.MinValue;
			foreach (var element in source) {
				max = Math.Max (selector (element), max);
				empty = false;
			}
			if (empty)
				throw NoMatchingElement ();
			return max;
		}

		public static double Max<TSource> (this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			var max = double.MinValue;
			foreach (var element in source) {
				max = Math.Max (selector (element), max);
				empty = false;
			}
			if (empty)
				throw NoMatchingElement ();
			return max;
		}

		public static float Max<TSource> (this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			var max = float.MinValue;
			foreach (var element in source) {
				max = Math.Max (selector (element), max);
				empty = false;
			}
			if (empty)
				throw NoMatchingElement ();
			return max;
		}

		public static decimal Max<TSource> (this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			var max = decimal.MinValue;
			foreach (var element in source) {
				max = Math.Max (selector (element), max);
				empty = false;
			}
			if (empty)
				throw NoMatchingElement ();
			return max;
		}

		static U Iterate<T, U> (IEnumerable<T> source, U initValue, Func<T, U, U> selector)
		{
			bool empty = true;
			foreach (var element in source) {
				initValue = selector (element, initValue);
				empty = false;
			}

			if (empty)
				throw NoMatchingElement ();

			return initValue;
		}

		public static int? Max<TSource> (this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			int? max = null;
			foreach (var element in source) {
				int? item = selector (element);

				if (!max.HasValue)
					max = item;
				else if (item > max)
					max = item;
				empty = false;
			}

			if (empty)
				return null;
			return max;
		}

		public static long? Max<TSource> (this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			long? max = null;
			foreach (var element in source) {
				long? item = selector (element);

				if (!max.HasValue)
					max = item;
				else if (item > max)
					max = item;
				empty = false;
			}

			if (empty)
				return null;
			return max;
		}

		public static double? Max<TSource> (this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			double? max = null;
			foreach (var element in source) {
				double? item = selector (element);

				if (!max.HasValue)
					max = item;
				else if (item > max)
					max = item;
				empty = false;
			}

			if (empty)
				return null;
			return max;
		}

		public static float? Max<TSource> (this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			float? max = null;
			foreach (var element in source) {
				float? item = selector (element);

				if (!max.HasValue)
					max = item;
				else if (item > max)
					max = item;
				empty = false;
			}

			if (empty)
				return null;
			return max;
		}

		public static decimal? Max<TSource> (this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			decimal? max = null;
			foreach (var element in source) {
				decimal? item = selector (element);

				if (!max.HasValue)
					max = item;
				else if (item > max)
					max = item;
				empty = false;
			}

			if (empty)
				return null;
			return max;
		}

		public static TResult Max<TSource, TResult> (this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			Check.SourceAndSelector (source, selector);

			return source.Select (selector).Max ();
		}

		#endregion

		#region Min

		public static int Min (this IEnumerable<int> source)
		{
			Check.Source (source);

			bool empty = true;
			var min = int.MaxValue;
			foreach (var element in source) {
				min = Math.Min (element, min);
				empty = false;
			}
			if (empty)
				throw EmptySequence ();
			return min;
		}

		public static long Min (this IEnumerable<long> source)
		{
			Check.Source (source);

			bool empty = true;
			var min = long.MaxValue;
			foreach (var element in source) {
				min = Math.Min (element, min);
				empty = false;
			}
			if (empty)
				throw EmptySequence ();
			return min;
		}

		public static double Min (this IEnumerable<double> source)
		{
			Check.Source (source);

			bool empty = true;
			var min = double.MaxValue;
			foreach (var element in source) {
				min = Math.Min (element, min);
				empty = false;
			}
			if (empty)
				throw EmptySequence ();
			return min;
		}

		public static float Min (this IEnumerable<float> source)
		{
			Check.Source (source);

			bool empty = true;
			var min = float.MaxValue;
			foreach (var element in source) {
				min = Math.Min (element, min);
				empty = false;
			}
			if (empty)
				throw EmptySequence ();
			return min;
		}

		public static decimal Min (this IEnumerable<decimal> source)
		{
			Check.Source (source);

			bool empty = true;
			var min = decimal.MaxValue;
			foreach (var element in source) {
				min = Math.Min (element, min);
				empty = false;
			}
			if (empty)
				throw EmptySequence ();
			return min;
		}

		public static int? Min (this IEnumerable<int?> source)
		{
			Check.Source (source);

			bool empty = true;
			var min = int.MaxValue;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				min = Math.Min (element.Value, min);
				empty = false;
			}

			if (empty)
				return null;

			return min;
		}

		public static long? Min (this IEnumerable<long?> source)
		{
			Check.Source (source);

			bool empty = true;
			var min = long.MaxValue;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				min = Math.Min (element.Value, min);
				empty = false;
			}

			if (empty)
				return null;

			return min;
		}

		public static double? Min (this IEnumerable<double?> source)
		{
			Check.Source (source);

			bool empty = true;
			var min = double.MaxValue;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				min = Math.Min (element.Value, min);
				empty = false;
			}

			if (empty)
				return null;

			return min;
		}

		public static float? Min (this IEnumerable<float?> source)
		{
			Check.Source (source);

			bool empty = true;
			var min = float.MaxValue;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				min = Math.Min (element.Value, min);
				empty = false;
			}

			if (empty)
				return null;

			return min;
		}

		public static decimal? Min (this IEnumerable<decimal?> source)
		{
			Check.Source (source);

			bool empty = true;
			var min = decimal.MaxValue;

			foreach (var element in source) {
				if (!element.HasValue)
					continue;

				min = Math.Min (element.Value, min);
				empty = false;
			}

			if (empty)
				return null;

			return min;
		}

		public static TSource Min<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			var comparer = Comparer<TSource>.Default;

			TSource min = default (TSource);

			if (default (TSource) == null) {
				foreach (var element in source) {
					if (element == null)
						continue;

					if (min == null || comparer.Compare (element, min) < 0)
						min = element;
				}
			} else {
				bool empty = true;
				foreach (var element in source) {
					if (empty) {
						min = element;
						empty = false;
						continue;
					}
					if (comparer.Compare (element, min) < 0)
						min = element;
				}
				if (empty)
					throw EmptySequence ();
			}
			return min;
		}

		public static int Min<TSource> (this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			var min = int.MaxValue;
			foreach (var element in source) {
				min = Math.Min (selector (element), min);
				empty = false;
			}
			if (empty)
				throw NoMatchingElement ();
			return min;
		}

		public static long Min<TSource> (this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			var min = long.MaxValue;
			foreach (var element in source) {
				min = Math.Min (selector (element), min);
				empty = false;
			}
			if (empty)
				throw NoMatchingElement ();
			return min;
		}

		public static double Min<TSource> (this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			var min = double.MaxValue;
			foreach (var element in source) {
				min = Math.Min (selector (element), min);
				empty = false;
			}
			if (empty)
				throw NoMatchingElement ();
			return min;
		}

		public static float Min<TSource> (this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			var min = float.MaxValue;
			foreach (var element in source) {
				min = Math.Min (selector (element), min);
				empty = false;
			}
			if (empty)
				throw NoMatchingElement ();
			return min;
		}

		public static decimal Min<TSource> (this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			var min = decimal.MaxValue;
			foreach (var element in source) {
				min = Math.Min (selector (element), min);
				empty = false;
			}
			if (empty)
				throw NoMatchingElement ();
			return min;
		}

		public static int? Min<TSource> (this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			int? min = null;
			foreach (var element in source) {
				int? item = selector (element);

				if (!min.HasValue)
					min = item;
				else if (item < min)
					min = item;
				empty = false;
			}

			if (empty)
				return null;
			return min;
		}

		public static long? Min<TSource> (this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			long? min = null;
			foreach (var element in source) {
				long? item = selector (element);

				if (!min.HasValue)
					min = item;
				else if (item < min)
					min = item;
				empty = false;
			}

			if (empty)
				return null;
			return min;
		}

		public static float? Min<TSource> (this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			float? min = null;
			foreach (var element in source) {
				float? item = selector (element);

				if (!min.HasValue)
					min = item;
				else if (item < min)
					min = item;
				empty = false;
			}

			if (empty)
				return null;
			return min;
		}

		public static double? Min<TSource> (this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			double? min = null;
			foreach (var element in source) {
				double? item = selector (element);

				if (!min.HasValue)
					min = item;
				else if (item < min)
					min = item;
				empty = false;
			}

			if (empty)
				return null;
			return min;
		}

		public static decimal? Min<TSource> (this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			Check.SourceAndSelector (source, selector);

			bool empty = true;
			decimal? min = null;
			foreach (var element in source) {
				decimal? item = selector (element);

				if (!min.HasValue)
					min = item;
				else if (item < min)
					min = item;
				empty = false;
			}

			if (empty)
				return null;
			return min;
		}

		public static TResult Min<TSource, TResult> (this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			Check.SourceAndSelector (source, selector);

			return source.Select (selector).Min ();
		}

		#endregion

		#region OfType

		public static IEnumerable<TResult> OfType<TResult> (this IEnumerable source)
		{
			Check.Source (source);

			return CreateOfTypeIterator<TResult> (source);
		}

		static IEnumerable<TResult> CreateOfTypeIterator<TResult> (IEnumerable source)
		{
			foreach (object element in source)
				if (element is TResult)
					yield return (TResult)element;
		}

		#endregion

		#region OrderBy

		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey> (this IEnumerable<TSource> source,
				Func<TSource, TKey> keySelector)
		{
			return OrderBy<TSource, TKey> (source, keySelector, null);
		}

		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey> (this IEnumerable<TSource> source,
				Func<TSource, TKey> keySelector,
				IComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector (source, keySelector);

			return new OrderedSequence<TSource, TKey> (source, keySelector, comparer, SortDirection.Ascending);
		}

		#endregion

		#region OrderByDescending

		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey> (this IEnumerable<TSource> source,
				Func<TSource, TKey> keySelector)
		{
			return OrderByDescending<TSource, TKey> (source, keySelector, null);
		}

		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey> (this IEnumerable<TSource> source,
				Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector (source, keySelector);

			return new OrderedSequence<TSource, TKey> (source, keySelector, comparer, SortDirection.Descending);
		}

		#endregion

		#region Range

		public static IEnumerable<int> Range (int start, int count)
		{
			if (count < 0)
				throw new ArgumentOutOfRangeException ("count");

			if (((long)start + count) - 1L > int.MaxValue)
				throw new ArgumentOutOfRangeException ();

			return CreateRangeIterator (start, count);
		}

		static IEnumerable<int> CreateRangeIterator (int start, int count)
		{
			for (int i = 0; i < count; i++)
				yield return start + i;
		}

		#endregion

		#region Repeat

		public static IEnumerable<TResult> Repeat<TResult> (TResult element, int count)
		{
			if (count < 0)
				throw new ArgumentOutOfRangeException ();

			return CreateRepeatIterator (element, count);
		}

		static IEnumerable<TResult> CreateRepeatIterator<TResult> (TResult element, int count)
		{
			for (int i = 0; i < count; i++)
				yield return element;
		}

		#endregion

		#region Reverse

		public static IEnumerable<TSource> Reverse<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			return CreateReverseIterator (source);
		}

		static IEnumerable<TSource> CreateReverseIterator<TSource> (IEnumerable<TSource> source)
		{
			var array = source.ToArray ();

			for (int i = array.Length - 1; i >= 0; i--)
				yield return array [i];
		}

		#endregion

		#region Select

		public static IEnumerable<TResult> Select<TSource, TResult> (this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			Check.SourceAndSelector (source, selector);

			return CreateSelectIterator (source, selector);
		}

		static IEnumerable<TResult> CreateSelectIterator<TSource, TResult> (IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			foreach (var element in source)
				yield return selector (element);
		}

		public static IEnumerable<TResult> Select<TSource, TResult> (this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			Check.SourceAndSelector (source, selector);

			return CreateSelectIterator (source, selector);
		}

		static IEnumerable<TResult> CreateSelectIterator<TSource, TResult> (IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			int counter = 0;
			foreach (TSource element in source) {
				yield return selector (element, counter);
				counter++;
			}
		}

		#endregion

		#region SelectMany

		public static IEnumerable<TResult> SelectMany<TSource, TResult> (this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
		{
			Check.SourceAndSelector (source, selector);

			return CreateSelectManyIterator (source, selector);
		}

		static IEnumerable<TResult> CreateSelectManyIterator<TSource, TResult> (IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
		{
			foreach (TSource element in source)
				foreach (TResult item in selector (element))
					yield return item;
		}

		public static IEnumerable<TResult> SelectMany<TSource, TResult> (this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
		{
			Check.SourceAndSelector (source, selector);

			return CreateSelectManyIterator (source, selector);
		}

		static IEnumerable<TResult> CreateSelectManyIterator<TSource, TResult> (IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
		{
			int counter = 0;
			foreach (TSource element in source) {
				foreach (TResult item in selector (element, counter))
					yield return item;
				counter++;
			}
		}

		public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult> (this IEnumerable<TSource> source,
			Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			Check.SourceAndCollectionSelectors (source, collectionSelector, resultSelector);

			return CreateSelectManyIterator (source, collectionSelector, resultSelector);
		}

		static IEnumerable<TResult> CreateSelectManyIterator<TSource, TCollection, TResult> (IEnumerable<TSource> source,
			Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> selector)
		{
			foreach (TSource element in source)
				foreach (TCollection collection in collectionSelector (element))
					yield return selector (element, collection);
		}

		public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult> (this IEnumerable<TSource> source,
			Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
		{
			Check.SourceAndCollectionSelectors (source, collectionSelector, resultSelector);

			return CreateSelectManyIterator (source, collectionSelector, resultSelector);
		}

		static IEnumerable<TResult> CreateSelectManyIterator<TSource, TCollection, TResult> (IEnumerable<TSource> source,
			Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> selector)
		{
			int counter = 0;
			foreach (TSource element in source)
				foreach (TCollection collection in collectionSelector (element, counter++))
					yield return selector (element, collection);
		}

		#endregion

		#region Single

		static TSource Single<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate, Fallback fallback)
		{
			var found = false;
			var item = default (TSource);

			foreach (var element in source) {
				if (!predicate (element))
					continue;

				if (found)
					throw MoreThanOneMatchingElement ();

				found = true;
				item = element;
			}

			if (!found && fallback == Fallback.Throw)
				throw NoMatchingElement ();

			return item;
		}

		public static TSource Single<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			var found = false;
			var item = default (TSource);

			foreach (var element in source) {
				if (found)
					throw MoreThanOneElement ();

				found = true;
				item = element;
			}

			if (!found)
				throw NoMatchingElement ();

			return item;
		}

		public static TSource Single<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			return source.Single (predicate, Fallback.Throw);
		}

		#endregion

		#region SingleOrDefault

		public static TSource SingleOrDefault<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			var found = false;
			var item = default (TSource);

			foreach (var element in source) {
				if (found)
					throw MoreThanOneMatchingElement ();

				found = true;
				item = element;
			}

			return item;
		}

		public static TSource SingleOrDefault<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			return source.Single (predicate, Fallback.Default);
		}

		#endregion

		#region Skip

		public static IEnumerable<TSource> Skip<TSource> (this IEnumerable<TSource> source, int count)
		{
			Check.Source (source);

			return CreateSkipIterator (source, count);
		}

		static IEnumerable<TSource> CreateSkipIterator<TSource> (IEnumerable<TSource> source, int count)
		{
			var enumerator = source.GetEnumerator ();
			try {
				while (count-- > 0)
					if (!enumerator.MoveNext ())
						yield break;

				while (enumerator.MoveNext ())
					yield return enumerator.Current;

			} finally {
				enumerator.Dispose ();
			}
		}

		#endregion

		#region SkipWhile

		public static IEnumerable<TSource> SkipWhile<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			return CreateSkipWhileIterator (source, predicate);
		}

		static IEnumerable<TSource> CreateSkipWhileIterator<TSource> (IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			bool yield = false;

			foreach (TSource element in source) {
				if (yield)
					yield return element;
				else
					if (!predicate (element)) {
					yield return element;
					yield = true;
				}
			}
		}

		public static IEnumerable<TSource> SkipWhile<TSource> (this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			return CreateSkipWhileIterator (source, predicate);
		}

		static IEnumerable<TSource> CreateSkipWhileIterator<TSource> (IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			int counter = 0;
			bool yield = false;

			foreach (TSource element in source) {
				if (yield)
					yield return element;
				else
					if (!predicate (element, counter)) {
					yield return element;
					yield = true;
				}
				counter++;
			}
		}

		#endregion

		#region Sum

		public static int Sum (this IEnumerable<int> source)
		{
			Check.Source (source);
			int total = 0;

			foreach (var element in source)
				total = checked (total + element);
			return total;
		}

		public static int? Sum (this IEnumerable<int?> source)
		{
			Check.Source (source);

			int total = 0;
			foreach (var element in source) {
				if (element.HasValue)
					total = checked (total + element.Value);
			}
			return total;
		}

		public static int Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			Check.SourceAndSelector (source, selector);
			int total = 0;

			foreach (var element in source)
				total = checked (total + selector (element));

			return total;
		}

		public static int? Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			Check.SourceAndSelector (source, selector);

			int total = 0;
			foreach (var element in source) {
				var value = selector (element);
				if (value.HasValue)
					total = checked (total + value.Value);
			}
			return total;
		}

		public static long Sum (this IEnumerable<long> source)
		{
			Check.Source (source);

			long total = 0;

			foreach (var element in source)
				total = checked (total + element);
			return total;
		}

		public static long? Sum (this IEnumerable<long?> source)
		{
			Check.Source (source);

			long total = 0;
			foreach (var element in source) {
				if (element.HasValue)
					total = checked (total + element.Value);
			}
			return total;
		}

		public static long Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			Check.SourceAndSelector (source, selector);

			long total = 0;
			foreach (var element in source)
				total = checked (total + selector (element));
			return total;
		}

		public static long? Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			Check.SourceAndSelector (source, selector);

			long total = 0;
			foreach (var element in source) {
				var value = selector (element);
				if (value.HasValue)
					total = checked (total + value.Value);
			}
			return total;
		}

		public static double Sum (this IEnumerable<double> source)
		{
			Check.Source (source);

			double total = 0;

			foreach (var element in source)
				total = checked (total + element);
			return total;
		}

		public static double? Sum (this IEnumerable<double?> source)
		{
			Check.Source (source);

			double total = 0;
			foreach (var element in source) {
				if (element.HasValue)
					total = checked (total + element.Value);
			}
			return total;
		}

		public static double Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			Check.SourceAndSelector (source, selector);

			double total = 0;

			foreach (var element in source)
				total = checked (total + selector (element));
			return total;
		}

		public static double? Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			Check.SourceAndSelector (source, selector);

			double total = 0;
			foreach (var element in source) {
				var value = selector (element);
				if (value.HasValue)
					total = checked (total + value.Value);
			}
			return total;
		}

		public static float Sum (this IEnumerable<float> source)
		{
			Check.Source (source);

			float total = 0;

			foreach (var element in source)
				total = checked (total + element);
			return total;
		}

		public static float? Sum (this IEnumerable<float?> source)
		{
			Check.Source (source);

			float total = 0;
			foreach (var element in source) {
				if (element.HasValue)
					total = checked (total + element.Value);
			}
			return total;

		}

		public static float Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			Check.SourceAndSelector (source, selector);
			float total = 0;
			foreach (var element in source)
				total = checked (total + selector (element));
			return total;
		}

		public static float? Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			Check.SourceAndSelector (source, selector);

			float total = 0;
			foreach (var element in source) {
				var value = selector (element);
				if (value.HasValue)
					total = checked (total + value.Value);
			}
			return total;
		}

		public static decimal Sum (this IEnumerable<decimal> source)
		{
			Check.Source (source);
			decimal total = 0;

			foreach (var element in source)
				total = checked (total + element);
			return total;
		}

		public static decimal? Sum (this IEnumerable<decimal?> source)
		{
			Check.Source (source);

			decimal total = 0;
			foreach (var element in source) {
				if (element.HasValue)
					total = checked (total + element.Value);
			}
			return total;

		}

		public static decimal Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			Check.SourceAndSelector (source, selector);
			decimal total = 0;

			foreach (var element in source)
				total = checked (total + selector (element));
			return total;
		}

		public static decimal? Sum<TSource> (this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			Check.SourceAndSelector (source, selector);

			decimal total = 0;
			foreach (var element in source) {
				var value = selector (element);
				if (value.HasValue)
					total = checked (total + value.Value);
			}
			return total;
		}

		#endregion

		#region Take

		public static IEnumerable<TSource> Take<TSource> (this IEnumerable<TSource> source, int count)
		{
			Check.Source (source);

			return CreateTakeIterator (source, count);
		}

		static IEnumerable<TSource> CreateTakeIterator<TSource> (IEnumerable<TSource> source, int count)
		{
			if (count <= 0)
				yield break;

			int counter = 0;
			foreach (TSource element in source) {
				yield return element;

				if (++counter == count)
					yield break;
			}
		}

		#endregion

		#region TakeWhile

		public static IEnumerable<TSource> TakeWhile<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			return CreateTakeWhileIterator (source, predicate);
		}

		static IEnumerable<TSource> CreateTakeWhileIterator<TSource> (IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			foreach (var element in source) {
				if (!predicate (element))
					yield break;

				yield return element;
			}
		}

		public static IEnumerable<TSource> TakeWhile<TSource> (this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			return CreateTakeWhileIterator (source, predicate);
		}

		static IEnumerable<TSource> CreateTakeWhileIterator<TSource> (IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			int counter = 0;
			foreach (var element in source) {
				if (!predicate (element, counter))
					yield break;

				yield return element;
				counter++;
			}
		}

		#endregion

		#region ThenBy

		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey> (this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return ThenBy<TSource, TKey> (source, keySelector, null);
		}

		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey> (this IOrderedEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector (source, keySelector);

			var orderedEnumerable = source as OrderedEnumerable <TSource>;
			return orderedEnumerable.CreateOrderedEnumerable (keySelector, comparer, false);
		}

		#endregion

		#region ThenByDescending

		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey> (this IOrderedEnumerable<TSource> source,
			Func<TSource, TKey> keySelector)
		{
			return ThenByDescending<TSource, TKey> (source, keySelector, null);
		}

		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey> (this IOrderedEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector (source, keySelector);

			var orderedEnumerable = source as OrderedEnumerable <TSource>;
			return orderedEnumerable.CreateOrderedEnumerable (keySelector, comparer, true);
		}

		#endregion

		#region ToArray

		public static TSource [] ToArray<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			TSource[] array;
			var collection = source as ICollection<TSource>;
			if (collection != null) {
				if (collection.Count == 0)
					return new TSource[0];

				array = new TSource [collection.Count];
				collection.CopyTo (array, 0);
				return array;
			}

			int pos = 0;
			array = new TSource[0];
			foreach (var element in source) {
				if (pos == array.Length) {
					if (pos == 0)
						array = new TSource [4];
					else
						Array.Resize (ref array, pos * 2);
				}

				array [pos++] = element;
			}

			if (pos != array.Length)
				Array.Resize (ref array, pos);

			return array;
		}

		#endregion

		#region ToDictionary
		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement> (this IEnumerable<TSource> source,
				Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return ToDictionary<TSource, TKey, TElement> (source, keySelector, elementSelector, null);
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement> (this IEnumerable<TSource> source,
				Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeyElementSelectors (source, keySelector, elementSelector);

			if (comparer == null)
				comparer = EqualityComparer<TKey>.Default;

			var dict = new Dictionary<TKey, TElement> (comparer);
			foreach (var e in source)
				dict.Add (keySelector (e), elementSelector (e));

			return dict;
		}

		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey> (this IEnumerable<TSource> source,
				Func<TSource, TKey> keySelector)
		{
			return ToDictionary (source, keySelector, null);
		}

		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey> (this IEnumerable<TSource> source,
				Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector (source, keySelector);

			if (comparer == null)
				comparer = EqualityComparer<TKey>.Default;

			var dict = new Dictionary<TKey, TSource> (comparer);
			foreach (var e in source)
				dict.Add (keySelector (e), e);

			return dict;
		}
		#endregion

		#region ToList
		public static List<TSource> ToList<TSource> (this IEnumerable<TSource> source)
		{
			Check.Source (source);

			return new List<TSource> (source);
		}
		#endregion

		#region ToLookup

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return ToLookup<TSource, TKey> (source, keySelector, null);
		}

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector (source, keySelector);
			List<TSource> nullKeyElements = null;

			var dictionary = new Dictionary<TKey, List<TSource>> (comparer ?? EqualityComparer<TKey>.Default);
			foreach (var element in source) {
				var key = keySelector (element);

				List<TSource> list;

				if (key == null) {
					if (nullKeyElements == null)
						nullKeyElements = new List<TSource> ();

					list = nullKeyElements;
				} else if (!dictionary.TryGetValue (key, out list)) {
					list = new List<TSource> ();
					dictionary.Add (key, list);
				}

				list.Add (element);
			}

			return new Lookup<TKey, TSource> (dictionary, nullKeyElements);
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return ToLookup<TSource, TKey, TElement> (source, keySelector, elementSelector, null);
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement> (this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeyElementSelectors (source, keySelector, elementSelector);

			List<TElement> nullKeyElements = null;

			var dictionary = new Dictionary<TKey, List<TElement>> (comparer ?? EqualityComparer<TKey>.Default);
			foreach (var element in source) {
				var key = keySelector (element);

				List<TElement> list;

				if (key == null) {
					if (nullKeyElements == null)
						nullKeyElements = new List<TElement> ();

					list = nullKeyElements;
				} else if (!dictionary.TryGetValue (key, out list)) {
					list = new List<TElement> ();
					dictionary.Add (key, list);
				}

				list.Add (elementSelector (element));
			}

			return new Lookup<TKey, TElement> (dictionary, nullKeyElements);
		}

		#endregion

		#region SequenceEqual

		public static bool SequenceEqual<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.SequenceEqual (second, null);
		}

		public static bool SequenceEqual<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Check.FirstAndSecond (first, second);

			if (comparer == null)
				comparer = EqualityComparer<TSource>.Default;

			using (IEnumerator<TSource> first_enumerator = first.GetEnumerator (),
				second_enumerator = second.GetEnumerator ()) {

				while (first_enumerator.MoveNext ()) {
					if (!second_enumerator.MoveNext ())
						return false;

					if (!comparer.Equals (first_enumerator.Current, second_enumerator.Current))
						return false;
				}

				return !second_enumerator.MoveNext ();
			}
		}

		#endregion

		#region Union

		public static IEnumerable<TSource> Union<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			Check.FirstAndSecond (first, second);

			return first.Union (second, null);
		}

		public static IEnumerable<TSource> Union<TSource> (this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Check.FirstAndSecond (first, second);

			if (comparer == null)
				comparer = EqualityComparer<TSource>.Default;

			return CreateUnionIterator (first, second, comparer);
		}

		static IEnumerable<TSource> CreateUnionIterator<TSource> (IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			var items = new HashSet<TSource> (comparer);
			foreach (var element in first) {
				if (! items.Contains (element)) {
					items.Add (element);
					yield return element;
				}
			}

			foreach (var element in second) {
				if (! items.Contains (element)) {
					items.Add (element);
					yield return element;
				}
			}
		}

		#endregion

		#region Where

		public static IEnumerable<TSource> Where<TSource> (this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			// It cannot be IList<TSource> because it may break on user implementation
			var array = source as TSource[];
			if (array != null)
				return CreateWhereIterator (array, predicate);

			return CreateWhereIterator (source, predicate);
		}

		static IEnumerable<TSource> CreateWhereIterator<TSource> (IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			foreach (TSource element in source)
				if (predicate (element))
					yield return element;
		}

		static IEnumerable<TSource> CreateWhereIterator<TSource> (TSource[] source, Func<TSource, bool> predicate)
		{
			for (int i = 0; i < source.Length; ++i) {
				var element = source [i];
				if (predicate (element))
					yield return element;
			}
		}

		public static IEnumerable<TSource> Where<TSource> (this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Check.SourceAndPredicate (source, predicate);

			var array = source as TSource[];
			if (array != null)
				return CreateWhereIterator (array, predicate);

			return CreateWhereIterator (source, predicate);
		}

		static IEnumerable<TSource> CreateWhereIterator<TSource> (IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			int counter = 0;
			foreach (TSource element in source) {
				if (predicate (element, counter))
					yield return element;
				counter++;
			}
		}

		static IEnumerable<TSource> CreateWhereIterator<TSource> (TSource[] source, Func<TSource, int, bool> predicate)
		{
			for (int i = 0; i < source.Length; ++i) {
				var element = source [i];
				if (predicate (element, i))
					yield return element;
			}
		}

		#endregion

		#region Exception helpers

		static Exception EmptySequence ()
		{
			return new InvalidOperationException ("Sequence contains no elements");
		}

		static Exception NoMatchingElement ()
		{
			return new InvalidOperationException ("Sequence contains no matching element");
		}

		static Exception MoreThanOneElement ()
		{
			return new InvalidOperationException ("Sequence contains more than one element");
		}

		static Exception MoreThanOneMatchingElement ()
		{
			return new InvalidOperationException ("Sequence contains more than one matching element");
		}

		#endregion
	}
}
