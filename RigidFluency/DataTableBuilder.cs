using System;
using System.Data;

namespace RigidFluency
{
    public class DataTableBuilder<T>
    {
        private System.Collections.Generic.IEnumerable<T> data;

        public DataTableBuilder(System.Collections.Generic.IEnumerable<T> data)
        {
            this.data = data;
        }

        public DataTable Build()
        {
            var table = new DataTable();
            foreach (var row in data)
                table.Rows.Add(table.NewRow());
            return table;
        }
    }
}
