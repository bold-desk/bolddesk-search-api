//-----------------------------------------------------------------------
// <copyright file="FluentValidationExceptionHandlerMiddleware.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Middleware
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using BoldDesk.Search.Core.Extensions;
    using BoldDesk.Search.Localization.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Localization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Syncfusion.HelpDesk.Core.Localization;
    using Syncfusion.HelpDesk.Core.ValidationErrors;

    /// <summary>
    /// Custom Exception Handler Middleware class.
    /// </summary>
    public class FluentValidationExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidationExceptionHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next">Request Delegate Value.</param>
        public FluentValidationExceptionHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// Invoke Method.
        /// </summary>
        /// <param name="context">Http Context.</param>
        /// <param name="localizer">Base String Localizer value.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context, ILocalizer localizer)
        {
            try
            {
                await next(context).ConfigureAwait(false);
            }
            catch (FluentValidation.ValidationException ex)
            {
                await FluentValidationExceptionHandlerAsync(context, ex, localizer).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Custom Exception Handler Async.
        /// </summary>
        /// <param name="context">Http Context.</param>
        /// <param name="exception">Exception Details.</param>
        /// <param name="localizer">Base String Localizer value.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">If content is null - Exception</exception>
        private Task FluentValidationExceptionHandlerAsync(HttpContext? context, FluentValidation.ValidationException exception, ILocalizer? localizer)
        {
            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonResult = JsonConvert.SerializeObject(
                 new ValidationError
                 {
                     Message = localizer?.GetBaseLocalizerValue(SharedResourceConstants.ValidationFailed) ?? string.Empty,
                     Errors = exception.Errors.ToList().ToFieldErrors(),
                     StatusCode = (int)HttpStatusCode.BadRequest
                 },
                 new JsonSerializerSettings
                 {
                     ContractResolver = contractResolver
                 });

            return context.Response.WriteAsync(jsonResult);
        }
    }
}