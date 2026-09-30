#if UNITY_EDITOR


using Praenaris.Attributes;
using System.Linq;
using System;
using UnityEditor.Search;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace Praenaris.Editor.Attributes
{
	[CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
	public class RequireInterfaceAttributeDrawer : PropertyDrawer
	{
		private const float PICKER_BUTTON_WIDTH = 19f;


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.ObjectReference) {
				EditorGUI.HelpBox(position, $"{nameof(RequireInterfaceAttribute)} works just on object references.", MessageType.Error);
				return;
			}

			Type interfaceType = ((RequireInterfaceAttribute)base.attribute).InterfaceType;
			label = EditorGUI.BeginProperty(position, label, property);
			label.tooltip = interfaceType.Name;

			Rect pickerButton = new(position.xMax - PICKER_BUTTON_WIDTH, position.y, PICKER_BUTTON_WIDTH, position.height);
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && pickerButton.Contains(Event.current.mousePosition)) {
				ShowPicker(property, interfaceType);
				Event.current.Use();	// Keeps the default, unfiltered picker from opening
			}

			EditorGUI.BeginChangeCheck();
			UnityObject newReference = EditorGUI.ObjectField(position, label, property.objectReferenceValue, this.FieldElementType, true);
			if (EditorGUI.EndChangeCheck()) {
				UnityObject resolvedReference = Resolve(newReference, interfaceType, this.FieldElementType);
				if ((newReference == null) || (resolvedReference != null))	// Invalid references are rejected, keeping the previous one
					property.objectReferenceValue = resolvedReference;
			}

			EditorGUI.EndProperty();
		}


		private void ShowPicker(SerializedProperty property, Type interfaceType)
		{
			SerializedObject serializedObject = property.serializedObject;
			string propertyPath = property.propertyPath;	// The property itself does not outlive this GUI pass
			Type fieldType = this.FieldElementType;

			SearchService.ShowPicker(
				context: SearchService.CreateContext("scene", BuildQuery(interfaceType)),
				selectHandler: (item, canceled) => {
					if (canceled) return;
					serializedObject.Update();
					serializedObject.FindProperty(propertyPath).objectReferenceValue = Resolve(item?.ToObject(), interfaceType, fieldType);
					serializedObject.ApplyModifiedProperties();
				},
				trackingHandler: null,
				filterHandler: item => (Resolve(item.ToObject(), interfaceType, fieldType) != null),
				subset: null,
				title: interfaceType.Name,
				itemSize: 64f,
				defaultWidth: 400f,
				defaultHeight: 450f,
				flags: SearchFlags.None
			);
		}


		//	The scene provider lists nothing for an empty query, so it starts with the component types implementing the interface: "t:A or t:B".
		private static string BuildQuery(Type interfaceType) =>
			string.Join(" or ", TypeCache.GetTypesDerivedFrom(interfaceType)
				.Where(type => !type.IsAbstract && typeof(Component).IsAssignableFrom(type))
				.Select(type => $"t:{type.Name}"));

		private static UnityObject Resolve(UnityObject candidate, Type interfaceType, Type fieldType)
		{
			UnityObject resolved = candidate switch {
				null => null,
				_ when interfaceType.IsInstanceOfType(candidate) => candidate,
				GameObject gameObject => gameObject.GetComponent(interfaceType),
				Component component => component.GetComponent(interfaceType),	// Sibling component implementing it
				_ => null,
			};
			return fieldType.IsInstanceOfType(resolved) ? resolved : null;
		}


		private Type FieldElementType =>
			fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType() :
			fieldInfo.FieldType.IsGenericType ? fieldInfo.FieldType.GetGenericArguments()[0] :
			fieldInfo.FieldType;
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