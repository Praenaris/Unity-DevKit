using DragonResonance.Behaviours;
using DragonResonance.Extensions;
using Praenaris.Serializables;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Praenaris.GUI
{
	public partial class MarkdownLayout : PossumBehaviour
	{
		private const string ParagraphName = "paragraph";

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
		[SerializeField] private SerializableKeyValuePair<string, Sprite>[] _imagesMap = { };

		[Header("Formatting")]
		[SerializeField] private GameObject _quotePrefab = null;
		[SerializeField] private GameObject _separatorPrefab = null;

		private readonly List<string> _blockLines = new();	// The lines of the paragraph or quote being read, until something ends it
		private GameObject _blockPrefab = null;
		private string _blockName = null;


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
					UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(base.gameObject.scene);
				#endif
			}

		#endregion


		#region Privates

			[ContextMenu(nameof(ParseData))]
			private void ParseData()
			{
				_blockLines.Clear();
				_blockName = null;
				foreach (string rawLine in _inputMarkdown.text.Split('\n')) {
					string line = rawLine.Trim();	// Also drops the "\r" of Windows line endings
					if (line.IsEmpty()) {
						FlushBlock();	// A blank line ends the paragraph or quote
						continue;
					}

					if (TryParseHeading(line)) continue;
					if (TryParseUnderlinedHeading(line)) continue;	// Before the separator, as "---" under a paragraph is a heading
					if (TryParseSeparator(line)) continue;
					if (TryParseImage(line)) continue;
					if (TryParseQuote(line)) continue;
					AppendBlockLine(_paragraphPrefab, ParagraphName, line);	// Anything not recognized is a paragraph
				}
				FlushBlock();
			}


			private GameObject AppendItem(GameObject prefab, string elementName, string source)
			{
				if (prefab == null) {
					Error($"No {elementName} prefab assigned, skipping \"{source}\"");
					return null;
				}
				return InstantiateItem(prefab);
			}

			private void AppendText(GameObject prefab, string elementName, string text)
			{
				GameObject item = AppendItem(prefab, elementName, text);
				if (item == null) return;

				TMP_Text label = item.GetComponentInChildren<TMP_Text>();
				label.text = text;
				RecordItemChanges(label);
			}

			private GameObject InstantiateItem(GameObject prefab)
			{
				#if UNITY_EDITOR
				if (!Application.isPlaying)
					return (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, _outputContainer);	// Keeps the prefab while on Editor
				#endif
				return Instantiate(prefab, _outputContainer);
			}


			private static void RecordItemChanges(Component component)
			{
				#if UNITY_EDITOR
				if (!Application.isPlaying)
					UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(component);
				#endif
			}

		#endregion


		#region Privates - Texts / Fallbacks

			private void AppendBlockLine(GameObject prefab, string elementName, string line)
			{
				if (elementName != _blockName) {
					FlushBlock();
					_blockPrefab = prefab;
					_blockName = elementName;
				}
				if (!line.IsEmpty())
					_blockLines.Add(line);
			}


			private void FlushBlock()
			{
				if (!_blockLines.IsEmpty())
					AppendText(_blockPrefab, _blockName, string.Join(" ", _blockLines));
				_blockLines.Clear();
				_blockName = null;
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