// -----------------------------------------------------------------------
// <copyright file="ModelValidationAttribute.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.CustomValidationAttributes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

/// <summary>
/// Model Validation Attribute Filter.
/// </summary>
public class ModelValidationAttribute : ActionFilterAttribute
{
    /// <inheritdoc/>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context == null || context.ModelState?.IsValid != false)
        {
            return;
        }

        context.Result = new BadRequestObjectResult(new ApiBadRequestResponse(context.ModelState));
    }
}
