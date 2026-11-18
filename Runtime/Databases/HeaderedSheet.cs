using System.Collections.Generic;
using System.Linq;


namespace DragonResonance.Databases
{
	public partial class HeaderedSheet<T> : Sheet<T>
	{
		#region Constructors

			public HeaderedSheet() { }
			public HeaderedSheet(List<List<T>> data) : base(data) { }

		#endregion


		#region Publics

			public int GetHeadingColumnIndex(T cellContent) => (this.HeadingRow?.IndexOf(cellContent) ?? -1);
			public int GetHeadingRowIndex(T cellContent) => (this.HeadingColumn?.IndexOf(cellContent) ?? -1);


			public bool TryGet(T headingColumn, T headingRow, out T cellContent)
			{
				int columnIndex = GetHeadingColumnIndex(headingColumn);
				int rowIndex = GetHeadingRowIndex(headingRow);

				if ((columnIndex == -1) || (rowIndex == -1)) {
					cellContent = default;
					return false;
				}
				else {
					cellContent = base.Get(columnIndex, rowIndex);
					return true;
				}
			}

			public bool TrySet(T headingColumn, T headingRow, T cellContent)
			{
				int rowIndex = GetHeadingRowIndex(headingRow);
				int columnIndex = GetHeadingColumnIndex(headingColumn);

				if ((columnIndex == -1) || (rowIndex == -1)) {
					return false;
				}
				else {
					base.Set(columnIndex, rowIndex, cellContent);
					return true;
				}
			}

		#endregion


		#region Properties

			public List<T> HeadingColumn => (base._data.Any() ? base._data.Select(row => row.FirstOrDefault()).ToList() : null);	// Returns null if none or empty rows
			public List<T> HeadingRow => base._data.FirstOrDefault();	// Returns null if none

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