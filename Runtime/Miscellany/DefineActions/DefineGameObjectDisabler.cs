using UnityEngine;


namespace Praenaris.Miscellany
{
	public class DefineGameObjectDisabler : ADefineAction<GameObject>
	{
		[SerializeField] private bool _enableInstead = false;


		#region Events

			protected override void OnValidate()
			{
				base.OnValidate();
				if (!_enableInstead) return;
				foreach (GameObject target in base.Targets)
					if ((target != null) && this.transform.IsChildOf(target.transform))
						Warning($"{target.name} can't be enabled from here: this component sits on it or below it, and won't wake up while it's disabled.");
			}

		#endregion


		#region Inheritables

			protected override void Perform(GameObject target) => target.SetActive(_enableInstead);

		#endregion


		#region Properties

			protected override GameObject[] DefaultTargets => new[] { base.gameObject };

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