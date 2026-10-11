using DragonResonance.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR_WIN
using Microsoft.Win32;
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
using System.IO;
using System.Xml;
#endif


namespace Praenaris.Editor.Tools
{
	/// <summary>
	/// Browses and edits the PlayerPrefs the Editor uses for the current project
	/// (<see cref="PlayerSettings.companyName"/> / <see cref="PlayerSettings.productName"/>).
	/// Keys are listed from the platform storage; types are resolved through the PlayerPrefs API itself,
	/// since the storage doesn't reliably tell ints and floats apart (Windows tags both as REG_DWORD).
	/// </summary>
	public class PlayerPrefsEditor : EditorWindow
	{
		private const string SearchModeEditorPrefKey = "Praenaris.PlayerPrefsEditor.SearchMode";
		private const string ShowUnityKeysEditorPrefKey = "Praenaris.PlayerPrefsEditor.ShowUnityKeys";
		private const string AutoRefreshEditorPrefKey = "Praenaris.PlayerPrefsEditor.AutoRefresh";
		private const string MissingStringSentinel = "\u0000PlayerPrefsEditor.Missing\u0000";
		private const double AutoRefreshInterval = 1d;
		private const float TypeWidth = 60f;
		private const float ButtonWidth = 24f;
		private const float ColumnMinWidth = 100f;

		private static readonly string[] UnityKeyPrefixes = { "unity.", "Unity", "Screenmanager" };
#if UNITY_EDITOR_WIN
		private static readonly Regex WindowsHashSuffix = new(@"_h\d+$", RegexOptions.Compiled);
#endif
		private static readonly Color AlternateRowColor = new(0f, 0f, 0f, 0.08f);
		private static readonly Color DirtyMarkColor = new(1f, 0.75f, 0.2f, 1f);
		private static readonly Color InvalidKeyColor = new(1f, 0.45f, 0.45f, 1f);

		private readonly List<PrefEntry> _entries = new();
		private Vector2 _scroll = Vector2.zero;
		private string _search = "";
		private ESearchMode _searchMode = ESearchMode.Key;
		private bool _showUnityKeys = false;
		private bool _autoRefresh = false;
		private double _nextAutoRefreshTime = 0d;
		private bool _isListingSupported = true;
		private int _shownCount = 0;
		private int _hiddenUnityCount = 0;
		private string _status = "";


		#region Enums

			private enum EPrefType { Int, Float, String }
			private enum ESearchMode { Key, Value }
			private enum ERowAction { None, Save, Revert, Delete }

		#endregion


		#region Entries

			/// <summary>A key as it is in storage (<c>Stored*</c>) plus its pending edits. Values are kept as invariant strings.</summary>
			private class PrefEntry
			{
				public string StoredKey;
				public EPrefType StoredType;
				public string StoredValue;
				public string Key;
				public EPrefType Type;
				public string Value;

				public bool IsNew => StoredKey == null;
				public bool IsDirty => IsNew || Key != StoredKey || Type != StoredType || Value != StoredValue;

				public static PrefEntry FromStorage(string key, EPrefType type, string value) => new() {
					StoredKey = key, StoredType = type, StoredValue = value,
					Key = key, Type = type, Value = value,
				};

				public void Revert()
				{
					Key = StoredKey;
					Type = StoredType;
					Value = StoredValue;
				}

				public void MarkStored()
				{
					StoredKey = Key;
					StoredType = Type;
					StoredValue = Value;
				}
			}

		#endregion


		#region Events

			private void OnEnable()
			{
				_searchMode = (ESearchMode)EditorPrefs.GetInt(SearchModeEditorPrefKey, (int)ESearchMode.Key);
				_showUnityKeys = EditorPrefs.GetBool(ShowUnityKeysEditorPrefKey, false);
				_autoRefresh = EditorPrefs.GetBool(AutoRefreshEditorPrefKey, false);
				Refresh(keepPending: false);
			}

			private void OnFocus() => Refresh(keepPending: true);

			private void OnInspectorUpdate()
			{
				if (!_autoRefresh || EditorApplication.timeSinceStartup < _nextAutoRefreshTime) return;
				_nextAutoRefreshTime = EditorApplication.timeSinceStartup + AutoRefreshInterval;
				Refresh(keepPending: true);
			}

			private void OnGUI()
			{
				DrawToolbar();
				DrawHeader();
				DrawEntries();
				DrawStatusBar();
			}

		#endregion


		#region Publics

			[MenuItem("Window/Praenaris/PlayerPrefs Editor")]
			public static void CreateWindow() => GetWindow<PlayerPrefsEditor>("PlayerPrefs");

