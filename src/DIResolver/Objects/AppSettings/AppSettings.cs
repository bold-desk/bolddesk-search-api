//-----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects.AppSettings;

using BoldDesk.Search.DIResolver.Objects.Settings;
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
    /// Gets or sets Encryption settings.
    /// </summary>
    public EncryptionSettings EncryptionSettings { get; set; } = new EncryptionSettings();
}
