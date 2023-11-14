// -----------------------------------------------------------------------
// <copyright file="CustomBadRequest.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.CustomValidationAttributes
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Syncfusion.HelpDesk.Core.Utilities;

    /// <summary>
    /// Custom Bad Request class.
    /// </summary>
    public class CustomBadRequest : ValidationProblemDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomBadRequest"/> class.
        /// </summary>
        /// <param name="context">Action context.</param>
        public CustomBadRequest(ActionContext context)
        {
            if (context == null)
            {
                return;
            }

            Title = "Validation failed";
            Detail = "The inputs supplied to the API are invalid";
            Status = 400;
            ConstructErrorMessages(context);
            Type = context.HttpContext.TraceIdentifier;
        }

        private void ConstructErrorMessages(ActionContext context)
        {
            foreach (var keyModelStatePair in context.ModelState)
            {
                var key = keyModelStatePair.Key;

                if (!string.IsNullOrWhiteSpace(keyModelStatePair.Key))
                {
                    key = keyModelStatePair.Key.ToLowerFirstChar();
                }

                var errors = keyModelStatePair.Value.Errors;
                if (errors?.Count > 0)
                {
                    if (errors.Count == 1)
                    {
                        var errorMessage = GetErrorMessage(errors[0]);
                        Errors.Add(key, new string[] { errorMessage });
                    }
                    else
                    {
                        var errorMessages = new string[errors.Count];
                        for (var i = 0; i < errors.Count; i++)
                        {
                            errorMessages[i] = GetErrorMessage(errors[i]);
                        }

                        Errors.Add(key, errorMessages);
                    }
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
}