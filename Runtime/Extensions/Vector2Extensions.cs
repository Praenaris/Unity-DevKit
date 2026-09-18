using System.Runtime.CompilerServices;
using System;
using UnityEngine;


namespace DragonResonance.Extensions
{
	public static class Vector2Extensions
	{
		#region Operations

			public static bool Approximately(this Vector2 vectorA, Vector2 vectorB) => ((vectorB - vectorA).sqrMagnitude <= Mathf.Epsilon);

			public static Vector2 SortedLow2High(this Vector2 vector) => (vector.x > vector.y) ? (new Vector2(vector.y, vector.x)) : vector;
			public static Vector2 SortedHigh2Low(this Vector2 vector) => (vector.x < vector.y) ? (new Vector2(vector.y, vector.x)) : vector;

			public static float Lerp(this Vector2 vector, float t) => Mathf.Lerp(vector.x, vector.y, t);
			public static float InverseLerp(this Vector2 vector, float value) => Mathf.InverseLerp(vector.x, vector.y, value);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Vector2 Rotated(this Vector2 vector, float radians)
			{
				float sin = MathF.Sin(radians);
				float cos = MathF.Cos(radians);
				return new Vector2((vector.x * cos) - (vector.y * sin), (vector.x * sin) + (vector.y * cos));
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Vector2 RotatedDegrees(this Vector2 vector, float degrees) => vector.Rotated(degrees * Mathf.Deg2Rad);

		#endregion


		#region Search

			public static float Random(this Vector2 vector) => UnityEngine.Random.Range(vector.x, vector.y);

		#endregion


		#region Components

			public static float AverageOfTheTwo(this Vector2 vector) => ((vector.x + vector.y) / 2f);

			public static Vector2 WithX(this Vector2 vector, float x) => new(x, vector.y);
			public static Vector2 WithY(this Vector2 vector, float y) => new(vector.x, y);

		#endregion


		#region Casts

			public static Vector2 ToVector2(this Vector2 vector) => new(vector.x, vector.y);
			public static Vector2Int ToVector2Int(this Vector2 vector) => new((int)vector.x, (int)vector.y);
			public static Vector3 ToVector3(this Vector2 vector) => new(vector.x, vector.y, 0f);
			public static Vector3Int ToVector3Int(this Vector2 vector) => new((int)vector.x, (int)vector.y, 0);

			public static Quaternion ToUpRotation(this Vector2 direction) =>
				Quaternion.LookRotation(Vector3.forward, direction);
			public static Quaternion ToRightRotation(this Vector2 direction) =>
				Quaternion.LookRotation(Vector3.forward, Vector2.Perpendicular(direction));

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