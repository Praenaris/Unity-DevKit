using DragonResonance.Behaviours;
using DragonResonance.Extensions;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;


namespace Praenaris.GUI
{
	public partial class MarkdownLayout : PossumBehaviour
	{
		[SerializeField] private TextAsset _inputMarkdown = null;
		[SerializeField] private Transform _outputContainer = null;
		[SerializeField] private bool _regenerateOnStart = false;

		[Header("Texts")]
		[SerializeField] private GameObject _paragraphPrefab = null;

		[Header("Headings")]
		[SerializeField] private GameObject _heading1Prefab = null;
		[SerializeField] private GameObject _heading2Prefab = null;
		[SerializeField] private GameObject _heading3Prefab = null;
		[SerializeField] private GameObject _heading4Prefab = null;
		[SerializeField] private GameObject _heading5Prefab = null;
		[SerializeField] private GameObject _heading6Prefab = null;

		[Header("Media")]
		[SerializeField] private GameObject _imagePrefab = null;

		[Header("Formatting")]
		[SerializeField] private GameObject _quotePrefab = null;
		[SerializeField] private GameObject _separatorPrefab = null;


		private readonly List<string> _paragraphLines = new();	// The lines of the paragraph being read, until something ends it


		#region Events

			private void Start()
			{
				if (_regenerateOnStart)
					Regenerate();
			}

		#endregion


		#region Publics

			[ContextMenu(nameof(Regenerate))]
			public void Regenerate()
			{
				DestroyChildren(_outputContainer);
				ParseData();

				#if UNITY_EDITOR
				if (!Application.isPlaying)
					UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(base.gameObject.scene);	// So the generated items get saved
				#endif
			}

		#endregion


		#region Privates

			[ContextMenu(nameof(ParseData))]
			private void ParseData()
			{
				string[] lines = _inputMarkdown.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
				_paragraphLines.Clear();
				foreach (string line in lines) {
					if (string.IsNullOrWhiteSpace(line)) {
						FlushParagraph();	// A blank line ends the paragraph
						continue;
					}

					if (TryParseHeading(line)) continue;

					_paragraphLines.Add(line.Trim());	// Anything not recognized above is part of a paragraph
				}
				FlushParagraph();
			}

			private void AppendText(GameObject prefab, string elementName, string text)
			{
				if (prefab == null) {
					Error($"No {elementName} prefab assigned, skipping \"{text}\"");
					return;
				}

				GameObject item = InstantiateItem(prefab);
				TMP_Text label = item.GetComponentInChildren<TMP_Text>();
				label.text = text;

				#if UNITY_EDITOR
				if (!Application.isPlaying)
					UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(label);	// Or the prefab instance reverts the text when reloaded
				#endif
			}

			private GameObject InstantiateItem(GameObject prefab)
			{
				#if UNITY_EDITOR
				if (!Application.isPlaying)
					return (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, _outputContainer);	// Keeps the prefab link while editing
				#endif
				return Instantiate(prefab, _outputContainer);
			}

		#endregion


		#region Privates - Texts / Fallbacks

			// Every recognized element flushes the paragraph being read before appending itself, so the order is kept
			private void FlushParagraph()
			{
				if (_paragraphLines.IsEmpty()) return;

				AppendText(_paragraphPrefab, "paragraph", string.Join(" ", _paragraphLines));	// Consecutive lines are one paragraph
				_paragraphLines.Clear();
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