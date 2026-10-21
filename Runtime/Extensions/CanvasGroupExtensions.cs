using UnityEngine;


namespace DragonResonance.Extensions
{
	public static class CanvasGroupExtensions
	{
		public static void MakeVisible(this CanvasGroup target) => target.alpha = 1f;
		public static void MakeInvisible(this CanvasGroup target) => target.alpha = 0f;

		public static bool IsFullyVisible(this CanvasGroup target) => target.alpha.IsApproximately(1f);
		public static bool IsFullyInvisible(this CanvasGroup target) => target.alpha.IsApproximately(0f);
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