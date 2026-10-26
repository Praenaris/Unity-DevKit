#if ENABLE_INPUT_SYSTEM


using DragonResonance.Attributes;
using DragonResonance.Behaviours;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;


namespace Praenaris.GUI
{
	[RequireComponent(typeof(Button))]
	public class ButtonInputShortcut : PossumBehaviour
	{
		[SerializeField] private bool _autoEnableInput = false;
		[SerializeField] private bool _autoDisableInput = false;
		[SerializeField] private bool _invokeEventsDirectly = false;
		[SerializeField] private bool _useInputActionReference = false;
		[HideIf(nameof(_useInputActionReference))] [SerializeField] private InputAction _clickInput = null;
		[ShowIf(nameof(_useInputActionReference))] [SerializeField] private InputActionReference _clickInputReference = null;


		private static int _lastClickFrame = -1;	// One press clicks one button, even if several share the key


		#region Events

			private void OnEnable()
			{
				if (!_useInputActionReference) {
					_clickInput.performed += OnClickInputPerformed;
					if (_autoEnableInput) _clickInput.Enable();
				}
				else {
					_clickInputReference.action.performed += OnClickInputPerformed;
					if (_autoEnableInput) _clickInputReference.action.Enable();
				}
			}

			private void OnDisable()
			{
				if (!_useInputActionReference) {
					if (_autoDisableInput) _clickInput.Disable();
					_clickInput.performed -= OnClickInputPerformed;
				}
				else {
					if (_autoDisableInput) _clickInputReference.action.Disable();
					_clickInputReference.action.performed -= OnClickInputPerformed;
				}
			}


			private void OnClickInputPerformed(InputAction.CallbackContext _)
			{
				Button button = GetComponent<Button>();
				if (!button.IsInteractable() || (Time.frameCount == _lastClickFrame)) return;
				_lastClickFrame = Time.frameCount;

				if (!_invokeEventsDirectly)
					ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
				else
					button.onClick.Invoke();
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