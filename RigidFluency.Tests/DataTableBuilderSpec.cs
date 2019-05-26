using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace RigidFluency.Tests
{
    public class DataTableBuilderSpec
    {        
        class Person 
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
        private readonly DataTableBuilder<Person> _builder;
        private readonly Fixture _fixture;
        private readonly Random _random;
        private readonly Person[] _data;

        public DataTableBuilderSpec()
        {
            _fixture = new Fixture(); 
            _random = new Random(); 
            _data = _fixture.Create<Person[]>();
            _builder = new DataTableBuilder<Person>(_data); 
        }

        [Fact]
        public void NumberOfRowsIsEqualToNumberOfDataRows()
        {
            _builder.Build().Rows.Count.Should().Be(_data.Length);
        }

        [Fact]
        public void ColumnNamesShouldBeTheNamesOfTheAddedColumns()
        {
            IEnumerable<string> ColumnNames(DataTable table){
                foreach (DataColumn column in table.Columns)
                    yield return column.ColumnName;
            }

            var columns = Enumerable.Repeat(_fixture.Create<string>(), _random.Next(10, 50))
                .Distinct()
                .Select(n => ( ColumnName: n, DataProjection: (Func<Person, object>)(_ => new { })))
                .ToArray();
            
            var builder = _builder;
            foreach(var (ColumnName, DataProjection) in columns)
                builder = builder.AddColumn(ColumnName, DataProjection);

            ColumnNames(builder.Build()).Should().BeEquivalentTo(columns.Select(c => c.ColumnName));
        }
    }
}
