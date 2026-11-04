#if UNITY_EDITOR


using DragonResonance.Extensions;
using System;
using UnityEditor;


namespace Praenaris.Editor.Extensions
{
	public static class SerializedPropertyExtensions
	{
		#region Publics

			public static SerializedProperty FindSiblingProperty(this SerializedProperty property, string siblingName)
			{
				string path = property.propertyPath;
				int arrayIndex = path.LastIndexOf(".Array.data[", StringComparison.Ordinal);
				if (arrayIndex.IsPositiveOrZero())
					path = path.Substring(0, arrayIndex);

				int separatorIndex = path.LastIndexOf('.');
				return property.serializedObject.FindProperty(separatorIndex.IsNegative() ? siblingName : $"{path[..(separatorIndex + 1)]}{siblingName}");
			}

		#endregion
	}
}


#endif


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