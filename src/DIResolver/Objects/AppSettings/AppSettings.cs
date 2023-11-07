//-----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects.AppSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Class for App Settings.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Gets or Sets API Route Prefix.
    /// </summary>
    public string ApiRoutePrefix { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets enable https value.
    /// </summary>
    public string EnableHTTPS { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets enable swagger value.
    /// </summary>
    public string EnableSwagger { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Encryption settings.
    /// </summary>
    public EncryptionSettings EncryptionSettings { get; set; } = new EncryptionSettings();

    /// <summary>
    /// Gets or sets Identity Server Configuration.
    /// </summary>
    public IdentityServerConfiguration IdentityServerConfiguration { get; set; } = new IdentityServerConfiguration();
}
