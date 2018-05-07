using System;
using Newtonsoft.Json;
using Xunit;

namespace Tiger.Types.Json.UnitTests
{
    /// <summary>Tests related to <see cref="OptionTypeConverter"/>.</summary>
    public sealed class OptionJsonConverterTests
    {
        public const string sentinel = "sentinel";
        public const string none = "null";
        public const string someInt = "42";
        public const string someString = @"""" + sentinel + @"""";

        public static readonly TheoryData<string, Type, object> DeserializeSource =
            new TheoryData<string, Type, object>
            {
                { none, typeof(Option<int>), Option<int>.None },
                { someInt, typeof(Option<int>), Option.From(42) },
                { none, typeof(Option<string>), Option<string>.None },
                { someString, typeof(Option<string>), Option.From(sentinel) }
            };

        [Theory(DisplayName = "Option values deserialize correctly.")]
        [MemberData(nameof(DeserializeSource))]
        public void Deserialize(string json, Type serializationType, object expected) =>
            Assert.Equal(expected, JsonConvert.DeserializeObject(json, serializationType, new OptionJsonConverter()));

        public static readonly TheoryData<object, string> SerializeSource =
            new TheoryData<object, string>
            {
                { Option<int>.None, none },
                { Option.From(42), someInt },
                { Option<string>.None, none },
                { Option.From(sentinel), someString }
            };

        [Theory(DisplayName = "Option values serialize correctly.")]
        [MemberData(nameof(SerializeSource))]
        public void Serialize(object value, string expected) =>
            Assert.Equal(expected, JsonConvert.SerializeObject(value, new OptionJsonConverter()));

        [Theory(DisplayName = "Option JSON Converters advertise their conversions correctly.")]
        [InlineData(typeof(Option<int>), true)]
        [InlineData(typeof(Option<string>), true)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(string), false)]
        public void CanConvert(Type serializationType, bool expected) =>
            Assert.Equal(expected, new OptionJsonConverter().CanConvert(serializationType));
    }
}
