using DragonResonance.Behaviours;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;


namespace Praenaris.GUI
{
	/// <summary>
	/// Rolls the content of a vertical <see cref="ScrollRect"/> upwards at a constant speed, like a credits screen.
	/// The lead-in and lead-out spaces are virtual (one viewport height each), so the content layout stays untouched.
	/// The <see cref="ScrollRect"/> is disabled while this component is enabled, blocking user scrolling and clamping.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(ScrollRect))]
	public class CreditsScroller : PossumBehaviour
	{
		[SerializeField] private float _speed = 100f;
		[SerializeField] private bool _playOnEnable = true;
		[SerializeField] private bool _loop = false;
		[SerializeField] private bool _useUnscaledTime = true;
		[Tooltip("Starts with the content right below the viewport, as if there was an empty screen before it.")]
		[SerializeField] private bool _leadInSpace = true;
		[Tooltip("Ends with the content right above the viewport, as if there was an empty screen after it.")]
		[SerializeField] private bool _leadOutSpace = true;


		public UnityEvent OnFinished = null;


		private readonly Vector3[] _corners = new Vector3[4];
		private ScrollRect _scrollRect_internal = null;	// Caching only, use the property instead
		private bool _scrollRectWasEnabled = false;
		private bool _playing = false;
		private float _distance = 0f;
		private float _speedMultiplier = 1f;


		#region Events

			private void OnEnable()
			{
				_scrollRectWasEnabled = this.ScrollRect.enabled;
				this.ScrollRect.enabled = false;

				Rewind();
				if (_playOnEnable)
					Play();
			}

			private void OnDisable()
			{
				_playing = false;
				this.ScrollRect.enabled = _scrollRectWasEnabled;
			}

			private void Update()
			{
				if (!_playing) return;

				_distance += (_speed * _speedMultiplier * (_useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime));

				float totalDistance = this.TotalDistance;
				if (_distance >= totalDistance) {
					if (_loop && (totalDistance > 0f)) {
						_distance %= totalDistance;
					}
					else {
						_distance = totalDistance;
						_playing = false;
						ApplyPosition();
						OnFinished?.Invoke();
						return;
					}
				}

				ApplyPosition();
			}

		#endregion


		#region Publics

			[ContextMenu(nameof(Play))]
			public void Play() => _playing = true;

			[ContextMenu(nameof(Pause))]
			public void Pause() => _playing = false;

			[ContextMenu(nameof(Restart))]
			public void Restart()
			{
				Rewind();
				Play();
			}

			[ContextMenu(nameof(Rewind))]
			public void Rewind()
			{
				Canvas.ForceUpdateCanvases();	// Ensures the content size is already laid out
				_distance = 0f;
				ApplyPosition();
			}

		#endregion


		#region Privates

			private void ApplyPosition()
			{
				RectTransform content = this.ScrollRect.content;
				if (content == null) return;

				float delta = (this.StartTop + _distance - this.ContentTop);
				content.position += this.Viewport.TransformVector(0f, delta, 0f);
			}

			private void RefreshContentCorners()
			{
				this.ScrollRect.content.GetWorldCorners(_corners);
				for (int i = 0; i < _corners.Length; i++)
					_corners[i] = this.Viewport.InverseTransformPoint(_corners[i]);
			}

		#endregion


		#region Properties

			public ScrollRect ScrollRect => (_scrollRect_internal = GetComponentIfNull(_scrollRect_internal));
			public RectTransform Viewport => ((this.ScrollRect.viewport != null) ? this.ScrollRect.viewport : this.rectTransform);

			public bool IsPlaying => _playing;
			public float Distance => _distance;
			public float NormalizedProgress => ((this.TotalDistance > 0f) ? Mathf.Clamp01(_distance / this.TotalDistance) : 1f);

			public float StartTop => (_leadInSpace ? this.Viewport.rect.yMin : this.Viewport.rect.yMax);
			public float EndTop => ((_leadOutSpace ? this.Viewport.rect.yMax : this.Viewport.rect.yMin) + this.ContentHeight);
			public float TotalDistance => Mathf.Max(0f, (this.EndTop - this.StartTop));

			public float ContentTop
			{
				get {
					RefreshContentCorners();
					return _corners[1].y;
				}
			}

			public float ContentHeight
			{
				get {
					if (this.ScrollRect.content == null) return 0f;
					RefreshContentCorners();
					return (_corners[1].y - _corners[0].y);
				}
			}


			public float Speed
			{
				get => _speed;
				set => _speed = value;
			}

			public float SpeedMultiplier
			{
				get => _speedMultiplier;
				set => _speedMultiplier = value;
			}

			public bool Loop
			{
				get => _loop;
				set => _loop = value;
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
