using DragonResonance.Extensions;
using System.Collections.Generic;
using System.Linq;


namespace DragonResonance.Databases
{
	public partial class HeaderedSheet<T>	// CSV Importer
	{
		#region Publics

			public void JoinTSV(string content, string separator = "\t", string delimiter = "\"") => JoinCSV(content, separator, delimiter);
			public void JoinCSV(string content, string separator = ";", string delimiter = "\"")
			{
				List<List<T>> rows = ParseRows(content, separator, delimiter);
				if (rows.IsEmpty()) return;
				if (base._data.IsEmpty()) {
					base.InsertRows(0, rows);
					return;
				}

				List<T> headingRow = this.HeadingRow;
				foreach (T heading in rows[0].Where(heading => !IsBlank(heading)))
					if (!headingRow.Contains(heading))
						headingRow.Add(heading);
				base.NormalizeDataMatrix();

				Dictionary<T, List<T>> rowsByKey = new();
				foreach (List<T> row in base._data.Skip(1).Where(row => !IsBlank(row[0])))
					rowsByKey.TryAdd(row[0], row);	// The first one

				foreach (List<T> row in MatchColumns(rows)) {
					if (IsBlank(row[0]) || !rowsByKey.TryGetValue(row[0], out List<T> keyRow)) {
						base._data.Add(row);
						if (!IsBlank(row[0]))
							rowsByKey.Add(row[0], row);
					}
					else {
						for (int columnIndex = 1; columnIndex < row.Count; columnIndex++)
							if (!IsBlank(row[columnIndex]))
								keyRow[columnIndex] = row[columnIndex];	// The later wins, an empty cell never overwrites
					}
				}
			}

		#endregion


		#region Inheritables

			protected override void AppendRows(List<List<T>> rows) => base.InsertRows(base._data.Count, MatchColumns(rows));
			protected override void PrependRows(List<List<T>> rows) => base.InsertRows((base._data.IsEmpty() ? 0 : 1), MatchColumns(rows));	// Under the heading row

		#endregion


		#region Privates

			private static bool IsBlank(T cell) =>
				((cell == null) || ((cell is string text) && string.IsNullOrWhiteSpace(text)));

			private List<List<T>> MatchColumns(List<List<T>> rows)
			{
				if (base._data.IsEmpty() || rows.IsEmpty()) return rows;

				int[] targetColumns = rows[0].Select(heading => (IsBlank(heading) ? -1 : GetHeadingColumnIndex(heading))).ToArray();
				int columnCount = base.ColumnCount;

				return rows.Skip(1).Select(row => {
					List<T> matchedRow = Enumerable.Repeat(default(T), columnCount).ToList();
					for (int columnIndex = 0; columnIndex < row.Count.UpperClamp(targetColumns.Length); columnIndex++)
						if (!targetColumns[columnIndex].IsNegative())
							matchedRow[targetColumns[columnIndex]] = row[columnIndex];
					return matchedRow;
				}).ToList();
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