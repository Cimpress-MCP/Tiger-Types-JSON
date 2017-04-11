// ReSharper disable All

using System;
using Newtonsoft.Json;
using Xunit;

namespace Tiger.Types.Json.UnitTests
{
    /// <summary>Tests related to <see cref="EitherJsonConverter"/>.</summary>
    public sealed class EitherJsonConverterTests
    {
        const string sentinel = "sentinel";
        const string someInt = @"42";
        const string someString = @"""" + sentinel + @"""";

        public static readonly TheoryData<object, string> SerializeData =
            new TheoryData<object, string>
            {
                { Either.Left<string, int>(sentinel), someString },
                { Either.Right<string, int>(42), someInt },
                { Either.Left<int, string>(42), someInt },
                { Either.Right<int, string>(sentinel), someString }
            };

        [Theory(DisplayName = "Either values serialize correctly.")]
        [MemberData(nameof(SerializeData))]
        public void Serialize(object value, string expected)
        {
            // arrange
            var sut = new EitherJsonConverter();

            // act
            var actual = JsonConvert.SerializeObject(value, sut);

            // assert
            Assert.Equal(expected, actual);
        }

        [Theory(DisplayName = "Either JSON Converters advertise their conversions correctly.")]
        [InlineData(typeof(Either<int, string>), true)]
        [InlineData(typeof(Either<string, int>), true)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(string), false)]
        public void CanConvert(Type serializationType, bool expected)
        {
            // arrange
            var sut = new EitherJsonConverter();

            // act
            var actual = sut.CanConvert(serializationType);

            // assert
            Assert.Equal(expected, actual);
        }
    }
}
