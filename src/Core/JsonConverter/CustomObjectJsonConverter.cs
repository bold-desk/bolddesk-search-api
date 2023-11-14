// -----------------------------------------------------------------------
// <copyright file="CustomObjectJsonConverter.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace Syncfusion.HelpDesk.Agent.Core.JsonConverter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Syncfusion.HelpDesk.Core.Extensions;

    /// <summary>
    /// Custom Object JsonConverter Class.
    /// </summary>
    /// <typeparam name="T">Type Parameter.</typeparam>
    public class CustomObjectJsonConverter<T> : JsonConverter<T>
        where T : new()
    {
        /// <summary>
        /// Reads and converts the JSON to type T.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var objInstance = Activator.CreateInstance(typeToConvert);
            var result = ReadValue(ref reader, typeToConvert, options, objInstance);
            return result != null ? (T)result : new T();
        }

        /// <summary>
        /// Writes a specified value as JSON.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }

        private object? ReadValue(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, object? objInstance)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType.ToString()}, only objects are supported");
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return objInstance;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();

                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    throw new JsonException();
                }

                reader.Read();

                var propertyInfo = objInstance?.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                var value = GetObjectValue(ref reader, typeToConvert, options, propertyInfo?.PropertyType);
                propertyInfo?.SetValue(objInstance, value);
            }

            return objInstance;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "Suppression here is OK.")]
        private object? GetObjectValue(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, Type? property)
        {
            Type propertyType = property ?? typeof(string);

            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    {
                        var value = reader.GetString();

                        if (property == null)
                        {
                            return value;
                        }

                        if (IsTypeMatching(property, typeof(DateTime)) && value.TryParseToDateTime(out DateTime dateValue))
                        {
                            return dateValue;
                        }
                        else if (IsTypeMatching(propertyType, typeof(int?)) && value.TryParseToInt(out int intValue))
                        {
                            return intValue;
                        }
                        else if (IsTypeMatching(propertyType, typeof(long?)) && value.TryParseToLong(out long longValue))
                        {
                            return longValue;
                        }
                        else if (IsTypeMatching(propertyType, typeof(bool)) && value.TryParseToBoolean(out bool boolValue))
                        {
                            return boolValue;
                        }
                        else if (IsTypeMatching(propertyType, typeof(decimal)) && value.TryParseToDecimal(out decimal decimalValue))
                        {
                            return decimalValue;
                        }
                        else
                        {
                            return value;
                        }
                    }

                case JsonTokenType.False:
                    return false;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.Number:
                    {
                        if (property != null)
                        {
                            if (IsTypeMatching(propertyType, typeof(int)) && reader.TryGetInt32(out int intValue))
                            {
                                return intValue;
                            }

                            if (IsTypeMatching(propertyType, typeof(long)) && reader.TryGetInt64(out long longValue))
                            {
                                return longValue;
                            }
                        }

                        return reader.GetDecimal();
                    }

                case JsonTokenType.StartObject:
                    {
                        var objInstance = Activator.CreateInstance(propertyType);
                        return ReadValue(ref reader, typeToConvert, options, objInstance);
                    }

                case JsonTokenType.StartArray:
                    {
                        var argumentType = propertyType.GenericTypeArguments;
                        Type listBaseType = (property != null && argumentType.Length > 0) ? (argumentType.FirstOrDefault() ?? typeof(object)) : typeof(object);
                        var listDataType = typeof(List<>);
                        var listGenericType = listDataType.MakeGenericType(listBaseType);
                        var listInstance = Activator.CreateInstance(listGenericType);
                        IList? list = listInstance != null ? (IList)listInstance : null;

                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            var value = GetObjectValue(ref reader, typeToConvert, options, listBaseType);
                            if (value != null && list != null)
                            {
                                list.Add(value);
                            }
                        }

                        return list;
                    }

                default:
                    throw new JsonException($"'{reader.TokenType.ToString()}' is not supported");
            }
        }

        private bool IsTypeMatching(Type propertyType, Type standardDataType)
        {
            if ((propertyType == typeof(DateTime) || propertyType == typeof(DateTime?)) && standardDataType == typeof(DateTime))
            {
                return true;
            }
            else if ((propertyType == typeof(int) || propertyType == typeof(int?)) && standardDataType == typeof(int))
            {
                return true;
            }
            else if ((propertyType == typeof(long) || propertyType == typeof(long?)) && standardDataType == typeof(long))
            {
                return true;
            }
            else if ((propertyType == typeof(bool) || propertyType == typeof(bool?)) && standardDataType == typeof(bool))
            {
                return true;
            }
            else if ((propertyType == typeof(decimal) || propertyType == typeof(decimal?)) && standardDataType == typeof(decimal))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}