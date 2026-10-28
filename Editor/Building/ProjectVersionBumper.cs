#if UNITY_EDITOR


using DragonResonance.Logging;
using Praenaris.Miscellany;
using UnityEditor;
using UnityEngine;


namespace Praenaris.Editor.Building
{
	/// <summary>
	/// Bumps the project version (the Player Settings' bundleVersion) up and down by its semantic parts, keeping its label.
	/// A window of its own, and a block any other editor can draw with <see cref="DrawBumperGUI"/>.
	/// </summary>
	public class ProjectVersionBumper : EditorWindow
	{
		private const float ButtonWidth = 22f;
		private const float ValueWidth = 36f;
		private const float SeparatorHeight = 1f;

		private static readonly Color SeparatorColor = new(0.5f, 0.5f, 0.5f, 0.5f);
		private static readonly GUIContent DownLabel = new("-");
		private static readonly GUIContent UpLabel = new("+");
		private static readonly GUIContent LabelLabel = new("Label");
		private static GUIStyle _valueStyle_internal = null;	// Caching only, use the property instead


		#region Events

			private void OnGUI()
			{
				EditorGUILayout.Space();
				DrawBumperGUI();
			}

		#endregion


		#region Publics

			[MenuItem("Window/Praenaris/Version Bumper")]
			public static void CreateWindow() => GetWindow<ProjectVersionBumper>("Version Bumper");

			/// <summary>The current version, then a row per part with its lower and bump buttons, and its label.</summary>
			public static void DrawBumperGUI()
			{
				if (!SemanticVersion.TryParse(PlayerSettings.bundleVersion, out SemanticVersion version)) {
					EditorGUILayout.HelpBox($"The project version \"{PlayerSettings.bundleVersion}\" isn't semantic (MAJOR.MINOR.PATCH, optionally -label). Fix it in the Player Settings to bump it.", MessageType.Warning);
					return;
				}

				EditorGUILayout.LabelField($"Version {version}", EditorStyles.boldLabel);
				EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, SeparatorHeight), SeparatorColor);
				DrawPartRow("Major", version.Major, version.MajorDown(), version.MajorUp());
				DrawPartRow("Minor", version.Minor, version.MinorDown(), version.MinorUp());
				DrawPartRow("Patch", version.Patch, version.PatchDown(), version.PatchUp());
				DrawLabelRow(version);
			}

		#endregion


		#region Privates

			private static void DrawPartRow(string partName, int partValue, SemanticVersion downVersion, SemanticVersion upVersion)
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PrefixLabel(partName);	// Labels on the left column, controls on the right one

					EditorGUI.BeginDisabledGroup(partValue <= 0);
					{
						if (GUILayout.Button(DownLabel, EditorStyles.miniButton, GUILayout.Width(ButtonWidth)))
							SetVersion(downVersion);
					}
					EditorGUI.EndDisabledGroup();
					GUILayout.Label(partValue.ToString(), ValueStyle, GUILayout.Width(ValueWidth));
					if (GUILayout.Button(UpLabel, EditorStyles.miniButton, GUILayout.Width(ButtonWidth)))
						SetVersion(upVersion);

					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
			}

			private static void DrawLabelRow(SemanticVersion version)
			{
				string label = EditorGUILayout.DelayedTextField(LabelLabel, version.Label ?? string.Empty);	// Applied once confirmed, not on every key
				if (label == (version.Label ?? string.Empty)) return;

				SemanticVersion labeledVersion = version.WithLabel(label.Trim());
				if (SemanticVersion.TryParse(labeledVersion.ToString(), out _))
					SetVersion(labeledVersion);
				else
					HLogger.LogWarning($"\"{label}\" isn't a valid version label, only letters, digits, dots and dashes", typeof(ProjectVersionBumper));
			}

			private static void SetVersion(SemanticVersion version)
			{
				string previousVersion = PlayerSettings.bundleVersion;
				PlayerSettings.bundleVersion = version.ToString();
				HLogger.LogInfo($"Project version {previousVersion} -> {PlayerSettings.bundleVersion}", typeof(ProjectVersionBumper));
			}

		#endregion


		#region Properties

			private static GUIStyle ValueStyle => (_valueStyle_internal ??= new GUIStyle(EditorStyles.label) {
				alignment = TextAnchor.MiddleCenter,
			});

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