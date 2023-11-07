//-----------------------------------------------------------------------
// <copyright file="AgentSettingsFromQuery.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Objects
{
    /// <summary>
    /// Agent settings from query class
    /// </summary>
    public class AgentSettingsFromQuery
    {
        /// <summary>
        /// Gets or sets Display name.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets email.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Short code.
        /// </summary>
        public string ShortCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Color Code.
        /// </summary>
        public string ColorCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether agent has all brand access or not
        /// </summary>
        public bool HasAllBrandAccess { get; set; }

        /// <summary>
        /// Gets or sets the agent ticket access scope id
        /// </summary>
        public int AgentTicketAccessScopeId { get; set; }

        /// <summary>
        /// Gets or sets Ticket Fields.
        /// </summary>
        public string? TicketFields { get; set; }

        /// <summary>
        /// Gets or sets Timezone ID.
        /// </summary>
        public int? TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets Timezone offset.
        /// </summary>
        public string? TimeZoneOffset { get; set; }

        /// <summary>
        /// Gets or sets Timezone Name.
        /// </summary>
        public string? TimeZoneName { get; set; }

        /// <summary>
        /// Gets or sets Timezone short code.
        /// </summary>
        public string? TimeZoneShortCode { get; set; }

        /// <summary>
        /// Gets or sets Windows time zone identifier.
        /// </summary>
        public string? WindowsTimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets Language Id.
        /// </summary>
        public int? LanguageId { get; set; }

        /// <summary>
        /// Gets or sets Ticket Layout ID.
        /// </summary>
        public int? TicketLayoutId { get; set; }

        /// <summary>
        /// Gets or sets Sort Reply ID.
        /// </summary>
        public int? SortReplyId { get; set; }

        /// <summary>
        /// Gets or sets default ticket view ID.
        /// </summary>
        public string? DefaultTicketViewId { get; set; }

        /// <summary>
        /// Gets or sets ticket per page count.
        /// </summary>
        public string? TicketPerPageCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether keyboard shortcut enabled or not .
        /// </summary>
        public bool? EnableShortcut { get; set; }

        /// <summary>
        /// Gets or sets Brands.
        /// </summary>
        public string? Brands { get; set; }

        /// <summary>
        /// Gets or sets default message Filter ID.
        /// </summary>
        public string? DefaultMessageFilterId { get; set; }

        /// <summary>
        /// Gets or sets Language short code.
        /// </summary>
        public string? LanguageShortCode { get; set; }
    }
}
