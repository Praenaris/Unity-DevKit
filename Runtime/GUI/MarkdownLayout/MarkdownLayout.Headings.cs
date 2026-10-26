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
				int level = (line.Length - line.TrimStart('#').Length);
				if ((level < 1) || (level > 6)) return false;
				if ((level < line.Length) && !char.IsWhiteSpace(line[level])) return false;	// "#hashtag" isn't a heading

				string text = line.Substring(level).Trim();
				string unclosedText = text.TrimEnd('#');
				if (unclosedText.IsEmpty() || char.IsWhiteSpace(unclosedText[^1]))	// Only a separate closing sequence, "C#" stays as it is
					text = unclosedText.TrimEnd();

				FlushBlock();
				AppendText(GetHeadingPrefab(level), $"heading {level}", text);
				return true;
			}

			/// <summary>A paragraph underlined with "===" (heading 1) or "---" (heading 2), of any length and without spaces.</summary>
			private bool TryParseUnderlinedHeading(string line)
			{
				if ((_blockName != ParagraphName) || _blockLines.IsEmpty()) return false;	// Underlining nothing, "---" is a separator and "===" text

				char marker = line[0];
				if ((marker != '=') && (marker != '-')) return false;
				foreach (char character in line) {
					if (character != marker) return false;	// "- - -" is a separator even under a paragraph
				}

				int level = ((marker == '=') ? 1 : 2);
				string text = string.Join(" ", _blockLines);
				_blockLines.Clear();	// The paragraph becomes the heading
				_blockName = null;
				AppendText(GetHeadingPrefab(level), $"heading {level}", text);
				return true;
			}

			private GameObject GetHeadingPrefab(int level) => level switch {
				1 => _heading1Prefab,
				2 => _heading2Prefab,
				3 => _heading3Prefab,
				4 => _heading4Prefab,
				5 => _heading5Prefab,
				_ => _heading6Prefab,
			};

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