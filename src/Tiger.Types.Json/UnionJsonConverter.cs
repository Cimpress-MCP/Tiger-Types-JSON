using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using static System.Diagnostics.Contracts.Contract;

namespace Tiger.Types.Json
{
    /// <summary>
    /// Provides the capabilities to serialize <see cref="Union{T1,T2}"/> to JSON.
    /// </summary>
    public class UnionJsonConverter
        : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanRead { get; } = false;

        /// <inheritdoc/>
        public override bool CanConvert([CanBeNull] Type objectType)
        {
            return objectType != null &&
                   objectType.IsConstructedGenericType &&
                   objectType.GetGenericTypeDefinition() == typeof(Union<,>); // todo(cosborn) or 3 or 4
        }

        /// <inheritdoc/>
        public override void WriteJson(
            JsonWriter writer,
            [CanBeNull] object value,
            [NotNull] JsonSerializer serializer)
        {
            if (value == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            // todo(cosborn) or 3 or 4
            var objectType = value.GetType();
            Assume(objectType.IsConstructedGenericType, Resources.IncompatibleValue);
            Assume(objectType.GetGenericTypeDefinition() == typeof(Union<,>), Resources.IncompatibleValue);

            var types = objectType.GenericTypeArguments;
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

        /// <inheritdoc/>
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
