//-----------------------------------------------------------------------
// <copyright file="OrganizationSearchSettings.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.Core.Objects.Settings
{
    using System;
    using Syncfusion.HelpDesk.Core.Objects;

    /// <summary>
    /// OrganizationSettingObjects - class declaration.
    /// </summary>
    public class OrganizationSearchSettings : GeneralCommonSettings
    {
        /// <summary>
        /// Gets or sets Portal Name.
        /// </summary>
        public string PortalName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Logo URL.
        /// </summary>
        public Uri? LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets Favicon URL.
        /// </summary>
        public Uri? FaviconUrl { get; set; }

        /// <summary>
        /// Gets or sets Logo Link back URL.
        /// </summary>
        public Uri? LogoLinkbackUrl { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether user can Control Availability Status.
        /// </summary>
        public bool CanUserControlAvailabilityStatus { get; set; }

        /// <summary>
        /// Gets or sets Brand Id.
        /// </summary>
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto assign ticket on solve.
        /// </summary>
        public bool AutoAssignTicketOnSolve { get; set; }

        /// <summary>
        /// Gets or sets Org Owner Id.
        /// </summary>
        public long OrgOwnerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether attachment multiple contact group required or not.
        /// </summary>
        public bool MultipleContactGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include Powered By or not.
        /// </summary>
        public bool IncludePoweredBy { get; set; }

        /// <summary>
        /// Gets or sets editor type Id.
        /// </summary>
        public short EditorTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value for default language.
        /// </summary>
        public string? DefaultLanguage { get; set; }

        /// <summary>
        /// Gets or sets a value for default language Name.
        /// </summary>
        public string? DefaultLanguageName { get; set; }

        /// <summary>
        /// Gets or sets a value for default language Id.
        /// </summary>
        public int DefaultLanguageId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multi language enabled or not.
        /// </summary>
        public bool IsMultiLanguageEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value for additional language.
        /// </summary>
        public string? SupportedLanguages { get; set; }

        /// <summary>
        /// Gets or sets a value for customer portal enabled language.
        /// </summary>
        public string? CustomerPortalEnabledLanguages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multi language enabled once or not.
        /// </summary>
        public bool IsMultiLanguageEnabledOnce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSeoMandatory is enabled or not.
        /// </summary>
        public bool IsSeoMandatory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Is Auto Save Enabled is enabled or not.
        /// </summary>
        public bool IsAutoSaveEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets CC Settings for an Organization.
        /// </summary>
        public CCSettingsObject CcConfiguration { get; set; } = new CCSettingsObject();

        /// <summary>
        /// Gets or sets Organization Id.
        /// </summary>
        public int OrgId { get; set; }
    }
}