		#endregion


		#region GUI

			private void DrawToolbar()
			{
				using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
					if (GUILayout.Button(new GUIContent("Add", "Add a new key"), EditorStyles.toolbarButton, GUILayout.Width(40f)))
						AddNewEntry();

					if (GUILayout.Button(new GUIContent("Refresh", "Reload the keys from storage, keeping unsaved edits"), EditorStyles.toolbarButton, GUILayout.Width(55f)))
						Refresh(keepPending: true);

					EditorGUI.BeginChangeCheck();
					_autoRefresh = GUILayout.Toggle(_autoRefresh, new GUIContent("Auto", "Refresh every second (useful in Play Mode)"), EditorStyles.toolbarButton, GUILayout.Width(40f));
					if (EditorGUI.EndChangeCheck())
						EditorPrefs.SetBool(AutoRefreshEditorPrefKey, _autoRefresh);

					GUILayout.Space(6f);
					_search = EditorGUILayout.TextField(_search, EditorStyles.toolbarSearchField, GUILayout.MinWidth(80f));

					EditorGUI.BeginChangeCheck();
					_searchMode = (ESearchMode)EditorGUILayout.EnumPopup(_searchMode, EditorStyles.toolbarPopup, GUILayout.Width(55f));
					if (EditorGUI.EndChangeCheck())
						EditorPrefs.SetInt(SearchModeEditorPrefKey, (int)_searchMode);

					EditorGUI.BeginChangeCheck();
					_showUnityKeys = GUILayout.Toggle(_showUnityKeys, new GUIContent("Unity keys", "Show the keys Unity itself stores (unity.*, Screenmanager*, ...)"), EditorStyles.toolbarButton, GUILayout.Width(70f));
					if (EditorGUI.EndChangeCheck())
						EditorPrefs.SetBool(ShowUnityKeysEditorPrefKey, _showUnityKeys);

					GUILayout.Space(6f);
					using (new EditorGUI.DisabledScope(!_entries.Any(entry => entry.IsDirty))) {
						if (GUILayout.Button(new GUIContent("Save All", "Save every pending edit"), EditorStyles.toolbarButton, GUILayout.Width(60f)))
							SaveEntries(_entries.Where(entry => entry.IsDirty).ToList());

						if (GUILayout.Button(new GUIContent("Revert All", "Discard every pending edit"), EditorStyles.toolbarButton, GUILayout.Width(65f)))
							RevertAll();
					}

					if (GUILayout.Button(new GUIContent("Delete All", "Delete every PlayerPrefs key of this project"), EditorStyles.toolbarButton, GUILayout.Width(65f)))
						AskDeleteAll();
				}
			}

			private void DrawHeader()
			{
				using (new EditorGUILayout.HorizontalScope()) {
					GUILayout.Space(6f);
					GUILayout.Label("Key", EditorStyles.miniBoldLabel, GUILayout.MinWidth(ColumnMinWidth), GUILayout.ExpandWidth(true));
					GUILayout.Label("Type", EditorStyles.miniBoldLabel, GUILayout.Width(TypeWidth));
					GUILayout.Label("Value", EditorStyles.miniBoldLabel, GUILayout.MinWidth(ColumnMinWidth), GUILayout.ExpandWidth(true));
					GUILayout.Space((ButtonWidth + 3f) * 3f);
				}
			}

			private void DrawEntries()
			{
				List<PrefEntry> visibleEntries = _entries.Where(IsVisible).ToList();
				_shownCount = visibleEntries.Count;
				_hiddenUnityCount = _showUnityKeys ? 0 : _entries.Count(entry => !entry.IsNew && IsUnityKey(entry.Key));

				using var scroll = new EditorGUILayout.ScrollViewScope(_scroll);
				_scroll = scroll.scrollPosition;

				if (!_isListingSupported)
					EditorGUILayout.HelpBox("Listing PlayerPrefs isn't supported on this platform. Added keys can still be edited.", MessageType.Warning);
				else if (visibleEntries.Count == 0)
					EditorGUILayout.LabelField("No PlayerPrefs found.", EditorStyles.centeredGreyMiniLabel);

				PrefEntry actionEntry = null;
				ERowAction action = ERowAction.None;
				for (int i = 0; i < visibleEntries.Count; i++) {
					ERowAction rowAction = DrawEntry(visibleEntries[i], i);
					if (rowAction != ERowAction.None) {
						actionEntry = visibleEntries[i];
						action = rowAction;
					}
				}

				switch (action) {
					case ERowAction.Save: SaveEntries(new[] { actionEntry }); break;
					case ERowAction.Revert: RevertEntry(actionEntry); break;
					case ERowAction.Delete: DeleteEntry(actionEntry); break;
				}
			}

