#if UNITY_EDITOR


using Praenaris.Miscellany;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace Praenaris.Editor.Miscellany
{
	[CustomEditor(typeof(RectBoundsCollider))]
	internal class RectBoundsColliderEditor : UnityEditor.Editor
	{
		private readonly BoxBoundsHandle _handle = new() { axes = (PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y) };


		#region Events

			private void OnSceneGUI()
			{
				RectBoundsCollider bounds = (RectBoundsCollider)base.target;
				using (new Handles.DrawingScope(Color.yellow, bounds.transform.localToWorldMatrix)) {
					_handle.center = bounds.Area.center;
					_handle.size = bounds.Area.size;

					EditorGUI.BeginChangeCheck();
					_handle.DrawHandle();
					if (EditorGUI.EndChangeCheck()) {
						Undo.RecordObjects(new UnityObject[] { bounds, bounds.EdgeCollider }, "Resize Rect Bounds");
						bounds.Area = new Rect(((Vector2)_handle.center - ((Vector2)_handle.size / 2f)), _handle.size);
					}
				}
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