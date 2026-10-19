#if UNITY_EDITOR


using Praenaris.Attributes;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine.SceneManagement;
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

			this.TryAutoFill(property);

			EditorGUI.PropertyField(position, property, label, true);
		}


		private void TryAutoFill(SerializedProperty property)
		{
			if (!property.hasMultipleDifferentValues && (property.objectReferenceValue != null)) return;

			Type type = base.fieldInfo.FieldType;
			AutoReferenceSource sources = GetSources(type, ((AutoReferenceAttribute)base.attribute).Sources);
			UnityObject[] targets = property.serializedObject.targetObjects;

			if (targets.Length == 1) {
				UnityObject reference = Resolve(targets[0], type, sources);
				if (reference == null) return;

				property.objectReferenceValue = reference;
				property.serializedObject.ApplyModifiedProperties();	// Not every inspector applies a change made outside a change check
				return;
			}

			foreach (UnityObject target in targets) {	// Each target resolves its own reference (e.g. its own component)
				using SerializedObject targetObject = new(target);
				SerializedProperty targetProperty = targetObject.FindProperty(property.propertyPath);
				if (targetProperty.objectReferenceValue != null) continue;

				UnityObject reference = Resolve(target, type, sources);
				if (reference == null) continue;

				targetProperty.objectReferenceValue = reference;
				targetObject.ApplyModifiedProperties();
			}
		}


		private static UnityObject Resolve(UnityObject target, Type type, AutoReferenceSource sources)
		{
			UnityObject reference = null;

			if (IsComponent(type) && (target is Component component)) {
				GameObject gameObject = component.gameObject;

				if (sources.HasFlag(AutoReferenceSource.Self) && TryPick(gameObject.GetComponents(type), out reference))
					return reference;
				if (sources.HasFlag(AutoReferenceSource.Children) && TryPick(gameObject.GetComponentsInChildren(type, true).Where(candidate => (candidate.gameObject != gameObject)), out reference))
					return reference;
				if (sources.HasFlag(AutoReferenceSource.Parents) && TryPick(gameObject.GetComponentsInParent(type, true).Where(candidate => (candidate.gameObject != gameObject)), out reference))
					return reference;
				if (sources.HasFlag(AutoReferenceSource.Scene) && !EditorUtility.IsPersistent(target) && TryPick(FindInScene(gameObject.scene, type), out reference))	// Assets can't reference scene objects
					return reference;
			}

			if (sources.HasFlag(AutoReferenceSource.Project) && TryPick(FindInProject(type), out reference))
				return reference;

			return null;
		}


		private static bool TryPick(IEnumerable<UnityObject> candidates, out UnityObject reference)	// Found only if unique; several candidates stop the search, since picking one would be a guess
		{
			UnityObject[] distinctCandidates = candidates.Distinct().Take(2).ToArray();
			reference = ((distinctCandidates.Length == 1) ? distinctCandidates[0] : null);
			return (distinctCandidates.Length > 0);
		}


		private static IEnumerable<UnityObject> FindInScene(Scene scene, Type type) =>	// The object's own scene, or its prefab stage in Prefab Mode
			((scene.IsValid() && scene.isLoaded) ? scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren(type, true)) : Enumerable.Empty<UnityObject>());

		private static IEnumerable<UnityObject> FindInProject(Type type) =>
			AssetDatabase.FindAssets($"t:{type.Name}")
				.Select(guid => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), type))
				.Where(asset => (asset != null));

		private static AutoReferenceSource GetSources(Type type, AutoReferenceSource sources) =>
			((sources != AutoReferenceSource.Automatic) ? sources : (IsComponent(type) ? (AutoReferenceSource.Self | AutoReferenceSource.Scene) : AutoReferenceSource.Project));

		private static bool IsComponent(Type type) => typeof(Component).IsAssignableFrom(type);
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