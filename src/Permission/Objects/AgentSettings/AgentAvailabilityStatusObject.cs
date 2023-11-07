// -----------------------------------------------------------------------
// <copyright file="AgentAvailabilityStatusObject.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Permission.Objects
{
    /// <summary>
    /// Agent Availability Status Object.
    /// </summary>
    public class AgentAvailabilityStatusObject
    {
        /// <summary>
        /// Gets or sets Agent Support Channel Availability
        /// </summary>
        public AgentAvailabilityObject AgentSupportChannelAvailability { get; set; } = new AgentAvailabilityObject();
    }
}
