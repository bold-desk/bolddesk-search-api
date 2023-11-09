//-----------------------------------------------------------------------
// <copyright file="AzureBlobSettings.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects.AppSettings;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Class for Azure key vault settings.
/// </summary>
[ExcludeFromCodeCoverage]
public class AzureBlobSettings
{
    /// <summary>
    /// Gets or sets blob connection string.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Ok>")]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets blob name.
    /// </summary>
    public string BlobName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets container name.
    /// </summary>
    public string ContainerName { get; set; } = string.Empty;
}
