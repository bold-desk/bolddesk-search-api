// -----------------------------------------------------------------------
// <copyright file="ApiBadRequestResponse.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.CustomValidationAttributes;

using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Syncfusion.HelpDesk.Core.Utilities;
using Syncfusion.HelpDesk.Core.ValidationErrors;

/// <summary>
/// Custom Bad Request class.
/// </summary>
public class ApiBadRequestResponse : ValidationError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiBadRequestResponse"/> class.
    /// </summary>
    /// <param name="modelState">Model State.</param>
    public ApiBadRequestResponse(ModelStateDictionary modelState)
    {
        if (modelState?.IsValid != false)
        {
            return;
        }

#pragma warning disable CA1303 // Do not pass literals as localized parameters
        Message = "Validation Failed";
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        StatusCode = (int)HttpStatusCode.BadRequest;

        Errors = new System.Collections.Generic.List<FieldErrors>();

        foreach (var keyModelStatePair in modelState)
        {
            var key = keyModelStatePair.Key;

            if (!string.IsNullOrWhiteSpace(keyModelStatePair.Key))
            {
                key = keyModelStatePair.Key.ToLowerFirstChar();
            }

            var errors = keyModelStatePair.Value.Errors;
            if (errors?.Count > 0)
            {
               var errorMessage = GetErrorMessage(errors[0]);
               Errors.Add(new FieldErrors(key, errorMessage));
            }
        }
    }

    private string GetErrorMessage(ModelError error)
    {
        if (!string.IsNullOrEmpty(error.ErrorMessage) && error.ErrorMessage.Contains("field is required", System.StringComparison.InvariantCulture))
        {
            return "This field is required.";
        }

        return string.IsNullOrEmpty(error.ErrorMessage) ?
            "Validation Failed." :
        error.ErrorMessage;
    }
}
