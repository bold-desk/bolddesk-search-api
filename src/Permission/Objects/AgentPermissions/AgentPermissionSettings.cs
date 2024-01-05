//-----------------------------------------------------------------------
// <copyright file="AgentPermissionSettings.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Objects
{
    using System.Collections.Generic;

    /// <summary>
    /// AgentPermissionSettings - Class Declaration.
    /// </summary>
    public class AgentPermissionSettings
    {
        /// <summary>
        /// Gets or sets the list of agent permission
        /// </summary>
        public List<int> Permissions { get; set; } = new List<int>();
    }
}
