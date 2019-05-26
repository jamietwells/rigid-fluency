using System;
using System.Collections.Generic;
using System.Data;

namespace RigidFluency
{
    public class DataTableBuilder<T>
    {
        private readonly System.Collections.Generic.IEnumerable<T> _data;
        private List<string> _columns;

        public DataTableBuilder(System.Collections.Generic.IEnumerable<T> data)
        {
            _data = data;
            _columns = new List<string>();
        }

        public DataTable Build()
        {
            var table = new DataTable();
            foreach(var col in _columns)
                table.Columns.Add(col);
            foreach (var row in _data)
                table.Rows.Add(table.NewRow());
            return table;
        }

        public DataTableBuilder<T> AddColumn<ColType>(string columnName, Func<T, ColType> dataProjection)
        {
            _columns.Add(columnName);
            return this;
        }
    }
}
