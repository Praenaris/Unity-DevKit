#if UNITY_EDITOR


using DragonResonance.Attributes;
using System.Globalization;
using UnityEditor;
using UnityEngine;




namespace DragonResonance.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRangeAttributeDrawer : PropertyDrawer
    {
	    private const float HORIZONTAL_WIDTH = 5f;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinMaxRangeAttribute rangeAttribute = (MinMaxRangeAttribute)base.attribute ?? new MinMaxRangeAttribute(0, 1);
            SerializedProperty xRangeProperty = property.FindPropertyRelative(nameof(Vector2.x));
            SerializedProperty yRangeProperty = property.FindPropertyRelative(nameof(Vector2.y));

            int originalIndent = EditorGUI.indentLevel;
            float fieldWidth = UnityEngine.GUI.skin.textField.CalcSize(new GUIContent(rangeAttribute.Max.ToString(CultureInfo.InvariantCulture))).x + HORIZONTAL_WIDTH;
            bool isVector2Int = property.propertyType == SerializedPropertyType.Vector2Int;

            float minValue = isVector2Int ? xRangeProperty.intValue :
	            (rangeAttribute.RoundToInt ? Mathf.RoundToInt(xRangeProperty.floatValue) : xRangeProperty.floatValue);
            float maxValue = isVector2Int ? yRangeProperty.intValue :
	            (rangeAttribute.RoundToInt ? Mathf.RoundToInt(yRangeProperty.floatValue) : yRangeProperty.floatValue);

            label = EditorGUI.BeginProperty(position, label, property);
            {
	            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

	            Rect left = new(position.x, position.y, fieldWidth, position.height);
	            Rect right = new(position.x + position.width - left.width, position.y, fieldWidth, position.height);

	            EditorGUI.indentLevel = 0;
	            position.x += fieldWidth + HORIZONTAL_WIDTH;
	            position.width -= (fieldWidth + HORIZONTAL_WIDTH) * 2;

	            minValue = Mathf.Clamp(isVector2Int ?
			            EditorGUI.IntField(left, GUIContent.none, (int)minValue) :
			            EditorGUI.FloatField(left, GUIContent.none, minValue), rangeAttribute.Min, maxValue);
	            maxValue = Mathf.Clamp(isVector2Int ?
			            EditorGUI.IntField(right, GUIContent.none, (int)maxValue) :
			            EditorGUI.FloatField(right, GUIContent.none, maxValue), minValue, rangeAttribute.Max);

	            EditorGUI.MinMaxSlider(position, GUIContent.none, ref minValue, ref maxValue, rangeAttribute.Min, rangeAttribute.Max);

	            if (isVector2Int) {
		            xRangeProperty.intValue = Mathf.RoundToInt(minValue);
		            yRangeProperty.intValue = Mathf.RoundToInt(maxValue);
	            }
	            else {
		            xRangeProperty.floatValue = rangeAttribute.RoundToInt ? Mathf.RoundToInt(minValue) : minValue;
		            yRangeProperty.floatValue = rangeAttribute.RoundToInt ? Mathf.RoundToInt(maxValue) : maxValue;
	            }

	            EditorGUI.indentLevel = originalIndent;
            }
            EditorGUI.EndProperty();
        }
    }
}


#endif




/*                                                                              */
/*            |>  Based on the script made by Unity Technologies. <|            */
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