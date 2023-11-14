//-----------------------------------------------------------------------
// <copyright file="AzureKeyVaultSettings.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects.AppSettings
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Class for Azure key vault settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AzureKeyVaultSettings
    {
        /// <summary>
        /// Gets or sets vault URI.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Ok>")]
        public string VaultURI { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets tenant id.
        /// </summary>
        public string TenantId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets client id.
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets client secret.
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets key name.
        /// </summary>
        public string KeyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets key id.
        /// </summary>
        public string KeyId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets data protection key id.
        /// </summary>
        public string DataProtectionKeyId { get; set; } = string.Empty;
    }
}