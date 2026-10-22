using DragonResonance.Behaviours;
using DragonResonance.Extensions;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


namespace DragonResonance.GUI
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class SpriteSequencePlayer : PossumBehaviour
	{
		[Header("Configuration")]
		[SerializeField] private bool _playing = true;
		[SerializeField] private bool _loop = true;
		[SerializeField] private float _speed = 4f;
		[SerializeField] private List<Sprite> _sprites = new();

		[Header("Targets")]
		[SerializeField] private Image[] _images = { };
		[SerializeField] private SpriteRenderer[] _spriteRenderers = { };


		private float _timeOffset = 0f;
		private int _shownSpriteIndex = -1;


		#region Events

			private void OnEnable() => ResetIndex();
			private void OnValidate() => ResetIndex();
			private void Update() => RefreshSprite();

		#endregion


		#region Publics

			public void Play() => _playing = true;

			public void Stop() => _playing = false;


			public void PlayOnce()
			{
				_timeOffset = Time.realtimeSinceStartup;
				_loop = false;
				ResetIndex();
				Play();
			}

			public void PlayLoop()
			{
				_timeOffset = 0f;	// Keeps every looping player in sync
				_loop = true;
				ResetIndex();
				Play();
			}

		#endregion


		#region Privates

			private void ResetIndex() => _shownSpriteIndex = -1;

			private void RefreshSprite()
			{
				if (!_playing || _sprites.IsEmpty()) return;

				int spriteIndex = this.CurrentSpriteIndex;
				if (spriteIndex != _shownSpriteIndex) {
					ApplySprite(_sprites[spriteIndex]);
					_shownSpriteIndex = spriteIndex;
				}

				if (!_loop && (spriteIndex == _sprites.LastIndex()))
					Stop();
			}


			private void ApplySprite(Sprite sprite)
			{
				foreach (Image image in _images) {
					image.overrideSprite = sprite;
					#if UNITY_EDITOR
						if (!Application.isPlaying)
							image.Rebuild(CanvasUpdate.PreRender);
					#endif
				}

				foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
					spriteRenderer.sprite = sprite;
			}

		#endregion


		#region Properties

			public float SpriteDuration => (1f / _speed);
			public float OffsetRealtime => (Time.realtimeSinceStartup - _timeOffset);
			public int SpritesPassed => (int)(this.OffsetRealtime * _speed.ClampLowerZero());
			public int CurrentSpriteIndex => (_loop ? (this.SpritesPassed % _sprites.Count.LowerClamp(1)) : this.SpritesPassed.UpperClamp(_sprites.LastIndex()));

			public float TimeOffset => _timeOffset;

			public bool Playing { get => _playing; set => _playing = value; }
			public bool Loop { get => _loop; set => _loop = value; }
			public float Speed { get => _speed; set => _speed = value; }
			public List<Sprite> Sprites => _sprites;

		#endregion
	}


	#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(SpriteSequencePlayer), true)]
	public class ImageSpriteSheetPlayerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			SpriteSequencePlayer spriteSequencePlayer = (SpriteSequencePlayer)base.target;

			UnityEditor.EditorGUILayout.Separator();
			UnityEditor.EditorGUILayout.LabelField("Playing", $"{spriteSequencePlayer.Playing}");
			UnityEditor.EditorGUILayout.LabelField("Sprite Duration", $"{spriteSequencePlayer.SpriteDuration}");
			UnityEditor.EditorGUILayout.LabelField("Sprites Passed", $"{spriteSequencePlayer.SpritesPassed}");
			UnityEditor.EditorGUILayout.LabelField("Time Offset", $"{spriteSequencePlayer.TimeOffset}");

			UnityEditor.EditorGUILayout.Separator();
			if (GUILayout.Button(nameof(SpriteSequencePlayer.Play)))
				spriteSequencePlayer.Play();
			if (GUILayout.Button(nameof(SpriteSequencePlayer.Stop)))
				spriteSequencePlayer.Stop();

			UnityEditor.EditorGUILayout.Separator();
			if (GUILayout.Button(nameof(SpriteSequencePlayer.PlayOnce)))
				spriteSequencePlayer.PlayOnce();
			if (GUILayout.Button(nameof(SpriteSequencePlayer.PlayLoop)))
				spriteSequencePlayer.PlayLoop();
		}
	}
	#endif
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