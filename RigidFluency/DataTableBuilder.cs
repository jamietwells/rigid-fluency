using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RigidFluency
{
    public class DataTableBuilder<T>
    {
        private readonly System.Collections.Generic.IEnumerable<T> _data;
        private IEnumerable<(string ColumnName,  Func<T, object> DataProjection, Type columnType)> _columns;

        private DataTableBuilder(System.Collections.Generic.IEnumerable<T> data, IEnumerable<(string ColumnName,  Func<T, object> DataProjection, Type columnType)> columns)
        {
            _data = data;
            _columns = columns;
        }

        public DataTableBuilder(System.Collections.Generic.IEnumerable<T> data) : this(data, Enumerable.Empty<(string ColumnName,  Func<T, object> DataProjection, Type columnType)>())
        { }

        public DataTable Build()
        {
            var table = new DataTable();
            foreach (var col in _columns)
                table.Columns.Add(col.ColumnName, col.columnType);
            foreach (var row in _data){
                var newRow = table.NewRow();
                foreach(var col in _columns)
                    newRow[col.ColumnName] = col.DataProjection(row);
                table.Rows.Add(newRow);
            }
            return table;
        }

        public DataTableBuilder<T> AddColumn<ColType>(string columnName, Func<T, ColType> dataProjection) =>
            new DataTableBuilder<T>(_data, _columns.Concat<(string ColumnName,  Func<T, object> DataProjection, Type columnType)>((ColumnName: columnName, DataProjection: (Func<T, object>)(row => (object)dataProjection(row)), columnType: typeof(ColType)) ));
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
