using UnityEngine;


namespace Praenaris.Miscellany
{
	[DisallowMultipleComponent]
	public class RectBoundsCollider : ABoundsCollider
	{
		[SerializeField] private Rect _area = new(-5f, -5f, 10f, 10f);


		#region Events

		#if UNITY_EDITOR
			protected override void OnValidate()
			{
				_area.size = Vector2.Max(_area.size, Vector2.zero);
				base.OnValidate();
			}
		#endif

		#endregion


		#region Inheritables

			protected override Vector2[] GetWallLine(float wallRadius)
			{
				Rect line = new((_area.xMin - wallRadius), (_area.yMin - wallRadius), (_area.width + (wallRadius * 2f)), (_area.height + (wallRadius * 2f)));
				return new[] {
					new Vector2(line.xMin, line.yMin),
					new Vector2(line.xMin, line.yMax),
					new Vector2(line.xMax, line.yMax),
					new Vector2(line.xMax, line.yMin),
					new Vector2(line.xMin, line.yMin),
				};
			}

			protected override bool ContainsLocal(Vector2 localPoint) => _area.Contains(localPoint);
			protected override Vector2 ClosestLocalPoint(Vector2 localPoint) =>
				new(Mathf.Clamp(localPoint.x, _area.xMin, _area.xMax), Mathf.Clamp(localPoint.y, _area.yMin, _area.yMax));
			protected override Vector2 RandomLocalPoint() => new(Random.Range(_area.xMin, _area.xMax), Random.Range(_area.yMin, _area.yMax));

			protected override void DrawLocalGizmo() => Gizmos.DrawWireCube(_area.center, _area.size);

		#endregion


		#region Properties

			public Rect Area { get => _area; set { _area = value; Rebuild(); } }

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