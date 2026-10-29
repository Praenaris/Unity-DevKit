using DragonResonance.Behaviours;
using Praenaris.Attributes;
using UnityEngine;


namespace Praenaris.Miscellany
{
	[RequireComponent(typeof(LineRenderer))]
	[DisallowMultipleComponent]
	[ExecuteAlways]
	public class TransformsLineRenderer : PossumBehaviour
	{
		[AutoReference] [SerializeField] private LineRenderer _lineRenderer = null;
		[SerializeField] private Transform[] _points = { };


		#region Events

			private void LateUpdate() => Refresh();	// After everything has moved this frame

		#endregion


		#region Publics

			[ContextMenu(nameof(Refresh))]
			public void Refresh()
			{
				if (_lineRenderer.positionCount != _points.Length)
					_lineRenderer.positionCount = _points.Length;

				for (int pointIndex = 0; pointIndex < _points.Length; pointIndex++)
					_lineRenderer.SetPosition(pointIndex,
						(_lineRenderer.useWorldSpace ?
							_points[pointIndex].position :
							this.transform.InverseTransformPoint(_points[pointIndex].position)));
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