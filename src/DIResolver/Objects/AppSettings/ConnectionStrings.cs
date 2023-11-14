//-----------------------------------------------------------------------
// <copyright file="ConnectionStrings.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects.AppSettings
{
    /// <summary>
    /// Connection Strings class.
    /// </summary>
    public class ConnectionStrings
    {
        /// <summary>
        /// Gets or sets.
        /// </summary>
        public string CatalogEntity { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets automation entity.
        /// </summary>
        public string AutomationEntity { get; set; } = string.Empty;
    }
}