#if UNITY_EDITOR


using DragonResonance.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.Build;
using UnityEditor;
using UnityEngine;


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
				SetupBuildDefinition((containsDefinition ? FormatToggledDefinition(definition) : definition), true);
			}

			public static void SetupBuildDefinition(string definition, bool overrideState)
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


/*       ________________________________________________________________       */
/*           _________   _______ ________  _______  _______  ___    _           */
/*           |        \ |______/ |______| |  _____ |       | |  \   |           */
/*           |________/ |     \_ |      | |______| |_______| |   \__|           */
/*           ______ _____ _____ _____ __   _ _____ __   _ _____ _____           */
/*           |____/ |____ [___  |   | | \  | |___| | \  | |     |____           */
/*           |    \ |____ ____] |___| |  \_| |   | |  \_| |____ |____           */
/*       ________________________________________________________________       */
/*                                                                              */
/*           David Tabernero M.  <https://github.com/davidtabernerom>           */
/*           Dragon Resonance    <https://github.com/dragonresonance>           */
/*                  Copyright © 2021-2026. All rights reserved.                 */
/*                Licensed under the Apache License, Version 2.0.               */
/*                         See LICENSE.md for more info.                        */
/*       ________________________________________________________________       */
/*                                                                              */