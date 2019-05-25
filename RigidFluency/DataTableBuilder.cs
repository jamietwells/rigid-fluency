using System;
using System.Data;

namespace RigidFluency
{
    public class DataTableBuilder<T>
    {
        private readonly System.Collections.Generic.IEnumerable<T> _data;
        private int _columns;

        public DataTableBuilder(System.Collections.Generic.IEnumerable<T> data)
        {
            this._data = data;
        }

        public DataTable Build()
        {
            var table = new DataTable();
            for (var i = 0; i < _columns; i++)
                table.Columns.Add(default(string));
            foreach (var row in _data)
                table.Rows.Add(table.NewRow());
            return table;
        }

        public DataTableBuilder<T> AddColumn<ColType>(string columnName, Func<T, ColType> dataProjection)
        {
            _columns++;
            return this;
        }
    }
}
