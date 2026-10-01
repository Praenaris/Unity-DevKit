#if UNITY_EDITOR


using Praenaris.Attributes;
using System;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace Praenaris.Editor.Attributes
{
	[CustomPropertyDrawer(typeof(AutoReferenceAttribute))]
	public class AutoReferenceAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.ObjectReference || !typeof(UnityObject).IsAssignableFrom(base.fieldInfo.FieldType)) {
				EditorGUI.HelpBox(position, $"{nameof(AutoReferenceAttribute)} works just on object references.", MessageType.Error);
				return;
			}

			if (property.objectReferenceValue == null)
				this.TryAutoFill(property);

			EditorGUI.PropertyField(position, property, label, true);
		}


		private void TryAutoFill(SerializedProperty property)
		{
			Type type = base.fieldInfo.FieldType;
			string[] guids = AssetDatabase.FindAssets($"t:{type.Name}");

			foreach (string guid in guids) {
				UnityObject asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), type);
				if (asset == null) continue;

				property.objectReferenceValue = asset;
				return;
			}
		}
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