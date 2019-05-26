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
            var columns = GetRandomColumns();

            var dataTable = columns
                .Aggregate(_builder, (b, c) => b.AddColumn(c.ColumnName, c.DataProjection))
                .Build();

            GetColumns(dataTable)
                .Select(c => c.ColumnName)
                .Should()
                .BeEquivalentTo(columns.Select(c => c.ColumnName));
        }

        [Fact]
        public void DataTableBuilderShouldBeImmutable()
        {
            _ = GetRandomColumns()
                .Aggregate(_builder, (b, c) => b.AddColumn(c.ColumnName, c.DataProjection));

            var dataTable =
                _builder
                .AddColumn("Name", p => p.Name)
                .AddColumn("Age", p => p.Age)
                .Build();

            GetColumns(dataTable)
                .Select(c => c.ColumnName)
                .Should()
                .BeEquivalentTo(new[] { "Name", "Age" });
        }

        [Fact]
        public void RowValuesShouldBeMappedCorrectly()
        {
            var dataTable =
                _builder
                .AddColumn("Name", p => p.Name)
                .AddColumn("Age", p => p.Age)
                .Build();

            GetRows(dataTable)
                .Select(c => new { Name = c["Name"], Age = c["Age"] })
                .Should()
                .BeEquivalentTo(_data.Select(d => new { d.Name, d.Age }));
        }

        private (string ColumnName, Func<Person, object> DataProjection)[] GetRandomColumns() =>
            Enumerable.Range(0, _random.Next(10, 50))
                .Select(c => _fixture.Create<string>())
                .Distinct()
                .Select(n => (ColumnName: n, DataProjection: (Func<Person, object>)(_ => new { })))
                .ToArray();

        private IEnumerable<DataColumn> GetColumns(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
                yield return col;
        }

        private IEnumerable<DataRow> GetRows(DataTable table)
        {
            foreach (DataRow row in table.Rows)
                yield return row;
        }
    }
}