			private ERowAction DrawEntry(PrefEntry entry, int index)
			{
				ERowAction action = ERowAction.None;
				Rect rowRect = EditorGUILayout.BeginHorizontal();
				if (Event.current.type == EventType.Repaint) {
					if (index % 2 == 1)
						EditorGUI.DrawRect(rowRect, AlternateRowColor);
					if (entry.IsDirty)
						EditorGUI.DrawRect(new Rect(rowRect.x, rowRect.y, 2f, rowRect.height), DirtyMarkColor);
				}
				GUILayout.Space(6f);

				bool isKeyValid = IsKeyValid(entry);
				Color previousBackground = UnityEngine.GUI.backgroundColor;
				if (!isKeyValid)
					UnityEngine.GUI.backgroundColor = InvalidKeyColor;
				entry.Key = EditorGUILayout.TextField(entry.Key ?? "", GUILayout.MinWidth(ColumnMinWidth), GUILayout.ExpandWidth(true));
				UnityEngine.GUI.backgroundColor = previousBackground;

				EditorGUI.BeginChangeCheck();
				EPrefType type = (EPrefType)EditorGUILayout.EnumPopup(entry.Type, GUILayout.Width(TypeWidth));
				if (EditorGUI.EndChangeCheck()) {
					entry.Value = ConvertValue(entry.Value, type);
					entry.Type = type;
				}

				entry.Value = DrawValueField(entry.Type, entry.Value);

				using (new EditorGUI.DisabledScope(!entry.IsDirty)) {
					using (new EditorGUI.DisabledScope(!isKeyValid)) {
						if (IconButton("SaveActive", "Save"))
							action = ERowAction.Save;
					}
					if (IconButton("Refresh", entry.IsNew ? "Discard" : "Revert"))
						action = ERowAction.Revert;
				}
				if (IconButton("TreeEditor.Trash", "Delete"))
					action = ERowAction.Delete;

				EditorGUILayout.EndHorizontal();
				return action;
			}

