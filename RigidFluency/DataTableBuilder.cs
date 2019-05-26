using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RigidFluency
{
    public class DataTableBuilder<T>
    {
        private readonly System.Collections.Generic.IEnumerable<T> _data;
        private IEnumerable<DataColumn<T>> _columns;

        private DataTableBuilder(System.Collections.Generic.IEnumerable<T> data, IEnumerable<DataColumn<T>> columns)
        {
            _data = data;
            _columns = columns;
        }

        public DataTableBuilder(System.Collections.Generic.IEnumerable<T> data) 
            : this(data, Enumerable.Empty<DataColumn<T>>())
        { }

        public DataTable Build()
        {
            var table = new DataTable();
            foreach (var col in _columns)
                table.Columns.Add(col.ColumnName, col.ColumnType);
            foreach (var row in _data)
            {
                var newRow = table.NewRow();
                foreach (var col in _columns)
                    newRow[col.ColumnName] = col.DataProjection(row);
                table.Rows.Add(newRow);
            }
            return table;
        }

        public DataTableBuilder<T> AddColumn<ColType>(string columnName, Func<T, ColType> dataProjection) =>
            AddColumn(DataColumn<T>.Create(columnName, dataProjection));
        
        private DataTableBuilder<T> AddColumn(DataColumn<T> column) =>
            new DataTableBuilder<T>(_data, _columns.Concat(column));

        class DataColumn<TRow>
        {
            public static DataColumn<TRow> Create<TCol>(string columnName, Func<TRow, TCol> dataProjection) => 
                new DataColumn<TRow>(columnName, r => dataProjection(r), typeof(TCol));

            private DataColumn(string columnName, Func<TRow, object> dataProjection, Type colType)
            {
                ColumnName = columnName;
                DataProjection = dataProjection;
                ColumnType = colType;
            }

            public string ColumnName { get; }
            public Func<TRow, object> DataProjection { get; }
            public Type ColumnType { get; }
        }
    }

    public static class LinqConcatExtension
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T item)
        {
            foreach (var i in items)
                yield return i;
            yield return item;
        }
    }
}
