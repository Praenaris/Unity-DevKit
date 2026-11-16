#if UNITY_EDITOR


using DragonResonance.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Praenaris.Editor.Attributes
{
	public abstract class AComboBoxAttributeDrawer : PropertyDrawer
	{
		private const float DROPDOWN_BUTTON_WIDTH = 19f;
		private const float WARNING_ICON_WIDTH = 18f;


		#region Events

			public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			{
				if (property.propertyType != SerializedPropertyType.String) {
					EditorGUI.HelpBox(position, $"{base.attribute.GetType().Name} works just on strings.", MessageType.Error);
					return;
				}

				string[] items = GetItems(property).Where(item => !string.IsNullOrEmpty(item)).Distinct().ToArray();
				label = EditorGUI.BeginProperty(position, label, property);
				string value = property.stringValue;
				bool showsWarning = (!string.IsNullOrEmpty(value) && !items.Any(item => value.Contains(item)));	// Anything around a valid item is fine
				float warningWidth = (showsWarning ? WARNING_ICON_WIDTH : 0f);

				Rect textField = new(position.x, position.y, (position.width - warningWidth - DROPDOWN_BUTTON_WIDTH), position.height);
				Rect warningIcon = new(textField.xMax, position.y, warningWidth, position.height);
				Rect dropdownButton = new(warningIcon.xMax, position.y, DROPDOWN_BUTTON_WIDTH, position.height);

				EditorGUI.BeginChangeCheck();
				string newValue = EditorGUI.DelayedTextField(textField, label, value);
				if (EditorGUI.EndChangeCheck())
					property.stringValue = newValue.Trim();

				if (showsWarning)
					EditorGUI.LabelField(warningIcon, new GUIContent(EditorGUIUtility.IconContent("console.warnicon.sml").image, this.UnknownItemTooltip));

				if (EditorGUI.DropdownButton(dropdownButton, GUIContent.none, FocusType.Passive))
					ShowMenu(dropdownButton, property, items);

				EditorGUI.EndProperty();
			}

		#endregion


		#region Inheritables

			protected abstract IEnumerable<string> GetItems(SerializedProperty property);
			protected virtual string GetItemPath(string item) => item;	// Slashes nest the item into submenus

		#endregion


		#region Privates

			private void ShowMenu(Rect dropdownButton, SerializedProperty property, string[] items)
			{
				SerializedObject serializedObject = property.serializedObject;
				string propertyPath = property.propertyPath;	// The property itself does not outlive this GUI pass
				string currentValue = property.stringValue;

				GenericMenu menu = new();
				foreach (string item in items)
					menu.AddItem(new GUIContent(GetItemPath(item)), (item == currentValue), () => SetValue(serializedObject, propertyPath, item));
				if (items.IsEmpty())
					menu.AddDisabledItem(new GUIContent("No items"));
				menu.DropDown(dropdownButton);
			}

			private static void SetValue(SerializedObject serializedObject, string propertyPath, string value)
			{
				serializedObject.Update();
				serializedObject.FindProperty(propertyPath).stringValue = value;
				serializedObject.ApplyModifiedProperties();
			}

		#endregion


		#region Properties

			protected virtual string UnknownItemTooltip => "None of the suggested items is in the text.";

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