using DragonResonance.Behaviours;
using DragonResonance.Extensions;
using Image = UnityEngine.UI.Image;
using UnityEngine.UI;
using UnityEngine;


namespace DragonResonance.GUI
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Image))]
	public class ImageSpritesheetPlayer : PossumBehaviour
	{
		[SerializeField] private bool _playing = true;
		[SerializeField] private bool _loop = true;
		[SerializeField] private float _speed = 4f;
		[SerializeField] private Sprite[] _sprites = { };

		private Image _image_internal = null;	// Caching only, use the property instead
		private float _timeOffset = 0f;


		#region Events

			private void Update() => RefreshSprite();

		#endregion


		#region Publics

			[ContextMenu(nameof(Play))]
			public void Play() => _playing = true;

			[ContextMenu(nameof(Stop))]
			public void Stop() => _playing = false;


			[ContextMenu(nameof(PlayOnce))]
			public void PlayOnce()
			{
				_timeOffset = Time.realtimeSinceStartup;
				_loop = false;
				Play();
			}

			[ContextMenu(nameof(PlayLoop))]
			public void PlayLoop()
			{
				_timeOffset = 0f;
				_loop = true;
				Play();
			}

		#endregion


		#region Privates

			private void RefreshSprite()
			{
				if (!_playing || _sprites.Length.IsZero()) return;

				int spriteIndex = this.CurrentSpriteIndex;
				this.Image.overrideSprite = _sprites[spriteIndex];

				if ((spriteIndex == _sprites.LastIndex()) && !_loop)
					Stop();

				#if UNITY_EDITOR
				this.Image.Rebuild(CanvasUpdate.PreRender);
				#endif
			}

		#endregion


		#region Properties

			public float SpriteDuration => (1 / _speed);
			public float OffsetRealtime => (Time.realtimeSinceStartup - _timeOffset);
			public int SpritesPassed => (int)(this.OffsetRealtime / this.SpriteDuration);
			public int CurrentSpriteIndex => (this.SpritesPassed % _sprites.Length.LowerClamp(1));

			public Image Image => (_image_internal = GetComponentIfNull<Image>(_image_internal));
			public float TimeOffset => _timeOffset;


			public bool Playing
			{
				get => _playing;
				set => _playing = value;
			}

			public bool Loop
			{
				get => _loop;
				set => _loop = value;
			}

		#endregion
	}


	#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(ImageSpritesheetPlayer), true)]
	public class ImageSpriteSheetPlayerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			ImageSpritesheetPlayer imageSpritesheetPlayer = (ImageSpritesheetPlayer)base.target;

			UnityEditor.EditorGUILayout.LabelField("Playing", $"{imageSpritesheetPlayer.Playing}");
			UnityEditor.EditorGUILayout.LabelField("Sprite Duration", $"{imageSpritesheetPlayer.SpriteDuration}");
			UnityEditor.EditorGUILayout.LabelField("Sprites Passed", $"{imageSpritesheetPlayer.SpritesPassed}");
			UnityEditor.EditorGUILayout.LabelField("Time Offset", $"{imageSpritesheetPlayer.TimeOffset}");

			UnityEditor.EditorGUILayout.Separator();
			if (GUILayout.Button(nameof(ImageSpritesheetPlayer.Play)))
				imageSpritesheetPlayer.Play();
			if (GUILayout.Button(nameof(ImageSpritesheetPlayer.Stop)))
				imageSpritesheetPlayer.Stop();

			UnityEditor.EditorGUILayout.Separator();
			if (GUILayout.Button(nameof(ImageSpritesheetPlayer.PlayOnce)))
				imageSpritesheetPlayer.PlayOnce();
			if (GUILayout.Button(nameof(ImageSpritesheetPlayer.PlayLoop)))
				imageSpritesheetPlayer.PlayLoop();
		}
	}
	#endif
}


/*       ________________________________________________________________       */
/*           _________   _______ ________  _______  _______  ___    _           */
/*           |        \ |______/ |______| |  _____ |       | |  \   |           */
/*           |________/ |     \_ |      | |______| |_______| |   \__|           */
/*           ______ _____ _____ _____ __   _ _____ __   _ _____ _____           */
/*           |____/ |____ [___  |   | | \  | |___| | \  | |     |____           */
/*           |    \ |____ ____] |___| |  \_| |   | |  \_| |____ |____           */
/*       ________________________________________________________________       */
/*                                                                              */
/*           David Tabernero M.  <https://github.com/davidtabernerom>           */
/*           Dragon Resonance    <https://github.com/dragonresonance>           */
/*                  Copyright © 2021-2026. All rights reserved.                 */
/*                Licensed under the Apache License, Version 2.0.               */
/*                         See LICENSE.md for more info.                        */
/*       ________________________________________________________________       */
/*                                                                              */