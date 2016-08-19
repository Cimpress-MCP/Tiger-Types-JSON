// ReSharper disable All
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using Tiger.Types;

namespace Tiger.JsonTypes.UnitTests
{
    /// <summary>Tests related to <see cref="UnionJsonConverter"/>.</summary>
    [TestFixture(TestOf = typeof(UnionJsonConverter))]
    public sealed class UnionJsonConverterTestFixture
    {
        const string sentinel = "sentinel";
        const string none = @"null";
        const string someInt = @"42";
        const string someString = @"""" + sentinel + @"""";

        static readonly TestCaseData[] SerializeSource =
        {
            new TestCaseData(Union.From<string, int>(sentinel)).Returns(someString),
            new TestCaseData(Union.From<string, int>(42)).Returns(someInt),
            new TestCaseData(Union.From<int, string>(42)).Returns(someInt),
            new TestCaseData(Union.From<int, string>(sentinel)).Returns(someString)
        };

        [TestCaseSource(nameof(SerializeSource))]
        public string Serialize(object value)
        {
            // arrange, act
            return JsonConvert.SerializeObject(value, new UnionJsonConverter());
        }

        [TestCase(typeof(Union<int, string>), ExpectedResult = true)]
        [TestCase(typeof(Union<string, int>), ExpectedResult = true)]
        [TestCase(typeof(int), ExpectedResult = false)]
        [TestCase(typeof(string), ExpectedResult = false)]
        public bool CanConvert(Type serializationType)
        {
            // arrange
            var converter = new UnionJsonConverter();

            // act
            return converter.CanConvert(serializationType);
        }
    }
}
