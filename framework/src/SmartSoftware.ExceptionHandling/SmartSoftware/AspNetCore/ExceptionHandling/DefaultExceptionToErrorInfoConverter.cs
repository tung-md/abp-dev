using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SmartSoftware.Authorization;
using SmartSoftware.Data;
using SmartSoftware.DependencyInjection;
using SmartSoftware.Domain.Entities;
using SmartSoftware.ExceptionHandling;
using SmartSoftware.ExceptionHandling.Localization;
using SmartSoftware.Http;
using SmartSoftware.Http.Client;
using SmartSoftware.Localization;
using SmartSoftware.Localization.ExceptionHandling;
using SmartSoftware.Validation;

namespace SmartSoftware.AspNetCore.ExceptionHandling;

public class DefaultExceptionToErrorInfoConverter : IExceptionToErrorInfoConverter, ITransientDependency
{
    protected SmartSoftwareExceptionLocalizationOptions LocalizationOptions { get; }
    protected IStringLocalizerFactory StringLocalizerFactory { get; }
    protected IStringLocalizer<SmartSoftwareExceptionHandlingResource> L { get; }
    protected IServiceProvider ServiceProvider { get; }

    public DefaultExceptionToErrorInfoConverter(
        IOptions<SmartSoftwareExceptionLocalizationOptions> localizationOptions,
        IStringLocalizerFactory stringLocalizerFactory,
        IStringLocalizer<SmartSoftwareExceptionHandlingResource> stringLocalizer,
        IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        StringLocalizerFactory = stringLocalizerFactory;
        L = stringLocalizer;
        LocalizationOptions = localizationOptions.Value;
    }

    public RemoteServiceErrorInfo Convert(Exception exception, bool includeSensitiveDetails)
    {
        var exceptionHandlingOptions = CreateDefaultOptions();
        exceptionHandlingOptions.SendExceptionsDetailsToClients = includeSensitiveDetails;
        exceptionHandlingOptions.SendStackTraceToClients = includeSensitiveDetails;

        var errorInfo = CreateErrorInfoWithoutCode(exception, exceptionHandlingOptions);

        if (exception is IHasErrorCode hasErrorCodeException)
        {
            errorInfo.Code = hasErrorCodeException.Code;
        }

        return errorInfo;
    }

    public RemoteServiceErrorInfo Convert(Exception exception, Action<SmartSoftwareExceptionHandlingOptions>? options = null)
    {
        var exceptionHandlingOptions = CreateDefaultOptions();
        options?.Invoke(exceptionHandlingOptions);

        var errorInfo = CreateErrorInfoWithoutCode(exception, exceptionHandlingOptions);

        if (exception is IHasErrorCode hasErrorCodeException)
        {
            errorInfo.Code = hasErrorCodeException.Code;
        }

        return errorInfo;
    }

    protected virtual RemoteServiceErrorInfo CreateErrorInfoWithoutCode(Exception exception, SmartSoftwareExceptionHandlingOptions options)
    {
        if (options.SendExceptionsDetailsToClients)
        {
            return CreateDetailedErrorInfoFromException(exception, options.SendStackTraceToClients);
        }

        exception = TryToGetActualException(exception);

        if (exception is SmartSoftwareRemoteCallException remoteCallException && remoteCallException.Error != null)
        {
            return remoteCallException.Error;
        }

        if (exception is SmartSoftwareDbConcurrencyException)
        {
            return new RemoteServiceErrorInfo(L["SmartSoftwareDbConcurrencyErrorMessage"]);
        }

        if (exception is EntityNotFoundException)
        {
            return CreateEntityNotFoundError((exception as EntityNotFoundException)!);
        }

        var errorInfo = new RemoteServiceErrorInfo();

        if (exception is IUserFriendlyException || exception is SmartSoftwareRemoteCallException)
        {
            errorInfo.Message = exception.Message;
            errorInfo.Details = (exception as IHasErrorDetails)?.Details;
        }

        if (exception is IHasValidationErrors)
        {
            if (errorInfo.Message.IsNullOrEmpty())
            {
                errorInfo.Message = L["ValidationErrorMessage"];
            }

            if (errorInfo.Details.IsNullOrEmpty())
            {
                errorInfo.Details = GetValidationErrorNarrative((exception as IHasValidationErrors)!);
            }

            errorInfo.ValidationErrors = GetValidationErrorInfos((exception as IHasValidationErrors)!);
        }

        TryToLocalizeExceptionMessage(exception, errorInfo);

        if (errorInfo.Message.IsNullOrEmpty())
        {
            errorInfo.Message = L["InternalServerErrorMessage"];
        }

        errorInfo.Data = exception.Data;

        return errorInfo;
    }

    protected virtual void TryToLocalizeExceptionMessage(Exception exception, RemoteServiceErrorInfo errorInfo)
    {
        if (exception is ILocalizeErrorMessage localizeErrorMessageException)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                errorInfo.Message = localizeErrorMessageException.LocalizeMessage(new LocalizationContext(scope.ServiceProvider));
            }

