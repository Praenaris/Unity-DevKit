using UnityEngine;


namespace DragonResonance.Extensions
{
	public static class FloatExtensions
	{
		#region Verifications

			public static bool IsPositive(this float testValue) => (testValue > 0f);
			public static bool IsPositiveOrZero(this float testValue) => (testValue >= 0f);
			public static bool IsNegative(this float testValue) => (testValue < 0f);
			public static bool IsNegativeOrZero(this float testValue) => (testValue <= 0f);

			public static bool IsBetween(this float testValue, int minInclusive, int maxExclusive) =>
				((testValue >= minInclusive) && (testValue < maxExclusive));
			public static bool IsBetweenInclusive(this float testValue, int minInclusive, int maxInclusive) =>
				((testValue >= minInclusive) && (testValue <= maxInclusive));
			public static bool IsBetweenExclusive(this float testValue, int minExclusive, int maxExclusive) =>
				((testValue > minExclusive) && (testValue < maxExclusive));

			public static bool Approximately(this float source, float value) =>
				(Mathf.Approximately(source, value));

		#endregion


		#region Operations

			public static float Complementary(this float value) => (1f - value);
			public static float Clamp(this float value, Vector2 range) => Clamp(value, range.x, range.y);
			public static float Clamp(this float value, float min, float max) => Mathf.Clamp(value, min, max);

			public static float SumToAverage(this float currentAverage, float newValue, int valueCount) =>
				(((currentAverage * (valueCount - 1)) + newValue) / valueCount);

			public static float SumToAbsolute(this float currentValue, float addend) =>
				(Mathf.Sign(currentValue) * (Mathf.Abs(currentValue) + addend));
			public static float SubtractToAbsolute(this float currentValue, float substrahend) =>
				(Mathf.Sign(currentValue) * (Mathf.Abs(currentValue) - substrahend));
			public static float MultiplyToAbsolute(this float currentValue, float factor) =>
				(Mathf.Sign(currentValue) * (Mathf.Abs(currentValue) * factor));
			public static float DivideToAbsolute(this float currentValue, float divisor) =>
				(Mathf.Sign(currentValue) * (Mathf.Abs(currentValue) / divisor));

			public static float ClampAbsolute(this float currentValue, float maxAbsolute) =>
				(Mathf.Sign(currentValue) * UpperClamp(Mathf.Abs(currentValue), maxAbsolute));
			public static float UpperClamp(this float currentValue, float maxValue) =>
				((currentValue > maxValue) ? maxValue : currentValue);
			public static float LowerClamp(this float currentValue, float minValue) =>
				((currentValue < minValue) ? minValue : currentValue);

			public static float ClampUpperZero(this float currentValue) => currentValue.UpperClamp(0f);
			public static float ClampLowerZero(this float currentValue) => currentValue.LowerClamp(0f);
			public static float ClampUpperOne(this float currentValue) => currentValue.UpperClamp(1f);
			public static float ClampLowerOne(this float currentValue) => currentValue.LowerClamp(1f);


			public static float AddToAverage(this float currentAverage, float newValue, ref int currentSamples, int maxSamples)
			{
				float totalValue = currentAverage * currentSamples;

				if (currentSamples >= maxSamples) {
					currentSamples--;
					totalValue -= currentAverage;
				}

				totalValue += newValue;
				currentSamples++;

				if (currentSamples > maxSamples) {
					currentSamples--;
					totalValue -= currentAverage;
				}

				return (totalValue / currentSamples);
			}

		#endregion


		#region Randomizations

			public static bool RollChance(this float probability) => (Random.value < probability);

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