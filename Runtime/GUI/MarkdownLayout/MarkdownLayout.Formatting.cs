using DragonResonance.Extensions;
using System.Collections.Generic;
using System.Linq;


namespace Praenaris.GUI
{
	public partial class MarkdownLayout	// Formatting
	{
		private readonly List<string> _quoteLines = new();	// The lines of the quote being read, until something ends it


		#region Privates - Formatting

			/// <summary>Lines starting with ">", consecutive ones being the same quote.</summary>
			private bool TryParseQuote(string line)
			{
				string trimmedLine = line.Trim();
				if (!trimmedLine.StartsWith(">")) return false;

				FlushParagraph();
				_quoteLines.Add(trimmedLine.Substring(1).Trim());
				return true;
			}

			private void FlushQuote()
			{
				if (_quoteLines.IsEmpty()) return;

				AppendText(_quotePrefab, "quote", string.Join(" ", _quoteLines.Where(quoteLine => !quoteLine.IsEmpty())));	// Consecutive lines are one quote
				_quoteLines.Clear();
			}

			/// <summary>Three or more "-", "*" or "_" alone on a line, optionally spaced ("---", "* * *").</summary>
			private bool TryParseSeparator(string line)
			{
				string compactLine = string.Concat(line.Where(character => !char.IsWhiteSpace(character)));
				if ((compactLine.Length < 3) || ("-*_".IndexOf(compactLine[0]) < 0)) return false;
				if (compactLine.Any(character => (character != compactLine[0]))) return false;	// "-*-" isn't a separator

				FlushParagraph();
				AppendItem(_separatorPrefab, "separator");
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