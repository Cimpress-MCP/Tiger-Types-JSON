using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using static System.Diagnostics.Contracts.Contract;
using static Tiger.Types.Json.Resources;

namespace Tiger.Types.Json
{
    /// <summary>
    /// Provides the capabilities to serialize and deserialize <see cref="Option{TSome}"/> to and from JSON.
    /// </summary>
    public sealed class OptionJsonConverter
        : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert([CanBeNull] Type objectType) =>
            objectType != null &&
            objectType.IsConstructedGenericType &&
            objectType.GetGenericTypeDefinition() == typeof(Option<>);

        /// <inheritdoc/>
        public override void WriteJson(
            JsonWriter writer,
            [NotNull] object value,
            [NotNull] JsonSerializer serializer)
        {
            var objectType = value.GetType();
            Assume(objectType.IsConstructedGenericType, IncompatibleValue);
            Assume(objectType.GetGenericTypeDefinition() == typeof(Option<>), IncompatibleValue);

            var typeInfo = objectType.GetTypeInfo();

            // note(cosborn) See if the Option<TSome> is in None state.
            var isNone = (bool)typeInfo
                .GetDeclaredProperty(nameof(Option<object>.IsNone))
                .GetValue(value);
            if (isNone)
            { // note(cosborn) It was None!
                serializer.Serialize(writer, null);
                return;
            }

            // note(cosborn) Now that we know that it's in the Some state...
            var underlyingValue = typeInfo
                .GetDeclaredProperty(nameof(Option<object>.Value))
                .GetValue(value);
            var underlyingType = Option.GetUnderlyingType(objectType);
            serializer.Serialize(writer, underlyingValue, underlyingType);
        }

        /// <inheritdoc/>
        [NotNull]
        public override object ReadJson(
            [NotNull] JsonReader reader,
            [NotNull] Type objectType,
            object existingValue,
            [NotNull] JsonSerializer serializer)
        {
            var underlyingType = Option.GetUnderlyingType(objectType);
            Assume(underlyingType != null, IncompatibleType);

            var typeInfo = objectType.GetTypeInfo();
            var ctor = typeInfo.DeclaredConstructors.Single(c => c.GetParameters().Length == 1);

            return Option.From(reader.ValueType)
                         .Bind(_ => Option.From(serializer.Deserialize(reader, underlyingType)))
                         .Map(v => ctor.Invoke(new[] { v }))
                         .GetValueOrDefault(() => Activator.CreateInstance(objectType));
        }
    }
}
