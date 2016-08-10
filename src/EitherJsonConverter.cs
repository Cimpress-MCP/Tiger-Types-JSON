using Newtonsoft.Json;
using System;
using System.Reflection;
using Tiger.Types;
using Tiger.JsonTypes.Properties;
using static System.Diagnostics.Contracts.Contract;

namespace Tiger.JsonTypes
{
    /// <summary>
    /// Provides the capabilities to serialize <see cref="Either{TLeft,TRight}"/> to JSON.
    /// </summary>
    
    public sealed class EitherJsonConverter
        : JsonConverter
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="JsonConverter" /> can read JSON.
        /// </summary>
        public override bool CanRead { get; } = false;

        /// <summary>Determines whether this instance can convert the specified object type.</summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <see langword="true"/> if this instance can convert the specified object type;
        /// otherwise, <see langword="true"/>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType != null &&
                   objectType.IsGenericType &&
                   objectType.GetGenericTypeDefinition() == typeof(Either<,>);
        }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="JsonWriterException"><paramref name="value"/> could not be written.</exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value), Resources.IncompatibleValue); }

            var objectType = value.GetType();
            Assume(objectType.IsGenericType, Resources.IncompatibleValue);
            Assume(objectType.GetGenericTypeDefinition() == typeof(Either<,>), Resources.IncompatibleValue);

            dynamic dynamicValue = value;
            if (!dynamicValue.IsLeft && !dynamicValue.IsRight)
            {
                throw new JsonWriterException(Resources.EitherCannotBeBottom);
            }

            var types = objectType.GetGenericArguments();
            if (dynamicValue.IsRight)
            {
                serializer.Serialize(writer, dynamicValue.Value, types[1]);
                return;
            }

            var leftValueField = objectType.GetField("LeftValue", BindingFlags.Instance | BindingFlags.NonPublic);
            if (leftValueField == null)
            {
                throw new JsonWriterException(Resources.ThisIsABug);
            }
            serializer.Serialize(writer, leftValueField.GetValue(value), types[0]);
        }


        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException("CanRead is false.");
        }
    }
}
