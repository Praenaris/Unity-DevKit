using UnityEngine;


namespace Praenaris.Extensions
{
	public static class ComponentExtensions
	{
		#region Publics

			public static T GetComponentIfNull<T>(this Component component, T statement) where T : Component =>
				((statement == null) ? component.GetComponent<T>() : statement);
			public static T GetComponentInChildrenIfNull<T>(this Component component, T statement, bool includeInactive = true) where T : Component =>
				((statement == null) ? component.GetComponentInChildren<T>(includeInactive) : statement);
			public static T GetComponentInParentIfNull<T>(this Component component, T statement, bool includeInactive = true) where T : Component =>
				((statement == null) ? component.GetComponentInParent<T>(includeInactive) : statement);

			public static bool CanBeEnabled(this Component component) =>
				(component is Behaviour or Collider or LODGroup or Renderer);

			public static bool TrySetEnabled(this Component component, bool enabled)
			{
				switch (component) {
					case Behaviour behaviour:
						behaviour.enabled = enabled;
						return true;
					case Collider collider:
						collider.enabled = enabled;
						return true;
					case LODGroup lodGroup:
						lodGroup.enabled = enabled;
						return true;
					case Renderer renderer:
						renderer.enabled = enabled;
						return true;
					default:
						return false;
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