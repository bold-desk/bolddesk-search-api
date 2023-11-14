//-----------------------------------------------------------------------
// <copyright file="GeneralCommonSettings.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Core.Objects.Settings
{
    using System;

    /// <summary>
    /// General and ticket common settings.
    /// </summary>
    public class GeneralCommonSettings
    {
        /// <summary>
        /// Gets or sets TimeZone ID.
        /// </summary>
        public int TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets Timezone offset.
        /// </summary>
        public string TimeZoneOffset { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Timezone name.
        /// </summary>
        public string TimeZoneName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Timezone short code.
        /// </summary>
        public string TimeZoneShortCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets IANA time zone name.
        /// </summary>
        public string IANATimeZoneName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Date Format Id.
        /// </summary>
        public int DateFormatId { get; set; }

        /// <summary>
        /// Gets or sets DateFormat.
        /// </summary>
        public string DateFormat { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Time Format Id.
        /// </summary>
        public int TimeFormatId { get; set; }

        /// <summary>
        /// Gets or sets TimeFormat.
        /// </summary>
        public string TimeFormat { get; set; } = string.Empty;

        // Property disabled as no support provided in localization.
        ///// <summary>
        ///// Gets or sets Language.
        ///// </summary>
        // public string Language { get; set; } = string.Empty;

        ///// <summary>
        ///// Gets or sets Language Id.
        ///// </summary>
        // public int LanguageId { get; set; }

        /// <summary>
        ///  Gets or sets the UploadFileSize.
        /// </summary>
        public long UploadFileSize { get; set; }

        /// <summary>
        ///  Gets or sets the AllowedFileExtensions.
        /// </summary>
        public string AllowedFileExtensions { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the CdnUrl.
        /// </summary>
        public Uri? CdnUrl { get; set; }
    }
}