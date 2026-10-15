# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repo is

`com.praenaris.devkit` (**DevKit**): a generic, publicly distributed Unity UPM package (Unity 6000.0+, Apache-2.0) of behaviour bases, inspector attributes, extension methods, GUI helpers, event channels, logging and Editor tools. Owned by Praenaris, maintained by David Tabernero M., published on OpenUPM and GitHub (`https://github.com/Praenaris/Unity-DevKit.git`, mirror on GitLab).

- **Keep it generic.** Anyone can install this package, so nothing in it (code, comments, docs, this file) refers to any specific project that uses it: no project names, types, scenes, assets, paths or naming conventions from elsewhere. A helper is added because it is useful in general.
- The repo is usually worked on as an **embedded package**: cloned into the `Packages/` folder of some host Unity project, where Unity compiles it from source and changes are live immediately. Users of the package only get changes once a version is released.
- DevKit has **no package dependencies** in `package.json` (and the README says so). Anything that touches an optional package must be guarded (see Assemblies).
- It is a public API: renaming, moving or changing the signature of a public member breaks every consumer. Treat that as a breaking change (minor version bump, and call it out).


## Branches, versions and releasing

### Local sessions: hands off git

When working locally (on the maintainer's machine, not in a cloud session), **never run anything that changes git state unless explicitly told to**: no `add` / staging, `commit`, `merge`, `pull`, `push`, `rebase`, `reset`, `checkout` / `switch`, `stash`, `tag`, `branch` or `restore`, and no PRs. Only edit files in the working tree and leave the git side to the maintainer. Read-only commands (`status`, `diff`, `log`, `show`, `blame`, `tag --list`) are fine. Permission for one git action covers that action only, not later ones. The rules below describe how commits and releases are done when asked to do them.

### Conventions

- Work happens on `develop`; `master` is the release branch and is never committed to by hand.
- **Every set of changes bumps `package.json` `version`**, as part of the change itself and **without committing** (the maintainer commits; the bump goes into that same commit):
  - **Patch** (`4.6.1 → 4.6.2`): backwards compatible: bugfixes and a few small additions.
  - **Minor** (`4.6.2 → 4.7.0`, patch reset to 0): bigger features (e.g. a new tool or subsystem) or any breaking change (renamed/moved/removed public members, namespace moves, changed signatures or behaviour consumers rely on).
  - **Major**: never; only the maintainer bumps it.
  - One bump per set of changes, not per edit. Compare `package.json` with the committed one (`git diff -- package.json`) and the latest tag (`git tag --sort=-v:refname | head -1`): if the working tree already carries an uncommitted bump, the new changes share it, only raised from patch to minor if they need it. If the version is ahead of the latest tag because of an unpushed commit, ask whether to bump again.
  - Say which bump you made and why in the summary.
- **Pushing is releasing.** `.github/workflows/auto-merge-and-tag.yml` runs on every push to any branch: if no `v<version>` tag exists, it merges the pushed branch into `master`, tags it, and the tag triggers `auto-sign.yml` (signs the package with the Unity UPM CLI and creates a GitHub Release). OpenUPM picks the tag up from there. So, outside cloud sessions (see below), **never push unless asked**, and a push without a version bump releases nothing.
- Commit messages: one imperative sentence, no prefix and no body, often joining two changes with a comma ("Add PlayerPrefs Editor tool, import FloatingBob miscellany script", "Fix LayerMask Drawer, add With...() extensions to vectors"). Verbs in use: Add, Fix, Develop, Rework, Optimize, Change, Create, Update, Remove, Import.

### Cloud sessions

When running in a cloud session (Claude Code on the web, or any session that was handed its own branch):
- **At the start of every cloud session**, before anything else, merge the development branch into the session's branch to be up to date: `git fetch origin develop` then `git merge origin/develop`. Resolve conflicts if any, and report what came in.
- **Stay on the session's own branch.** Work, commit and push freely there.
- **Don't touch any other branch** unless explicitly asked: no checkout, commit, merge, push, rebase, delete or pull request on `develop`, `master` or anyone else's branch, and don't open, update or merge PRs on your own initiative either.
- **Don't bump `package.json` `version` in a cloud session unless asked.** The auto-merge-and-tag workflow runs on pushes to *any* branch, so pushing the session's branch with a new, untagged version would merge it straight into `master` and publish a release. With the version left as merged from `develop` (already tagged), pushing the session branch releases nothing; the bump happens when the work lands on `develop`. Mention in the summary that the bump is still pending.


## Assemblies

| asmdef | Contents | Notes |
| --- | --- | --- |
| `Praenaris.DevKit.asmdef` (root) | `Runtime/**` **and** every `Editor/**` folder except `Editor/Tools/` | `rootNamespace: Praenaris`, `autoReferenced: true`. References `Unity.InputSystem`, `Unity.Netcode.Runtime`, `Unity.TextMeshPro`. |
| `Editor/Tools/Praenaris.DevKit.Editor.Tools.asmdef` | `Editor/Tools/**` (the Editor windows) | Editor-only, `autoReferenced: false`, references DevKit + `UnityEngine.UI`. |

Consequences:
- Because most of `Editor/` compiles into the **runtime** assembly, every file there is wrapped whole in `#if UNITY_EDITOR` (first line, two blank lines, usings… then `#endif` two blank lines after the namespace's closing `}`, and two more before the footer). A new file anywhere under `Editor/` outside `Editor/Tools/` must do the same or player builds break. Files under `Editor/Tools/` are mostly wrapped too, by convention, though their asmdef already makes them Editor-only.
- Optional-package code is gated by the asmdef's `versionDefines`: `UNITY_NGO` (Netcode, e.g. `OpossumBehaviour`, `NetworkPhysicsRigidbody`), `UNITY_URP`, `UNITY_HDRP`. Input System code uses `ENABLE_INPUT_SYSTEM`. New code using an optional package needs such a guard.
- Other defines used in code: `ENABLE_LOGGING` (the whole `Log` API is `[Conditional("ENABLE_LOGGING")]`), `DEMO_BUILD`, `OUTPUT_BUILD_PROPERTIES`, toggled from the Editor's **Build/** menu (`Editor/Building/*Toggler.cs`, on top of `BuildDefines`). `TEST_INTEGRATION` gates the whole `Runtime/Integration/` Steam/EOS façade (namespace `PossumScream.Integration`), which is therefore compiled out by default; `DISABLESTEAMWORKS` / `EOS_DISABLE` gate inside it.
- `Resources/Spinbox.prefab` sits in a `Resources/` folder, so it ships in the build of every project that installs the package.


## Layout

- `Runtime/Behaviours/`: `PossumBehaviour` (MonoBehaviour base, split into partials: core caching helpers `GetComponentIfNull` / `FindComponentIfNull` / `DestroyChildren` and `rectTransform` / `children`; `.Logging` with per-component `_logMask` and inherited `Log` / `Emphasis` / `Warning` / `Error` / `Exception` methods; `.Utilities`). `OpossumBehaviour` is the same for `NetworkBehaviour` (NGO only). Plus `Singleton*` / `PersistentSingleton*` variants, `PanelPossumBehaviour`, `SingletonScriptableObject<T>`. Keep the Possum and Opossum halves in sync when changing one.
- `Runtime/Attributes/` + `Editor/Attributes/`: attribute + drawer pairs (`ReadOnly`, `ShowIf` / `HideIf`, `MinMaxRange`, `Layer`, `Tag`, `AnimatorParameter`, `SpritePreview`, `TypeFilter`, `AutoReference`, `RequireInterface`, and `ADropdownArrayAttributeDrawer` as a base). A new attribute is always a pair: the attribute in `Runtime/Attributes/`, the drawer in `Editor/Attributes/`.
- `Runtime/Extensions/`: ~30 static `*Extensions` classes, one per extended type. Grep here before adding a helper anywhere; extend the matching class instead of creating a new one.
- `Runtime/Events/`: `AEventChannelSO` / `EventChannelSO` family (`Int`, `Float`, `String`, `GameObject`, `Transform`, `MonoBehaviour`, `ScriptableObject`).
- `Runtime/Logging/`: static `Log` (`Info` / `Emphasis` / `Warning` / `Error` / `Exception`, caller name and file stamped via `[CallerMemberName]` / `[CallerFilePath]`), `HLogger`, `ELogLevel`, `ELogSeverity`. Inside a `PossumBehaviour` subclass, bare `Log` resolves to the inherited method, not this class.
- `Runtime/GUI/`: `Panel<T>`, `Spinbox`, `Cursorman`, selectable helpers, autoselection, parallax, responsive layout, spritesheet player.
- `Runtime/Mathematics/` (`MathX`, `ColorX`, `SSemanticVersion`, `TransformPoint`, `Randomizers/` with Mersenne Twister and `UnrepeatedRandom`), `Runtime/Databases/` (CSV/TSV `Sheet`, `HeaderedSheet`), `Runtime/Serializables/` (`SerializableKeyValuePair`, `SerializableItemGroup`), `Runtime/Miscellany/` (slideshows, hierarchy separator, define-symbol based destroyers/disablers, `Version`, …), `Runtime/Debugging/`, `Runtime/Networking/`.
- `Editor/Building/` (define togglers, `BuildPropertiesStamper`, `PreloadedAssets`), `Editor/Tools/` (GUID Finder, Scene Git Changes / `SceneDiffTool` (active scene, or the prefab open in Prefab Mode), PlayerPrefs Editor, EventSystem selection indicator, prefs cleaner, troublesome-scale checker, `BuildingPipeline/`), `Editor/GUI/`, `Editor/Importers/` (TSV importer), `Editor/Debugging/`, `Editor/Miscellany/`, `Editor/Serializables/`.
- Menus: legacy items live under `Tools/Dragon Resonance/` and `Window/Dragon Resonance/`; the newest tool uses `Window/Praenaris/`. New menu items go under `Praenaris`. The define togglers live under `Build/`.


## Namespaces (mid-rename)

The package is moving from the old brand names to `Praenaris.*`:
- Most files are still `DragonResonance.<Folder>` (`DragonResonance.Extensions`, `.Behaviours`, `.GUI`, `.Attributes`, `.Editor.Attributes`, …); `Runtime/Integration/` is `PossumScream.Integration.*`.
- Newer or reworked files use `Praenaris.<Folder>`: `Praenaris.Events`, `Praenaris.Serializables`, `Praenaris.Attributes` (`AutoReference`, `RequireInterface`), `Praenaris.Extensions` (Rigidbody ones), `Praenaris.GUI` (`Cursorman`, `GUISelectOnHover`), `Praenaris.Editor.*`.
- A brand-new file goes into `Praenaris.<Folder>`; an edited file keeps its namespace. Moving an existing file to `Praenaris.*` breaks every consumer's `using`s, so do it only when asked, as its own change with a minor version bump.


## Code style

This package is the reference for the maintainer's C# style. Read a whole file before writing a new one: `Runtime/GUI/Panel.cs`, `Runtime/Miscellany/Slideshow/Slideshow.cs` and `Runtime/Behaviours/PossumBehaviour.cs` are good models. Some recently imported files (e.g. `Spinbox.cs`, `PlayerPrefsEditor.cs`) don't follow it yet; don't copy them.

**Formatting**
- `.editorconfig` (`*.cs`): tabs (width 4), LF, UTF-8, **no** final newline.
- Hybrid braces: Allman for namespace / type / method declarations, K&R for control flow (`if (cond) {`), `else` on a new line after the closing `}`. Single-statement bodies drop the braces and go on the next line, indented; guard clauses stay inline (`if (list.IsEmpty()) return;`).
- Two blank lines between usings and `namespace`, around each `#region`, and between member groups; one blank line between related members.
- **Every dependency list is sorted alphabetically**: `using` directives, asmdef `references`, `package.json` dependencies, and any similar list. For `using`s this is an ordinal sort of the whole lines, which naturally puts a child namespace before its parent (`using System.Linq;` before `using System;`, `using UnityEngine.Events;` before `using UnityEngine;`) and treats `System` like any other name (not first). Aliases (`using UnityObject = UnityEngine.Object;`) sort by alias name. When touching a file or list that isn't sorted, sort it.
- Expression-bodied members whenever they fit; wrap long ones after `=>` onto an indented next line. Parenthesise returned conditionals: `=> ((statement == null) ? GetComponent<T>() : statement);`.
- Inline trailing comments are separated by a tab.

**Structure**
- `#region` blocks in a fixed order: **Constructors → Events → Publics → Inheritables → Privates → Properties** (properties always last). Region contents are indented one extra level. Split a large region with a suffix: `#region Publics - Visibility`, `#region Privates - Git`.
- `Events` holds Unity messages and handlers; `#if UNITY_EDITOR` guards inside a region sit at member indent.

**Naming**
- Private/protected fields `_camelCase`, **always explicitly initialized**, even to the default (`= null`, `= 0`, `= false`, `= new()`).
- Lazy cached backing fields take an `_x_internal` suffix and are read only through their property: `public CanvasGroup CanvasGroup => (_canvasGroup_internal = GetComponentIfNull(_canvasGroup_internal));`. In an `EditorWindow` or other serialized Editor class, mark these caches `[NonSerialized]`, or a domain reload brings them back as empty non-null instances.
- Read own members through `this.` (`this.CanvasGroup`, `this.transform`) and base members through `base.` (`base.transform`, `base.serializedObject`).
- Type prefixes: `A` abstract (`AEventChannelSO`), `E` enum (`ELogLevel`), `S` serializable struct (`SSemanticVersion`, `SBuildConfiguration`), `I` interface.
- **Explicit types, never `var`.**
- `[SerializeField] private T _field = null;` on one line; extra attributes stack on the same line. `[ContextMenu(nameof(Method))]` for methods worth invoking from the Inspector.
- English everywhere.

**File skeleton**: (`#if UNITY_EDITOR` + two blank lines, for Editor files) → usings → two blank lines → `namespace` block → two blank lines → (`#endif` + two blank lines, for Editor files) → the **PRAENARIS Apache copyright notice**, ending with **no newline** after its last line.
- **Every code file carries the PRAENARIS notice**, no exceptions: new files get it, and a file being edited that lacks it or still has another one gets it then. Two older variants exist and must be replaced whenever their file is touched: the `DRAGON RESONANCE` ASCII-art footer ("Copyright © 2021-2026") and, in `Runtime/Networking/NetworkPhysicsRigidbody.cs`, a `Proto√olKers` notice. A file with no notice at all gets one added.
- Copy the notice byte-for-byte from `Runtime/Behaviours/PossumBehaviour.cs` (big `PRAENARIS` ASCII art, "Licensed under the Apache License, Version 2.0", "Copyright © 2026"); never retype it. After writing a file, confirm it ends without a newline (`tail -c 4 file | xxd` must end in `2f`, the notice's closing `*/`).


## Compile-checking without the Editor

There's no CLI build or test suite; the Editor is usually open and holds the project lock. This needs a local Unity install and a host project, so it isn't available in cloud sessions: there, say the change is not compile-checked. Compile the package from source with the Roslyn that ships with Unity instead, after every `.cs` change and before reporting work done (both assemblies build cleanly in a few seconds):

1. From the Unity-generated `Praenaris.DevKit.csproj` in the host project's root (one level above `Packages/`), take every `<Reference><HintPath>` as `-r:` and the full `<DefineConstants>` as `-define:`, converting backslashes to `/` (a short Python script with `xml.etree` does this well; build any literal backslash with `chr(92)`, since Bash halves them).
2. Take the sources from disk, not from the csproj (its file list is stale after files are added outside the Editor): every `.cs` in this repo except `Editor/Tools/**` for `Praenaris.DevKit`; `Editor/Tools/**` for `Praenaris.DevKit.Editor.Tools`, built from `Praenaris.DevKit.Editor.Tools.csproj`'s refs plus the freshly built DevKit DLL.
3. Write a response file with `-langversion:9.0 -nostdlib+ -target:library -out:<scratch>/devkit.dll -nowarn:CS0436,CS0169,CS0414,CS0649,CS0108,CS0219,CS0162,CS0067`, the refs, defines and quoted source paths, then run:
   ```bash
   dotnet "<Unity Editor install>/Editor/Data/DotNetSdkRoslyn/csc.dll" -noconfig @devkit.rsp
   ```
   `-noconfig` must be on the command line (it is ignored inside an rsp, CS2023). Don't add the `MonoBleedingEdge` 4.8-api assemblies (duplicate corlib, CS0518). Put the rsp and output DLLs in the scratchpad, not in the repo.

The csproj only carries the host project's defines, so code behind `UNITY_NGO`, `TEST_INTEGRATION` or `DEMO_BUILD` is only checked if those are added to `-define:` by hand.


## Files Unity cares about

- Every file and folder has a `.meta` committed alongside it. When creating a file outside the Editor, Unity generates its `.meta` on the next refocus; it belongs in the same commit as the file. When moving or renaming, move the `.meta` too so the GUID survives (consumers' scenes and prefabs reference DevKit scripts by GUID).
- Keep `package.json` fields other than `version` stable unless asked (`unity`, `name` and `displayName` affect every consumer).