using DragonResonance.Extensions;
using System.Collections.Generic;
using System.Linq;


namespace DragonResonance.Databases
{
	public partial class Sheet<T>
	{
		protected List<List<T>> _data = new();


		#region Constructors

			public Sheet()
			{
				Clear();
			}

			public Sheet(List<List<T>> data)
			{
				_data = data;
				NormalizeDataMatrix();
			}

		#endregion


		#region Publics

			public void Clear() => _data.Clear();

			public void AddRow() => AddRow(Enumerable.Empty<T>());
			public void AddRow(IEnumerable<T> data)
			{
				_data.Add(data.ToList());
				NormalizeDataMatrix();
			}

			public void AddColumn() => AddColumn(Enumerable.Empty<T>());
			public void AddColumn(IEnumerable<T> data)
			{
				List<T> column = data.ToList();

				int neededRows = column.Count;
				while (_data.Count < neededRows)
					AddRow();

				for (int cellIndex = 0; cellIndex < column.Count; cellIndex++)
					_data[cellIndex].Add(column[cellIndex]);

				NormalizeDataMatrix();
			}

			public T Get(int column, int row) => _data[row][column];
			public void Set(int column, int row, T value) => _data[row][column] = value;

		#endregion


		#region Inheritables

			protected void NormalizeDataMatrix()
			{
				int cachedColumnCount = this.ColumnCount;
				foreach (List<T> row in _data)
					while (row.Count < cachedColumnCount)
						row.Add(default);
			}

		#endregion


		#region Properties

			public List<List<T>> Data => _data;

			public int ColumnCount => (!_data.IsEmpty() ? _data.Max(row => row.Count) : 0);
			public int RowCount => _data.Count;

			public T this[int column, int row]
			{
				get => Get(column, row);
				set => Set(column, row, value);
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