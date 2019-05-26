using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RigidFluency
{
    public class DataTableBuilder<T>
    {
        private readonly System.Collections.Generic.IEnumerable<T> _data;
        private IEnumerable<string> _columns;

        private DataTableBuilder(System.Collections.Generic.IEnumerable<T> data, IEnumerable<string> columns)
        {
            _data = data;
            _columns = columns;
        }

        public DataTableBuilder(System.Collections.Generic.IEnumerable<T> data) : this(data, Enumerable.Empty<string>())
        { }

        public DataTable Build()
        {
            var table = new DataTable();
            foreach (var col in _columns)
                table.Columns.Add(col);
            foreach (var row in _data)
                table.Rows.Add(table.NewRow());
            return table;
        }

        public DataTableBuilder<T> AddColumn<ColType>(string columnName, Func<T, ColType> dataProjection) =>
            new DataTableBuilder<T>(_data, _columns.Concat(new[] { columnName }));
    }
}