            return;
        }

        if (!(exception is IHasErrorCode exceptionWithErrorCode))
        {
            return;
        }

        if (exceptionWithErrorCode.Code.IsNullOrWhiteSpace() ||
            !exceptionWithErrorCode.Code!.Contains(":"))
        {
            return;
        }

        var codeNamespace = exceptionWithErrorCode.Code.Split(':')[0];

        var localizationResourceType = LocalizationOptions.ErrorCodeNamespaceMappings.GetOrDefault(codeNamespace);
        if (localizationResourceType == null)
        {
            return;
        }

        var stringLocalizer = StringLocalizerFactory.Create(localizationResourceType);
        var localizedString = stringLocalizer[exceptionWithErrorCode.Code];
        if (localizedString.ResourceNotFound)
        {
            return;
        }

        var localizedValue = localizedString.Value;

        if (exception.Data != null && exception.Data.Count > 0)
        {
            foreach (var key in exception.Data.Keys)
            {
                localizedValue = localizedValue.Replace("{" + key + "}", exception.Data[key]?.ToString());
            }
        }

        errorInfo.Message = localizedValue;
    }

    protected virtual RemoteServiceErrorInfo CreateEntityNotFoundError(EntityNotFoundException exception)
    {
        if (exception.EntityType != null)
        {
            return new RemoteServiceErrorInfo(
                string.Format(
                    L["EntityNotFoundErrorMessage"],
                    exception.EntityType.Name,
                    exception.Id
                )
            );
        }

        return new RemoteServiceErrorInfo(exception.Message);
    }

    protected virtual Exception TryToGetActualException(Exception exception)
    {
        if (exception is AggregateException aggException && aggException.InnerException != null)
        {
            if (aggException.InnerException is SmartSoftwareValidationException ||
                aggException.InnerException is SmartSoftwareAuthorizationException ||
                aggException.InnerException is EntityNotFoundException ||
                aggException.InnerException is IBusinessException)
            {
                return aggException.InnerException;
            }
        }

        return exception;
    }

    protected virtual RemoteServiceErrorInfo CreateDetailedErrorInfoFromException(Exception exception, bool sendStackTraceToClients)
    {
        var detailBuilder = new StringBuilder();

        AddExceptionToDetails(exception, detailBuilder, sendStackTraceToClients);

        var errorInfo = new RemoteServiceErrorInfo(exception.Message, detailBuilder.ToString(), data: exception.Data);

        if (exception is SmartSoftwareValidationException)
        {
            errorInfo.ValidationErrors = GetValidationErrorInfos((exception as SmartSoftwareValidationException)!);
        }

        return errorInfo;
    }

    protected virtual void AddExceptionToDetails(Exception exception, StringBuilder detailBuilder, bool sendStackTraceToClients)
    {
        //Exception Message
        detailBuilder.AppendLine(exception.GetType().Name + ": " + exception.Message);

        //Additional info for UserFriendlyException
        if (exception is IUserFriendlyException &&
            exception is IHasErrorDetails)
        {
            var details = ((IHasErrorDetails)exception).Details;
            if (!details.IsNullOrEmpty())
            {
                detailBuilder.AppendLine(details);
            }
        }

        //Additional info for SmartSoftwareValidationException
        if (exception is SmartSoftwareValidationException validationException)
        {
            if (validationException.ValidationErrors.Count > 0)
            {
                detailBuilder.AppendLine(GetValidationErrorNarrative(validationException));
            }
        }

        //Exception StackTrace
        if (sendStackTraceToClients && !string.IsNullOrEmpty(exception.StackTrace))
        {
            detailBuilder.AppendLine("STACK TRACE: " + exception.StackTrace);
        }

        //Inner exception
        if (exception.InnerException != null)
        {
            AddExceptionToDetails(exception.InnerException, detailBuilder, sendStackTraceToClients);
        }

        //Inner exceptions for AggregateException
        if (exception is AggregateException aggException)
        {
            if (aggException.InnerExceptions.IsNullOrEmpty())
            {
                return;
            }

            foreach (var innerException in aggException.InnerExceptions)
            {
                AddExceptionToDetails(innerException, detailBuilder, sendStackTraceToClients);
            }
        }
    }

    protected virtual RemoteServiceValidationErrorInfo[] GetValidationErrorInfos(IHasValidationErrors validationException)
    {
        var validationErrorInfos = new List<RemoteServiceValidationErrorInfo>();

        foreach (var validationResult in validationException.ValidationErrors)
        {
            var validationError = new RemoteServiceValidationErrorInfo(validationResult.ErrorMessage!);

            if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
            {
                validationError.Members = validationResult.MemberNames.Select(m => m.ToCamelCase()).ToArray();
            }

            validationErrorInfos.Add(validationError);
        }

        return validationErrorInfos.ToArray();
    }

    protected virtual string GetValidationErrorNarrative(IHasValidationErrors validationException)
    {
        var detailBuilder = new StringBuilder();
        detailBuilder.AppendLine(L["ValidationNarrativeErrorMessageTitle"]);

        foreach (var validationResult in validationException.ValidationErrors)
        {
            detailBuilder.AppendFormat(" - {0}", validationResult.ErrorMessage);
            detailBuilder.AppendLine();
        }

        return detailBuilder.ToString();
    }

    protected virtual SmartSoftwareExceptionHandlingOptions CreateDefaultOptions()
    {
        return new SmartSoftwareExceptionHandlingOptions
        {
            SendExceptionsDetailsToClients = false,
            SendStackTraceToClients = true
        };
    }
}
