using System;
using UnityEngine;


namespace DragonResonance.Extensions
{
	public static class IntExtensions
	{
		#region Verifications

			public static bool IsPositive(this int testValue) => (testValue > 0);
			public static bool IsPositiveOrZero(this int testValue) => (testValue >= 0);
			public static bool IsNegative(this int testValue) => (testValue < 0);
			public static bool IsNegativeOrZero(this int testValue) => (testValue <= 0);
			public static bool IsZero(this int testValue) => (testValue == 0);
			public static bool IsNotZero(this int testValue) => !IsZero(testValue);

			public static bool IsEven(this int testValue) => ((testValue & 1) == 0);
			public static bool IsOdd(this int testValue) => !IsEven(testValue);

			public static bool IsBetween(this int testValue, int minInclusive, int maxExclusive) =>
				((testValue >= minInclusive) && (testValue < maxExclusive));
			public static bool IsBetweenInclusive(this int testValue, int minInclusive, int maxInclusive) =>
				((testValue >= minInclusive) && (testValue <= maxInclusive));
			public static bool IsBetweenExclusive(this int testValue, int minExclusive, int maxExclusive) =>
				((testValue > minExclusive) && (testValue < maxExclusive));

		#endregion


		#region Operations

			public static int Clamp(this int value, Vector2Int range) => Clamp(value, range.x, range.y);
			public static int Clamp(this int value, int min, int max) => Mathf.Clamp(value, min, max);
			public static int UpperClamp(this int currentValue, int maxValue) =>
				((currentValue > maxValue) ? maxValue : currentValue);
			public static int LowerClamp(this int currentValue, int minValue) =>
				((currentValue < minValue) ? minValue : currentValue);

			public static int ClampUpperZero(this int currentValue) => currentValue.UpperClamp(0);
			public static int ClampLowerZero(this int currentValue) => currentValue.LowerClamp(0);
			public static int ClampUpperOne(this int currentValue) => currentValue.UpperClamp(1);
			public static int ClampLowerOne(this int currentValue) => currentValue.LowerClamp(1);

			public static int NextCyclic(this int currentValue, int upperLimit, int absoluteIncreaseStep = 1) =>
				(currentValue = ((currentValue + absoluteIncreaseStep) % upperLimit));
			public static int PreviousCyclic(this int currentValue, int upperLimit, int absoluteDecreaseStep = 1) =>
				(currentValue = (((currentValue - absoluteDecreaseStep) + upperLimit) % upperLimit));

		#endregion


		#region Iterations

			public static void Times(this int repetitions, Action<int> action)
			{
				for (int loopIndex = 0; loopIndex < repetitions; loopIndex++)
					action.Invoke(loopIndex);
			}

		#endregion


		#region Casts

			public static bool AsBool(this int numericValue) => (numericValue != 0);
			public static char AsChar(this int numericValue) => (char)(numericValue + '0');

		#endregion
	}
}


/*       ________________________________________________________________       */
/*           _________   _______ ________  _______  _______  ___    _           */
/*           |        \ |______/ |______| |  _____ |       | |  \   |           */
/*           |________/ |     \_ |      | |______| |_______| |   \__|           */
/*           ______ _____ _____ _____ __   _ _____ __   _ _____ _____           */
/*           |____/ |____ [___  |   | | \  | |___| | \  | |     |____           */
/*           |    \ |____ ____] |___| |  \_| |   | |  \_| |____ |____           */
/*       ________________________________________________________________       */
/*                                                                              */
/*           David Tabernero M.  <https://github.com/davidtabernerom>           */
/*           Dragon Resonance    <https://github.com/dragonresonance>           */
/*                  Copyright © 2021-2026. All rights reserved.                 */
/*                Licensed under the Apache License, Version 2.0.               */
/*                         See LICENSE.md for more info.                        */
/*       ________________________________________________________________       */
/*                                                                              */