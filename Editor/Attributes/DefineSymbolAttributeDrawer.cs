#if UNITY_EDITOR


using Praenaris.Attributes;
using Praenaris.Miscellany;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.Build;
using UnityEditor;


namespace Praenaris.Editor.Attributes
{
	[CustomPropertyDrawer(typeof(DefineSymbolAttribute))]
	public class DefineSymbolAttributeDrawer : AComboBoxAttributeDrawer
	{
		#region Inheritables

			protected override IEnumerable<string> GetItems(SerializedProperty property) => PlayerSettings
				.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup))
				.Split(';')
				.Select(symbol => symbol.Trim().TrimStart('_'))	// Disabled symbols are prefixed with an underscore
				.Where(symbol => !DefineSymbols.IsDetectable(symbol))
				.OrderBy(symbol => symbol, StringComparer.Ordinal)
				.Concat(DefineSymbols.Detectable);

			protected override string GetItemPath(string item) =>
				(DefineSymbols.IsDetectable(item) ? $"Built-in/{item}" : $"Project/{item}");

		#endregion


		#region Properties

			protected override string UnknownItemTooltip => "Neither a symbol of this project nor one DevKit can detect.";

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