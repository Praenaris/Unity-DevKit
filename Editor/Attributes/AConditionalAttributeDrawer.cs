#if UNITY_EDITOR


using Praenaris.Editor.Extensions;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;


namespace Praenaris.Editor.Attributes
{
	public abstract class AConditionalAttributeDrawer : PropertyDrawer
	{
		#region Events

			public override VisualElement CreatePropertyGUI(SerializedProperty property)
			{
				PropertyField field = new(property, base.preferredLabel);	// Draws the next drawer in line, or the default field
				SerializedProperty conditionProperty = FindConditionProperty(property);
				if (conditionProperty == null) {
					VisualElement container = new();
					container.Add(new HelpBox(this.MissingConditionMessage, HelpBoxMessageType.Warning));
					container.Add(field);
					return container;
				}

				UpdateDisplay(field, conditionProperty);
				field.TrackPropertyValue(conditionProperty, changedProperty => UpdateDisplay(field, changedProperty));
				return field;
			}

			public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
				(ShouldShow(property) ? EditorGUI.GetPropertyHeight(property, label, true) : -EditorGUIUtility.standardVerticalSpacing);	// Also takes back the spacing after it

			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				if (ShouldShow(property))
					EditorGUI.PropertyField(position, property, label, true);
			}

		#endregion


		#region Inheritables

			protected abstract bool IsVisibleWhen(bool condition);

		#endregion


		#region Privates

			private SerializedProperty FindConditionProperty(SerializedProperty property)
			{
				SerializedProperty conditionProperty = property.FindSiblingProperty(this.ConditionFieldName);
				return (((conditionProperty != null) && (conditionProperty.propertyType == SerializedPropertyType.Boolean)) ? conditionProperty : null);
			}

			private bool ShouldShow(SerializedProperty property) => IsVisible(FindConditionProperty(property));
			private bool IsVisible(SerializedProperty conditionProperty) =>
				((conditionProperty == null) || conditionProperty.hasMultipleDifferentValues || IsVisibleWhen(conditionProperty.boolValue));

			private void UpdateDisplay(VisualElement field, SerializedProperty conditionProperty) =>
				field.style.display = (IsVisible(conditionProperty) ? DisplayStyle.Flex : DisplayStyle.None);

		#endregion


		#region Properties

			protected abstract string ConditionFieldName { get; }
			private string MissingConditionMessage => $"{this.ConditionFieldName} is not a bool field next to {base.fieldInfo.Name}.";

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