//-----------------------------------------------------------------------
// <copyright file="DateTimeToDateTimeOffsetJsonSerializerConverter.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace Syncfusion.HelpDesk.Core.JsonConverter;

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Syncfusion.HelpDesk.Core.Utilities;

/// <summary>
/// Date Time to Date Time Offset Json Serializer Converter class.
/// </summary>
public class DateTimeToDateTimeOffsetJsonSerializerConverter : JsonConverter<DateTime>
{
    private readonly TimeSpan offset;
    private readonly bool isTimeZoneConversionNeeded;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeToDateTimeOffsetJsonSerializerConverter"/> class.
    /// </summary>
    /// <param name="offset">Time difference between the user preferred time zone and Coordinated Universal Time (UTC).</param>
    /// <param name="isTimeZoneConversionNeeded">Time Zone Conversion Needed.</param>
    public DateTimeToDateTimeOffsetJsonSerializerConverter(in TimeSpan offset, bool isTimeZoneConversionNeeded)
    {
        this.offset = offset;
        this.isTimeZoneConversionNeeded = isTimeZoneConversionNeeded;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeToDateTimeOffsetJsonSerializerConverter"/> class.
    /// </summary>
    /// <param name="offset">Time difference between the user preferred time zone and Coordinated Universal Time (UTC).</param>
    /// <param name="isTimeZoneConversionNeeded">Time Zone Conversion Needed.</param>
    public DateTimeToDateTimeOffsetJsonSerializerConverter(string offset, bool isTimeZoneConversionNeeded)
    {
        this.offset = offset.ToUtcOffset();
        this.isTimeZoneConversionNeeded = isTimeZoneConversionNeeded;
    }

    /// <summary>
    /// Reads and converts the JSON to type.
    /// </summary>
    /// <param name="reader">JSON Reader.</param>
    /// <param name="typeToConvert">Type To Convert.</param>
    /// <param name="options">Json Serializer Options.</param>
    /// <returns>Return the value in DateTime.</returns>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(DateTime), "The value must in DateTime type.");
        return DateTime.Parse(reader.GetString(), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Writes a specified value as JSON.
    /// </summary>
    /// <param name="writer">JSON Writer.</param>
    /// <param name="value">Date Time value.</param>
    /// <param name="options">Json Serializer Options.</param>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        if (writer is null)
        {
            return;
        }

        if (isTimeZoneConversionNeeded)
        {
            writer.WriteStringValue(value.ToUniversalDateTimeFormat());
        }
        else
        {
            writer.WriteStringValue(new DateTimeOffset(new DateTime(value.Ticks, DateTimeKind.Unspecified), offset));
        }
    }
}
