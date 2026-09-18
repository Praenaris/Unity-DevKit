using System;
using UnityEngine;


namespace DragonResonance.Extensions
{
	public static class Vector3Extensions
	{
		#region Operations

			public static bool Approximately(this Vector3 vectorA, Vector3 vectorB) => ((vectorB - vectorA).sqrMagnitude <= Mathf.Epsilon);
			public static Vector3 ProjectOntoPlane(this Vector3 vector, Vector3 planeNormal) => (vector - Vector3.Dot(vector, planeNormal) * planeNormal);
			public static Vector3 ClampMagnitude(this Vector3 vector, float maxMagnitude) => (vector.normalized * Mathf.Min(vector.magnitude, maxMagnitude));

		#endregion


		#region Components

			public static Vector3 NormalizedComponents(this Vector3 vector) => new(Math.Sign(vector.x), Math.Sign(vector.y), Math.Sign(vector.z));
			public static Vector3 AbsoluteComponents(this Vector3 vector) => new(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
			public static float AverageOfTheThree(this Vector3 vector) => ((vector.x + vector.y + vector.z) / 3f);

			public static Vector3 WithX(this Vector3 vector, float x) => new(x, vector.y, vector.z);
			public static Vector3 WithY(this Vector3 vector, float y) => new(vector.x, y, vector.z);
			public static Vector3 WithZ(this Vector3 vector, float z) => new(vector.x, vector.y, z);

		#endregion


		#region Casts

			public static Vector2 ToVector2(this Vector3 vector) => new Vector2(vector.x, vector.y);
			public static Vector2Int ToVector2Int(this Vector3 vector) => new Vector2Int((int)vector.x, (int)vector.y);
			public static Vector3 ToVector3(this Vector3 vector) => new Vector3(vector.x, vector.y, vector.z);
			public static Vector3Int ToVector3Int(this Vector3 vector) => new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);

			public static Quaternion ToUpRotation(this Vector3 direction) =>
				Quaternion.FromToRotation(Vector3.up, direction);
			public static Quaternion ToRightRotation(this Vector3 direction) =>
				Quaternion.FromToRotation(Vector3.right, direction);
			public static Quaternion ToForwardRotation(this Vector3 direction) =>
				Quaternion.FromToRotation(Vector3.forward, direction);

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