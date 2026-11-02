using System.Collections.Generic;


namespace Praenaris.Miscellany
{
	public static class DefineSymbols
	{
		private static readonly HashSet<string> _defined = new();
		private static readonly SortedSet<string> _detectable = new();


		#region Constructors

			static DefineSymbols()
			{
				AddUnitySymbols();
				AddDevkitSymbols();
			}

		#endregion


		#region Publics

			public static bool IsDefined(string symbol) => _defined.Contains(symbol);
			public static bool IsDetectable(string symbol) => _detectable.Contains(symbol);

			public static void Register(params string[] symbols)
			{
				foreach (string symbol in symbols)
					if (!string.IsNullOrWhiteSpace(symbol))
						AddDefined(symbol.Trim());
			}

		#endregion


		#region Privates

			private static void AddDefined(string symbol)
			{
				_defined.Add(symbol);
				_detectable.Add(symbol);
			}

			private static void AddUndefined(string symbol) => _detectable.Add(symbol);

			private static void AddDevkitSymbols()
			{
				#if DEMO_BUILD
					AddDefined("DEMO_BUILD");
				#else
					AddUndefined("DEMO_BUILD");
				#endif
				#if ENABLE_LOGGING
					AddDefined("ENABLE_LOGGING");
				#else
					AddUndefined("ENABLE_LOGGING");
				#endif
				#if OUTPUT_BUILD_PROPERTIES
					AddDefined("OUTPUT_BUILD_PROPERTIES");
				#else
					AddUndefined("OUTPUT_BUILD_PROPERTIES");
				#endif
				#if TEST_INTEGRATION
					AddDefined("TEST_INTEGRATION");
				#else
					AddUndefined("TEST_INTEGRATION");
				#endif
				#if UNITY_HDRP
					AddDefined("UNITY_HDRP");
				#else
					AddUndefined("UNITY_HDRP");
				#endif
				#if UNITY_NGO
					AddDefined("UNITY_NGO");
				#else
					AddUndefined("UNITY_NGO");
				#endif
				#if UNITY_URP
					AddDefined("UNITY_URP");
				#else
					AddUndefined("UNITY_URP");
				#endif
			}

