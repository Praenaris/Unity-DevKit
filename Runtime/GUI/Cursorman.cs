using DragonResonance.Attributes;
using DragonResonance.Behaviours;
using UnityEngine.InputSystem;
using UnityEngine;


namespace Praenaris.GUI
{
	public class Cursorman : SingletonPossumBehaviour<Cursorman>
	{
		[SerializeField] private Vector2 _hotspot = Vector2.zero;
		[SerializeField] private Texture2D _normalTexture = null;
		[SerializeField] private Texture2D _pressedTexture = null;

		[Header("Autohide")]
		[SerializeField] private bool _autoHide = false;
		[ShowIf(nameof(_autoHide))] [SerializeField] [Min(0f)] private float _autoHideWait = 4f;
		[ShowIf(nameof(_autoHide))] [SerializeField] [Min(0f)] private float _movementThreshold = 0.1f;


		private float _timeSinceLastMove = 0f;


		#region Events

			private void Start()
			{
				_timeSinceLastMove = 0f;
			}

			private void Update()
			{
				RefreshTexture();
				RefreshAutoHide();
			}

		#endregion


		#region Publics

			public void Show()
			{
				_timeSinceLastMove = 0f;	// Just in case it was shown manually
				Cursor.visible = true;
			}

			public void Hide()
			{
				Cursor.visible = false;
			}


			public void Customize(Texture2D texture)
			{
				Cursor.SetCursor(texture, _hotspot, CursorMode.Auto);
			}

		#endregion


		#region Privates

			private void RefreshTexture()
			{
				if (this.LeftButtonWasPressed)
					Customize(_pressedTexture);
				else if (this.LeftButtonWasReleased)
					Customize(_normalTexture);
			}

			private void RefreshAutoHide()
			{
				if (!_autoHide) return;

				if (this.HasMoved || this.AnyButtonActivity) {
					_timeSinceLastMove = 0f;
					if (!this.IsVisible)
						Show();
				}
				else if ((_timeSinceLastMove += Time.unscaledDeltaTime) >= _autoHideWait) {
					if (this.IsVisible)
						Hide();
				}
			}

		#endregion


		#region Properties

			public bool IsVisible => Cursor.visible;

			public Vector2 CurrentMovementDelta => Mouse.current.delta.ReadValue();
			public bool LeftButtonWasPressed => Mouse.current.leftButton.wasPressedThisFrame;
			public bool LeftButtonWasReleased => Mouse.current.leftButton.wasReleasedThisFrame;
			public bool RightButtonWasPressed => Mouse.current.rightButton.wasPressedThisFrame;
			public bool RightButtonWasReleased => Mouse.current.rightButton.wasReleasedThisFrame;
			public bool MiddleButtonWasPressed => Mouse.current.middleButton.wasPressedThisFrame;
			public bool MiddleButtonWasReleased => Mouse.current.middleButton.wasReleasedThisFrame;
			public bool AnyButtonWasPressed => (this.LeftButtonWasPressed || this.RightButtonWasPressed || this.MiddleButtonWasPressed);
			public bool AnyButtonWasReleased => (this.LeftButtonWasReleased || this.RightButtonWasReleased || this.MiddleButtonWasReleased);
			public bool AnyButtonActivity => (this.AnyButtonWasPressed || this.AnyButtonWasReleased);
			public bool HasMoved => this.CurrentMovementDelta.sqrMagnitude > (_movementThreshold * _movementThreshold);

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