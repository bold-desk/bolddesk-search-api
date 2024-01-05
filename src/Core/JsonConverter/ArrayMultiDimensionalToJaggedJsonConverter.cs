// -----------------------------------------------------------------------
// <copyright file="ArrayMultiDimensionalToJaggedJsonConverter.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace Syncfusion.HelpDesk.Core.JsonConverter;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Syncfusion.HelpDesk.Core.Extensions;

/// <summary>
/// Multi-Dimensional Array To Jagged Array Json Converter Class.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1814:Prefer jagged arrays over multidimensional", Justification = "Ok")]
public class ArrayMultiDimensionalToJaggedJsonConverter : JsonConverter<long[,]>
{
    /// <summary>
    /// Reads and converts the JSON to type.
    /// </summary>
    /// <param name="reader">JSON Reader.</param>
    /// <param name="typeToConvert">Type To Convert.</param>
    /// <param name="options">Json Serializer Options.</param>
    /// <returns>Return the value in DateTime.</returns>
    public override long[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Writes a specified value as JSON by converting Multi-dimensional array[,] to Jagged array[][].
    /// </summary>
    /// <param name="writer">JSON Writer.</param>
    /// <param name="value">Multi-dimensional array value.</param>
    /// <param name="options">Json Serializer Options.</param>
    public override void Write(Utf8JsonWriter writer, long[,] value, JsonSerializerOptions options)
    {
        if (writer is null)
        {
            return;
        }

        var jaggedArray = ConvertMultiDimensionalToJaggedArray(value);
        writer.WriteStringValue(JsonSerializer.Serialize(jaggedArray));
    }

    private long[][] ConvertMultiDimensionalToJaggedArray(long[,] multiArray)
    {
        if (multiArray?.Length > 0)
        {
            var jaggedArray = new long[multiArray.GetLength(0)][];

            for (int outer = multiArray.GetLowerBound(0); outer <= multiArray.GetUpperBound(0); outer++)
            {
                jaggedArray[outer] = new long[multiArray.GetLength(1)];

                for (int inner = multiArray.GetLowerBound(1); inner <= multiArray.GetUpperBound(1); inner++)
                {
                    jaggedArray[outer][inner] = multiArray.GetValue(outer, inner).ToLong();
                }
            }

            return jaggedArray;
        }

        return Array.Empty<long[]>();
    }
}
