//-----------------------------------------------------------------------
// <copyright file="AgentSettings.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Objects
{
    using System.Collections.Generic;

    /// <summary>
    /// Agent settings class
    /// </summary>
    public class AgentSettings
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
        /// Gets or sets a value indicating whether agent is available or not
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether agent has all brand access or not
        /// </summary>
        public bool HasAllBrandAccess { get; set; }

        /// <summary>
        /// Gets or sets the ticket access scope id
        /// </summary>
        public int TicketAccessScopeId { get; set; }

        /// <summary>
        /// Gets or sets Ticket Fields.
        /// </summary>
        public string? TicketFields { get; set; }

        /// <summary>
        /// Gets or sets TimeZone ID.
        /// </summary>
        public int? TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets TimeZone offset.
        /// </summary>
        public string? TimeZoneOffset { get; set; }

        /// <summary>
        /// Gets or sets TimeZone Name.
        /// </summary>
        public string? TimeZoneName { get; set; }

        /// <summary>
        /// Gets or sets TimeZone short code.
        /// </summary>
        public string? TimeZoneShortCode { get; set; }

        /// <summary>
        /// Gets or sets IANA time zone name.
        /// </summary>
        public string? IANATimeZoneName { get; set; }

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
        public int? DefaultTicketViewId { get; set; }

        /// <summary>
        /// Gets or sets ticket per page count.
        /// </summary>
        public int? TicketPerPageCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether keyboard shortcut enabled or not .
        /// </summary>
        public bool? EnableShortcut { get; set; } = true;

        /// <summary>
        /// Gets or sets Brands.
        /// </summary>
        public string? Brands { get; set; }

        /// <summary>
        /// Gets or sets default message Filter ID.
        /// </summary>
        public int? DefaultMessageFilterId { get; set; }

        /// <summary>
        /// Gets or sets Language short code.
        /// </summary>
        public string? LanguageShortCode { get; set; }

        /// <summary>
        /// Gets or sets Agent Availability Status.
        /// </summary>
        public AgentAvailabilityStatusObject AgentAvailabilityStatus { get; set; } = new AgentAvailabilityStatusObject();
    }
}
