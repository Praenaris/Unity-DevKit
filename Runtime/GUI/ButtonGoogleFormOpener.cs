using DragonResonance.Miscellany;
using System.Text;
using UnityEngine;


namespace DragonResonance.GUI
{
	public class ButtonGoogleFormOpener : ButtonUrlOpener
	{
		[Header("Field IDs")]
		[SerializeField] private string _operatingSystemEntryID = "entry.123456789";
		[SerializeField] private string _gamePlatformEntryID = "entry.123456789";
		[SerializeField] private string _gameVariantEntryID = "entry.123456789";
		[SerializeField] private string _gameVersionEntryID = "entry.123456789";


		#region Publics

			public override void Open() => Application.OpenURL(FormattedUrl());

		#endregion


		#region Privates

			public string FormattedUrl()
			{
				string FormatParamPair(string id, string value) => $"{id}={value}";

				string baseUrl = base._url;
				StringBuilder url = new();
				{
					url.Append(baseUrl);
					url.Append(baseUrl.EndsWith('?') ? "" : "?");

					url.Append("usp=pp_url");	// Pre-filled form

					if (!string.IsNullOrWhiteSpace(_operatingSystemEntryID)) {	// Operating System (Windows, Linux, ...)
						#if UNITY_STANDALONE_WIN
							url.Append('&' + FormatParamPair(_operatingSystemEntryID, "Windows"));
						#elif UNITY_STANDALONE_LINUX
							url.Append('&' + FormatParamPair(_operatingSystemEntryID, "Linux"));
						#elif UNITY_STANDALONE_OSX
							url.Append('&' + FormatParamPair(_operatingSystemEntryID, "macOS"));
						#endif
					}

					if (!string.IsNullOrWhiteSpace(_gamePlatformEntryID)) {	// Game Platform (Steam, Epic Games, ...)
						#if (STEAMWORKS_INTEGRATION && !DISABLESTEAMWORKS)
							url.Append('&' + FormatParamPair(_gamePlatformEntryID, "Steam"));
						#endif
					}

					if (!string.IsNullOrWhiteSpace(_gameVariantEntryID)) {	// Game Variant (Demo, Release, ...)
						#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
							url.Append('&' + FormatParamPair(_gameVariantEntryID, "Beta"));
						#elif DEMO_BUILD
							url.Append('&' + FormatParamPair(_gameVariantEntryID, "Demo"));
						#else
							url.Append('&' + FormatParamPair(_gameVariantEntryID, "Release"));
						#endif
					}

					if (!string.IsNullOrWhiteSpace(_gameVersionEntryID)) {	// Game Version (v0.1.0, v0.7.2-tulip, ...)
						url.Append('&' + FormatParamPair(_gameVersionEntryID, Version.vAppVersion));
					}
				}
				return url.ToString();
			}

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