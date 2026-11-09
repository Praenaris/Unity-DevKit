using System.Collections.Generic;
using System.Linq;
using System;


namespace DragonResonance.Extensions
{
	public static class IEnumerableExtensions
	{
		#region Publics - Verification

			public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => enumerable switch {
				ICollection<T> collection => (collection.Count == 0),
				IReadOnlyCollection<T> readOnlyCollection => (readOnlyCollection.Count == 0),
				_ => !enumerable.Any(),
			};

			public static int LastIndex<T>(this IEnumerable<T> enumerable) => enumerable switch {
				ICollection<T> collection => (collection.Count - 1),
				IReadOnlyCollection<T> readOnlyCollection => (readOnlyCollection.Count - 1),
				_ => (enumerable.Count() - 1),
			};

			public static bool IsLastItem<T>(this IEnumerable<T> enumerable, T item) => IsLastItem(enumerable, item, EqualityComparer<T>.Default);
			public static bool IsLastItem<T>(this IEnumerable<T> enumerable, T item, IEqualityComparer<T> comparer)
			{
				if (enumerable is IList<T> list)
					return (list.Count.IsNotZero() && comparer.Equals(list[^1], item));
				if (enumerable is IReadOnlyList<T> readOnlyList)
					return (readOnlyList.Count.IsNotZero() && comparer.Equals(readOnlyList[^1], item));

				//	Only walked when it can't be indexed
				bool hasItems = false;
				T lastItem = default;
				foreach (T currentItem in enumerable) {
					hasItems = true;
					lastItem = currentItem;
				}
				return (hasItems && comparer.Equals(lastItem, item));
			}

		#endregion


		#region Publics - Matching

			public static T FirstMatch<T>(this IEnumerable<T> contentEnumerable, IEnumerable<T> containerEnumerable) =>
				FirstMatchOrFallback(contentEnumerable, containerEnumerable, default);
			public static T FirstMatchOrFallback<T>(this IEnumerable<T> contentEnumerable, IEnumerable<T> containerEnumerable, T fallback)
			{
				HashSet<T> container = new(containerEnumerable);
				foreach (T item in contentEnumerable)
					if (container.Contains(item))
						return item;

				return fallback;
			}

			public static bool MatchesAny<T>(this IEnumerable<T> enumerableA, IEnumerable<T> enumerableB)
			{
				HashSet<T> setA = new(enumerableA);
				return enumerableB.Any(setA.Contains);
			}

			public static int Matches<T>(this IEnumerable<T> enumerableA, IEnumerable<T> enumerableB)
			{
				HashSet<T> setA = new(enumerableA);
				return enumerableB.Count(setA.Contains);
			}

		#endregion


		#region Publics - Searching

			public static int IndexOf<T>(this IEnumerable<T> enumerable, T value) => IndexOf(enumerable, value, EqualityComparer<T>.Default);
			public static int IndexOf<T>(this IEnumerable<T> enumerable, T value, IEqualityComparer<T> comparer)
			{
				int index = 0;
				foreach (T item in enumerable) {
					if (comparer.Equals(item, value)) return index;
					index++;
				}

				return -1;
			}

			public static int IndexOf<T>(this IEnumerable<T> source, IEnumerable<T> match)
			{
				T[] sourceArray = source.ToArray();
				foreach (T matchItem in match) {
					int index = Array.IndexOf(sourceArray, matchItem);
					if (index.IsPositiveOrZero()) return index;
				}

				return -1;
			}

		#endregion


		#region Publics - Iteration

			public static int RandomIndex<T>(this IEnumerable<T> enumerable) => UnityEngine.Random.Range(0, enumerable.Count());

			public static T Random<T>(this IEnumerable<T> enumerable)
			{
				IList<T> list = ((enumerable as IList<T>) ?? enumerable.ToArray());	// Lists and arrays are not copied
				return list[UnityEngine.Random.Range(0, list.Count)];
			}

			public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
			{
				T[] items = enumerable.ToArray();
				for (int itemIndex = (items.Length - 1); itemIndex.IsPositiveOrZero(); itemIndex--) {	// Fisher-Yates, yielding as it goes
					int pickedIndex = UnityEngine.Random.Range(0, (itemIndex + 1));
					yield return items[pickedIndex];
					items[pickedIndex] = items[itemIndex];
				}
			}

			public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
			{
				foreach (T item in enumerable)
					action(item);
			}

		#endregion
	}
}


/*                                                                                                                */
/*       `7MM"""Mq.`7MM"""Mq.       db     `7MM"""YMM  `7MN.   `7MF'     db     `7MM"""Mq. `7MMF' .M"""bgd        */
/*         MM   `MM. MM   `MM.     ;MM:      MM    `7    MMN.    M      ;MM:      MM   `MM.  MM  ,MI    "Y        */
/*         MM   ,M9  MM   ,M9     ,V^MM.     MM   d      M YMb   M     ,V^MM.     MM   ,M9   MM  `MMb.            */
/*         MMmmdM9   MMmmdM9     ,M  `MM     MMmmMM      M  `MN. M    ,M  `MM     MMmmdM9    MM    `YMMNq.        */
/*         MM        MM  YM.     AbmmmqMA    MM   Y  ,   M   `MM.M    AbmmmqMA    MM  YM.    MM  .     `MM        */
/*         MM        MM   `Mb.  A'     VML   MM     ,M   M     YMM   A'     VML   MM   `Mb.  MM  Mb     dM        */
/*       .JMML.    .JMML. .JMM.AMA.   .AMMA.JMMmmmmMMM .JML.    YM .AMA.   .AMMA.JMML. .JMM.JMML.P"Ybmmd"         */
/*                                                                                                                */
/*                 Licensed under the Apache License, Version 2.0.  See LICENSE.md for more info.                 */
/*                                     Copyright © 2026. All rights reserved.                                     */
/*                                                                                                                */