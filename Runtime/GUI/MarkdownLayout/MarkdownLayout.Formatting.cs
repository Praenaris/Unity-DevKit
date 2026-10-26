namespace Praenaris.GUI
{
	public partial class MarkdownLayout	// Formatting
	{
		#region Privates - Formatting

			/// <summary>Lines starting with ">", consecutive ones being the same quote.</summary>
			private bool TryParseQuote(string line)
			{
				if (line[0] != '>') return false;

				AppendBlockLine(_quotePrefab, "quote", line.Substring(1).TrimStart());
				return true;
			}

			/// <summary>Three or more "-", "*" or "_" alone on a line, optionally spaced ("---", "* * *").</summary>
			private bool TryParseSeparator(string line)
			{
				char marker = line[0];
				if ((marker != '-') && (marker != '*') && (marker != '_')) return false;

				int markerCount = 0;
				foreach (char character in line) {
					if (character == marker)
						markerCount++;
					else if (!char.IsWhiteSpace(character))
						return false;	// "-*-" or "- item" aren't separators
				}
				if (markerCount < 3) return false;

				FlushBlock();
				AppendItem(_separatorPrefab, "separator", line);
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