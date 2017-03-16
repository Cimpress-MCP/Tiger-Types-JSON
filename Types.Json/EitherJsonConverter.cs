using System;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using static System.Diagnostics.Contracts.Contract;

namespace Tiger.Types.Json
{
    /// <summary>
    /// Provides the capabilities to serialize <see cref="Either{TLeft,TRight}"/> to JSON.
    /// </summary>
    public sealed class EitherJsonConverter
        : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanRead { get; } = false;

        /// <inheritdoc/>
        public override bool CanConvert([CanBeNull] Type objectType)
        {
            return objectType != null &&
                   objectType.IsConstructedGenericType &&
                   objectType.GetGenericTypeDefinition() == typeof(Either<,>);
        }

        /// <inheritdoc/>
        /// <exception cref="JsonWriterException"><paramref name="value"/> could not be written.</exception>
        public override void WriteJson(
            JsonWriter writer,
            [NotNull] object value,
            [NotNull] JsonSerializer serializer)
        {
            var objectType = value.GetType();
            Assume(objectType.IsConstructedGenericType, Resources.IncompatibleValue);
            Assume(objectType.GetGenericTypeDefinition() == typeof(Either<,>), Resources.IncompatibleValue);

            dynamic dynamicValue = value;
            if (!dynamicValue.IsLeft && !dynamicValue.IsRight)
            {
                throw new JsonWriterException(Resources.EitherCannotBeBottom);
            }

            var types = objectType.GenericTypeArguments; // note(cosborn) [TLeft, TRight]
            if (dynamicValue.IsRight)
            {
                serializer.Serialize(writer, dynamicValue.Value, types[1]);
                return;
            }

            var typeInfo = objectType.GetTypeInfo();
            var leftValueProperty = typeInfo.GetDeclaredProperty("LeftValue");
            if (leftValueProperty == null)
            {
                throw new JsonWriterException(Resources.ThisIsABug);
            }

            serializer.Serialize(writer, leftValueProperty.GetValue(value), types[0]);
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
