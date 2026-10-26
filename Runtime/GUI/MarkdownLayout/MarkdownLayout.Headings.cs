using DragonResonance.Extensions;
using UnityEngine;


namespace Praenaris.GUI
{
	public partial class MarkdownLayout	// Headings
	{
		#region Privates - Headings

			/// <summary>From "# Heading 1" to "###### Heading 6", optionally closed with hashes ("## Heading ##").</summary>
			private bool TryParseHeading(string line)
			{
				GameObject GetHeadingPrefab(int level) => level switch {
					1 => _heading1Prefab,
					2 => _heading2Prefab,
					3 => _heading3Prefab,
					4 => _heading4Prefab,
					5 => _heading5Prefab,
					_ => _heading6Prefab,
				};

				string trimmedLine = line.Trim();
				int level = 0;
				while ((level < trimmedLine.Length) && (trimmedLine[level] == '#'))
					level++;
				if ((level < 1) || (level > 6)) return false;
				if ((level < trimmedLine.Length) && !char.IsWhiteSpace(trimmedLine[level])) return false;	// "#hashtag" isn't a heading

				string text = trimmedLine.Substring(level).Trim();
				string unclosedText = text.TrimEnd('#');
				if (unclosedText.IsEmpty() || char.IsWhiteSpace(unclosedText[^1]))	// Only a separate closing sequence, "C#" stays as it is
					text = unclosedText.TrimEnd();

				FlushParagraph();
				AppendText(GetHeadingPrefab(level), $"heading {level}", text);
				return true;
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