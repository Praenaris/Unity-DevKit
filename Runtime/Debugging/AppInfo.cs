using DragonResonance.Behaviours;
using DragonResonance.Miscellany;
using System.Text;
using UnityEngine.Events;
using UnityEngine;


namespace DragonResonance.Debugging
{
	public class AppInfo : PossumBehaviour
	{
		[SerializeField] private UnityEvent<string> _targets = null;


		private readonly StringBuilder _toStringOutput = new();


		#region Events

			private void Start() => Refresh();

		#endregion


		#region Publics

			public void Refresh() => this._targets?.Invoke(ToString());

		#endregion


		#region Properties

			public override string ToString()
			{
				_toStringOutput.Clear();

				_toStringOutput.AppendLine($"AppName:    {Application.productName}");
				_toStringOutput.AppendLine($"ComName:    {Application.companyName}");
				_toStringOutput.AppendLine($"ShortVer:   {Application.version}");
				_toStringOutput.Append(    $"FullVer:    {Version.vFullVersionLower}");

				return _toStringOutput.ToString();
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