using DragonResonance.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;


namespace Praenaris.GUI
{
	public class Spinbox : Selectable, ICanvasElement
	{
		[Header("Controls")]
		[SerializeField] private Button _previousButton = null;
		[SerializeField] private Button _nextButton = null;
		[SerializeField] private TMP_Text _labelText = null;

		[Header("Behaviour")]
		[SerializeField] private bool _loop = false;
		[SerializeField] private int _selectedIndex = 0;
		[SerializeField] private string _emptyValue = "-";
		[SerializeField] private List<string> _options = new();


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


		#region Publics

			public void UpdateSpin() => SpinTo(_selectedIndex);

			public void SpinPrevious() => SpinTo((_loop && !_options.IsEmpty()) ? _selectedIndex.PreviousCyclic(_options.Count) : (_selectedIndex - 1));
			public void SpinNext() => SpinTo((_loop && !_options.IsEmpty()) ? _selectedIndex.NextCyclic(_options.Count) : (_selectedIndex + 1));

			public void SpinTo(string value)
			{
				int index = _options.IndexOf(value);
				if (index.IsPositiveOrZero())
					SpinTo(index);
			}

			public void SpinTo(int index)
			{
				int currentSelectedIndex = _selectedIndex;
				_selectedIndex = (!_options.IsEmpty() ? index.Clamp(0, _options.LastIndex()) : -1);
				if (_selectedIndex != currentSelectedIndex)
					OnValueChanged?.Invoke(_selectedIndex);

				UpdateControls();
			}

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