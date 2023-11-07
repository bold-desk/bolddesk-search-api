// -----------------------------------------------------------------------
// <copyright file="UserTimeZoneInfo.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.Core.Objects;

using System;
using System.Globalization;
using Syncfusion.HelpDesk.Multitenant;

/// <summary>
/// User Time Zone Info Class.
/// </summary>
public class UserTimeZoneInfo
{
    /// <summary>
    /// Gets or sets time zone offset value as <see cref="TimeSpan"/> which represent user time zone offset value if preferred any; otherwise organization time zone offset value.
    /// </summary>
    public TimeSpan UtcOffset { get; set; }

    /// <summary>
    /// Gets or sets time zone offset value which represent user time zone offset value if preferred any; otherwise organization time zone offset value.
    /// </summary>
    public string TimeZoneOffset { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets IANA time zone name which represent user IANA time zone name if preferred any; otherwise organization IANA time zone name.
    /// </summary>
    public string IANATimeZoneName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether time zone conversion needed or not.
    /// </summary>
    public bool IsTimeZoneConversionNeeded { get; set; }
}
