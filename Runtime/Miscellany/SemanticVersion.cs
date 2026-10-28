using DragonResonance.Logging;
using System.Text.RegularExpressions;
using System;


namespace Praenaris.Miscellany
{
	public readonly struct SemanticVersion
	{
		private static readonly Regex SEMVER_REGEX = new(
			@"^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(?:-(?<label>[0-9A-Za-z\-\.]+))?$",
			RegexOptions.Compiled
		);

		private readonly int _major;
		private readonly int _minor;
		private readonly int _patch;
		private readonly string _label;


		#region Constructors

			public SemanticVersion(int major, int minor, int patch, string label = null)
			{
				_major = major;
				_minor = minor;
				_patch = patch;
				_label = label;
			}

			public SemanticVersion(string version)
			{
				Match match = SEMVER_REGEX.Match(version);
				if (match.Success) {
					_major = int.Parse(match.Groups["major"].Value);
					_minor = int.Parse(match.Groups["minor"].Value);
					_patch = int.Parse(match.Groups["patch"].Value);
					_label = (match.Groups["label"].Success ? match.Groups["label"].Value : null);
				}
				else {
					HLogger.LogError($"Version {version} is not semantic", typeof(SemanticVersion));
					_major = _minor = _patch = 0;
					_label = null;
				}
			}

		#endregion


		#region Publics

			/// <summary>Like the string constructor, but without logging an error when the version isn't semantic.</summary>
			public static bool TryParse(string version, out SemanticVersion semanticVersion)
			{
				bool isSemantic = SEMVER_REGEX.IsMatch(version ?? string.Empty);
				semanticVersion = (isSemantic ? new SemanticVersion(version) : default);
				return isSemantic;
			}

			public SemanticVersion MajorUp() => new((_major + 1), 0, 0, _label);
			public SemanticVersion MinorUp() => new(_major, (_minor + 1), 0, _label);
			public SemanticVersion PatchUp() => new(_major, _minor, (_patch + 1), _label);

			public SemanticVersion MajorDown() => new(Math.Max(0, (_major - 1)), _minor, _patch, _label);
			public SemanticVersion MinorDown() => new(_major, Math.Max(0, (_minor - 1)), _patch, _label);
			public SemanticVersion PatchDown() => new(_major, _minor, Math.Max(0, (_patch - 1)), _label);

			public SemanticVersion WithLabel(string label) => new(_major, _minor, _patch, (string.IsNullOrEmpty(label) ? null : label));

		#endregion


		#region Properties

			public int Major => _major;
			public int Minor => _minor;
			public int Patch => _patch;
			public string Label => _label;

			public override string ToString() => ($"{_major}.{_minor}.{_patch}" + ((_label != null) ? $"-{_label}" : string.Empty));

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