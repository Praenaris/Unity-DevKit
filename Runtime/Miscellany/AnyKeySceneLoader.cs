#if ENABLE_INPUT_SYSTEM


using DragonResonance.Attributes;
using DragonResonance.Behaviours;
using Praenaris.Attributes;
using System.Collections;
using System;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;


namespace Praenaris.Miscellany
{
	public class AnyKeySceneLoader : PossumBehaviour
	{
		[Scene] [SerializeField] private string _nextScene = null;
		[SerializeField] private bool _loadInBackground = false;
		[ShowIf(nameof(_loadInBackground))] [SerializeField] [Min(0f)] private float _loadingDelay = 1f;
		[ShowIf(nameof(_loadInBackground))] [SerializeField] [Min(0f)] private float _activationDelay = 2f;


		private IDisposable _anyButtonPressListener = null;
		private AsyncOperation _backgroundLoading = null;


		#region Events

			private void OnEnable()
			{
				_anyButtonPressListener = InputSystem.onAnyButtonPress.CallOnce(OnAnyButtonPressed);
				if (_loadInBackground)
					StartCoroutine(StartSceneLoading());
			}

			private void OnDisable()
			{
				_anyButtonPressListener?.Dispose();
			}

			private void OnAnyButtonPressed(InputControl pressedControl) => LoadNextScene();

		#endregion


		#region Publics

			[ContextMenu(nameof(LoadNextScene))]
			public void LoadNextScene()
			{
				if (!_loadInBackground)
					SceneManager.LoadScene(_nextScene, LoadSceneMode.Single);
				else
					StartCoroutine(StartSceneActivation());
			}

		#endregion


		#region Privates

			private IEnumerator StartSceneLoading()
			{
				yield return new WaitForSecondsRealtime(_loadingDelay);
				TryStartBackgroundLoading();
			}

			private IEnumerator StartSceneActivation()
			{
				yield return new WaitForSecondsRealtime(_activationDelay);
				if (TryStartBackgroundLoading())	// Starts it now if the loading delay hasn't passed yet
					_backgroundLoading.allowSceneActivation = true;
			}

			private bool TryStartBackgroundLoading()
			{
				if (_backgroundLoading != null) return true;

				if ((_backgroundLoading = SceneManager.LoadSceneAsync(_nextScene, LoadSceneMode.Single)) != null) {
					_backgroundLoading.allowSceneActivation = false;
					return true;
				}

				return false; // Not in the build
			}

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