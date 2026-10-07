using System;


namespace Praenaris.Serializables
{
	[Serializable]
	public struct SerializableItemGroup<T1, T2>
	{
		public T1 First;
		public T2 Second;

		public SerializableItemGroup(T1 first, T2 second)
		{
			First = first;
			Second = second;
		}

		public void Deconstruct(out T1 first, out T2 second)
		{
			first = First;
			second = Second;
		}

		public static implicit operator (T1, T2)(SerializableItemGroup<T1, T2> item) =>
			(item.First, item.Second);
		public static implicit operator SerializableItemGroup<T1, T2>((T1, T2) item) =>
			new(item.Item1, item.Item2);
	}


	[Serializable]
	public struct SerializableItemGroup<T1, T2, T3>
	{
		public T1 First;
		public T2 Second;
		public T3 Third;

		public SerializableItemGroup(T1 first, T2 second, T3 third)
		{
			First = first;
			Second = second;
			Third = third;
		}

		public void Deconstruct(out T1 first, out T2 second, out T3 third)
		{
			first = First;
			second = Second;
			third = Third;
		}

		public static implicit operator (T1, T2, T3)(SerializableItemGroup<T1, T2, T3> item) =>
			(item.First, item.Second, item.Third);
		public static implicit operator SerializableItemGroup<T1, T2, T3>((T1, T2, T3) item) =>
			new(item.Item1, item.Item2, item.Item3);
	}


	[Serializable]
	public struct SerializableItemGroup<T1, T2, T3, T4>
	{
		public T1 First;
		public T2 Second;
		public T3 Third;
		public T4 Fourth;

		public SerializableItemGroup(T1 first, T2 second, T3 third, T4 fourth)
		{
			First = first;
			Second = second;
			Third = third;
			Fourth = fourth;
		}

		public void Deconstruct(out T1 first, out T2 second, out T3 third, out T4 fourth)
		{
			first = First;
			second = Second;
			third = Third;
			fourth = Fourth;
		}

		public static implicit operator (T1, T2, T3, T4)(SerializableItemGroup<T1, T2, T3, T4> item) =>
			(item.First, item.Second, item.Third, item.Fourth);
		public static implicit operator SerializableItemGroup<T1, T2, T3, T4>((T1, T2, T3, T4) item) =>
			new(item.Item1, item.Item2, item.Item3, item.Item4);
	}


	[Serializable]
	public struct SerializableItemGroup<T1, T2, T3, T4, T5>
	{
		public T1 First;
		public T2 Second;
		public T3 Third;
		public T4 Fourth;
		public T5 Fifth;

		public SerializableItemGroup(T1 first, T2 second, T3 third, T4 fourth, T5 fifth)
		{
			First = first;
			Second = second;
			Third = third;
			Fourth = fourth;
			Fifth = fifth;
		}

		public void Deconstruct(out T1 first, out T2 second, out T3 third, out T4 fourth, out T5 fifth)
		{
			first = First;
			second = Second;
			third = Third;
			fourth = Fourth;
			fifth = Fifth;
		}

		public static implicit operator (T1, T2, T3, T4, T5)(SerializableItemGroup<T1, T2, T3, T4, T5> item) =>
			(item.First, item.Second, item.Third, item.Fourth, item.Fifth);
		public static implicit operator SerializableItemGroup<T1, T2, T3, T4, T5>((T1, T2, T3, T4, T5) item) =>
			new(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5);
	}


	[Serializable]
	public struct SerializableItemGroup<T1, T2, T3, T4, T5, T6>
	{
		public T1 First;
		public T2 Second;
		public T3 Third;
		public T4 Fourth;
		public T5 Fifth;
		public T6 Sixth;

		public SerializableItemGroup(T1 first, T2 second, T3 third, T4 fourth, T5 fifth, T6 sixth)
		{
			First = first;
			Second = second;
			Third = third;
			Fourth = fourth;
			Fifth = fifth;
			Sixth = sixth;
		}

		public void Deconstruct(out T1 first, out T2 second, out T3 third, out T4 fourth, out T5 fifth, out T6 sixth)
		{
			first = First;
			second = Second;
			third = Third;
			fourth = Fourth;
			fifth = Fifth;
			sixth = Sixth;
		}

		public static implicit operator (T1, T2, T3, T4, T5, T6)(SerializableItemGroup<T1, T2, T3, T4, T5, T6> item) =>
			(item.First, item.Second, item.Third, item.Fourth, item.Fifth, item.Sixth);
		public static implicit operator SerializableItemGroup<T1, T2, T3, T4, T5, T6>((T1, T2, T3, T4, T5, T6) item) =>
			new(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5, item.Item6);
	}