			private void DrawStatusBar()
			{
				using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
					string counts = $"{_shownCount} shown";
					if (_hiddenUnityCount > 0)
						counts += $", {_hiddenUnityCount} Unity keys hidden";
					GUILayout.Label(string.IsNullOrEmpty(_status) ? counts : $"{counts}  ·  {_status}", EditorStyles.miniLabel);
					GUILayout.FlexibleSpace();
					GUILayout.Label(new GUIContent(StorageLocation, "Where the Editor stores this project's PlayerPrefs"), EditorStyles.miniLabel);
				}
			}

			private static string DrawValueField(EPrefType type, string value)
			{
				GUILayoutOption[] options = { GUILayout.MinWidth(ColumnMinWidth), GUILayout.ExpandWidth(true) };
				EditorGUI.BeginChangeCheck();
				switch (type) {
					case EPrefType.Int:
						int intValue = EditorGUILayout.IntField(ParseInt(value), options);
						return EditorGUI.EndChangeCheck() ? FormatInt(intValue) : value;
					case EPrefType.Float:
						float floatValue = EditorGUILayout.FloatField(ParseFloat(value), options);
						return EditorGUI.EndChangeCheck() ? FormatFloat(floatValue) : value;
					default:
						string stringValue = EditorGUILayout.TextField(value ?? "", options);
						return EditorGUI.EndChangeCheck() ? stringValue : value;
				}
			}

			private static bool IconButton(string iconName, string tooltip)
			{
				return GUILayout.Button(EditorGUIUtility.TrIconContent(iconName, tooltip), GUILayout.Width(ButtonWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight));
			}

		#endregion


		#region Actions

			private void AddNewEntry()
			{
				_entries.Insert(0, new PrefEntry { Key = "", Type = EPrefType.String, Value = "" });
				_search = "";
				_scroll = Vector2.zero;
				UnityEngine.GUI.FocusControl(null);
			}

			private void SaveEntries(IReadOnlyCollection<PrefEntry> entries)
			{
				int saved = 0;
				int skipped = 0;
				foreach (PrefEntry entry in entries) {
					if (!IsKeyValid(entry)) {
						skipped++;
						continue;
					}
					WriteEntry(entry);
					saved++;
				}
				PlayerPrefs.Save();
				SortEntries();
				UnityEngine.GUI.FocusControl(null);

				_status = skipped > 0 ? $"Saved {saved}, skipped {skipped} with an empty or duplicated key." : $"Saved {saved}.";
			}

			private void RevertEntry(PrefEntry entry)
			{
				if (entry.IsNew)
					_entries.Remove(entry);
				else
					entry.Revert();
				UnityEngine.GUI.FocusControl(null);
			}

			private void RevertAll()
			{
				_entries.RemoveAll(entry => entry.IsNew);
				foreach (PrefEntry entry in _entries)
					entry.Revert();
				SortEntries();
				UnityEngine.GUI.FocusControl(null);
				_status = "Reverted every pending edit.";
			}

			private void DeleteEntry(PrefEntry entry)
			{
				if (!entry.IsNew) {
					PlayerPrefs.DeleteKey(entry.StoredKey);
					PlayerPrefs.Save();
					_status = $"Deleted '{entry.StoredKey}'.";
				}
				_entries.Remove(entry);
				UnityEngine.GUI.FocusControl(null);
			}

			private void AskDeleteAll()
			{
				if (!EditorUtility.DisplayDialog("Delete all PlayerPrefs?",
					$"Are you COMPLETELY sure you want to delete ALL the PlayerPrefs of {PlayerSettings.productName}, Unity's own keys included? THIS CANNOT BE UNDONE.",
					"Yes, delete",
					"No, cancel"))
					return;

				PlayerPrefs.DeleteAll();
				PlayerPrefs.Save();
				HLogger.LogInfo("Deleted all PlayerPrefs.", typeof(PlayerPrefsEditor));
				Refresh(keepPending: false);
				_status = "Deleted all PlayerPrefs.";
			}

		#endregion


		#region Entries management

			private void Refresh(bool keepPending)
			{
				List<PrefEntry> pendingEntries = keepPending ? _entries.Where(entry => entry.IsDirty).ToList() : new List<PrefEntry>();
				HashSet<string> pendingKeys = new(pendingEntries.Where(entry => !entry.IsNew).Select(entry => entry.StoredKey));

				_entries.Clear();
				_entries.AddRange(pendingEntries);

				try {
					string[] storedKeys = ReadStoredKeys();
					_isListingSupported = storedKeys != null;
					foreach (string key in storedKeys?.Distinct() ?? Enumerable.Empty<string>()) {
						if (pendingKeys.Contains(key)) continue;
						if (TryReadPref(key, out EPrefType type, out string value))
							_entries.Add(PrefEntry.FromStorage(key, type, value));
					}
				}
				catch (Exception exception) {
					_status = $"Couldn't read the PlayerPrefs: {exception.Message}";
					HLogger.LogError(_status, typeof(PlayerPrefsEditor));
				}

				SortEntries();
				Repaint();
			}

			private void SortEntries()
			{
				List<PrefEntry> sorted = _entries
					.OrderByDescending(entry => entry.IsNew)
					.ThenBy(entry => entry.IsNew ? "" : entry.StoredKey, StringComparer.OrdinalIgnoreCase)
					.ToList();
				_entries.Clear();
				_entries.AddRange(sorted);
			}

			private bool IsVisible(PrefEntry entry)
			{
				if (entry.IsNew) return true;
				if (!_showUnityKeys && IsUnityKey(entry.Key)) return false;
				if (string.IsNullOrEmpty(_search)) return true;

				string searched = _searchMode == ESearchMode.Key ? entry.Key : entry.Value;
				return searched != null && searched.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0;
			}

			private bool IsKeyValid(PrefEntry entry)
			{
				if (string.IsNullOrEmpty(entry.Key)) return false;
				return !_entries.Any(other => other != entry && (other.Key == entry.Key || other.StoredKey == entry.Key));
			}

			private static bool IsUnityKey(string key) => key != null && UnityKeyPrefixes.Any(prefix => key.StartsWith(prefix, StringComparison.Ordinal));

		#endregion


		#region PlayerPrefs

			/// <summary>Resolves a key's type by probing the PlayerPrefs getters: a getter of the wrong type always returns the given default.</summary>
			private static bool TryReadPref(string key, out EPrefType type, out string value)
			{
				if (PlayerPrefs.GetInt(key, 0) != 0 || PlayerPrefs.GetInt(key, 1) != 1) {
					type = EPrefType.Int;
					value = FormatInt(PlayerPrefs.GetInt(key));
					return true;
				}
				if (!Mathf.Approximately(PlayerPrefs.GetFloat(key, 0f), 0f) || !Mathf.Approximately(PlayerPrefs.GetFloat(key, 1f), 1f)) {
					type = EPrefType.Float;
					value = FormatFloat(PlayerPrefs.GetFloat(key));
					return true;
				}
				string stringValue = PlayerPrefs.GetString(key, MissingStringSentinel);
				if (stringValue != MissingStringSentinel) {
					type = EPrefType.String;
					value = stringValue;
					return true;
				}

				type = EPrefType.String;
				value = null;
				return false;
			}

			private static void WriteEntry(PrefEntry entry)
			{
				if (!entry.IsNew && (entry.StoredKey != entry.Key || entry.StoredType != entry.Type))
					PlayerPrefs.DeleteKey(entry.StoredKey);

				switch (entry.Type) {
					case EPrefType.Int: PlayerPrefs.SetInt(entry.Key, ParseInt(entry.Value)); break;
					case EPrefType.Float: PlayerPrefs.SetFloat(entry.Key, ParseFloat(entry.Value)); break;
					default: PlayerPrefs.SetString(entry.Key, entry.Value ?? ""); break;
				}
				entry.MarkStored();
			}

			private static string ConvertValue(string value, EPrefType toType) => toType switch {
				EPrefType.Int => FormatInt(Mathf.RoundToInt(ParseFloat(value))),
				EPrefType.Float => FormatFloat(ParseFloat(value)),
				_ => value ?? "",
			};

			private static int ParseInt(string value) => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : 0;
			private static float ParseFloat(string value) => float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result) ? result : 0f;
			private static string FormatInt(int value) => value.ToString(CultureInfo.InvariantCulture);
			private static string FormatFloat(float value) => value.ToString("R", CultureInfo.InvariantCulture);

		#endregion


		#region Platform storage

			/// <summary>Where the Editor keeps this project's PlayerPrefs: a registry key on Windows, a file elsewhere.</summary>
			private static string StorageLocation
			{
				get {
#if UNITY_EDITOR_WIN
					return $@"Software\Unity\UnityEditor\{PlayerSettings.companyName}\{PlayerSettings.productName}";
#elif UNITY_EDITOR_OSX
					return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Preferences", $"unity.{PlayerSettings.companyName}.{PlayerSettings.productName}.plist");
#elif UNITY_EDITOR_LINUX
					return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "unity3d", PlayerSettings.companyName, PlayerSettings.productName, "prefs");
#else
					return "Unknown";
#endif
				}
			}

			/// <summary>Lists the stored key names, or <c>null</c> if listing isn't supported on this platform.</summary>
			private static string[] ReadStoredKeys()
			{
#if UNITY_EDITOR_WIN
				// Value names carry a "_h<hash>" suffix; the values' kinds are unreliable (floats are 8-byte REG_DWORDs).
				using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(StorageLocation);
				if (registryKey == null) return Array.Empty<string>();
				return registryKey.GetValueNames().Select(name => WindowsHashSuffix.Replace(name, "")).ToArray();
#elif UNITY_EDITOR_OSX
				// Binary plist; let plutil turn it into XML. Unsaved prefs may not be on disk yet.
				PlayerPrefs.Save();
				if (!File.Exists(StorageLocation)) return Array.Empty<string>();
				XmlDocument plist = ParseXml(RunPlutilToXml(StorageLocation));
				XmlNode dictionary = plist.DocumentElement?["dict"];
				if (dictionary == null) return Array.Empty<string>();
				return dictionary.ChildNodes.OfType<XmlElement>().Where(node => node.Name == "key").Select(node => node.InnerText).ToArray();
#elif UNITY_EDITOR_LINUX
				// XML with <pref name="..." type="int|float|string">.
				PlayerPrefs.Save();
				if (!File.Exists(StorageLocation)) return Array.Empty<string>();
				XmlDocument prefs = ParseXml(File.ReadAllText(StorageLocation));
				return prefs.GetElementsByTagName("pref").OfType<XmlElement>().Select(node => node.GetAttribute("name")).ToArray();
#else
				return null;
#endif
			}

#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
			private static XmlDocument ParseXml(string xml)
			{
				var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null };
				using XmlReader reader = XmlReader.Create(new StringReader(xml), settings);
				var document = new XmlDocument { XmlResolver = null };
				document.Load(reader);
				return document;
			}

			private static string RunPlutilToXml(string plistPath)
			{
				var startInfo = new System.Diagnostics.ProcessStartInfo("plutil", $"-convert xml1 -o - \"{plistPath}\"") {
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true,
				};
				using System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo);
				if (process == null) return null;
				string output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				return output;
			}
#endif

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