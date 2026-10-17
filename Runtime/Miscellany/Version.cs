using UnityEngine;


namespace DragonResonance.Miscellany
{
	public static class Version
	{
		#region Publics

			public static string WithOS(this string versionString) => (AppOS != null) ? $"{versionString}-{AppOS}" : versionString;
			public static string WithPlatform(this string versionString) => (AppPlatform != null) ? $"{versionString}-{AppPlatform}" : versionString;
			public static string WithVariant(this string versionString) => $"{versionString}-{AppVariant}";

		#endregion


		#region Properties - Specific

			#if UNITY_STANDALONE_WIN
				public static string AppOS => "Windows";
			#elif UNITY_STANDALONE_LINUX
				public static string AppOS => "Linux";
			#elif UNITY_STANDALONE_OSX
				public static string AppOS => "macOS";
			#else
				public static string AppOS => null;
			#endif

			#if (STEAMWORKS_INTEGRATION && !DISABLESTEAMWORKS)
				public static string AppPlatform => "Steam";
			#else
				public static string AppPlatform => null;
			#endif

			#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
				public static string AppVariant => "Beta";
			#elif DEMO_BUILD
				public static string AppVariant => "Demo";
			#else
				public static string AppVariant => "Release";
			#endif

		#endregion


		#region Properties - General

			public static string AppVersion => Application.version;
			public static string AppVersionLower => AppVersion.ToLowerInvariant();
			public static string vAppVersion => $"v{Application.version}";
			public static string vAppVersionLower => vAppVersion.ToLowerInvariant();

			public static string FullVersion => AppVersion.WithOS().WithPlatform().WithVariant();
			public static string FullVersionLower => FullVersion.ToLowerInvariant();
			public static string vFullVersion => vAppVersion.WithOS().WithPlatform().WithVariant();
			public static string vFullVersionLower => vFullVersion.ToLowerInvariant();

			public static System.Version ToSystemVersion => new(AppVersion);

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