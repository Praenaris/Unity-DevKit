#if UNITY_EDITOR


using DragonResonance.Attributes;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;


namespace DragonResonance.Editor.Attributes
{
	[CustomPropertyDrawer(typeof(TypeFilterAttribute))]
	public class TypeFilterDrawer : ADropdownArrayAttributeDrawer
	{
		private static readonly Dictionary<Type, string[]> _itemsByBaseType = new();	// Statics are reset on every domain reload, like the TypeCache


		#region Inheritables

			protected override string[] GetItems(SerializedProperty property)
			{
				Type baseType = ((TypeFilterAttribute)base.attribute).BaseType;
				if (baseType == null) return Array.Empty<string>();

				if (!_itemsByBaseType.TryGetValue(baseType, out string[] items))
					_itemsByBaseType[baseType] = items = TypeCache.GetTypesDerivedFrom(baseType)
						.Prepend(baseType)
						.Where(type => !type.IsAbstract && !type.IsInterface)
						.OrderBy(type => type.FullName, StringComparer.Ordinal)
						.Select(type => type.AssemblyQualifiedName)
						.Prepend(string.Empty)
						.ToArray();
				return items;
			}

			protected override string GetItemLabel(string item) => item.Split(',')[0];	// Without the assembly

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