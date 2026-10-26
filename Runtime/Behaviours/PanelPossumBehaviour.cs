using DragonResonance.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace DragonResonance.Behaviours
{
	public abstract class PanelPossumBehaviour<T> : PossumBehaviour where T : PanelPossumBehaviour<T>
	{
		internal static readonly Queue<PanelPossumBehaviour<T>> _instances = new();
		public static event Action<PanelPossumBehaviour<T>> OnInstanced = null;


		/*
		[SerializeField] private bool _visible = true;
		[SerializeField] private bool _visibleOnStart = true;
		[SerializeField] private bool _alsoHandleInteractivity = true;


		private CanvasGroup _canvasGroup_internal = null;	// Caching only, use the property instead

		protected event Action<bool> OnVisibilityChange;
		*/


		#region Events

			protected void Awake()
			{
				AssessInstance();
				LateAwake();
			}

			/*
			protected void Start()
			{
				if (_visibleOnStart)
					Show();
			}
			*/

		#endregion


		#region Publics

			public static bool TryGetInstance(out PanelPossumBehaviour<T> instance) => ((instance = GetInstance()) != null);

			public static PanelPossumBehaviour<T> GetInstance()
			{
				if ((_instances.IsEmpty()) && (FindAnyObjectByType(typeof(T)) is PanelPossumBehaviour<T> instance))
					instance.AssessInstance();
				else
					instance = null;

				return instance;
			}

			/*
			public static T Find<T>() where T : BasicPanel => FindAnyObjectByType<T>();
			public static void Toggle<T>() where T : BasicPanel => Find<T>().Toggle();
			public static void Show<T>() where T : BasicPanel => Find<T>().Show();
			public static void Hide<T>() where T : BasicPanel => Find<T>().Hide();

			[ContextMenu(nameof(Toggle))] public void Toggle() => SetVisibility(!_visible);
			[ContextMenu(nameof(Show))] public void Show() => SetVisibility(true);
			[ContextMenu(nameof(Hide))] public void Hide() => SetVisibility(false);

			public void SetVisibility(bool visible)
			{
				_visible = visible;
				CanvasGroupVisibilityChangeAction.Invoke(this.CanvasGroup, _visible);
				if (_alsoHandleInteractivity)
					this.CanvasGroup.interactable = this.CanvasGroup.blocksRaycasts = _visible;
				OnVisibilityChange?.Invoke(_visible);
			}
			*/

		#endregion


		#region Inheritables

			protected virtual void LateAwake() { }

			protected virtual void InvokeInstantiationEvent() => OnInstanced?.Invoke(this);

			protected virtual void AssessInstance()
			{
				if (!this.Instanced) {
					_instances.Enqueue(this);
					InvokeInstantiationEvent();
				}
				else {
					Error("This instance is already on the instances list. Destroying...");
					Destroy(this);
				}
			}

		#endregion


		/*
		#region Privates

			protected virtual Action<CanvasGroup, bool> CanvasGroupVisibilityChangeAction => (canvasGroup, visible) => { canvasGroup.alpha = visible.AsInt(); };

			protected T SwapTo<T>() where T : BasicPanel
			{
				T target = Find<T>();
				this.Hide();
				target.Show();
				return target;
			}

		#endregion


		#region Properties

			public CanvasGroup CanvasGroup => (_canvasGroup_internal = GetComponentIfNull(_canvasGroup_internal));

		#endregion


		#if UNITY_EDITOR

			private void OnValidate() => SetVisibility(_visible);

		#endif
		*/


		#region Properties

			public IEnumerable<PanelPossumBehaviour<T>> Instances => _instances;
			public bool Instanced => _instances.Contains(this);
			public static PanelPossumBehaviour<T> Current => _instances.Peek();
			public static PanelPossumBehaviour<T> Instance => GetInstance();

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