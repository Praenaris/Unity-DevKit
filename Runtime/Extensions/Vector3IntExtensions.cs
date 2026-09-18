using UnityEngine;


namespace DragonResonance.Extensions
{
	public static class Vector3IntExtensions
	{
		#region Operations

			public static Vector3 CalculateNormalized(this Vector3Int vector3int) => (new Vector3(vector3int.x, vector3int.y, vector3int.z)).normalized;

		#endregion


		#region Components

			public static Vector3Int WithX(this Vector3Int vector, int x) => new(x, vector.y, vector.z);
			public static Vector3Int WithY(this Vector3Int vector, int y) => new(vector.x, y, vector.z);
			public static Vector3Int WithZ(this Vector3Int vector, int z) => new(vector.x, vector.y, z);

		#endregion


		#region Casts

			public static Vector2 ToVector2(this Vector3Int vector) => new Vector2(vector.x, vector.y);
			public static Vector2Int ToVector2Int(this Vector3Int vector) => new Vector2Int(vector.x, vector.y);
			public static Vector3 ToVector3(this Vector3Int vector) => new Vector3(vector.x, vector.y, vector.z);
			public static Vector3Int ToVector3Int(this Vector3Int vector) => new Vector3Int(vector.x, vector.y, vector.z);

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