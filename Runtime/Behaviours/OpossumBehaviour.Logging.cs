#if UNITY_NGO


using DragonResonance.Logging;
using System.Runtime.CompilerServices;
using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace DragonResonance.Behaviours
{
	public abstract partial class OpossumBehaviour	// Logging
	{
		[UnityEngine.Serialization.FormerlySerializedAs("_loggingMask")] [SerializeField] private ELogLevel _logMask = ELogLevel.Info | ELogLevel.Emphasis | ELogLevel.Warning | ELogLevel.Error | ELogLevel.Exception;


		#region Inheritables

			protected void Log(string message, [CallerMemberName] string callerName = null) => Log(message, this, callerName);
			protected void Log(string message, UnityObject context, [CallerMemberName] string callerName = null)
			{
				if (!_logMask.HasFlag(ELogLevel.Info)) return;
				Logging.Log.Info(message, context, callerName);
			}

			protected void Emphasis(string message, [CallerMemberName] string callerName = null) => Emphasis(message, this, callerName);
			protected void Emphasis(string message, UnityObject context, [CallerMemberName] string callerName = null)
			{
				if (!_logMask.HasFlag(ELogLevel.Emphasis)) return;
				Logging.Log.Emphasis(message, context, callerName);
			}

			protected void Warning(string message, [CallerMemberName] string callerName = null) => Warning(message, this, callerName);
			protected void Warning(string message, UnityObject context, [CallerMemberName] string callerName = null)
			{
				if (!_logMask.HasFlag(ELogLevel.Warning)) return;
				Logging.Log.Warning(message, context, callerName);
			}

			protected void Error(string message, [CallerMemberName] string callerName = null) => Error(message, this, callerName);
			protected void Error(string message, UnityObject context, [CallerMemberName] string callerName = null)
			{
				if (!_logMask.HasFlag(ELogLevel.Error)) return;
				Logging.Log.Error(message, context, callerName);
			}

			protected void Exception(Exception exception, string message, [CallerMemberName] string callerName = null) => Exception(exception, message, this, callerName);
			protected void Exception(Exception exception, string message, UnityObject context, [CallerMemberName] string callerName = null)
			{
				if (!_logMask.HasFlag(ELogLevel.Exception)) return;
				Logging.Log.Exception(exception, message, context, callerName);
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