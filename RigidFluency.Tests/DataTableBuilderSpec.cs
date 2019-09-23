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
        struct FakeStruct
        {
            private readonly double _a;
            private readonly double _b;

            public double A => _a;
            public double B => _b;

            public FakeStruct(double a, double b)
            {
                this._a = a;
                this._b = b;
            }
        }

        class Person
        {
            public string? Name { get; set; }
            public Person? AlwaysNullReference => null;
            public int Age { get; set; }
            public int? NullableInt { get; set; }
            public Guid? NullableGuid { get; set; }
            public FakeStruct? NullableStruct { get; set; }
            public FakeStruct? AlwaysNullStruct => null;
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
            _builder = _random.Next(0, 2) == 1
                ? new DataTableBuilder<Person>(_data)
                : DataTableBuilder.From(_data);
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
                .AddColumn("NullableInt", p => p.NullableInt)
                .AddColumn("NullableGuid", p => p.NullableGuid)
                .AddColumn("NullableStruct", p => p.NullableStruct)
                .AddColumn("AlwaysNullStruct", p => p.AlwaysNullStruct)
                .AddColumn("AlwaysNullReference", p => p.AlwaysNullReference)
                .Build();

            GetColumns(dataTable)
                .Select(c => c.ColumnName)
                .Should()
                .BeEquivalentTo(new[]
                {
                    "Name",
                    "Age",
                    "NullableInt",
                    "NullableGuid",
                    "NullableStruct",
                    "AlwaysNullStruct",
                    "AlwaysNullReference"
                });
        }

        [Fact]
        public void RowValuesShouldBeMappedCorrectly()
        {
            var dataTable =
                _builder
                .AddColumn("Name", p => p.Name)
                .AddColumn("Age", p => p.Age)
                .AddColumn("NullableInt", p => p.NullableInt)
                .AddColumn("NullableGuid", p => p.NullableGuid)
                .AddColumn("NullableStruct", p => p.NullableStruct)
                .AddColumn("AlwaysNullStruct", p => p.AlwaysNullStruct)
                .AddColumn("AlwaysNullReference", p => p.AlwaysNullReference)
                .Build();

            GetRows(dataTable)
                .Select(c => new
                {
                    Name = c["Name"],
                    Age = c["Age"],
                    NullableInt = (object)c["NullableInt"],
                    NullableGuid = (object)c["NullableGuid"],
                    NullableStruct = (object)c["NullableStruct"],
                    AlwaysNullStruct = (object)c["AlwaysNullStruct"],
                    AlwaysNullReference = (object)c["AlwaysNullReference"]
                })
                .Should()
                .BeEquivalentTo(_data.Select(d => new
                {
                    d.Name,
                    d.Age,
                    NullableInt = d.NullableInt ?? (object)DBNull.Value,
                    NullableGuid = d.NullableGuid ?? (object)DBNull.Value,
                    NullableStruct = d.NullableStruct ?? (object)DBNull.Value,
                    AlwaysNullStruct = d.AlwaysNullStruct ?? (object)DBNull.Value,
                    AlwaysNullReference = d.AlwaysNullReference ?? (object)DBNull.Value
                }));
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
