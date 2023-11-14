//-----------------------------------------------------------------------
// <copyright file="CORSPolicy.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects.AppSettings
{
    using System;

    /// <summary>
    /// CORSPolicy class.
    /// </summary>
    public class CORSPolicy
    {
        /// <summary>
        /// Gets or sets Whitelisted domains for CORS.
        /// </summary>
        public string[] WhitelistedDomains { get; set; } = Array.Empty<string>();
    }
}