using DragonResonance.Extensions;
using DragonResonance.Miscellany;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;


namespace Praenaris.Miscellany
{
	public class FadingImageSlideshow : Slideshow<Image>
	{
		[SerializeField] [Min(0f)] private float _fadeInDuration = 0.5f;
		[SerializeField] [Min(0f)] private float _fadeOutDuration = 0.5f;

		[SerializeField] private UnityEvent OnLastSlideFadedOut = new();


		private bool _slideshowStarted = false;


		#region Inheritables

			protected override void UpdateSlide(int currentIndex)
			{
				if (Application.isPlaying && base.gameObject.activeInHierarchy)
					StartCoroutine(FadeSlides(currentIndex));
				else
					ShowSlideInstantly(currentIndex);
			}

		#endregion


		#region Privates

			private IEnumerator FadeSlides(int currentIndex)
			{
				//	Fade Out current slide
				for (int slideIndex = 0; slideIndex < base._slides.Length; slideIndex++) {
					Image slide = base._slides[slideIndex];
					if ((slideIndex != currentIndex) && slide.gameObject.activeSelf)
						yield return FadeAlpha(slide, 1f, 0f, _fadeOutDuration);
					slide.gameObject.SetActive(false);
				}

				//	Check for return
				if (currentIndex.IsNegative()) {	// An uninitialized slideshow starts at -1
					if (_slideshowStarted) OnLastSlideFadedOut?.Invoke();	// Skips the initial reset, which also comes here
					yield break;
				}

				//	Fade In new slide
				_slideshowStarted = true;
				Image incomingSlide = base._slides[currentIndex];
				incomingSlide.gameObject.SetActive(true);
				yield return FadeAlpha(incomingSlide, 0f, 1f, _fadeInDuration);
			}

			private void ShowSlideInstantly(int currentIndex)
			{
				for (int slideIndex = 0; slideIndex < base._slides.Length; slideIndex++) {
					Image slide = base._slides[slideIndex];
					slide.gameObject.SetActive(slideIndex == currentIndex);
					slide.color = slide.color.WithAlpha((slideIndex == currentIndex).AsInt());
				}
			}


			private static IEnumerator FadeAlpha(Image slide, float fromAlpha, float toAlpha, float duration)
			{
				for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime) {
					float progress = (elapsedTime / duration);
					slide.color = slide.color.WithAlpha(Mathf.LerpUnclamped(fromAlpha, toAlpha, (progress * (2f - progress))));	// Quadratic ease out, PrimeTween's default
					yield return null;
				}
				slide.color = slide.color.WithAlpha(toAlpha);
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