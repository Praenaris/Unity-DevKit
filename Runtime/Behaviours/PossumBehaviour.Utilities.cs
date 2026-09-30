using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace DragonResonance.Behaviours
{
	public abstract partial class PossumBehaviour	// Utilities
	{
		#region Publics - Destroying

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


		#region Publics - Searching

			public static IEnumerable<GameObject> GetSceneRoots()
			{
				for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++) {
					Scene scene = SceneManager.GetSceneAt(sceneIndex);
					if (!scene.isLoaded) continue;
					foreach (GameObject root in scene.GetRootGameObjects())
						yield return root;
				}
			}


		#if UNITY_EDITOR
			protected static T FindFirstAssetIfNull<T>(UnityObject statement) where T : UnityObject
			{
				if (statement != null) return (T)statement;
				string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
				if (guids.Length == 0) return null;
				return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]));
			}
		#endif


			public static List<T> FindAllInterfaces<T>(bool includeInactive = false) where T : class
			{
				List<T> found = new();
				List<T> buffer = new();
				foreach (GameObject root in GetSceneRoots()) {
					root.GetComponentsInChildren(includeInactive, buffer);	// Clears the buffer on each call
					found.AddRange(buffer);
				}
				return found;
			}

			public static T FindFirstInterface<T>(bool includeInactive = false) where T : class
			{
				return GetSceneRoots()
					.Select(root => root.GetComponentInChildren<T>(includeInactive))
					.FirstOrDefault(found => (found != null));
			}

		#endregion
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