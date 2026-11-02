#if UNITY_EDITOR


using DragonResonance.Attributes;
using DragonResonance.Extensions;
using System.Linq;
using System;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;


namespace DragonResonance.Editor.Attributes
{
	[CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
	public class AnimatorParameterAttributeDrawer : ADropdownArrayAttributeDrawer
	{
		#region Inheritables

			protected override string[] GetItems(SerializedProperty property)
			{
				string animatorFieldName = ((AnimatorParameterAttribute)base.attribute).AnimatorFieldName;
				SerializedProperty animatorProperty = FindSiblingProperty(property, animatorFieldName);
				if ((animatorProperty == null) || !(animatorProperty.objectReferenceValue is Animator animator)) return Array.Empty<string>();

				AnimatorController controller = GetController(animator.runtimeAnimatorController);	// Read from the asset, an Animator lists no parameters until it plays
				if (controller == null) return Array.Empty<string>();

				return controller.parameters.Select(parameter => parameter.name).Prepend(string.Empty).ToArray();
			}

		#endregion


		#region Privates

			private static AnimatorController GetController(RuntimeAnimatorController runtimeController) => runtimeController switch {
				AnimatorOverrideController overrideController => GetController(overrideController.runtimeAnimatorController),
				AnimatorController controller => controller,
				_ => null,
			};

			private static SerializedProperty FindSiblingProperty(SerializedProperty property, string siblingName)
			{
				string path = property.propertyPath;
				int arrayIndex = path.LastIndexOf(".Array.data[", StringComparison.Ordinal);
				if (arrayIndex.IsPositiveOrZero())
					path = path.Substring(0, arrayIndex);	// An array element looks next to its array

				int separatorIndex = path.LastIndexOf('.');
				return property.serializedObject.FindProperty(separatorIndex.IsNegative() ? siblingName : $"{path.Substring(0, (separatorIndex + 1))}{siblingName}");
			}

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