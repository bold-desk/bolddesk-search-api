//-----------------------------------------------------------------------
// <copyright file="ServiceBusSettings.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Objects
{
    /// <summary>
    /// Service Bus Settings Class.
    /// </summary>
    public class ServiceBusSettings
    {
        /// <summary>
        /// Gets or sets Connection String.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets Version.
        /// </summary>
        public string Version { get; set; } = string.Empty;
    }
}