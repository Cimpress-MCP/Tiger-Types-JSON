// ReSharper disable All

using System;
using Newtonsoft.Json;
using Xunit;

namespace Tiger.Types.Json.UnitTests
{
    /// <summary>Tests related to <see cref="UnionJsonConverter"/>.</summary>
    public sealed class UnionJsonConverterTests
    {
        const string sentinel = "sentinel";
        const string none = @"null";
        const string someInt = @"42";
        const string someString = @"""" + sentinel + @"""";

        public static readonly TheoryData<object, string> SerializeSource =
            new TheoryData<object, string>
            {
                { Union.From<string, int>(sentinel), someString },
                { Union.From<string, int>(42), someInt },
                { Union.From<int, string>(42), someInt },
                { Union.From<int, string>(sentinel), someString }
            };

        [Theory(DisplayName = "Union values should serialize correctly.")]
        [MemberData(nameof(SerializeSource))]
        public void Serialize(object value, string expected)
        {
            // arrange
            var sut = new UnionJsonConverter();

            // act
            var actual = JsonConvert.SerializeObject(value, sut);

            // assert
            Assert.Equal(expected, actual);
        }

        [Theory(DisplayName = "Union JSON Converters advertise their conversions correctly.")]
        [InlineData(typeof(Union<int, string>), true)]
        [InlineData(typeof(Union<string, int>), true)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(string), false)]
        public void CanConvert(Type serializationType, bool expected)
        {
            // arrange
            var sut = new UnionJsonConverter();

            // act
            var actual = sut.CanConvert(serializationType);

            // assert
            Assert.Equal(expected, actual);
        }
    }
}
