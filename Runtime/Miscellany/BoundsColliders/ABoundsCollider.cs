using DragonResonance.Behaviours;
using DragonResonance.Extensions;
using UnityEngine;


namespace Praenaris.Miscellany
{
	[RequireComponent(typeof(EdgeCollider2D))]
	public abstract class ABoundsCollider : PossumBehaviour
	{
		[SerializeField] [Min(0.001f)] private float _thickness = 1f;


		private EdgeCollider2D _edgeCollider_internal = null;	// Caching only, use the property instead


		#region Events

		#if UNITY_EDITOR
			protected virtual void OnValidate() => Rebuild();
		#endif

		#if UNITY_EDITOR
			private void Reset() => Rebuild();
		#endif

			private void Awake() => Rebuild();

			private void OnDrawGizmosSelected()
			{
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.color = Color.yellow;
				DrawLocalGizmo();
			}

		#endregion


		#region Publics

			public bool Contains(Vector2 worldPoint) => ContainsLocal(ToLocal(worldPoint));
			public Vector2 ClosestPoint(Vector2 worldPoint) => ToWorld(ClosestLocalPoint(ToLocal(worldPoint)));
			public Vector2 RandomPoint() => ToWorld(RandomLocalPoint());

		#endregion


		#region Inheritables

			protected abstract Vector2[] GetWallLine(float wallRadius);	// Closed, through the middle of the walls, in local space
			protected abstract Vector2 GetLocalCenter();
			protected abstract bool ContainsLocal(Vector2 localPoint);
			protected abstract Vector2 ClosestLocalPoint(Vector2 localPoint);	// The point itself when it is inside
			protected abstract Vector2 RandomLocalPoint();
			protected abstract void DrawLocalGizmo();

			protected void Rebuild()
			{
				float wallRadius = (_thickness / 2f);
				this.EdgeCollider.offset = Vector2.zero;
				this.EdgeCollider.edgeRadius = wallRadius;
				this.EdgeCollider.points = GetWallLine(wallRadius);
			}

		#endregion


		#region Privates

			private Vector2 ToLocal(Vector2 worldPoint) => base.transform.InverseTransformPoint(worldPoint);
			private Vector2 ToWorld(Vector2 localPoint) => base.transform.TransformPoint(localPoint);

		#endregion


		#region Properties

			public Vector2 Center => ToWorld(GetLocalCenter());
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