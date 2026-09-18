#if UNITY_EDITOR


using DragonResonance.Attributes;
using UnityEditor;
using UnityEngine;


namespace DragonResonance.Editor.Attributes
{
	[CustomPropertyDrawer(typeof(LayerAttribute))]
	public class LayerAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Integer) {
				EditorGUI.HelpBox(position, $"{nameof(LayerAttribute)} works just on integers.", MessageType.Error);
				return;
			}

			label = EditorGUI.BeginProperty(position, label, property);

			EditorGUI.BeginChangeCheck();
			int newLayer = EditorGUI.LayerField(position, label, property.intValue);
			if (EditorGUI.EndChangeCheck())
				property.intValue = newLayer;

			EditorGUI.EndProperty();
		}
	}
}


#endif


/*                                                                              */
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