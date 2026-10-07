#if UNITY_EDITOR


using Praenaris.Serializables;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace DragonResonance.Editor.Serializables
{
	[CustomPropertyDrawer(typeof(SerializableKeyValuePair<,>))]
	[CustomPropertyDrawer(typeof(SerializableItemGroup<,>))]
	[CustomPropertyDrawer(typeof(SerializableItemGroup<,,>))]
	[CustomPropertyDrawer(typeof(SerializableItemGroup<,,,>))]
	[CustomPropertyDrawer(typeof(SerializableItemGroup<,,,,>))]
	[CustomPropertyDrawer(typeof(SerializableItemGroup<,,,,,>))]
	[CustomPropertyDrawer(typeof(SerializableItemGroup<,,,,,,>))]
	[CustomPropertyDrawer(typeof(SerializableItemGroup<,,,,,,,>))]
	[CustomPropertyDrawer(typeof(SerializableItemGroup<,,,,,,,,>))]
	public class SerializableFieldGroupDrawer : PropertyDrawer
	{
		private const float SPACING = 4f;


		private static readonly string[] FieldNames = {
			"Key", "Value",
			"First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth", "Ninth",
		};


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			{
				position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

				List<SerializedProperty> fields = GetFields(property);
				float fieldWidth = (position.width - SPACING * (fields.Count - 1)) / fields.Count;

				for (int fieldIndex = 0; fieldIndex < fields.Count; fieldIndex++) {
					float positionX = position.x + (fieldIndex * (fieldWidth + SPACING));
					Rect rect = new(positionX, position.y, fieldWidth, EditorGUIUtility.singleLineHeight);
					EditorGUI.PropertyField(rect, fields[fieldIndex], GUIContent.none);
				}
			}
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;


		private List<SerializedProperty> GetFields(SerializedProperty property) =>
			FieldNames.Select(property.FindPropertyRelative).Where(prop => (prop != null)).ToList();
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