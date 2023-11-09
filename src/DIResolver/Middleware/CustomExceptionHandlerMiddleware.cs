//-----------------------------------------------------------------------
// <copyright file="CustomExceptionHandlerMiddleware.cs" company="Syncfusion Private Limited">
// Copyright (c) Syncfusion Private Limited. All rights reserved.
// </copyright>
// <author>Syncfusion Bold Desk Team</author>
//-----------------------------------------------------------------------

namespace BoldDesk.Search.DIResolver.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using BoldDesk.Search.Localization.Services;
    using Fluid.Parser;
    using Microsoft.AspNetCore.Cors.Infrastructure;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Syncfusion.HelpDesk.Core.CustomException;
    using Syncfusion.HelpDesk.Core.Enums;
    using Syncfusion.HelpDesk.Core.Extensions;
    using Syncfusion.HelpDesk.Core.Language;
    using Syncfusion.HelpDesk.Core.Localization;
    using Syncfusion.HelpDesk.Core.Objects;
    using Syncfusion.HelpDesk.Core.Objects.Hosting;
    using Syncfusion.HelpDesk.Core.Utilities;
    using Syncfusion.HelpDesk.Core.ValidationErrors;
    using Syncfusion.HelpDesk.Logger;
    using Syncfusion.HelpDesk.Multitenant;

    /// <summary>
    /// Custom Exception Handler Middleware class.
    /// </summary>
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ICorsService corsService;
        private readonly CorsOptions corsOptions;
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomExceptionHandlerMiddleware"/> class.
        /// </summary>
        /// <param name="next">Request Delegate Value.</param>
        /// <param name="corsService">Cors Service</param>
        /// <param name="corsOptions">Cors option</param>
        /// <param name="environment">Hosting Environment.</param>
        public CustomExceptionHandlerMiddleware(RequestDelegate next, ICorsService corsService, IOptions<CorsOptions> corsOptions, IWebHostEnvironment environment)
        {
            this.next = next;
            this.corsService = corsService;
            this.corsOptions = corsOptions?.Value ?? new CorsOptions();
            this.environment = environment;
        }

        /// <summary>
        /// Invoke Method.
        /// </summary>
        /// <param name="context">Http Context.</param>
        /// <param name="logs">Logger Service.</param>
        /// <param name="localizer">Localizer value.</param>
        /// <param name="requestDetails">Request Details.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context, ILogs logs, ILocalizer localizer, RequestDetails requestDetails)
        {
            if (localizer is null)
            {
                throw new ArgumentNullException(nameof(localizer));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (requestDetails is null)
            {
                throw new ArgumentNullException(nameof(requestDetails));
            }

            try
            {
                await next(context).ConfigureAwait(false);

                var cultureCode = GetCurrentRequestCultureCode(context);

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    await ErrorHandledExceptionHandlerAsync(context, HttpStatusCode.Unauthorized, localizer.GetBaseLocalizerValueForSpecifiedLanguage(cultureCode, SharedResourceConstants.Unauthorized), ErrorTypeEnum.Unauthorized, requestDetails, logs).ConfigureAwait(false);
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.TooManyRequests)
                {
                    await ErrorHandledExceptionHandlerAsync(context, HttpStatusCode.TooManyRequests, localizer.GetBaseLocalizerValueForSpecifiedLanguage(cultureCode, SharedResourceConstants.APICallExceeded), ErrorTypeEnum.APICallQuotaExceeded, requestDetails, logs).ConfigureAwait(false);
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    await ErrorHandledExceptionHandlerAsync(context, HttpStatusCode.NotFound, localizer.GetBaseLocalizerValueForSpecifiedLanguage(cultureCode, SharedResourceConstants.NotFound), ErrorTypeEnum.NotFound, requestDetails, logs).ConfigureAwait(false);
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.MethodNotAllowed)
                {
                    await ErrorHandledExceptionHandlerAsync(context, HttpStatusCode.MethodNotAllowed, localizer.GetBaseLocalizerValueForSpecifiedLanguage(cultureCode, SharedResourceConstants.MethodNotAllowed), ErrorTypeEnum.MethodNotAllowed, requestDetails, logs).ConfigureAwait(false);
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.UnsupportedMediaType)
                {
                    await ErrorHandledExceptionHandlerAsync(context, HttpStatusCode.UnsupportedMediaType, "Unsupported Media Type", ErrorTypeEnum.None, requestDetails, logs).ConfigureAwait(false);
                }
            }
            catch (NoChangeException ex)
            {
                await NoChangeExceptionHandlerAsync(context, ex, localizer).ConfigureAwait(false);
            }
            catch (ValidationException ex)
            {
                await ValidationExceptionHandlerAsync(context, ex, localizer).ConfigureAwait(false);
            }
            catch (AccessDeniedException ex)
            {
                await AccessDeniedExceptionAsync(context, ex, localizer).ConfigureAwait(false);
            }
            catch (SubscriptionQuotaCountExceededValidationException ex)
            {
                await SubscriptionQuotaCountExceededValidationExceptionHandlerAsync(context, ex, localizer).ConfigureAwait(false);
            }
            catch (FeatureNotSupportedException ex)
            {
                await FeatureNotSupportedExceptionHandlerAsync(context, ex, localizer).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await CustomExceptionHandlerAsync(context, ex, logs, localizer).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// No change Exception Handler Async.
        /// </summary>
        /// <param name="context">Http Context.</param>
        /// <param name="exception">Exception Details.</param>
        /// <param name="localizer">String Localizer value.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        private Task NoChangeExceptionHandlerAsync(HttpContext context, NoChangeException exception, ILocalizer localizer)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var cultureCode = GetCurrentRequestCultureCode(context);

            var jsonResult = JsonConvert.SerializeObject(
                 new ValidationError
                 {
                     Message = localizer.GetBaseLocalizerValueForSpecifiedLanguage(cultureCode, SharedResourceConstants.NoChanges),
                     Errors = new ValidationError().AddError(exception.FieldName, localizer.GetBaseLocalizerValueForSpecifiedLanguage(cultureCode, SharedResourceConstants.NoChanges), ErrorTypeEnum.InvalidValue),
                     StatusCode = (int)HttpStatusCode.BadRequest
                 },
                 new JsonSerializerSettings
                 {
                     ContractResolver = contractResolver
                 });

            return context.Response.WriteAsync(jsonResult);
        }

        /// <summary>
        /// Validation Error Exception Handler Async.
        /// </summary>
        /// <param name="context">Http Context.</param>
        /// <param name="exception">Exception Details.</param>
        /// <param name="localizer">String Localizer value.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        private Task ValidationExceptionHandlerAsync(HttpContext context, ValidationException exception, ILocalizer localizer)
        {
            var statusCode = exception.StatusCode.IsNotDefault() ? exception.StatusCode : (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var cultureCode = GetCurrentRequestCultureCode(context);

            var commonMessage = string.IsNullOrWhiteSpace(exception.CommonMessage) ? localizer.GetBaseLocalizerValueForSpecifiedLanguage(cultureCode, SharedResourceConstants.ValidationFailed) : exception.CommonMessage;
            var errorType = exception.ErrorType ?? ErrorTypeEnum.None;
            string jsonResult = string.Empty;

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            if (exception.FieldErrors == null)
            {
                if (string.IsNullOrWhiteSpace(exception.FieldName) && string.IsNullOrWhiteSpace(exception.ErrorMessage))
                {
                    jsonResult = JsonConvert.SerializeObject(
                                     new ValidationError
                                     {
                                         Message = commonMessage,
                                         Errors = null,
                                         StatusCode = statusCode
                                     },
                                     new JsonSerializerSettings
                                     {
                                         ContractResolver = contractResolver
                                     });
                }
                else
                {
                    jsonResult = JsonConvert.SerializeObject(
                                     new ValidationError
                                     {
                                         Message = commonMessage,
                                         Errors = new ValidationError().AddError(exception.FieldName, exception.ErrorMessage, errorType),
                                         StatusCode = statusCode
                                     },
                                     new JsonSerializerSettings
                                     {
                                         ContractResolver = contractResolver
                                     });
                }
            }
            else
            {
                jsonResult = JsonConvert.SerializeObject(
                 new ValidationError
                 {
                     Message = commonMessage,
                     Errors = new ValidationError().AddErrorList(exception.FieldErrors),
                     StatusCode = statusCode
                 },
                 new JsonSerializerSettings
                 {
                     ContractResolver = contractResolver
                 });
            }

            return context.Response.WriteAsync(jsonResult);
        }

        /// <summary>
        /// Access Denied Error Exception Handler Async.
        /// </summary>
        /// <param name="context">Http Context.</param>
        /// <param name="exception">Exception Details.</param>
        /// <param name="localizer">String Localizer value.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        private Task AccessDeniedExceptionAsync(HttpContext context, AccessDeniedException exception, ILocalizer localizer)
        {
            var statusCode = exception.StatusCode.IsNotDefault() ? exception.StatusCode : (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var commonMessage = localizer.GetBaseLocalizerValueForSpecifiedLanguage(GetCurrentRequestCultureCode(context), SharedResourceConstants.AccessDenied);
            var errorType = exception.ErrorType ?? ErrorTypeEnum.None;
            string jsonResult = string.Empty;

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            if (exception.FieldErrors == null)
            {
                if (string.IsNullOrWhiteSpace(exception.FieldName) && string.IsNullOrWhiteSpace(exception.ErrorMessage))
                {
                    jsonResult = JsonConvert.SerializeObject(
                                     new ValidationError
                                     {
                                         Message = commonMessage,
                                         Errors = null,
                                         StatusCode = statusCode
                                     },
                                     new JsonSerializerSettings
                                     {
                                         ContractResolver = contractResolver
                                     });
                }
                else
                {
                    jsonResult = JsonConvert.SerializeObject(
                                     new ValidationError
                                     {
                                         Message = commonMessage,
                                         Errors = new ValidationError().AddError(exception.FieldName, commonMessage, errorType),
                                         StatusCode = statusCode
                                     },
                                     new JsonSerializerSettings
                                     {
                                         ContractResolver = contractResolver
                                     });
                }
            }
            else
            {
                jsonResult = JsonConvert.SerializeObject(
                 new ValidationError
                 {
                     Message = commonMessage,
                     Errors = new ValidationError().AddErrorList(exception.FieldErrors),
                     StatusCode = statusCode
                 },
                 new JsonSerializerSettings
                 {
                     ContractResolver = contractResolver
                 });
            }

            return context.Response.WriteAsync(jsonResult);
        }

        /// <summary>
        /// Custom Exception Handler Async.
        /// </summary>
        /// <param name="context">Http Context.</param>
        /// <param name="exception">Exception Details.</param>
        /// <param name="logs">Logger Service.</param>
        /// <param name="localizer">String Localizer value.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        private Task CustomExceptionHandlerAsync(HttpContext context, Exception exception, ILogs logs, ILocalizer localizer)
        {
            if (environment.IsDevelopment() || environment.IsEnvironment(HostingEnvironmentName.IntegrationTesting))
            {
                throw exception;
            }

            logs?.LogError(
                new LogObjects()
                {
                    Exception = exception,
                    FileName = exception.TargetSite?.DeclaringType?.Name ?? string.Empty,
                    MethodName = exception.TargetSite?.Name ?? string.Empty,
                    Source = exception.Source ?? string.Empty,
                    Tag = nameof(ApplicationTypeEnum.AgentPortal)
                });

            context.Response.Clear();
            corsService.ApplyResult(corsService.EvaluatePolicy(context, corsOptions.GetPolicy("CorsPolicyWhitelistedDomains")), context.Response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var cultureCode = GetCurrentRequestCultureCode(context);

            var unknownError = new FieldErrors(string.Empty, localizer.GetBaseLocalizerValueForSpecifiedLanguage(cultureCode, SharedResourceConstants.UnknownError), ErrorTypeEnum.UnknownError);
            List<FieldErrors>? error = new List<FieldErrors>() { unknownError };

            var jsonResult = JsonConvert.SerializeObject(
                    new ValidationError
                    {
                        Errors = error,
                        Message = localizer.GetBaseLocalizerValueForSpecifiedLanguage(cultureCode, SharedResourceConstants.UnknownError),
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    },
                    new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() }
                    });

            return context.Response.WriteAsync(jsonResult);
        }

        /// <summary>
        /// Error Handled Exception Handler Async.
        /// </summary>
        /// <param name="context">Http Context.</param>
        /// <param name="statusCode">Status Code.</param>
        /// <param name="errorMessage">Error Message.</param>
        /// <param name="errorTypeEnum">Error Type Enum.</param>
        /// <param name="requestDetails">Request Details.</param>
        /// <param name="logs">Logger.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        private Task ErrorHandledExceptionHandlerAsync(HttpContext context, HttpStatusCode statusCode, string errorMessage, ErrorTypeEnum errorTypeEnum, RequestDetails requestDetails, ILogs logs)
        {
            if (context.Response.ContentType == null)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;
            }

            var isErrorMessagePresent = context.Items.TryGetValue("errorMessage", out object? contextValue);

            var contextErrorMessage = contextValue?.ParseToString() ?? errorMessage;

            if (statusCode == HttpStatusCode.TooManyRequests)
            {
                var customErrorDetails = new Dictionary<string, string>();

                customErrorDetails.Add("RateLimitErrorMessage", contextErrorMessage);

                var tagList = !requestDetails.IsDeveloperApi ? new List<string>()
                    {
                        nameof(ApplicationTypeEnum.AgentPortal),
                        "agent-api-rate-limit-exceeded"
                    }
                    : new List<string>()
                    {
                        nameof(ApplicationTypeEnum.AgentPortal),
                        "dev-api-rate-limit-exceeded"
                    };

                logs?.LogMessageInExceptionless(
                new LogMessageObjects()
                {
                    TagList = tagList,
                    Message = errorMessage,
                    Source = !requestDetails.IsDeveloperApi ? nameof(ApplicationTypeEnum.AgentPortal) : "Developer API",
                    CustomErrorDetails = customErrorDetails
                });
            }

            var error = new FieldErrors(string.Empty, requestDetails.IsDeveloperApi || statusCode == HttpStatusCode.NotFound ? contextErrorMessage : errorMessage, errorTypeEnum);
            List<FieldErrors>? errors = new List<FieldErrors>() { error };

            var jsonResult = JsonConvert.SerializeObject(
                 new ValidationError
                 {
                     Errors = errors,
                     Message = requestDetails.IsDeveloperApi || statusCode == HttpStatusCode.NotFound ? contextErrorMessage : errorMessage,
                     StatusCode = (int)statusCode
                 },
                 new JsonSerializerSettings
                 {
                     ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() }
                 });

            return context.Response.WriteAsync(jsonResult);
        }

        /// <summary>
        /// Subscription quota exceeded Exception Handler Async.
        /// </summary>
        /// <param name="context">Http Context.</param>
        /// <param name="exception">Exception Details.</param>
        /// <param name="localizer">Localizer.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        private Task SubscriptionQuotaCountExceededValidationExceptionHandlerAsync(HttpContext context, SubscriptionQuotaCountExceededValidationException exception, ILocalizer localizer)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var message = localizer.GetBaseLocalizerValueForSpecifiedLanguage(GetCurrentRequestCultureCode(context), SharedResourceConstants.SubscriptionQuotaCountExceededValidationExceptionMessage);
            var errorType = exception.ErrorType ?? ErrorTypeEnum.None;

            var jsonResult = JsonConvert.SerializeObject(
                 new ValidationError
                 {
                     Message = message,
                     Errors = new ValidationError().AddError(exception.FieldName, message, errorType),
                     StatusCode = (int)HttpStatusCode.BadRequest
                 },
                 new JsonSerializerSettings
                 {
                     ContractResolver = contractResolver
                 });

            return context.Response.WriteAsync(jsonResult);
        }

        /// <summary>
        /// Feature not supported Exception Handler Async.
        /// </summary>
        /// <param name="context">Http Context.</param>
        /// <param name="exception">Exception Details.</param>
        /// <param name="localizer">Localizer</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        private Task FeatureNotSupportedExceptionHandlerAsync(HttpContext context, FeatureNotSupportedException exception, ILocalizer localizer)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var message = localizer.GetBaseLocalizerValueForSpecifiedLanguage(GetCurrentRequestCultureCode(context), SharedResourceConstants.FeatureNotSupportedExceptionMessage);
            var errorType = exception.ErrorType ?? ErrorTypeEnum.None;

            var jsonResult = JsonConvert.SerializeObject(
                 new ValidationError
                 {
                     Message = message,
                     Errors = new ValidationError().AddError(exception.FieldName, message, errorType),
                     StatusCode = (int)HttpStatusCode.BadRequest
                 },
                 new JsonSerializerSettings
                 {
                     ContractResolver = contractResolver
                 });

            return context.Response.WriteAsync(jsonResult);
        }

        private string GetCurrentRequestCultureCode(HttpContext context)
        {
            var isErrorMessagePresent = context.Items.TryGetValue("LanguageId", out object? languageId);

            var langId = languageId?.ToInt();

            return langId > 0 ? SupportedLanguages.GetLangCodeFromId(langId.Value) : "en-US";
        }
    }
}