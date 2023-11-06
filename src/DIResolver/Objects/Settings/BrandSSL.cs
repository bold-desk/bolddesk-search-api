//-----------------------------------------------------------------------
// <copyright file="BrandSSL.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects.Settings;

/// <summary>
/// BrandSSL - Class Declaration.
/// </summary>
public class BrandSSL
{
    /// <summary>
    /// Gets or sets Api Key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Api URL.
    /// </summary>
    public string ApiURL { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether is enabled or not.
    /// </summary>
    public bool IsEnabled { get; set; }
}
