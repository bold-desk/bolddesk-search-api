//-----------------------------------------------------------------------
// <copyright file="HasAnyPermissionAsyncFilter.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Permission.Filters
{
    using System.Linq;
    using System.Threading.Tasks;
    using BoldDesk.Permission.Authorization;
    using BoldDesk.Permission.Enums;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Localization;
    using Syncfusion.HelpDesk.Core.Localization;
    using Syncfusion.HelpDesk.Core.ValidationErrors;

    /// <summary>
    /// Has Any Permission Filter.
    /// </summary>
    public class HasAnyPermissionAsyncFilter : IAsyncActionFilter
    {
        private readonly PermissionEnum[] permissions = System.Array.Empty<PermissionEnum>();
        private readonly IPermissionService permissionService;
        private readonly IStringLocalizer<SharedResource> baseLocalizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HasAnyPermissionAsyncFilter"/> class.
        /// </summary>
        /// <param name="permissions">Permissions that we need to check.</param>
        /// <param name="permissionService">Permission service.</param>
        /// <param name="baseLocalizer">Base localizer.</param>
        public HasAnyPermissionAsyncFilter(PermissionEnum[] permissions, IPermissionService permissionService, IStringLocalizer<SharedResource> baseLocalizer)
        {
            this.permissions = permissions;
            this.permissionService = permissionService;
            this.baseLocalizer = baseLocalizer;
        }

        /// <summary>
        /// On Action Execution.
        /// </summary>
        /// <param name="context">Action Executing Context.</param>
        /// <param name="next">Action Execution Delegate.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context == null || next == null)
            {
                return;
            }

            bool permissionResult = await permissionService.HasPermissionAsync(permissions.Cast<int>().ToArray()).ConfigureAwait(false);
            if (!permissionResult)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

                var errorList = new ValidationError(baseLocalizer[SharedResourceConstants.AccessDenied]);
                var accessDeniedError = new FieldErrors("UserId", baseLocalizer[SharedResourceConstants.AccessDenied], ErrorTypeEnum.AccessDenied);
                errorList.AddError(accessDeniedError);

                var result = new BadRequestObjectResult(errorList);
                result.StatusCode = (int)StatusCodes.Status403Forbidden;
                context.Result = result;
                return;
            }

            await next().ConfigureAwait(false);
        }
    }
}
