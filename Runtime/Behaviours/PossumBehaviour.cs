using DragonResonance.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace DragonResonance.Behaviours
{
	public abstract partial class PossumBehaviour : MonoBehaviour
	{
		#region Publics

			public static void DestroyDynamically(GameObject gameObject)
			{
				#if UNITY_EDITOR
					if (!Application.isPlaying)
						DestroyImmediate(gameObject);
					else
				#endif
						Destroy(gameObject);
			}

			public static void DestroyChildren(Transform container)
			{
				for (int childIndex = container.childCount - 1; childIndex >= 0; childIndex--) {
					DestroyDynamically(container.GetChild(childIndex).gameObject);
				}
			}

		#endregion


		#region Privates

			protected T GetComponentIfNull<T>(T statement) where T : Component =>
				((statement == null) ? GetComponent<T>() : statement);
			protected T GetComponentInChildrenIfNull<T>(T statement) where T : Component =>
				((statement == null) ? GetComponentInChildren<T>() : statement);
			protected T GetComponentInParentIfNull<T>(T statement) where T : Component =>
				((statement == null) ? GetComponentInParent<T>() : statement);

			protected T FindComponentIfNull<T>(T statement) where T : Component =>
				FindComponentIfNull(statement, true);
			protected T FindComponentIfNull<T>(T statement, bool includeInactive) where T : Component =>
				FindComponentIfNull(statement, (includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude));
			protected T FindComponentIfNull<T>(T statement, FindObjectsInactive includeInactive) where T : Component =>
				((statement == null) ? FindAnyObjectByType<T>(includeInactive) : statement);

			protected void DestroyChildren() => DestroyChildren(this.transform);

		#endregion


		#region Properties

			public RectTransform rectTransform => (RectTransform)base.transform;
			public IEnumerable<Transform> children => base.transform.GetChildren();
			public IEnumerable<RectTransform> rectChildren => base.transform.GetRectChildren();

		#endregion


		#if UNITY_EDITOR

			protected static T FindFirstAssetIfNull<T>(UnityObject statement) where T : UnityObject
			{
				if (statement != null) return (T)statement;
				string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
				if (guids.Length == 0) return null;
				return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]));
			}

		#endif
	}
}


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