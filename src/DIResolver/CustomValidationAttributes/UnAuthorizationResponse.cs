// -----------------------------------------------------------------------
// <copyright file="UnAuthorizationResponse.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
// -----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.CustomValidationAttributes
{
    using System.Collections.Generic;
    using System.Net;
    using Syncfusion.HelpDesk.Core.ValidationErrors;

    /// <summary>
    /// UnAuthorizationResponse
    /// </summary>
    public class UnAuthorizationResponse : ValidationError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnAuthorizationResponse"/> class.
        /// Unauthorozation
        /// </summary>
        public UnAuthorizationResponse()
        {
            StatusCode = (int)HttpStatusCode.Unauthorized;
            Errors = new List<FieldErrors>();
            Message = string.Empty;
        }
    }
}