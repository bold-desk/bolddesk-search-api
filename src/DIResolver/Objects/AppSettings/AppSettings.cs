//-----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects.AppSettings;

using Syncfusion.HelpDesk.Multitenant;
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
    /// Gets or sets security headers log value.
    /// </summary>
    public string EnableRequestHeadersLogging { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Encryption settings.
    /// </summary>
    public EncryptionSettings EncryptionSettings { get; set; } = new EncryptionSettings();

    /// <summary>
    /// Gets or sets.
    /// </summary>
    public ExceptionLessObjects ExceptionLess { get; set; } = new ExceptionLessObjects();

    /// <summary>
    /// Gets or sets Syncfusion License.
    /// </summary>
    public SyncfusionLicenseObjects SyncfusionLicense { get; set; } = new SyncfusionLicenseObjects();

    /// <summary>
    /// Gets or sets Identity Server Configuration.
    /// </summary>
    public IdentityServerConfiguration IdentityServerConfiguration { get; set; } = new IdentityServerConfiguration();

    /// <summary>
    /// Gets or sets OAuth Client Id value.
    /// </summary>
    public string OAuthClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets OAuthValidAudience.
    /// </summary>
    public string OAuthValidAudience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets testing domain value.
    /// </summary>
    public string TestingDomain { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets.
    /// </summary>
    public string RedisConnection { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets azure vault settings.
    /// </summary>
    public AzureKeyVaultSettings AzureKeyVaultSettings { get; set; } = new AzureKeyVaultSettings();

    /// <summary>
    /// Gets or sets azure blob settings.
    /// </summary>
    public AzureBlobSettings AzureBlobSettings { get; set; } = new AzureBlobSettings();

    /// <summary>
    /// Gets or Sets Whitelisted IP.
    /// </summary>
    public WhitelistedIPDetails WhitelistedIP { get; set; } = new WhitelistedIPDetails();

    /// <summary>
    /// Gets or sets.
    /// </summary>
    public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
}
