// -----------------------------------------------------------------------
// <copyright file="IdentityServerConfiguration.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects.AppSettings
{
    /// <summary>
    /// Id server configuration.
    /// </summary>
    public class IdentityServerConfiguration
    {
        /// <summary>
        /// Gets or sets authority.
        /// </summary>
        public string? Authority { get; set; }

        /// <summary>
        /// Gets or sets authorizationUrl.
        /// </summary>
        public string? AuthorizationUrl { get; set; }

        /// <summary>
        /// Gets or sets token url.
        /// </summary>
        public string? TokenUrl { get; set; }

        /// <summary>
        /// Gets or sets OpenId Configuration url.
        /// </summary>
        public string? OpenIdConfigurationUrl { get; set; }
    }
}