	[Serializable]
	public struct SerializableItemGroup<T1, T2, T3, T4, T5, T6, T7>
	{
		public T1 First;
		public T2 Second;
		public T3 Third;
		public T4 Fourth;
		public T5 Fifth;
		public T6 Sixth;
		public T7 Seventh;

		public SerializableItemGroup(T1 first, T2 second, T3 third, T4 fourth, T5 fifth, T6 sixth, T7 seventh)
		{
			First = first;
			Second = second;
			Third = third;
			Fourth = fourth;
			Fifth = fifth;
			Sixth = sixth;
			Seventh = seventh;
		}

		public void Deconstruct(out T1 first, out T2 second, out T3 third, out T4 fourth, out T5 fifth, out T6 sixth, out T7 seventh)
		{
			first = First;
			second = Second;
			third = Third;
			fourth = Fourth;
			fifth = Fifth;
			sixth = Sixth;
			seventh = Seventh;
		}

		public static implicit operator (T1, T2, T3, T4, T5, T6, T7)(SerializableItemGroup<T1, T2, T3, T4, T5, T6, T7> item) =>
			(item.First, item.Second, item.Third, item.Fourth, item.Fifth, item.Sixth, item.Seventh);
		public static implicit operator SerializableItemGroup<T1, T2, T3, T4, T5, T6, T7>((T1, T2, T3, T4, T5, T6, T7) item) =>
			new(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5, item.Item6, item.Item7);
	}


	[Serializable]
	public struct SerializableItemGroup<T1, T2, T3, T4, T5, T6, T7, T8>
	{
		public T1 First;
		public T2 Second;
		public T3 Third;
		public T4 Fourth;
		public T5 Fifth;
		public T6 Sixth;
		public T7 Seventh;
		public T8 Eighth;

		public SerializableItemGroup(T1 first, T2 second, T3 third, T4 fourth, T5 fifth, T6 sixth, T7 seventh, T8 eighth)
		{
			First = first;
			Second = second;
			Third = third;
			Fourth = fourth;
			Fifth = fifth;
			Sixth = sixth;
			Seventh = seventh;
			Eighth = eighth;
		}

		public void Deconstruct(out T1 first, out T2 second, out T3 third, out T4 fourth, out T5 fifth, out T6 sixth, out T7 seventh, out T8 eighth)
		{
			first = First;
			second = Second;
			third = Third;
			fourth = Fourth;
			fifth = Fifth;
			sixth = Sixth;
			seventh = Seventh;
			eighth = Eighth;
		}

		public static implicit operator (T1, T2, T3, T4, T5, T6, T7, T8)(SerializableItemGroup<T1, T2, T3, T4, T5, T6, T7, T8> item) =>
			(item.First, item.Second, item.Third, item.Fourth, item.Fifth, item.Sixth, item.Seventh, item.Eighth);
		public static implicit operator SerializableItemGroup<T1, T2, T3, T4, T5, T6, T7, T8>((T1, T2, T3, T4, T5, T6, T7, T8) item) =>
			new(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5, item.Item6, item.Item7, item.Item8);
	}


	[Serializable]
	public struct SerializableItemGroup<T1, T2, T3, T4, T5, T6, T7, T8, T9>
	{
		public T1 First;
		public T2 Second;
		public T3 Third;
		public T4 Fourth;
		public T5 Fifth;
		public T6 Sixth;
		public T7 Seventh;
		public T8 Eighth;
		public T9 Ninth;

		public SerializableItemGroup(T1 first, T2 second, T3 third, T4 fourth, T5 fifth, T6 sixth, T7 seventh, T8 eighth, T9 ninth)
		{
			First = first;
			Second = second;
			Third = third;
			Fourth = fourth;
			Fifth = fifth;
			Sixth = sixth;
			Seventh = seventh;
			Eighth = eighth;
			Ninth = ninth;
		}

		public void Deconstruct(out T1 first, out T2 second, out T3 third, out T4 fourth, out T5 fifth, out T6 sixth, out T7 seventh, out T8 eighth, out T9 ninth)
		{
			first = First;
			second = Second;
			third = Third;
			fourth = Fourth;
			fifth = Fifth;
			sixth = Sixth;
			seventh = Seventh;
			eighth = Eighth;
			ninth = Ninth;
		}

		public static implicit operator (T1, T2, T3, T4, T5, T6, T7, T8, T9)(SerializableItemGroup<T1, T2, T3, T4, T5, T6, T7, T8, T9> item) =>
			(item.First, item.Second, item.Third, item.Fourth, item.Fifth, item.Sixth, item.Seventh, item.Eighth, item.Ninth);
		public static implicit operator SerializableItemGroup<T1, T2, T3, T4, T5, T6, T7, T8, T9>((T1, T2, T3, T4, T5, T6, T7, T8, T9) item) =>
			new(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5, item.Item6, item.Item7, item.Item8, item.Item9);
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