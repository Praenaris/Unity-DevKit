using DragonResonance.Behaviours;
using DragonResonance.Extensions;
using Praenaris.Attributes;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace Praenaris.Miscellany
{
	public abstract class ADefineAction<T> : PossumBehaviour where T : UnityObject
	{
		[SerializeField] private DefineCondition _condition = DefineCondition.AnyDefined;
		[DefineSymbol] [SerializeField] private string[] _symbols = { "DEMO_BUILD" };
		[SerializeField] private T[] _targets = { };


		#region Events

			private void Awake()
			{
				#if UNITY_EDITOR
					WarnUnregisteredSymbols();
				#endif

				if (!this.IsConditionMet) return;
				foreach (T target in _targets)
					if (target != null)
						Perform(target);
			}

			protected virtual void OnValidate()
			{
				for (int symbolIndex = 0; symbolIndex < _symbols.Length; symbolIndex++)
					_symbols[symbolIndex] = (_symbols[symbolIndex]?.Trim() ?? string.Empty);
			}

			protected virtual void Reset() => _targets = this.DefaultTargets;

		#endregion


		#region Inheritables

			protected abstract void Perform(T target);

		#endregion


		#region Privates

		#if UNITY_EDITOR
			private void WarnUnregisteredSymbols()
			{
				string[] compiledSymbols = UnityEditor.EditorUserBuildSettings.activeScriptCompilationDefines;
				foreach (string symbol in _symbols.Where(symbol => compiledSymbols.Contains(symbol) && !DefineSymbols.IsDefined(symbol)))
					Warning($"{symbol} is defined but can't be detected at runtime, so it counts as undefined. Register it with {nameof(DefineSymbols)}.{nameof(DefineSymbols.Register)}.");
			}
		#endif

		#endregion


		#region Properties

			public bool IsConditionMet => (!_symbols.IsEmpty() && _condition switch {
				DefineCondition.AllDefined => _symbols.All(DefineSymbols.IsDefined),
				DefineCondition.NoneDefined => !_symbols.Any(DefineSymbols.IsDefined),
				_ => _symbols.Any(DefineSymbols.IsDefined),
			});

			protected T[] Targets => _targets;
			protected virtual T[] DefaultTargets => new T[] { };

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