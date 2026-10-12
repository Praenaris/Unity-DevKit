using System.Collections.Generic;
using System.Linq;
using DragonResonance.Attributes;
using DragonResonance.Behaviours;
using DragonResonance.Extensions;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;




namespace DragonResonance.GUI
{
	public class Spinbox : Selectable, ICanvasElement
	{
		[SerializeField] private Button _previousButton = null;
		[SerializeField] private Button _nextButton = null;
		[SerializeField] private TMP_Text _labelText = null;
		[SerializeField] private int _selectedIndex = 0;
		[SerializeField] private string _emptyValue = "-";
		[SerializeField] private List<string> _options = new();
		[SerializeField] private bool _loop = false; // EKAIN FIX for looping languages


		public UnityEvent<int> OnValueChanged = null;




		#region Events


			#if UNITY_EDITOR
			protected override void OnValidate()
			{
				base.OnValidate();
				SpinTo(_selectedIndex);
			}
			#endif


			public override void OnMove(AxisEventData eventData)
			{
				if (!IsActive() || !IsInteractable()) {
					base.OnMove(eventData);
					return;
				}

				switch (eventData.moveDir) {
					case MoveDirection.Left:
						PressSpinPreviousButton();
						break;
					case MoveDirection.Right:
						PressSpinNextButton();
						break;
					case MoveDirection.Up:
						base.OnMove(eventData);
						break;
					case MoveDirection.Down:
						base.OnMove(eventData);
						break;
				}
			}


		#endregion




		#region Publics


			public void UpdateSpin() => SpinTo(_selectedIndex);
			public void SpinPrevious() => SpinTo(_selectedIndex - 1);
			public void SpinNext() => SpinTo(_selectedIndex + 1);

			public void SpinTo(string value)
			{
				int index = _options.IndexOf(value);
				if (index.IsPositiveOrZero())
					SpinTo(index);
			}

			public void SpinTo(int index)
			{
				int currentSelectedIndex = _selectedIndex;
				// START EKAIN FIX for looping languages
				// _selectedIndex = _options.IsEmpty() ? -1 : index.Clamp(0, (_options.Count - 1));
				if (_options.IsEmpty()) // EKAIN FIX looping languages
					_selectedIndex = -1;
				else if (_loop)
					_selectedIndex = (((index % _options.Count) + _options.Count) % _options.Count);
				else
					_selectedIndex = index.Clamp(0, (_options.Count - 1));
				// END EKAIN FIX for looping languages
				if (_selectedIndex != currentSelectedIndex)
					OnValueChanged?.Invoke(_selectedIndex);

				UpdateControls();
			}


		#endregion




		#region Publics - ICanvasElement


			public virtual void Rebuild(CanvasUpdate executing)
			{
				#if UNITY_EDITOR
					if (executing == CanvasUpdate.Prelayout)
						UpdateSpin();
				#endif
			}

			public virtual void LayoutComplete() { }
			public virtual void GraphicUpdateComplete() { }


		#endregion




		#region Privates


			private void PressSpinPreviousButton() => ExecuteEvents.Execute(_previousButton.gameObject,
				new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
			private void PressSpinNextButton() => ExecuteEvents.Execute(_nextButton.gameObject,
				new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);

			private void UpdateControls()
			{
				_labelText.text = this.SelectedValue;
			}


		#endregion




		#region Properties


			public int SelectedIndex => _selectedIndex;
			public string SelectedValue => !this.Options.IsEmpty() ? this.Options[this.SelectedIndex] : _emptyValue;

			public List<string> Options
			{
				get => _options;
				set {
					_options = value;
					UpdateSpin();
				}
			}


		#endregion
	}
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