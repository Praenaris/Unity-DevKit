using DragonResonance.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;


namespace DragonResonance.Databases
{
	public partial class Sheet<T>	// CSV Importer
	{
		#region Publics

			public void ImportTSV(string content, string separator = "\t", string delimiter = "\"") => ImportCSV(content, separator, delimiter);
			public void ImportCSV(string content, string separator = ";", string delimiter = "\"")	// Replaces the whole content
			{
				Clear();
				InsertRows(0, ParseRows(content, separator, delimiter));
			}

			public void AppendRowsTSV(string content, string separator = "\t", string delimiter = "\"") => AppendRowsCSV(content, separator, delimiter);
			public void AppendRowsCSV(string content, string separator = ";", string delimiter = "\"") =>	// After the existing rows, into the existing columns
				AppendRows(ParseRows(content, separator, delimiter));

			public void PrependRowsTSV(string content, string separator = "\t", string delimiter = "\"") => PrependRowsCSV(content, separator, delimiter);
			public void PrependRowsCSV(string content, string separator = ";", string delimiter = "\"") =>	// Before the existing rows, into the existing columns
				PrependRows(ParseRows(content, separator, delimiter));

			public void AppendColumnsTSV(string content, string separator = "\t", string delimiter = "\"") => AppendColumnsCSV(content, separator, delimiter);
			public void AppendColumnsCSV(string content, string separator = ";", string delimiter = "\"") =>	// After the existing columns, into the existing rows
				ExecuteTransposed(() => AppendRows(Transpose(ParseRows(content, separator, delimiter))));

			public void PrependColumnsTSV(string content, string separator = "\t", string delimiter = "\"") => PrependColumnsCSV(content, separator, delimiter);
			public void PrependColumnsCSV(string content, string separator = ";", string delimiter = "\"") =>	// Before the existing columns, into the existing rows
				ExecuteTransposed(() => PrependRows(Transpose(ParseRows(content, separator, delimiter))));

		#endregion


		#region Inheritables

			protected virtual void AppendRows(List<List<T>> rows) => InsertRows(_data.Count, FitToColumns(rows));
			protected virtual void PrependRows(List<List<T>> rows) => InsertRows(0, FitToColumns(rows));


			protected void InsertRows(int rowIndex, IEnumerable<List<T>> rows)
			{
				_data.InsertRange(rowIndex, rows);
				NormalizeDataMatrix();
			}

			protected List<List<T>> ParseRows(string content, string separator, string delimiter) =>
				ParseCSV(content, separator, delimiter).Select(row => row.Cast<T>().ToList()).ToList();

		#endregion


		#region Privates

			private List<List<T>> FitToColumns(List<List<T>> rows)
			{
				if (_data.IsEmpty()) return rows;

				int columnCount = this.ColumnCount;
				return rows.Select(row => row.Take(columnCount).ToList()).ToList();
			}

			private void ExecuteTransposed(Action rowsOperation)
			{
				_data = Transpose(_data);
				rowsOperation();
				_data = Transpose(_data);
			}

			private static List<List<T>> Transpose(List<List<T>> rows)
			{
				int columnCount = (rows.IsEmpty() ? 0 : rows.Max(row => row.Count));
				return Enumerable.Range(0, columnCount)
					.Select(columnIndex => rows.Select(row => ((columnIndex < row.Count) ? row[columnIndex] : default)).ToList())
					.ToList();
			}


			public IEnumerable<List<string>> ParseCSV(string content, string separator, string delimiter)
			{
				string[] lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

				foreach (string line in lines) {
					List<string> row = new();
					if (string.IsNullOrWhiteSpace(line)) {
						yield return row;
						continue;
					}

					StringBuilder current = new();
					bool insideDelimiter = false;
					for (int charIndex = 0; charIndex < line.Length; charIndex++) {
						string character = line[charIndex].ToString();

						if (character == delimiter) {
							//	Escapes quote delimiters ("""hello"" world" → "hello" world)
							if (insideDelimiter && ((charIndex + 1) < line.Length) && (line[charIndex + 1].ToString() == delimiter)) {
								current.Append(delimiter);
								charIndex++; // Skip the escaped character
							}
							else {
								insideDelimiter = !insideDelimiter;
							}
						}
						else if (character == separator && !insideDelimiter) {
							row.Add(current.ToString());
							current.Clear();
						}
						else {
							current.Append(character);
						}
					}

					row.Add(current.ToString());	// Last cell
					yield return row;
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