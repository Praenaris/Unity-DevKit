#if UNITY_EDITOR


using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;


namespace Praenaris.Editor.Miscellany
{
	internal class EllipseBoundsHandle : PrimitiveBoundsHandle
	{
		private const int WIREFRAME_SEGMENTS = 64;


		#region Inheritables

			protected override void DrawWireframe()
			{
				Vector3 radii = (GetSize() / 2f);
				Vector3[] points = new Vector3[WIREFRAME_SEGMENTS + 1];
				for (int pointIndex = 0; pointIndex <= WIREFRAME_SEGMENTS; pointIndex++) {
					float angle = ((pointIndex * Mathf.PI * 2f) / WIREFRAME_SEGMENTS);
					points[pointIndex] = (base.center + new Vector3((radii.x * Mathf.Cos(angle)), (radii.y * Mathf.Sin(angle)), 0f));
				}
				Handles.DrawPolyLine(points);
			}

		#endregion


		#region Properties

			public Vector2 Size { get => GetSize(); set => SetSize(value); }

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