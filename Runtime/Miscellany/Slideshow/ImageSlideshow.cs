using DragonResonance.Extensions;
using UnityEngine.UI;
using UnityEngine;


namespace DragonResonance.Miscellany
{
	public class ImageSlideshow : Slideshow<Sprite>
	{
		[SerializeField] protected Image _image = null;

		protected override void UpdateSlide(int currentIndex)
		{
			if (currentIndex.IsNegative())	// An uninitialized slideshow starts at -1
				_image.overrideSprite = null;
			else
				_image.overrideSprite = base._slides[currentIndex];
		}
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