// <copyright file="EitherJsonConverter.cs" company="Cimpress, Inc.">
//   Copyright 2017 Cimpress, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>

using System;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using static System.Diagnostics.Contracts.Contract;
using static Tiger.Types.Json.Resources;

namespace Tiger.Types.Json
{
    /// <summary>Provides the capabilities to serialize <see cref="Either{TLeft,TRight}"/> to JSON.</summary>
    public sealed class EitherJsonConverter
        : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanRead => false;

        /// <inheritdoc/>
        public override bool CanConvert([CanBeNull] Type objectType) =>
            objectType?.IsConstructedGenericType == true
            && objectType.GetGenericTypeDefinition() == typeof(Either<,>);

        /// <inheritdoc/>
        /// <exception cref="JsonWriterException"><paramref name="value"/> could not be written.</exception>
        public override void WriteJson(
            JsonWriter writer,
            [NotNull] object value,
            [NotNull] JsonSerializer serializer)
        {
            var objectType = value.GetType();
            Assume(objectType.IsConstructedGenericType, IncompatibleValue);
            Assume(objectType.GetGenericTypeDefinition() == typeof(Either<,>), IncompatibleValue);

            dynamic dynamicValue = value;
            if (!dynamicValue.IsLeft && !dynamicValue.IsRight)
            {
                throw new JsonWriterException(EitherCannotBeBottom);
            }

            var types = objectType.GenericTypeArguments; // note(cosborn) [TLeft, TRight]
            if (dynamicValue.IsRight)
            {
                serializer.Serialize(writer, dynamicValue.Value, types[1]);
                return;
            }

            var typeInfo = objectType.GetTypeInfo();
            var leftValueField = typeInfo.GetDeclaredField("_leftValue") ??
                throw new JsonWriterException(ThisIsABug);

            serializer.Serialize(writer, leftValueField.GetValue(value), types[0]);
        }

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">CanRead is false.</exception>
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer) => throw new NotSupportedException("CanRead is false.");
    }
}
