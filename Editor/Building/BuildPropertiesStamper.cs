#if UNITY_EDITOR && OUTPUT_BUILD_PROPERTIES


using DragonResonance.Logging;
using System.IO;
using System;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEngine;
using Version = DragonResonance.Miscellany.Version;


namespace DragonResonance.Editor.Building
{
	public class BuildPropertiesStamper : IPostprocessBuildWithReport
	{
		private const string OUTPUT_FILEPATH     = "build.properties";

		private const string APP_NAME_KEY        = "application_name";
		private const string APP_VERSION_KEY     = "application_version";
		private const string BUILD_TARGET_KEY    = "build_target";
		private const string BUILD_DATETIME_KEY  = "build_datetime";
		private const string BUILD_TIMESTAMP_KEY = "build_timestamp";
		private const string COMPANY_NAME_KEY    = "company_name";
		private const string ENGINE_NAME_KEY     = "engine_name";
		private const string ENGINE_VERSION_KEY  = "engine_version";


		#region Events

			public void OnPostprocessBuild(BuildReport report) => StampBuildingData(report);

		#endregion


		#region Publics

			public static string GetFormattedTimestampDatetime() => GetFormattedTimestampDatetime(DateTimeOffset.UtcNow);
			public static string GetFormattedTimestampDatetime(DateTimeOffset datetime) => $"{datetime:yyMMddHHmmss}";
			public static string GetFormattedLine(string key, string value) => $"{key}={value}";

			public static void StampBuildingData(BuildReport report)
			{
				string buildPath = report.summary.outputPath;
				bool isFolderBuild = (Directory.Exists(buildPath) && !string.Equals(Path.GetExtension(buildPath), ".app", StringComparison.OrdinalIgnoreCase));	// Web builds are the folder itself, macOS bundles mustn't be modified or their signature breaks
				string buildDirectory = (isFolderBuild ? buildPath : Path.GetDirectoryName(buildPath)) ?? ".";
				string outputPath = Path.Combine(buildDirectory, OUTPUT_FILEPATH);
				DateTimeOffset now = DateTimeOffset.UtcNow;	// Read once so both stamps match

				Log.Info($"Stamping data to {outputPath}...");
				{
					File.WriteAllLines(outputPath, new[] {
						GetFormattedLine(APP_NAME_KEY, Application.productName),
						GetFormattedLine(APP_VERSION_KEY, Version.AppVersionLower),
						GetFormattedLine(BUILD_TARGET_KEY, report.summary.platform.ToString()),
						GetFormattedLine(BUILD_DATETIME_KEY, GetFormattedTimestampDatetime(now)),
						GetFormattedLine(BUILD_TIMESTAMP_KEY, $"{now.ToUnixTimeSeconds()}"),
						GetFormattedLine(COMPANY_NAME_KEY, Application.companyName),
						GetFormattedLine(ENGINE_NAME_KEY, "Unity"),
						GetFormattedLine(ENGINE_VERSION_KEY, Application.unityVersion),
					});
				}
				Log.Info($"Done!");
			}

		#endregion


		#region Properties

			public int callbackOrder => 0;	// Implicit override

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