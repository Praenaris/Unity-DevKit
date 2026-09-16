using UnityEngine;


namespace Praenaris.Extensions
{
	public static class RigidbodyExtensions
	{
		#region Movement

			public static void Translate(this Rigidbody source, Vector3 translation) =>
				source.MovePosition(source.position + translation);

		#endregion


		#region Synchronization

			public static void SyncMovement(this Rigidbody source, Rigidbody target) =>
				SetMovement(source, target.position, target.rotation, target.linearVelocity, target.angularVelocity);

			public static void SyncPoint(this Rigidbody source, Rigidbody target) =>
				SetPoint(source, target.position, target.rotation);

			public static void SyncVelocity(this Rigidbody source, Rigidbody target) =>
				SetVelocity(source, target.linearVelocity, target.angularVelocity);

		#endregion


		#region Settings

			public static void SetMovement(this Rigidbody source, Pose pose, Vector3 linearVelocity, Vector3 angularVelocity) =>
				SetMovement(source, pose.position, pose.rotation, linearVelocity, angularVelocity);
			public static void SetMovement(this Rigidbody source, Vector3 position, Quaternion rotation, Vector3 linearVelocity, Vector3 angularVelocity)
			{
				SetPoint(source, position, rotation);
				SetVelocity(source, linearVelocity, angularVelocity);
			}

			public static void SetPoint(this Rigidbody source, Pose pose) => SetPoint(source, pose.position, pose.rotation);
			public static void SetPoint(this Rigidbody source, Vector3 position, Quaternion rotation)
			{
				source.position = position;
				source.rotation = rotation;
			}

			public static void SetVelocity(this Rigidbody source, Vector3 linearVelocity, Vector3 angularVelocity)
			{
				source.linearVelocity = linearVelocity;
				source.angularVelocity = angularVelocity;
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