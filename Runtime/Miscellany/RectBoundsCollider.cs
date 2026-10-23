using DragonResonance.Behaviours;
using DragonResonance.Extensions;
using UnityEngine;


namespace Praenaris.Miscellany
{
	[RequireComponent(typeof(EdgeCollider2D))]
	[DisallowMultipleComponent]
	public class RectBoundsCollider : PossumBehaviour
	{
		[SerializeField] private Rect _area = new(-5f, -5f, 10f, 10f);
		[SerializeField] [Min(0.001f)] private float _thickness = 1f;


		private EdgeCollider2D _edgeCollider_internal = null;	// Caching only, use the property instead


		#region Events

		#if UNITY_EDITOR
			private void OnValidate()
			{
				_area.size = Vector2.Max(_area.size, Vector2.zero);
				Rebuild();
			}
		#endif

		#if UNITY_EDITOR
			private void Reset() => Rebuild();
		#endif

			private void Awake() => Rebuild();

			private void OnDrawGizmosSelected()
			{
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireCube(_area.center, _area.size);
			}

		#endregion


		#region Publics

			public bool Contains(Vector2 worldPoint) => _area.Contains(ToLocal(worldPoint));

			public Vector2 ClosestPoint(Vector2 worldPoint)
			{
				Vector2 localPoint = ToLocal(worldPoint);
				return ToWorld(new Vector2(Mathf.Clamp(localPoint.x, _area.xMin, _area.xMax), Mathf.Clamp(localPoint.y, _area.yMin, _area.yMax)));
			}

			public Vector2 RandomPoint() => ToWorld(new Vector2(Random.Range(_area.xMin, _area.xMax), Random.Range(_area.yMin, _area.yMax)));

		#endregion


		#region Privates

			private void Rebuild()
			{
				float radius = (_thickness / 2f);
				Rect line = new((_area.xMin - radius), (_area.yMin - radius), (_area.width + _thickness), (_area.height + _thickness));	// Runs through the middle of the walls

				this.EdgeCollider.offset = Vector2.zero;
				this.EdgeCollider.edgeRadius = radius;
				this.EdgeCollider.points = new[] {
					new Vector2(line.xMin, line.yMin),
					new Vector2(line.xMin, line.yMax),
					new Vector2(line.xMax, line.yMax),
					new Vector2(line.xMax, line.yMin),
					new Vector2(line.xMin, line.yMin),
				};
			}

			private Vector2 ToLocal(Vector2 worldPoint) => base.transform.InverseTransformPoint(worldPoint);
			private Vector2 ToWorld(Vector2 localPoint) => base.transform.TransformPoint(localPoint);

		#endregion


		#region Properties

			public Rect Area { get => _area; set { _area = value; Rebuild(); } }
			public float Thickness { get => _thickness; set { _thickness = value.LowerClamp(0.001f); Rebuild(); } }

			public EdgeCollider2D EdgeCollider => (_edgeCollider_internal = GetComponentIfNull(_edgeCollider_internal));

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