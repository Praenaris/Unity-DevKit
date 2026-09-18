using UnityEngine;


namespace DragonResonance.Extensions
{
	public static class Vector2IntExtensions
	{
		#region Operations

			public static Vector2Int SortedLow2High(this Vector2Int vector) => ((vector.x > vector.y) ? (new Vector2Int(vector.y, vector.x)) : vector);
			public static Vector2Int SortedHigh2Low(this Vector2Int vector) => (vector.x < vector.y) ? (new Vector2Int(vector.y, vector.x)) : vector;

			public static float Lerp(this Vector2Int vector, float t) => Mathf.Lerp(vector.x, vector.y, t);
			public static float InverseLerp(this Vector2Int vector, float value) => Mathf.InverseLerp(vector.x, vector.y, value);

		#endregion


		#region Search

			public static int Random(this Vector2Int vector) => UnityEngine.Random.Range(vector.x, vector.y);
			public static int RandomInclusive(this Vector2Int vector) => UnityEngine.Random.Range(vector.x, (vector.y + 1));
			public static int RandomExclusive(this Vector2Int vector) => UnityEngine.Random.Range((vector.x + 1), vector.y);

		#endregion


		#region Components

			public static float AverageOfTheTwo(this Vector2Int vector) => ((vector.x + vector.y) / 2f);

			public static Vector2Int WithX(this Vector2Int vector, int x) => new(x, vector.y);
			public static Vector2Int WithY(this Vector2Int vector, int y) => new(vector.x, y);

		#endregion


		#region Casts

			public static Vector2 ToVector2(this Vector2Int vector) => new Vector2(vector.x, vector.y);
			public static Vector2Int ToVector2Int(this Vector2Int vector) => new Vector2Int(vector.x, vector.y);
			public static Vector3 ToVector3(this Vector2Int vector) => new Vector3(vector.x, vector.y, 0f);
			public static Vector3Int ToVector3Int(this Vector2Int vector) => new Vector3Int(vector.x, vector.y, 0);

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