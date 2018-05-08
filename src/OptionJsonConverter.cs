// <copyright file="OptionJsonConverter.cs" company="Cimpress, Inc.">
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
            objectType?.IsConstructedGenericType == true
            && objectType.GetGenericTypeDefinition() == typeof(Option<>);

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
            var ctor = typeInfo.DeclaredConstructors.Single(
                c => c.GetParameters().Length == 1 && c.GetParameters().Single().ParameterType == underlyingType);

            return Option.From(reader.ValueType)
                         .Bind(_ => Option.From(serializer.Deserialize(reader, underlyingType)))
                         .Map(v => ctor.Invoke(new[] { v }))
                         .GetValueOrDefault(() => Activator.CreateInstance(objectType));
        }
    }
}
