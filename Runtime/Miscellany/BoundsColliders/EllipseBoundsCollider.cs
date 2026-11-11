using DragonResonance.Extensions;
using UnityEngine;


namespace Praenaris.Miscellany
{
	[DisallowMultipleComponent]
	public class EllipseBoundsCollider : ABoundsCollider
	{
		private const int CLOSEST_POINT_ITERATIONS = 6;	// Fully converged even on very flat ellipses, 3 can still be millimetres off
		private const int MIN_SEGMENTS = 3;


		[SerializeField] private Rect _area = new(-5f, -5f, 10f, 10f);
		[SerializeField] [Min(MIN_SEGMENTS)] private int _segments = 64;


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
				Vector2 radii = this.Radii;
				Vector2[] points = new Vector2[_segments + 1];
				for (int pointIndex = 0; pointIndex < _segments; pointIndex++) {
					Vector2 direction = GetDirection(pointIndex);
					Vector2 normal = new Vector2((radii.y * direction.x), (radii.x * direction.y)).normalized;	// Offset along the normal, scaling the radii up would bend the inner edge off the ellipse
					points[pointIndex] = (_area.center + Vector2.Scale(radii, direction) + (normal * wallRadius));
				}
				points[_segments] = points[0];
				return points;
			}

			protected override Vector2 GetLocalCenter() => _area.center;

			protected override bool ContainsLocal(Vector2 localPoint)
			{
				Vector2 radii = this.Radii;
				if ((radii.x <= 0f) || (radii.y <= 0f)) return false;

				Vector2 offset = (localPoint - _area.center);
				return ((((offset.x * offset.x) / (radii.x * radii.x)) + ((offset.y * offset.y) / (radii.y * radii.y))) <= 1f);
			}

			protected override Vector2 ClosestLocalPoint(Vector2 localPoint)
			{
				if (ContainsLocal(localPoint)) return localPoint;

				Vector2 radii = this.Radii;
				Vector2 offset = (localPoint - _area.center);
				if ((radii.x <= 0f) || (radii.y <= 0f))	// A segment or a point
					return (_area.center + new Vector2(Mathf.Clamp(offset.x, -radii.x, radii.x), Mathf.Clamp(offset.y, -radii.y, radii.y)));

				return (_area.center + GetClosestPointOnEllipse(offset, radii));
			}

			protected override Vector2 RandomLocalPoint() => (_area.center + Vector2.Scale(this.Radii, Random.insideUnitCircle));

			protected override void DrawLocalGizmo()
			{
				Vector2 radii = this.Radii;
				Vector2 previousPoint = (_area.center + Vector2.Scale(radii, GetDirection(0)));
				for (int pointIndex = 1; pointIndex <= _segments; pointIndex++) {
					Vector2 point = (_area.center + Vector2.Scale(radii, GetDirection(pointIndex)));
					Gizmos.DrawLine(previousPoint, point);
					previousPoint = point;
				}
			}

		#endregion


		#region Privates

			private Vector2 GetDirection(int pointIndex)
			{
				float angle = ((pointIndex * Mathf.PI * 2f) / _segments);
				return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			}

			private static Vector2 GetClosestPointOnEllipse(Vector2 offset, Vector2 radii)	// Iterates on the curvature centre, solved in the first quadrant and mirrored back
			{
				Vector2 point = new(Mathf.Abs(offset.x), Mathf.Abs(offset.y));
				float focalSquared = ((radii.x * radii.x) - (radii.y * radii.y));
				Vector2 parameter = new(0.70710678f, 0.70710678f);	// Cosine and sine of the angle, starting at 45°

				for (int iteration = 0; iteration < CLOSEST_POINT_ITERATIONS; iteration++) {
					Vector2 onEllipse = Vector2.Scale(radii, parameter);
					Vector2 evolute = new(((focalSquared * parameter.x * parameter.x * parameter.x) / radii.x), ((-focalSquared * parameter.y * parameter.y * parameter.y) / radii.y));
					Vector2 toEllipse = (onEllipse - evolute);
					Vector2 toPoint = (point - evolute);

					Vector2 target = (evolute + (toPoint * (toEllipse.magnitude / toPoint.magnitude)));
					parameter = new Vector2(Mathf.Clamp01(target.x / radii.x), Mathf.Clamp01(target.y / radii.y)).normalized;
				}

				Vector2 closestPoint = Vector2.Scale(radii, parameter);
				return new Vector2((closestPoint.x * Mathf.Sign(offset.x)), (closestPoint.y * Mathf.Sign(offset.y)));
			}

		#endregion


		#region Properties

			public Rect Area { get => _area; set { _area = value; Rebuild(); } }
			public int Segments { get => _segments; set { _segments = value.LowerClamp(MIN_SEGMENTS); Rebuild(); } }

			private Vector2 Radii => (_area.size / 2f);

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