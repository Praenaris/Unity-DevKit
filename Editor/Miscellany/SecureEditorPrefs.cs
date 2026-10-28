#if UNITY_EDITOR


using DragonResonance.Logging;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System;
using UnityEditor;
using UnityEngine;


namespace Praenaris.Editor
{
	public static class SecureEditorPrefs
	{
		private const string KeyDerivationSalt = "Praenaris.SecureEditorPrefs";
		private const int KeyDerivationIterations = 10000;
		private const int KeySize = 32;	// AES-256

		private static readonly HashSet<string> _undecryptableKeys = new();	// Warned about once each, GUIs read them on every repaint
		private static byte[] _key_internal = null;	// Caching only, use the property instead


		#region Publics

			public static bool HasKey(string key) => EditorPrefs.HasKey(key);
			public static void DeleteKey(string key) => EditorPrefs.DeleteKey(key);

			public static string GetString(string key, string defaultValue = "") =>
				(EditorPrefs.HasKey(key) ? Decrypt(key, EditorPrefs.GetString(key), defaultValue) : defaultValue);

			public static void SetString(string key, string value)
			{
				_undecryptableKeys.Remove(key);
				EditorPrefs.SetString(key, Encrypt(value ?? string.Empty));
			}

		#endregion


		#region Privates

			private static string Encrypt(string value)
			{
				using Aes aes = Aes.Create();
				aes.Key = Key;
				aes.GenerateIV();	// A new one per value, so equal values don't look equal

				byte[] plainBytes = Encoding.UTF8.GetBytes(value);
				using ICryptoTransform encryptor = aes.CreateEncryptor();
				byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

				byte[] storedBytes = new byte[aes.IV.Length + cipherBytes.Length];
				aes.IV.CopyTo(storedBytes, 0);
				cipherBytes.CopyTo(storedBytes, aes.IV.Length);
				return Convert.ToBase64String(storedBytes);
			}

			private static string Decrypt(string key, string storedValue, string defaultValue)
			{
				try {
					byte[] storedBytes = Convert.FromBase64String(storedValue);
					using Aes aes = Aes.Create();
					aes.Key = Key;
					byte[] iv = new byte[aes.BlockSize / 8];
					Array.Copy(storedBytes, iv, iv.Length);
					aes.IV = iv;

					using ICryptoTransform decryptor = aes.CreateDecryptor();
					return Encoding.UTF8.GetString(decryptor.TransformFinalBlock(storedBytes, iv.Length, storedBytes.Length - iv.Length));
				}
				catch (Exception exception) when ((exception is CryptographicException) || (exception is FormatException) || (exception is ArgumentException)) {
					if (_undecryptableKeys.Add(key))
						HLogger.LogWarning($"\"{key}\" couldn't be decrypted (saved on another machine, or not by this tool?), using its default value: {exception.Message}", typeof(SecureEditorPrefs));
					return defaultValue;
				}
			}

			private static byte[] DeriveKey()
			{
				using Rfc2898DeriveBytes keyDerivation = new(SystemInfo.deviceUniqueIdentifier, Encoding.UTF8.GetBytes(KeyDerivationSalt), KeyDerivationIterations, HashAlgorithmName.SHA256);
				return keyDerivation.GetBytes(KeySize);
			}

		#endregion


		#region Properties

			private static byte[] Key => (_key_internal ??= DeriveKey());	// Derived once, it's slow on purpose

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