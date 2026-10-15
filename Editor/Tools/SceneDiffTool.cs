#if UNITY_EDITOR


using DragonResonance.Logging;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace Praenaris.Editor.Tools
{
	/// <summary>
	/// Lists the objects of the active scene, or of the prefab open in Prefab Mode, that are still
	/// uncommitted, by diffing the asset file on disk against its HEAD revision one YAML document at a
	/// time, and mapping every differing document back to the GameObject or prefab instance that owns it.
	/// </summary>
	public class SceneDiffTool : EditorWindow
	{
		private enum EChangeKind
		{
			Modified = 0,
			Added    = 1,
			Removed  = 2
		}

		private class Document
		{
			public int ClassId = 0;
			public string TypeName = "";
			public string Body = "";
		}

		private class ChangedDocument
		{
			public long FileId = 0L;
			public string TypeName = "";
		}

		private class SceneChange
		{
			public EChangeKind Kind = EChangeKind.Modified;
			public long FileId = 0L;
			public bool IsPrefabInstance = false;
			public bool IsSceneWide = false;
			public string Name = "";
			public string HierarchyPath = "";
			public string ContentHash = "";
			public bool IsReviewed = false;
			public bool WentStale = false;
			public GameObject Target = null;
			public readonly List<ChangedDocument> ChangedDocuments = new List<ChangedDocument>();
		}

		/// <summary>
		/// Fades whatever is drawn inside it, to mark an entry as already reviewed.
		/// </summary>
		private readonly struct DimScope : IDisposable
		{
			private readonly Color _previousColor;

			// Qualified, because a bare "GUI" resolves to the sibling DragonResonance.Editor.GUI namespace.
			public DimScope(bool isDimmed)
			{
				_previousColor = UnityEngine.GUI.color;
				if (isDimmed)
					UnityEngine.GUI.color = new Color(_previousColor.r, _previousColor.g, _previousColor.b, _previousColor.a * 0.45f);
			}

			public void Dispose() => UnityEngine.GUI.color = _previousColor;
		}

		private const int GameObjectClassId = 1;
		private const int PrefabInstanceClassId = 1001;
		private const long SceneWideFileId = 0L;
		private const int MaxListedParts = 6;
		private const string ReviewedPrefsKeyPrefix = "DragonResonance.SceneDiffTool.Reviewed.";
		private const string HideReviewedPrefsKey = "DragonResonance.SceneDiffTool.HideReviewed";
		private const ulong HashOffsetBasis = 14695981039346656037UL;
		private const ulong HashPrime = 1099511628211UL;

		private static readonly Regex DocumentHeaderRegex = new Regex(@"^--- !u!(\d+) &(-?\d+)", RegexOptions.Compiled);
		private static readonly Regex GameObjectFieldRegex = new Regex(@"^\s+m_GameObject: \{fileID: (-?\d+)\}", RegexOptions.Compiled | RegexOptions.Multiline);
		private static readonly Regex PrefabInstanceFieldRegex = new Regex(@"^\s+m_PrefabInstance: \{fileID: (-?\d+)\}", RegexOptions.Compiled | RegexOptions.Multiline);
		private static readonly Regex NameFieldRegex = new Regex(@"^\s+m_Name: (.*)$", RegexOptions.Compiled | RegexOptions.Multiline);
		private static readonly Regex NameModificationRegex = new Regex(@"propertyPath: m_Name\s*\n\s+value: (.*)", RegexOptions.Compiled);
		private static readonly Regex SourcePrefabRegex = new Regex(@"m_SourcePrefab: \{fileID: -?\d+, guid: ([0-9a-fA-F]+)", RegexOptions.Compiled);

		private static GUIStyle[] _tagStyles = null;

		private readonly List<SceneChange> _changes = new List<SceneChange>();
		private string _assetName = "";
		private string _assetFileName = "";
		private string _assetGuid = "";
		private string _message = "";
		private MessageType _messageType = MessageType.Info;
		private bool _isPrefab = false;
		private bool _assetIsDirty = false;
		private bool _hasScanned = false;
		private bool _hideReviewed = false;
		private int _reviewedCount = 0;
		private Vector2 _scrollPosition = Vector2.zero;


		#region Events

			private void OnEnable()
			{
				EditorSceneManager.sceneSaved += OnSceneSaved;
				EditorSceneManager.sceneOpened += OnSceneOpened;
				PrefabStage.prefabStageOpened += OnPrefabStageOpened;
				PrefabStage.prefabStageClosing += OnPrefabStageClosing;
				PrefabStage.prefabSaved += OnPrefabSaved;
				_hideReviewed = EditorPrefs.GetBool(HideReviewedPrefsKey, false);
				Refresh();
			}

			private void OnDisable()
			{
				EditorSceneManager.sceneSaved -= OnSceneSaved;
				EditorSceneManager.sceneOpened -= OnSceneOpened;
				PrefabStage.prefabStageOpened -= OnPrefabStageOpened;
				PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
				PrefabStage.prefabSaved -= OnPrefabSaved;
				EditorApplication.delayCall -= Refresh;
			}

			private void OnFocus() => Refresh();

			private void OnSceneSaved(Scene scene) => Refresh();

			private void OnSceneOpened(Scene scene, OpenSceneMode mode) => Refresh();

			private void OnPrefabStageOpened(PrefabStage prefabStage) => Refresh();

			private void OnPrefabStageClosing(PrefabStage prefabStage)	// Deferred, as the closing stage is still the current one at this point
			{
				EditorApplication.delayCall -= Refresh;
				EditorApplication.delayCall += Refresh;
			}

			private void OnPrefabSaved(GameObject prefabRoot) => Refresh();

			private void OnGUI()
			{
				DrawToolbar();
				DrawHeader();

				if (!string.IsNullOrEmpty(_message))
					EditorGUILayout.HelpBox(_message, _messageType);

				if (_changes.Count == 0) {
					if (_hasScanned && string.IsNullOrEmpty(_message))
						EditorGUILayout.HelpBox($"No uncommitted changes found in this {this.AssetKind}.", MessageType.Info);
					return;
				}

				DrawChangeList();
				DrawFooter();
			}

		#endregion


		#region Publics

			[MenuItem("Window/Dragon Resonance/Scene Git Changes")]
			public static void CreateWindow()
			{
				SceneDiffTool window = GetWindow<SceneDiffTool>("Git Changes");
				window.minSize = new Vector2(360f, 200f);
			}

		#endregion


		#region Privates - Drawing

			private void DrawToolbar()
			{
				using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
					if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60f)))
						Refresh();

					using (new EditorGUI.DisabledScope(_changes.Count == 0)) {
						if (GUILayout.Button("Select all", EditorStyles.toolbarButton, GUILayout.Width(70f)))
							SelectAll();
					}

					GUILayout.FlexibleSpace();

					bool hideReviewed = GUILayout.Toggle(_hideReviewed, "Hide reviewed", EditorStyles.toolbarButton, GUILayout.Width(95f));
					if (hideReviewed != _hideReviewed) {
						_hideReviewed = hideReviewed;
						EditorPrefs.SetBool(HideReviewedPrefsKey, _hideReviewed);
					}

					using (new EditorGUI.DisabledScope(_reviewedCount == 0)) {
						if (GUILayout.Button("Reset", EditorStyles.toolbarButton, GUILayout.Width(50f)))
							ResetReviewState();
					}
				}
			}

			private void DrawProgress()
			{
				int total = _changes.Count;
				if (total == 0) return;

				int remaining = total - _reviewedCount;
				string label = remaining == 0
					? $"All {total} reviewed"
					: $"{_reviewedCount} of {total} reviewed  -  {remaining} left to check";

				Rect rect = GUILayoutUtility.GetRect(0f, 18f, GUILayout.ExpandWidth(true));
				EditorGUI.ProgressBar(rect, (float)_reviewedCount / total, label);
			}

			private void DrawHeader()
			{
				EditorGUILayout.LabelField(_isPrefab ? "Prefab" : "Scene", EditorStyles.boldLabel);
				EditorGUILayout.LabelField(string.IsNullOrEmpty(_assetName) ? "<none>" : _assetName, EditorStyles.miniLabel);

				if (!_assetIsDirty) return;

				EditorGUILayout.HelpBox($"This {this.AssetKind} has unsaved changes. Git only sees the file on disk, so save the {this.AssetKind} to include them.", MessageType.Warning);
				if (GUILayout.Button($"Save {this.AssetKind} and refresh")) {
					SaveAsset();
					Refresh();
				}
			}

			private static void SaveAsset()
			{
				PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
				if (prefabStage == null) {
					EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
					return;
				}

				PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, prefabStage.assetPath);
				prefabStage.ClearDirtiness();
			}

			private void DrawChangeList()
			{
				EditorGUILayout.LabelField("Changes", EditorStyles.boldLabel);
				DrawProgress();

				if (_hideReviewed && _reviewedCount == _changes.Count) {
					EditorGUILayout.HelpBox($"Everything in this {this.AssetKind} has been reviewed.", MessageType.Info);
					return;
				}

				using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition)) {
					_scrollPosition = scrollView.scrollPosition;

					foreach (SceneChange change in _changes) {
						if (_hideReviewed && change.IsReviewed) continue;

						DrawChange(change);
					}
				}
			}

			private void DrawChange(SceneChange change)
			{
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
					using (new EditorGUILayout.HorizontalScope()) {
						DrawReviewToggle(change);

						using (new DimScope(change.IsReviewed)) {
							EditorGUILayout.LabelField(GetChangeTag(change.Kind), GetTagStyle(change.Kind), GUILayout.Width(70f));
							EditorGUILayout.LabelField(new GUIContent(change.Name, change.HierarchyPath), EditorStyles.boldLabel);

							using (new EditorGUI.DisabledScope(change.Target == null)) {
								if (GUILayout.Button("Ping", EditorStyles.miniButtonLeft, GUILayout.Width(40f)))
									EditorGUIUtility.PingObject(change.Target);

								if (GUILayout.Button("Select", EditorStyles.miniButtonRight, GUILayout.Width(50f)))
									Selection.activeGameObject = change.Target;
							}
						}
					}

					using (new DimScope(change.IsReviewed))
						EditorGUILayout.LabelField(BuildDescription(change, this.AssetKind), EditorStyles.miniLabel);
				}
			}

			private void DrawReviewToggle(SceneChange change)
			{
				GUIContent content = new GUIContent(string.Empty, change.IsReviewed
					? "Reviewed. Unticks itself if this object changes again."
					: "Tick once you have checked this object.");

				// GUILayout.Toggle rather than EditorGUILayout.Toggle: the latter reserves the whole
				// prefix-label width even for an empty label, which squeezes the box out of view.
				bool isReviewed = GUILayout.Toggle(change.IsReviewed, content, EditorStyles.toggle, GUILayout.Width(16f));
				if (isReviewed == change.IsReviewed) return;

				change.IsReviewed = isReviewed;
				change.WentStale = false;
				RecountReviewed();
				SaveReviewState();
			}

			private static GUIStyle GetTagStyle(EChangeKind kind)
			{
				if (_tagStyles == null) {
					_tagStyles = new GUIStyle[3];

					for (int index = 0; index < _tagStyles.Length; index++) {
						_tagStyles[index] = new GUIStyle(EditorStyles.miniLabel) { fontStyle = FontStyle.Bold };
						_tagStyles[index].normal.textColor = GetChangeColor((EChangeKind)index);
					}
				}

				return _tagStyles[(int)kind];
			}

			private void DrawFooter()
			{
				int added = 0;
				int modified = 0;
				int removed = 0;

				foreach (SceneChange change in _changes) {
					if (change.Kind == EChangeKind.Added) added++;
					else if (change.Kind == EChangeKind.Removed) removed++;
					else modified++;
				}

				EditorGUILayout.LabelField($"{added} added  |  {modified} modified  |  {removed} removed", EditorStyles.miniLabel);
			}

			private void SelectAll()
			{
				List<UnityObject> targets = new List<UnityObject>();

				foreach (SceneChange change in _changes) {
					if (change.Target != null)
						targets.Add(change.Target);
				}

				Selection.objects = targets.ToArray();
			}

			private static string GetChangeTag(EChangeKind kind)
			{
				switch (kind) {
					case EChangeKind.Added:   return "ADDED";
					case EChangeKind.Removed: return "REMOVED";
					default:                  return "MODIFIED";
				}
			}

			private static Color GetChangeColor(EChangeKind kind)
			{
				switch (kind) {
					case EChangeKind.Added:   return new Color(0.35f, 0.75f, 0.35f);
					case EChangeKind.Removed: return new Color(0.85f, 0.35f, 0.35f);
					default:                  return new Color(0.85f, 0.70f, 0.30f);
				}
			}

			private static string BuildDescription(SceneChange change, string assetKind)
			{
				StringBuilder builder = new StringBuilder();

				if (change.WentStale)
					builder.Append("changed since you reviewed it  -  ");

				if (!string.IsNullOrEmpty(change.HierarchyPath)) {
					builder.Append(change.HierarchyPath);
					builder.Append("  -  ");
				}
				else if (change.Kind == EChangeKind.Removed) {
					builder.Append($"not in the {assetKind} anymore  -  ");
				}

				int listedCount = Mathf.Min(change.ChangedDocuments.Count, MaxListedParts);
				for (int index = 0; index < listedCount; index++) {
					if (index > 0) builder.Append(", ");
					builder.Append(change.ChangedDocuments[index].TypeName);
				}

				int remaining = change.ChangedDocuments.Count - listedCount;
				if (remaining > 0)
					builder.Append($" (+{remaining} more)");

				return builder.ToString();
			}

		#endregion


		#region Privates - Scanning

			private void Refresh()
			{
				_changes.Clear();
				_message = "";
				_messageType = MessageType.Info;
				_hasScanned = false;
				_reviewedCount = 0;

				// A prefab open in Prefab Mode takes over from the scene behind it, as the Hierarchy does
				PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
				_isPrefab = (prefabStage != null);

				Scene scene = _isPrefab ? prefabStage.scene : SceneManager.GetActiveScene();
				string assetPath = _isPrefab ? prefabStage.assetPath : scene.path;
				string displayName = _isPrefab ? prefabStage.prefabContentsRoot.name : scene.name;
				_assetName = displayName;
				_assetFileName = "";
				_assetGuid = "";
				_assetIsDirty = scene.isDirty;

				if (!scene.IsValid() || string.IsNullOrEmpty(assetPath)) {
					SetMessage($"The active {this.AssetKind} has never been saved to disk, so there is nothing to compare it against.", MessageType.Warning);
					return;
				}

				string projectRoot = Directory.GetParent(Application.dataPath).FullName;
				string absoluteAssetPath = Path.GetFullPath(Path.Combine(projectRoot, assetPath));

				if (!File.Exists(absoluteAssetPath)) {
					SetMessage($"Could not find the {this.AssetKind} file on disk at '{absoluteAssetPath}'.", MessageType.Error);
					return;
				}

				string assetDirectory = Path.GetDirectoryName(absoluteAssetPath);
				_assetFileName = Path.GetFileName(absoluteAssetPath);
				_assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
				_assetName = $"{displayName}  ({assetPath})";

				// Git is run from the asset's own folder, so this also works when the Unity project is a
				// subfolder of the repository, or when the asset lives inside a submodule.
				if (!TryRunGit(assetDirectory, $"status --porcelain -- \"{_assetFileName}\"", out string statusOutput, out string statusError)) {
					SetMessage(statusError, MessageType.Error);
					return;
				}

				_hasScanned = true;

				if (string.IsNullOrWhiteSpace(statusOutput)) {
					// Nothing left to review in this asset, so the stored ticks are spent.
					ClearReviewState();
					return;
				}

				string statusCode = statusOutput.Length >= 2 ? statusOutput.Substring(0, 2) : statusOutput;
				if (statusCode.Contains("?") || statusCode.Contains("A")) {
					SetMessage($"'{_assetFileName}' has never been committed, so every object in it is new.", MessageType.Warning);
					return;
				}

				if (!TryRunGit(assetDirectory, $"show \"HEAD:./{_assetFileName}\"", out string committedText, out string showError)) {
					SetMessage(showError, MessageType.Error);
					return;
				}

				try {
					Dictionary<long, GameObject> gameObjectsByFileId = new Dictionary<long, GameObject>();
					Dictionary<long, GameObject> gameObjectsByPrefabInstanceId = new Dictionary<long, GameObject>();
					Transform hierarchyRoot = null;

					if (_isPrefab) {
						BuildPrefabIndex(assetPath, prefabStage.prefabContentsRoot, gameObjectsByFileId, gameObjectsByPrefabInstanceId);
						hierarchyRoot = prefabStage.prefabContentsRoot.transform.parent;	// Leaves out the editing environment, if any
					}
					else {
						BuildSceneIndex(scene, gameObjectsByFileId, gameObjectsByPrefabInstanceId);
					}

					BuildChanges(committedText, File.ReadAllText(absoluteAssetPath), gameObjectsByFileId, gameObjectsByPrefabInstanceId, hierarchyRoot);
				}
				catch (Exception exception) {
					HLogger.LogException(exception, typeof(SceneDiffTool));
					SetMessage($"Could not diff the {this.AssetKind}: {exception.Message}", MessageType.Error);
				}
			}

			private void BuildChanges(string committedText, string workingText, Dictionary<long, GameObject> gameObjectsByFileId, Dictionary<long, GameObject> gameObjectsByPrefabInstanceId, Transform hierarchyRoot)
			{
				Dictionary<long, Document> committed = ParseDocuments(committedText);
				Dictionary<long, Document> working = ParseDocuments(workingText);

				Dictionary<long, SceneChange> changesByOwner = new Dictionary<long, SceneChange>();

				foreach (KeyValuePair<long, Document> pair in working) {
					if (!committed.TryGetValue(pair.Key, out Document previous))
						Accumulate(changesByOwner, working, pair.Key, EChangeKind.Added);
					else if (!string.Equals(previous.Body, pair.Value.Body, StringComparison.Ordinal))
						Accumulate(changesByOwner, working, pair.Key, EChangeKind.Modified);
				}

				foreach (KeyValuePair<long, Document> pair in committed) {
					if (!working.ContainsKey(pair.Key))
						Accumulate(changesByOwner, committed, pair.Key, EChangeKind.Removed);
				}

				Dictionary<long, string> reviewState = LoadReviewState();

				foreach (SceneChange change in changesByOwner.Values) {
					ResolveTarget(change, gameObjectsByFileId, gameObjectsByPrefabInstanceId, hierarchyRoot);
					ResolveName(change, working, committed, _isPrefab ? "Prefab settings" : "Scene settings");

					// Sorting first keeps both the hash and the listed types stable across refreshes,
					// since the documents are walked in dictionary order.
					change.ChangedDocuments.Sort((left, right) => left.FileId.CompareTo(right.FileId));
					change.ContentHash = ComputeContentHash(change, working, committed);

					if (reviewState.TryGetValue(change.FileId, out string reviewedHash)) {
						if (string.Equals(reviewedHash, change.ContentHash, StringComparison.Ordinal))
							change.IsReviewed = true;
						else
							change.WentStale = true;
					}

					_changes.Add(change);
				}

				_changes.Sort(CompareChanges);

				RecountReviewed();
				SaveReviewState();
			}

			/// <summary>
			/// Attributes a single changed YAML document to the scene entry that owns it: components go
			/// to their GameObject, stripped objects go to their prefab instance, and anything that is
			/// not tied to an object (render settings, scene roots, ...) goes to a scene-wide entry.
			/// </summary>
			private static void Accumulate(Dictionary<long, SceneChange> changesByOwner, Dictionary<long, Document> documents, long fileId, EChangeKind kind)
			{
				long ownerId = ResolveOwner(documents, fileId, out bool isPrefabInstance, out bool isSceneWide);

				if (!changesByOwner.TryGetValue(ownerId, out SceneChange change)) {
					change = new SceneChange {
						FileId = ownerId,
						IsPrefabInstance = isPrefabInstance,
						IsSceneWide = isSceneWide,
						Kind = EChangeKind.Modified
					};
					changesByOwner.Add(ownerId, change);
				}

				// The owner's own document is the one that decides whether it was added or removed;
				// a changed component only ever means "this object was modified".
				if (ownerId == fileId)
					change.Kind = kind;

				if (documents.TryGetValue(fileId, out Document document))
					change.ChangedDocuments.Add(new ChangedDocument { FileId = fileId, TypeName = document.TypeName });
			}

			private static long ResolveOwner(Dictionary<long, Document> documents, long fileId, out bool isPrefabInstance, out bool isSceneWide)
			{
				isPrefabInstance = false;
				isSceneWide = false;

				long currentId = fileId;

				// Components point at their GameObject and stripped objects point at their prefab
				// instance, so at most a couple of hops are ever needed. The counter is only there to
				// keep a malformed file from looping forever.
				for (int hop = 0; hop < 4; hop++) {
					if (!documents.TryGetValue(currentId, out Document document))
						return currentId;

					if (document.ClassId == PrefabInstanceClassId) {
						isPrefabInstance = true;
						return currentId;
					}

					if (document.ClassId == GameObjectClassId) {
						if (TryGetFileIdField(PrefabInstanceFieldRegex, document.Body, out long prefabInstanceId)) {
							currentId = prefabInstanceId;
							continue;
						}
						return currentId;
					}

					if (TryGetFileIdField(GameObjectFieldRegex, document.Body, out long gameObjectId)) {
						currentId = gameObjectId;
						continue;
					}

					isSceneWide = true;
					return SceneWideFileId;
				}

				return currentId;
			}

			private static void ResolveTarget(SceneChange change, Dictionary<long, GameObject> gameObjectsByFileId, Dictionary<long, GameObject> gameObjectsByPrefabInstanceId, Transform hierarchyRoot)
			{
				if (change.IsSceneWide) return;

				Dictionary<long, GameObject> lookup = change.IsPrefabInstance ? gameObjectsByPrefabInstanceId : gameObjectsByFileId;
				if (!lookup.TryGetValue(change.FileId, out GameObject target) || target == null) return;

				change.Target = target;
				change.HierarchyPath = GetHierarchyPath(target, hierarchyRoot);
			}

			private static void ResolveName(SceneChange change, Dictionary<long, Document> working, Dictionary<long, Document> committed, string assetWideName)
			{
				if (change.IsSceneWide) {
					change.Name = assetWideName;
					return;
				}

				if (change.Target != null) {
					change.Name = change.Target.name;
					return;
				}

				if (!working.TryGetValue(change.FileId, out Document document))
					committed.TryGetValue(change.FileId, out document);

				change.Name = ReadDocumentName(document, change.FileId);
			}

			private static string ReadDocumentName(Document document, long fileId)
			{
				if (document == null)
					return $"<unknown object {fileId}>";

				if (document.ClassId == GameObjectClassId) {
					Match nameMatch = NameFieldRegex.Match(document.Body);
					if (nameMatch.Success)
						return UnquoteYaml(nameMatch.Groups[1].Value);
				}

				if (document.ClassId == PrefabInstanceClassId) {
					Match modificationMatch = NameModificationRegex.Match(document.Body);
					if (modificationMatch.Success) {
						string modifiedName = UnquoteYaml(modificationMatch.Groups[1].Value);
						if (!string.IsNullOrEmpty(modifiedName))
							return modifiedName;
					}

					Match sourceMatch = SourcePrefabRegex.Match(document.Body);
					if (sourceMatch.Success) {
						string assetPath = AssetDatabase.GUIDToAssetPath(sourceMatch.Groups[1].Value);
						if (!string.IsNullOrEmpty(assetPath))
							return Path.GetFileNameWithoutExtension(assetPath);
					}
				}

				return $"<{document.TypeName} {fileId}>";
			}

			private static int CompareChanges(SceneChange left, SceneChange right)
			{
				if (left.IsSceneWide != right.IsSceneWide)
					return left.IsSceneWide ? 1 : -1;

				if (left.Kind != right.Kind)
					return left.Kind.CompareTo(right.Kind);

				return string.Compare(left.Name, right.Name, StringComparison.OrdinalIgnoreCase);
			}

		#endregion


		#region Privates - Review state

			/// <summary>
			/// Reviewed entries are remembered together with a hash of the documents that were reviewed,
			/// so that a tick clears itself as soon as that object changes again.
			/// </summary>
			private Dictionary<long, string> LoadReviewState()
			{
				Dictionary<long, string> reviewState = new Dictionary<long, string>();
				if (string.IsNullOrEmpty(_assetGuid)) return reviewState;

				string raw = EditorPrefs.GetString(ReviewedPrefsKeyPrefix + _assetGuid, "");
				if (raw.Length == 0) return reviewState;

				foreach (string entry in raw.Split(';')) {
					int separator = entry.IndexOf(':');
					if (separator <= 0) continue;

					if (long.TryParse(entry.Substring(0, separator), NumberStyles.Integer, CultureInfo.InvariantCulture, out long fileId))
						reviewState[fileId] = entry.Substring(separator + 1);
				}

				return reviewState;
			}

			private void SaveReviewState()
			{
				if (string.IsNullOrEmpty(_assetGuid)) return;

				StringBuilder builder = new StringBuilder();
				foreach (SceneChange change in _changes) {
					if (!change.IsReviewed) continue;

					if (builder.Length > 0) builder.Append(';');
					builder.Append(change.FileId.ToString(CultureInfo.InvariantCulture));
					builder.Append(':');
					builder.Append(change.ContentHash);
				}

				string key = ReviewedPrefsKeyPrefix + _assetGuid;
				if (builder.Length == 0) EditorPrefs.DeleteKey(key);
				else EditorPrefs.SetString(key, builder.ToString());
			}

			private void ClearReviewState()
			{
				if (string.IsNullOrEmpty(_assetGuid)) return;

				EditorPrefs.DeleteKey(ReviewedPrefsKeyPrefix + _assetGuid);
			}

			private void ResetReviewState()
			{
				foreach (SceneChange change in _changes) {
					change.IsReviewed = false;
					change.WentStale = false;
				}

				_reviewedCount = 0;
				ClearReviewState();
			}

			private void RecountReviewed()
			{
				_reviewedCount = 0;

				foreach (SceneChange change in _changes) {
					if (change.IsReviewed) _reviewedCount++;
				}
			}

			/// <summary>
			/// FNV-1a over the entry's changed documents. Deterministic across sessions, unlike
			/// string.GetHashCode, which is what makes it safe to persist.
			/// </summary>
			private static string ComputeContentHash(SceneChange change, Dictionary<long, Document> working, Dictionary<long, Document> committed)
			{
				ulong hash = HashOffsetBasis;

				foreach (ChangedDocument changedDocument in change.ChangedDocuments) {
					if (!working.TryGetValue(changedDocument.FileId, out Document document))
						committed.TryGetValue(changedDocument.FileId, out document);

					FoldIntoHash(ref hash, changedDocument.FileId.ToString(CultureInfo.InvariantCulture));
					FoldIntoHash(ref hash, document == null ? "<removed>" : document.Body);
				}

				return hash.ToString("x16", CultureInfo.InvariantCulture);
			}

			private static void FoldIntoHash(ref ulong hash, string value)
			{
				for (int index = 0; index < value.Length; index++) {
					hash ^= value[index];
					hash *= HashPrime;
				}

				hash ^= '\n';
				hash *= HashPrime;
			}

		#endregion


		#region Privates - Scene index

			/// <summary>
			/// Maps every GameObject currently in the scene to the file IDs it is serialized under, so a
			/// changed YAML document can be turned back into something the user can click on.
			/// </summary>
			private static void BuildSceneIndex(Scene scene, Dictionary<long, GameObject> gameObjectsByFileId, Dictionary<long, GameObject> gameObjectsByPrefabInstanceId)
			{
				List<GameObject> gameObjects = new List<GameObject>();

				foreach (GameObject root in scene.GetRootGameObjects()) {
					foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
						gameObjects.Add(transform.gameObject);
				}

				if (gameObjects.Count == 0) return;

				GameObject[] gameObjectArray = gameObjects.ToArray();
				GlobalObjectId[] globalIds = new GlobalObjectId[gameObjectArray.Length];
				GlobalObjectId.GetGlobalObjectIdsSlow(gameObjectArray, globalIds);

				for (int index = 0; index < gameObjectArray.Length; index++) {
					long targetObjectId = unchecked((long)globalIds[index].targetObjectId);
					long targetPrefabId = unchecked((long)globalIds[index].targetPrefabId);

					if (targetPrefabId != 0L) {
						AddPrefabInstance(gameObjectsByPrefabInstanceId, targetPrefabId, gameObjectArray[index]);
						continue;
					}

					gameObjectsByFileId[targetObjectId] = gameObjectArray[index];
				}

				// Prefab instance roots are also indexed through their PrefabInstance object, since that
				// is the document the scene file actually stores their overrides in.
				foreach (GameObject gameObject in gameObjectArray) {
					if (!PrefabUtility.IsAnyPrefabInstanceRoot(gameObject)) continue;

					UnityObject handle = PrefabUtility.GetPrefabInstanceHandle(gameObject);
					if (handle == null) continue;

					long handleId = unchecked((long)GlobalObjectId.GetGlobalObjectIdSlow(handle).targetObjectId);
					AddPrefabInstance(gameObjectsByPrefabInstanceId, handleId, gameObject);
				}
			}

			private static void AddPrefabInstance(Dictionary<long, GameObject> gameObjectsByPrefabInstanceId, long prefabInstanceId, GameObject gameObject)
			{
				if (prefabInstanceId == 0L) return;

				GameObject instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);
				if (instanceRoot == null) instanceRoot = gameObject;

				gameObjectsByPrefabInstanceId[prefabInstanceId] = instanceRoot;
			}

			/// <summary>
			/// Prefab Mode edits a copy of the prefab whose objects don't carry the asset's file IDs, so
			/// the IDs are read from the asset itself, and each asset object is then matched to its copy
			/// in the stage by hierarchy position. Objects with no copy (unsaved structural changes) are
			/// pointed at the asset's own object instead.
			/// </summary>
			private static void BuildPrefabIndex(string assetPath, GameObject contentsRoot, Dictionary<long, GameObject> gameObjectsByFileId, Dictionary<long, GameObject> gameObjectsByPrefabInstanceId)
			{
				GameObject assetRoot = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
				if (assetRoot == null) return;

				foreach (Transform assetTransform in assetRoot.GetComponentsInChildren<Transform>(true)) {
					GameObject assetObject = assetTransform.gameObject;

					// Objects coming from a nested prefab (or from the base of a variant) are only stored
					// through the outermost PrefabInstance document of this file.
					if (PrefabUtility.IsPartOfPrefabInstance(assetObject)) {
						GameObject instanceRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(assetObject);
						UnityObject handle = (instanceRoot != null) ? PrefabUtility.GetPrefabInstanceHandle(instanceRoot) : null;
						if ((handle != null) && TryGetLocalFileId(handle, out long prefabInstanceId))
							gameObjectsByPrefabInstanceId[prefabInstanceId] = FindStageCounterpart(instanceRoot.transform, assetRoot.transform, contentsRoot);
						continue;
					}

					if (TryGetLocalFileId(assetObject, out long fileId))
						gameObjectsByFileId[fileId] = FindStageCounterpart(assetTransform, assetRoot.transform, contentsRoot);
				}
			}

			private static GameObject FindStageCounterpart(Transform assetTransform, Transform assetRoot, GameObject contentsRoot)
			{
				List<int> siblingIndices = new List<int>();
				for (Transform current = assetTransform; current != assetRoot; current = current.parent)
					siblingIndices.Add(current.GetSiblingIndex());

				Transform counterpart = contentsRoot.transform;
				for (int index = siblingIndices.Count - 1; index >= 0; index--) {
					if (siblingIndices[index] >= counterpart.childCount)
						return assetTransform.gameObject;
					counterpart = counterpart.GetChild(siblingIndices[index]);
				}

				return (counterpart.name == assetTransform.name) ? counterpart.gameObject : assetTransform.gameObject;
			}

			private static bool TryGetLocalFileId(UnityObject unityObject, out long fileId)
			{
				fileId = 0L;
				return (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(unityObject, out string _, out fileId) && (fileId != 0L));
			}

			private static string GetHierarchyPath(GameObject gameObject, Transform hierarchyRoot)
			{
				StringBuilder builder = new StringBuilder(gameObject.name);

				Transform current = gameObject.transform.parent;
				while ((current != null) && (current != hierarchyRoot)) {
					builder.Insert(0, '/').Insert(0, current.name);
					current = current.parent;
				}

				return builder.ToString();
			}

		#endregion


		#region Privates - YAML

			/// <summary>
			/// Splits a Unity YAML file into its documents, keyed by file ID. Every document starts with
			/// a "--- !u![class id] &amp;[file id]" header, optionally followed by "stripped".
			/// </summary>
			private static Dictionary<long, Document> ParseDocuments(string yaml)
			{
				Dictionary<long, Document> documents = new Dictionary<long, Document>();
				if (string.IsNullOrEmpty(yaml)) return documents;

				Document current = null;
				long currentFileId = 0L;
				StringBuilder body = new StringBuilder();

				foreach (string rawLine in yaml.Split('\n')) {
					string line = rawLine.TrimEnd('\r');

					if (line.StartsWith("--- !u!", StringComparison.Ordinal)) {
						Match header = DocumentHeaderRegex.Match(line);
						if (header.Success) {
							Flush(documents, current, currentFileId, body);

							current = new Document { ClassId = ParseInt(header.Groups[1].Value) };
							currentFileId = ParseLong(header.Groups[2].Value);
							body.Clear();
							continue;
						}
					}

					if (current == null) continue;

					// The first line of a document is its serialized type ("GameObject:", "Transform:", ...).
					if (current.TypeName.Length == 0 && line.Trim().Length > 0)
						current.TypeName = line.Trim().TrimEnd(':');

					body.Append(line).Append('\n');
				}

				Flush(documents, current, currentFileId, body);
				return documents;
			}

			private static void Flush(Dictionary<long, Document> documents, Document document, long fileId, StringBuilder body)
			{
				if (document == null) return;

				document.Body = body.ToString();
				documents[fileId] = document;
			}

			private static bool TryGetFileIdField(Regex regex, string body, out long fileId)
			{
				fileId = 0L;

				Match match = regex.Match(body);
				if (!match.Success) return false;

				fileId = ParseLong(match.Groups[1].Value);
				return (fileId != 0L);
			}

			private static string UnquoteYaml(string value)
			{
				string trimmed = value.Trim();
				if (trimmed.Length < 2) return trimmed;

				if (trimmed[0] == '\'' && trimmed[trimmed.Length - 1] == '\'')
					return trimmed.Substring(1, trimmed.Length - 2).Replace("''", "'");

				if (trimmed[0] == '"' && trimmed[trimmed.Length - 1] == '"')
					return trimmed.Substring(1, trimmed.Length - 2).Replace("\\\"", "\"").Replace("\\\\", "\\");

				return trimmed;
			}

			private static int ParseInt(string value) => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : 0;

			private static long ParseLong(string value) => long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long result) ? result : 0L;

		#endregion


		#region Privates - Git

			private static bool TryRunGit(string workingDirectory, string arguments, out string standardOutput, out string error)
			{
				standardOutput = "";
				error = "";

				ProcessStartInfo startInfo = new ProcessStartInfo("git", arguments) {
					WorkingDirectory = workingDirectory,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true,
					StandardOutputEncoding = Encoding.UTF8,
					StandardErrorEncoding = Encoding.UTF8
				};

				try {
					using (Process process = Process.Start(startInfo)) {
						if (process == null) {
							error = "Could not start 'git'. Is Git installed and available on the PATH?";
							return false;
						}

						// Standard error is drained on its own thread: a whole scene file goes through
						// standard output, and reading the two streams one after the other would risk
						// deadlocking on a full pipe.
						StringBuilder errorBuilder = new StringBuilder();
						process.ErrorDataReceived += (sender, eventArguments) => {
							if (eventArguments.Data != null) errorBuilder.AppendLine(eventArguments.Data);
						};
						process.BeginErrorReadLine();

						standardOutput = process.StandardOutput.ReadToEnd();
						process.WaitForExit();

						if (process.ExitCode == 0) return true;

						error = errorBuilder.ToString().Trim();
						if (error.Length == 0)
							error = $"'git {arguments}' failed with exit code {process.ExitCode}.";

						return false;
					}
				}
				catch (Exception exception) {
					error = $"Could not run 'git' ({exception.Message}). Is Git installed and available on the PATH?";
					return false;
				}
			}

		#endregion


		#region Privates - Miscellany

			private void SetMessage(string message, MessageType messageType)
			{
				_message = message;
				_messageType = messageType;
			}

		#endregion


		#region Properties

			private string AssetKind => (_isPrefab ? "prefab" : "scene");

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