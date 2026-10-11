using DragonResonance.Behaviours;
using UnityEngine;


namespace DragonResonance.Miscellany
{
	public class FloatingBob : PossumBehaviour
	{
		private const float TAU = (2f * Mathf.PI);

		[SerializeField] [Min(0f)] private float _amplitude = 0.05f;
		[SerializeField] [Min(0.01f)] private float _period = 3f;

		private Transform _transform = null;
		private float _angularFrequency = 0f;
		private float _phase = 0f;
		private float _offset = 0f;


		#region Events

		#if UNITY_EDITOR
			private void OnValidate() => CacheAngularFrequency();
		#endif

			private void Awake()
			{
				_transform = base.transform;
				_phase = (Random.value * TAU);
				CacheAngularFrequency();
			}

			private void OnDisable() => ApplyOffset(0f);

			private void Update() => ApplyOffset(Mathf.Sin((Time.time * _angularFrequency) + _phase) * _amplitude);

		#endregion


		#region Privates

			private void CacheAngularFrequency() => _angularFrequency = (TAU / _period);

			private void ApplyOffset(float offset)
			{
				_transform.localPosition += (Vector3.up * (offset - _offset));
				_offset = offset;
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