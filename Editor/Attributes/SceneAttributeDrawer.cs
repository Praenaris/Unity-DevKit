#if UNITY_EDITOR


using Praenaris.Attributes;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;


namespace Praenaris.Editor.Attributes
{
	[CustomPropertyDrawer(typeof(SceneAttribute))]
	public class SceneAttributeDrawer : PropertyDrawer
	{
		private const string WrongTypeMessage = nameof(SceneAttribute) + " works just on strings.";


		#region Events

			public override VisualElement CreatePropertyGUI(SerializedProperty property)
			{
				if (property.propertyType != SerializedPropertyType.String)
					return new HelpBox(WrongTypeMessage, HelpBoxMessageType.Error);

				VisualElement container = new();
				ObjectField sceneField = new(base.preferredLabel) { allowSceneObjects = false, objectType = typeof(SceneAsset) };
				HelpBox missingSceneBox = new(string.Empty, HelpBoxMessageType.Warning);
				sceneField.AddToClassList(ObjectField.alignedFieldUssClassName);
				sceneField.RegisterValueChangedCallback(changeEvent => {
					property.stringValue = GetScenePath(changeEvent.newValue);
					property.serializedObject.ApplyModifiedProperties();
				});
				container.Add(sceneField);
				container.Add(missingSceneBox);

				UpdateDisplay(sceneField, missingSceneBox, property);
				sceneField.TrackPropertyValue(property, changedProperty => UpdateDisplay(sceneField, missingSceneBox, changedProperty));
				return container;
			}

			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				if (property.propertyType != SerializedPropertyType.String) {
					EditorGUI.HelpBox(position, WrongTypeMessage, MessageType.Error);
					return;
				}

				label = EditorGUI.BeginProperty(position, label, property);
				SceneAsset currentScene = LoadScene(property.stringValue);
				if (IsMissing(property.stringValue, currentScene)) {
					label.image = EditorGUIUtility.IconContent("console.warnicon.sml").image;
					label.tooltip = MissingSceneMessage(property.stringValue);
				}

				EditorGUI.BeginChangeCheck();
				SceneAsset newScene = (SceneAsset)EditorGUI.ObjectField(position, label, currentScene, typeof(SceneAsset), false);
				if (EditorGUI.EndChangeCheck())
					property.stringValue = GetScenePath(newScene);

				EditorGUI.EndProperty();
			}

		#endregion


		#region Privates

			private static void UpdateDisplay(ObjectField sceneField, HelpBox missingSceneBox, SerializedProperty property)
			{
				SceneAsset scene = LoadScene(property.stringValue);
				sceneField.SetValueWithoutNotify(scene);
				sceneField.showMixedValue = property.hasMultipleDifferentValues;
				sceneField.EnableInClassList(BindingExtensions.prefabOverrideUssClassName, property.prefabOverride);
				missingSceneBox.text = MissingSceneMessage(property.stringValue);
				missingSceneBox.style.display = (IsMissing(property.stringValue, scene) ? DisplayStyle.Flex : DisplayStyle.None);
			}

			private static string GetScenePath(Object scene) => ((scene == null) ? string.Empty : AssetDatabase.GetAssetPath(scene));
			private static bool IsMissing(string scenePath, SceneAsset scene) => (!string.IsNullOrEmpty(scenePath) && (scene == null));
			private static SceneAsset LoadScene(string scenePath) => AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
			private static string MissingSceneMessage(string scenePath) => $"Missing scene: {scenePath}";

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