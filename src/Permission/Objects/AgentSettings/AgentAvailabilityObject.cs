//-----------------------------------------------------------------------
// <copyright file="AgentAvailabilityObject.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Objects
{
    /// <summary>
    /// Agent Availability Object.
    /// </summary>
    public class AgentAvailabilityObject
    {
        /// <summary>
        /// Gets or sets Support Channel Status Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Support Channel Status Name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Status Category Id.
        /// </summary>
        public int StatusCategoryId { get; set; }

        /// <summary>
        /// Gets or sets Color Code.
        /// </summary>
        public string? ColorCode { get; set; }

        /// <summary>
        /// Gets or sets Status Category.
        /// </summary>
        public string StatusCategory { get; set; } = string.Empty;
    }
}
