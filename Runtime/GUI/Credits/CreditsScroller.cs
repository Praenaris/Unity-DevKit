using DragonResonance.Attributes;
using DragonResonance.Behaviours;
using Praenaris.Attributes;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;


namespace Praenaris.GUI
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(ScrollRect))]
	public class CreditsScroller : PossumBehaviour
	{
		[AutoReference] [SerializeField] private ScrollRect _scrollRect = null;
		[SerializeField] private RectTransform _content = null;

		[Header("Behaviour")]
		[SerializeField] private bool _loop = false;
		[ShowIf(nameof(_loop))] [SerializeField] [Min(0f)] private float _loopWaitingTime = 2f;

		[Header("Scrolling")]
		[SerializeField] [Min(0f)] private float _baseSpeed = 40f;
		[SerializeField] [Min(0f)] private float _speedMultiplier = 1f;

		[Header("Appeareance")]
		[SerializeField] private bool _leadInViewportSpace = true;
		[SerializeField] [Min(0f)] private float _leadInExtraSpace = 80f;
		[SerializeField] private bool _leadOutViewportSpace = true;
		[SerializeField] [Min(0f)] private float _leadOutExtraSpace = 80f;

		public UnityEvent OnFinished = null;


		private float _loopWaitedTime = 0f;


		#region Events

			private void OnEnable() => Restart();
			private void Start() => Restart();

			private void Update()
			{
				if (!this.IsFinished) {
					MoveContent(this.FinalScrollSpeed * Time.deltaTime);
					if (this.IsFinished)
						OnFinished?.Invoke();
				}
				else if (_loop) {
					_loopWaitedTime += Time.deltaTime;
					if (_loopWaitedTime >= _loopWaitingTime)
						Restart();
				}
			}

		#endregion


		#region Publics

			[ContextMenu(nameof(Restart))]
			public void Restart()
			{
				Canvas.ForceUpdateCanvases();	// Ensures the content size is already laid out
				MoveContent(this.StartTop - this.ContentTop);
				_loopWaitedTime = 0f;
			}

		#endregion


		#region Privates

			private void MoveContent(float viewportDistance) =>
				_content.position += this.Viewport.TransformVector(0f, viewportDistance, 0f);

		#endregion


		#region Properties

			public RectTransform Viewport => this._scrollRect.viewport;
			public Rect ViewportRect => this.Viewport.rect;
			public float ContentHeight => _content.rect.height;
			public float LeadInHeight => ((_leadInViewportSpace ? this.ViewportRect.height : 0f) + _leadInExtraSpace);
			public float LeadOutHeight => ((_leadOutViewportSpace ? this.ViewportRect.height : 0f) + _leadOutExtraSpace);

			public float ContentTop => this.Viewport.InverseTransformPoint(_content.TransformPoint(0f, _content.rect.yMax, 0f)).y;	// In viewport space
			public float StartTop => (this.ViewportRect.yMax - this.LeadInHeight);	// The content top starts that much below the viewport top
			public float EndTop => (this.ViewportRect.yMin + this.LeadOutHeight + this.ContentHeight);	// The content bottom ends that much above the viewport bottom

			public float FinalScrollSpeed => (_baseSpeed * _speedMultiplier);
			public bool IsFinished => (this.ContentTop >= this.EndTop);
			public bool IsPlaying => (base.isActiveAndEnabled && !this.IsFinished);
			public float NormalizedProgress => Mathf.InverseLerp(this.StartTop, this.EndTop, this.ContentTop);

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