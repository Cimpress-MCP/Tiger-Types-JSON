using System;
using Newtonsoft.Json;
using Xunit;

namespace Tiger.Types.Json.UnitTests
{
    /// <summary>Tests related to <see cref="EitherJsonConverter"/>.</summary>
    public static class EitherJsonConverterTests
    {
        public const string sentinel = "sentinel";
        public const string someInt = "42";
        public const string someString = @"""" + sentinel + @"""";

        public static readonly TheoryData<object, string> SerializeData =
            new TheoryData<object, string>
            {
                { Either.From<string, int>(sentinel), someString },
                { Either.From<string, int>(42), someInt },
                { Either.From<int, string>(42), someInt },
                { Either.From<int, string>(sentinel), someString }
            };

        [Theory(DisplayName = "Either values serialize correctly.")]
        [MemberData(nameof(SerializeData))]
        public static void Serialize(object value, string expected) =>
            Assert.Equal(expected, JsonConvert.SerializeObject(value, new EitherJsonConverter()));

        [Theory(DisplayName = "Either JSON Converters advertise their conversions correctly.")]
        [InlineData(typeof(Either<int, string>), true)]
        [InlineData(typeof(Either<string, int>), true)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(string), false)]
        public static void CanConvert(Type serializationType, bool expected) =>
            Assert.Equal(expected, new EitherJsonConverter().CanConvert(serializationType));
    }
}
