using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace DragonResonance.Extensions
{
	public static class TransformExtensions
	{
		public static IEnumerable<Transform> GetChildren(this Transform parent) => parent.Cast<Transform>();
		public static IEnumerable<RectTransform> GetRectChildren(this Transform parent) => parent.OfType<RectTransform>();


		public static void TranslateLocal(this Transform transform, float x, float y, float z) => TranslateLocal(transform, new Vector3(x, y, z));
		public static void TranslateLocal(this Transform transform, Vector3 translation)
		{
			Vector3 localPosition = transform.localPosition;
			transform.localPosition = new Vector3(
				localPosition.x + translation.x,
				localPosition.y + translation.y,
				localPosition.z + translation.z);
		}
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