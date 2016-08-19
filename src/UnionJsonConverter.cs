using System;
using Newtonsoft.Json;
using Tiger.JsonTypes.Properties;
using Tiger.Types;
using static System.Diagnostics.Contracts.Contract;

namespace Tiger.JsonTypes
{
    /// <summary>
    /// Provides the capabilities to serialize <see cref="Union{T1,T2}"/> to JSON.
    /// </summary>
    public class UnionJsonConverter
        : JsonConverter
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="JsonConverter"/> can read JSON.
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
                   objectType.GetGenericTypeDefinition() == typeof(Union<,>); // todo(cosborn) or 3 or 4
        }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var objectType = value.GetType();
            Assume(objectType.IsGenericType, Resources.IncompatibleValue);
            // todo(cosborn) or 3 or 4
            Assume(objectType.GetGenericTypeDefinition() == typeof(Union<,>), Resources.IncompatibleValue);

            var types = objectType.GetGenericArguments();
            dynamic dynamicValue = value;

            // note(cosborn Try each state in turn.
            if (dynamicValue.IsState1)
            {
                serializer.Serialize(writer, dynamicValue.Value1, types[0]);
                return;
            }

            if (dynamicValue.IsState2)
            {
                serializer.Serialize(writer, dynamicValue.Value2, types[1]);
                return;
            }

            if (dynamicValue.IsState3)
            {
                serializer.Serialize(writer, dynamicValue.Value3, types[2]);
                return;
            }

            if (dynamicValue.IsState4)
            {
                serializer.Serialize(writer, dynamicValue.Value4, types[3]);
            }
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        /// <exception cref="NotSupportedException">CanRead is false.</exception>
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            throw new NotSupportedException("CanRead is false.");
        }
    }
}
