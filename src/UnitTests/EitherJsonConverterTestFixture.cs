// ReSharper disable All
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using Tiger.Types;

namespace Tiger.JsonTypes.UnitTests
{
    /// <summary>Tests related to <see cref="OptionTypeConverter"/>.</summary>
    [TestFixture(TestOf = typeof(OptionJsonConverter))]
    public sealed class EitherJsonConverterTestFixture
    {
        const string sentinel = "sentinel";
        const string none = @"null";
        const string someInt = @"42";
        const string someString = @"""" + sentinel + @"""";

        static readonly TestCaseData[] SerializeSource =
        {
            new TestCaseData(Either.Left<string, int>(sentinel)).Returns(someString),
            new TestCaseData(Either.Right<string, int>(42)).Returns(someInt),
            new TestCaseData(Either.Left<int, string>(42)).Returns(someInt),
            new TestCaseData(Either.Right<int, string>(sentinel)).Returns(someString)
        };

        [TestCaseSource(nameof(SerializeSource))]
        public string Serialize(object value)
        {
            // arrange, act
            return JsonConvert.SerializeObject(value, new EitherJsonConverter());
        }

        [TestCase(typeof(Either<int, string>), ExpectedResult = true)]
        [TestCase(typeof(Either<string, int>), ExpectedResult = true)]
        [TestCase(typeof(int), ExpectedResult = false)]
        [TestCase(typeof(string), ExpectedResult = false)]
        public bool CanConvert(Type serializationType)
        {
            // arrange
            var converter = new EitherJsonConverter();

            // act
            return converter.CanConvert(serializationType);
        }
    }
}
