#if UNITY_EDITOR


using DragonResonance.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.Build;
using UnityEditor;


namespace DragonResonance.Editor.Building
{
	[InitializeOnLoad]
	public static class BuildDefines
	{
		#region Constructors

			static BuildDefines() => ApplyDefinitions(new SortedSet<string>(
				BuildDefines.CurrentDefinitions,
				Comparer<string>.Create((defineA, defineB) =>
					string.Compare(defineA.TrimStart('_'), defineB.TrimStart('_'), StringComparison.OrdinalIgnoreCase))));

		#endregion


		#region Publics

			public static void ToggleBuildDefinition(string definition)
			{
				bool containsDefinition = BuildDefines.CurrentDefinitions.Contains(definition);
				SetDefinition((containsDefinition ? FormatToggledDefinition(definition) : definition), true);
			}

			public static void SetDefinitionState(string definition, bool enabled)
			{
				string enabledDefinition = definition.TrimStart('_');
				SetDefinition((enabled ? enabledDefinition : FormatToggledDefinition(enabledDefinition)), true);
			}

			public static void SetDefinition(string definition, bool overrideState)
			{
				//Debug.Log($"definition:{definition}, overrideState:{overrideState}");
				HashSet<string> definitions = new(BuildDefines.CurrentDefinitions);
				{
					string toggledDefinition = FormatToggledDefinition(definition);
					if (!overrideState) {
						if (!definitions.Contains(toggledDefinition))
							definitions.AddOrIgnore(definition);
					}
					else {
						definitions.Remove(toggledDefinition);
						definitions.AddOrIgnore(definition);
					}
				}
				//Debug.Log(string.Join(", ", definitions));
				ApplyDefinitions(definitions);
			}

			public static bool CheckEnabledDefinition(string definition) =>
				BuildDefines.CurrentDefinitions.Contains(definition.TrimStart('_'));

		#endregion


		#region Privates

			private static string FormatToggledDefinition(string definition) =>
				definition.StartsWith('_') ? definition.TrimStart('_') : $"_{definition}";

			private static void ApplyDefinitions(IEnumerable<string> definitions)
			{
				string[] cleanedDefinitions = definitions
					.Select(definition => definition?.Trim())
					.Where(definition => !string.IsNullOrEmpty(definition) && !string.IsNullOrEmpty(definition.Trim('_')))
					.ToArray();

				PlayerSettings.SetScriptingDefineSymbols(
					NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup),
					cleanedDefinitions);
			}

		#endregion


		#region Properties

			public static IEnumerable<string> CurrentDefinitions => PlayerSettings
				.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup))
				.Split(';');

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