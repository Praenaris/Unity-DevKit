using System.Linq;
using System.Text.RegularExpressions;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;


namespace Praenaris.GUI
{
	public partial class MarkdownLayout	// Media
	{
		private static readonly Regex ImageRegex = new(@"^!\[(?<alt>[^\]]*)\]\((?<path>[^\s)]+)(\s+""(?<title>[^""]*)"")?\)$", RegexOptions.Compiled);


		#region Privates - Media

			/// <summary>"![Alt text](Image.png "Title")", the alt text can be empty and the title is optional. The path is the key in the images map.</summary>
			private bool TryParseImage(string line)
			{
				if (!line.StartsWith("![", StringComparison.Ordinal)) return false;	// Cheap check before the regex
				Match match = ImageRegex.Match(line);
				if (!match.Success) return false;

				FlushBlock();
				GameObject item = AppendItem(_imagePrefab, "image", line);
				if (item == null) return true;

				string path = match.Groups["path"].Value;
				Sprite sprite = _imagesMap.FirstOrDefault(image => (image.Key == path)).Value;
				if (sprite != null) {
					Image image = item.GetComponentInChildren<Image>();
					image.sprite = sprite;
					RecordItemChanges(image);
				}
				else {
					Warning($"No image mapped as \"{path}\", showing its alt text instead");
					TMP_Text label = item.GetComponentInChildren<TMP_Text>();	// Optional in the image prefab
					if (label != null) {
						label.text = match.Groups["alt"].Value;
						RecordItemChanges(label);
					}
				}
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