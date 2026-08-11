using DragonResonance.Attributes;
using DragonResonance.Behaviours;
using DragonResonance.Extensions;
using UnityEngine.Events;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace DragonResonance.Miscellany
{
	public abstract class Slideshow<T> : PossumBehaviour where T : UnityObject
	{
		[SerializeField] protected int _slideIndex = 0;
		[SerializeField] protected bool _cyclical = true;

		[Header("Autoplay")]
		[SerializeField] private bool _autoplay = false;
		[ShowIf(nameof(_autoplay))] [SerializeField] private float _autoplayInterval = 4f;
		[ShowIf(nameof(_autoplay))] [SerializeField] private float _autoplayInitialDelay = 0f;

		[SerializeField] protected T[] _slides = { };

		[SerializeField] protected UnityEvent OnFirstSlideReached;
		[SerializeField] protected UnityEvent OnLastSlideReached;
		[SerializeField] protected UnityEvent OnAutoplayEnd;


		private float _remaining = -1f;


		#region Events

			#if UNITY_EDITOR
			private void OnValidate()
			{
				if (!Application.isPlaying) {
					GoTo(_slideIndex);
				}
			}
			#endif

			private void Start()
			{
				if (_autoplay) {
					GoTo(-1);
				}
				_remaining = _autoplayInitialDelay;
			}

			private void Update()
			{
				if (_autoplay && ((_remaining -= Time.deltaTime) < 0f)) {
					if (!this.IsLastSlide) {
						Next();
						_remaining = _autoplayInterval;
					}
					else {
						GoTo(-1);
						OnAutoplayEnd?.Invoke();
						this.enabled = false;
					}
				}
			}

		#endregion


		#region Publics

			[ContextMenu(nameof(Previous))]
			public void Previous()
			{
				if (_slides.IsEmpty()) return;	// Avoids a divide-by-zero in *Cyclic() below

				if (_cyclical)
					GoTo(_slideIndex.PreviousCyclic(_slides.Length));
				else
					GoTo((_slideIndex - 1).LowerClamp(this.FirstSlideIndex));
			}

			[ContextMenu(nameof(Next))]
			public void Next()
			{
				if (_slides.IsEmpty()) return;	// Avoids a divide-by-zero in *Cyclic() below

				if (_cyclical)
					GoTo(_slideIndex.NextCyclic(_slides.Length));
				else
					GoTo((_slideIndex + 1).UpperClamp(this.LastSlideIndex));
			}


			public void GoTo(int index)
			{
				int previousSlideIndex = _slideIndex;
				UpdateSlide(_slideIndex = SanitizedIndex(index));
				if (_slideIndex != previousSlideIndex) {
					if (this.IsFirstSlide) OnFirstSlideReached?.Invoke();
					if (this.IsLastSlide) OnLastSlideReached?.Invoke();
				}
			}

		#endregion


		#region Privates

			protected int SanitizedIndex(int index) => _slides.IsEmpty() ? -1 : index.Clamp(-1, (_slides.Length - 1));

			protected virtual void UpdateSlide(int currentIndex) { }

		#endregion


		#region Properties

			public int SlideIndex => _slideIndex;
			public int FirstSlideIndex => 0;
			public int LastSlideIndex => _slides.Length - 1;
			public bool IsFirstSlide => (_slideIndex == this.FirstSlideIndex);
			public bool IsLastSlide => (_slideIndex == this.LastSlideIndex);

		#endregion
	}
}


/*       ________________________________________________________________       */
/*           _________   _______ ________  _______  _______  ___    _           */
/*           |        \ |______/ |______| |  _____ |       | |  \   |           */
/*           |________/ |     \_ |      | |______| |_______| |   \__|           */
/*           ______ _____ _____ _____ __   _ _____ __   _ _____ _____           */
/*           |____/ |____ [___  |   | | \  | |___| | \  | |     |____           */
/*           |    \ |____ ____] |___| |  \_| |   | |  \_| |____ |____           */
/*       ________________________________________________________________       */
/*                                                                              */
/*           David Tabernero M.  <https://github.com/davidtabernerom>           */
/*           Dragon Resonance    <https://github.com/dragonresonance>           */
/*                  Copyright © 2021-2026. All rights reserved.                 */
/*                Licensed under the Apache License, Version 2.0.               */
/*                         See LICENSE.md for more info.                        */
/*       ________________________________________________________________       */
/*                                                                              */