			private static void AddUnitySymbols()
			{
				#if CSHARP_7_3_OR_NEWER
					AddDefined("CSHARP_7_3_OR_NEWER");
				#else
					AddUndefined("CSHARP_7_3_OR_NEWER");
				#endif
				#if DEVELOPMENT_BUILD
					AddDefined("DEVELOPMENT_BUILD");
				#else
					AddUndefined("DEVELOPMENT_BUILD");
				#endif
				#if ENABLE_IL2CPP
					AddDefined("ENABLE_IL2CPP");
				#else
					AddUndefined("ENABLE_IL2CPP");
				#endif
				#if ENABLE_INPUT_SYSTEM
					AddDefined("ENABLE_INPUT_SYSTEM");
				#else
					AddUndefined("ENABLE_INPUT_SYSTEM");
				#endif
				#if ENABLE_LEGACY_INPUT_MANAGER
					AddDefined("ENABLE_LEGACY_INPUT_MANAGER");
				#else
					AddUndefined("ENABLE_LEGACY_INPUT_MANAGER");
				#endif
				#if ENABLE_MONO
					AddDefined("ENABLE_MONO");
				#else
					AddUndefined("ENABLE_MONO");
				#endif
				#if ENABLE_VR
					AddDefined("ENABLE_VR");
				#else
					AddUndefined("ENABLE_VR");
				#endif
				#if ENABLE_WINMD_SUPPORT
					AddDefined("ENABLE_WINMD_SUPPORT");
				#else
					AddUndefined("ENABLE_WINMD_SUPPORT");
				#endif
				#if NET_4_6
					AddDefined("NET_4_6");
				#else
					AddUndefined("NET_4_6");
				#endif
				#if NET_STANDARD
					AddDefined("NET_STANDARD");
				#else
					AddUndefined("NET_STANDARD");
				#endif
				#if NET_STANDARD_2_0
					AddDefined("NET_STANDARD_2_0");
				#else
					AddUndefined("NET_STANDARD_2_0");
				#endif
				#if NET_STANDARD_2_1
					AddDefined("NET_STANDARD_2_1");
				#else
					AddUndefined("NET_STANDARD_2_1");
				#endif
				#if NETSTANDARD
					AddDefined("NETSTANDARD");
				#else
					AddUndefined("NETSTANDARD");
				#endif
				#if NETSTANDARD2_1
					AddDefined("NETSTANDARD2_1");
				#else
					AddUndefined("NETSTANDARD2_1");
				#endif
				#if UNITY_6000_0_OR_NEWER
					AddDefined("UNITY_6000_0_OR_NEWER");
				#else
					AddUndefined("UNITY_6000_0_OR_NEWER");
				#endif
				#if UNITY_6000_1_OR_NEWER
					AddDefined("UNITY_6000_1_OR_NEWER");
				#else
					AddUndefined("UNITY_6000_1_OR_NEWER");
				#endif
				#if UNITY_6000_2_OR_NEWER
					AddDefined("UNITY_6000_2_OR_NEWER");
				#else
					AddUndefined("UNITY_6000_2_OR_NEWER");
				#endif
				#if UNITY_6000_3_OR_NEWER
					AddDefined("UNITY_6000_3_OR_NEWER");
				#else
					AddUndefined("UNITY_6000_3_OR_NEWER");
				#endif
				#if UNITY_6000_4_OR_NEWER
					AddDefined("UNITY_6000_4_OR_NEWER");
				#else
					AddUndefined("UNITY_6000_4_OR_NEWER");
				#endif
				#if UNITY_6000_5_OR_NEWER
					AddDefined("UNITY_6000_5_OR_NEWER");
				#else
					AddUndefined("UNITY_6000_5_OR_NEWER");
				#endif
				#if UNITY_64
					AddDefined("UNITY_64");
				#else
					AddUndefined("UNITY_64");
				#endif
				#if UNITY_ANALYTICS
					AddDefined("UNITY_ANALYTICS");
				#else
					AddUndefined("UNITY_ANALYTICS");
				#endif
				#if UNITY_ANDROID
					AddDefined("UNITY_ANDROID");
				#else
					AddUndefined("UNITY_ANDROID");
				#endif
				#if UNITY_ASSERTIONS
					AddDefined("UNITY_ASSERTIONS");
				#else
					AddUndefined("UNITY_ASSERTIONS");
				#endif
				#if UNITY_CLOUD_BUILD
					AddDefined("UNITY_CLOUD_BUILD");
				#else
					AddUndefined("UNITY_CLOUD_BUILD");
				#endif
				#if UNITY_EDITOR
					AddDefined("UNITY_EDITOR");
				#else
					AddUndefined("UNITY_EDITOR");
				#endif
				#if UNITY_EDITOR_LINUX
					AddDefined("UNITY_EDITOR_LINUX");
				#else
					AddUndefined("UNITY_EDITOR_LINUX");
				#endif
				#if UNITY_EDITOR_OSX
					AddDefined("UNITY_EDITOR_OSX");
				#else
					AddUndefined("UNITY_EDITOR_OSX");
				#endif
				#if UNITY_EDITOR_WIN
					AddDefined("UNITY_EDITOR_WIN");
				#else
					AddUndefined("UNITY_EDITOR_WIN");
				#endif
				#if UNITY_EMBEDDED_LINUX
					AddDefined("UNITY_EMBEDDED_LINUX");
				#else
					AddUndefined("UNITY_EMBEDDED_LINUX");
				#endif
				#if UNITY_FACEBOOK_INSTANT_GAMES
					AddDefined("UNITY_FACEBOOK_INSTANT_GAMES");
				#else
					AddUndefined("UNITY_FACEBOOK_INSTANT_GAMES");
				#endif
				#if UNITY_GAMECORE
					AddDefined("UNITY_GAMECORE");
				#else
					AddUndefined("UNITY_GAMECORE");
				#endif
				#if UNITY_IOS
					AddDefined("UNITY_IOS");
				#else
					AddUndefined("UNITY_IOS");
				#endif
				#if UNITY_PS4
					AddDefined("UNITY_PS4");
				#else
					AddUndefined("UNITY_PS4");
				#endif
				#if UNITY_PS5
					AddDefined("UNITY_PS5");
				#else
					AddUndefined("UNITY_PS5");
				#endif
				#if UNITY_QNX
					AddDefined("UNITY_QNX");
				#else
					AddUndefined("UNITY_QNX");
				#endif
				#if UNITY_SERVER
					AddDefined("UNITY_SERVER");
				#else
					AddUndefined("UNITY_SERVER");
				#endif
				#if UNITY_STANDALONE
					AddDefined("UNITY_STANDALONE");
				#else
					AddUndefined("UNITY_STANDALONE");
				#endif
				#if UNITY_STANDALONE_LINUX
					AddDefined("UNITY_STANDALONE_LINUX");
				#else
					AddUndefined("UNITY_STANDALONE_LINUX");
				#endif
				#if UNITY_STANDALONE_OSX
					AddDefined("UNITY_STANDALONE_OSX");
				#else
					AddUndefined("UNITY_STANDALONE_OSX");
				#endif
				#if UNITY_STANDALONE_WIN
					AddDefined("UNITY_STANDALONE_WIN");
				#else
					AddUndefined("UNITY_STANDALONE_WIN");
				#endif
				#if UNITY_SWITCH
					AddDefined("UNITY_SWITCH");
				#else
					AddUndefined("UNITY_SWITCH");
				#endif
				#if UNITY_TVOS
					AddDefined("UNITY_TVOS");
				#else
					AddUndefined("UNITY_TVOS");
				#endif
				#if UNITY_VISIONOS
					AddDefined("UNITY_VISIONOS");
				#else
					AddUndefined("UNITY_VISIONOS");
				#endif
				#if UNITY_WEBGL
					AddDefined("UNITY_WEBGL");
				#else
					AddUndefined("UNITY_WEBGL");
				#endif
				#if UNITY_WSA
					AddDefined("UNITY_WSA");
				#else
					AddUndefined("UNITY_WSA");
				#endif
				#if UNITY_WSA_10_0
					AddDefined("UNITY_WSA_10_0");
				#else
					AddUndefined("UNITY_WSA_10_0");
				#endif
				#if UNITY_XBOXONE
					AddDefined("UNITY_XBOXONE");
				#else
					AddUndefined("UNITY_XBOXONE");
				#endif
			}

		#endregion


		#region Properties

			public static IReadOnlyCollection<string> Defined => _defined;
			public static IReadOnlyCollection<string> Detectable => _detectable;	// IReadOnlyCollection mandatory as its Sorted

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