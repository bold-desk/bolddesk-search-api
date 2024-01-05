//-----------------------------------------------------------------------
// <copyright file="IPermissionService.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Authorization
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BoldDesk.Permission.Objects;

    /// <summary>
    /// Permission service interface
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// Get the list of settings available for specific user.
        /// </summary>
        /// <returns>Returns list of settings available for specific user.</returns>
        Task<AgentSettings> GetAgentSettingsAsync();

        /// <summary>
        /// Get the list of permission available for specific user.
        /// </summary>
        /// <param name="currentUserId">Current User Id.</param>
        /// <returns>Returns list of permission available for specific user.</returns>
        Task<AgentPermissionSettings> GetAgentPermissionsAsync(long? currentUserId = null);

        /// <summary>
        /// Get the list of permission available based on role or user
        /// </summary>
        /// <param name="roleId">Role id when it role based</param>
        /// <param name="requestType">Request type to get the permission list. It may be user based or role based.</param>
        /// <param name="userId">User Id</param>
        /// <returns>Returns list of permission available based on role or user.</returns>
        Task<List<PermissionListObject>> GetPermissionsAsync(int? roleId = null, string? requestType = null, long? userId = null);

        /// <summary>
        /// Method to check the user's permission
        /// </summary>
        /// <param name="permissionIds">Permission Ids to check.</param>
        /// <param name="hasAllPermissionCheck">Boolean value to specify has all permission check</param>
        /// <param name="currentUserId">Current User Id.</param>
        /// <returns>Returns true if user has that permission else false.</returns>
        Task<bool> HasPermissionAsync(int[] permissionIds, bool hasAllPermissionCheck = false, long? currentUserId = null);

        /// <summary>
        /// Check whether requested user has access to reply ticket from email or not
        /// </summary>
        /// <param name="userId">User ID to check access</param>
        /// <param name="permissionId">Permission ID to check access</param>
        /// <returns>User availability</returns>
        Task<bool> CheckUserAccessToReplyTicket(long userId, int permissionId);

        /// <summary>
        /// Checking impersonating agent has any permission from admin module
        /// </summary>
        /// <param name="userId">Impersonating agent Id</param>
        /// <returns>Returns 'true' if impersonating agent has any one permission from admin module, else 'false'</returns>
        Task<bool> HasAnyAdminModulePermission(long userId);

        /// <summary>
        /// Method to check the user's permission and return matching permissions if any
        /// </summary>
        /// <param name="permissionIds">Permission Ids to check.</param>
        /// <param name="hasAllPermissionCheck">Boolean value to specify whether all/any permissions to check</param>
        /// <param name="currentUserId">Current User Id.</param>
        /// <returns>Returns true if user has that permission else false along with list of matching permissions if any.</returns>
        Task<(bool, List<int>?)> TryGetPermissionAsync(int[] permissionIds, bool hasAllPermissionCheck = false, long? currentUserId = null);
    }
}
