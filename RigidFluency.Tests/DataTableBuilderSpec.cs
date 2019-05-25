using System;
using System.Collections.Generic;
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
        private readonly Person[] _data;

        public DataTableBuilderSpec()
        {
            _data = new Fixture().Create<Person[]>();
            _builder = new DataTableBuilder<Person>(_data); 
        }

        [Fact]
        public void NumberOfRowsIsEqualToNumberOfDataRows()
        {
            _builder.Build().Rows.Count.Should().Be(_data.Length);
        }
    }
}
