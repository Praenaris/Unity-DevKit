#if UNITY_EDITOR


using UnityEditor;


namespace DragonResonance.Editor.Building
{
	[InitializeOnLoad]
	public static class OutputBuildPropertiesToggler
	{
		private const string OUTPUT_BUILD_PROPERTIES_DEFINE = "OUTPUT_BUILD_PROPERTIES";


		#region Constructors

			static OutputBuildPropertiesToggler() => BuildDefines.SetDefinition(OUTPUT_BUILD_PROPERTIES_DEFINE, false);

		#endregion


		#region Publics

			#if OUTPUT_BUILD_PROPERTIES
				[MenuItem("Build/Output Build Properties [ON]/Disable output")]
			#else
				[MenuItem("Build/Output Build Properties [OFF]/Enable output")]
			#endif
			public static void SwitchLogging() => BuildDefines.ToggleBuildDefinition(OUTPUT_BUILD_PROPERTIES_DEFINE);

		#endregion
	}
}


#endif


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