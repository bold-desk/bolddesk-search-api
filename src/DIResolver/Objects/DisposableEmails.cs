//-----------------------------------------------------------------------
// <copyright file="DisposableEmails.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects;

using System.Collections.Generic;

/// <summary>
/// Class Declaration - Disposable Emails
/// </summary>
public class DisposableEmails
{
    /// <summary>
    /// Gets or sets Email.
    /// </summary>
    public List<string> BlackListEmails { get; set; } = new List<string>();
}