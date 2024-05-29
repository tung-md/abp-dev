﻿using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using SmartSoftware.AspNetCore.Components.Web.Extensibility;
using SmartSoftware.DependencyInjection;
using SmartSoftware.Http.Client.Authentication;
using SmartSoftware.Http.Client;
using Microsoft.Extensions.Options;
using SmartSoftware.MultiTenancy;

namespace SmartSoftware.AspNetCore.Components.WebAssembly.Extensibility;

public class WebAssemblyLookupApiRequestService : ILookupApiRequestService, ITransientDependency
{
    public IHttpClientFactory HttpClientFactory { get; }
    public IRemoteServiceHttpClientAuthenticator HttpClientAuthenticator { get; }
    public IRemoteServiceConfigurationProvider RemoteServiceConfigurationProvider { get; }
    public ICurrentTenant CurrentTenant { get; }

    public WebAssemblyLookupApiRequestService(IHttpClientFactory httpClientFactory,
        IRemoteServiceHttpClientAuthenticator httpClientAuthenticator,
        ICurrentTenant currentTenant,
        IRemoteServiceConfigurationProvider remoteServiceConfigurationProvider)
    {
        HttpClientFactory = httpClientFactory;
        HttpClientAuthenticator = httpClientAuthenticator;
        CurrentTenant = currentTenant;
        RemoteServiceConfigurationProvider = remoteServiceConfigurationProvider;
    }

    public async Task<string> SendAsync(string url)
    {
        var client = HttpClientFactory.CreateClient(nameof(WebAssemblyLookupApiRequestService));
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        AddHeaders(requestMessage);

        var uri = new Uri(url, UriKind.RelativeOrAbsolute);
        if (!uri.IsAbsoluteUri)
        {
            var remoteServiceConfig = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            if (remoteServiceConfig == null)
            {
                throw new SmartSoftwareException("Remote service configuration 'Default' was not found!");
            }
            client.BaseAddress = new Uri(remoteServiceConfig.BaseUrl);
            await HttpClientAuthenticator.Authenticate(new RemoteServiceHttpClientAuthenticateContext(client, requestMessage, new RemoteServiceConfiguration(remoteServiceConfig.BaseUrl), string.Empty));
        }

        var response = await client.SendAsync(requestMessage);

        return await response.Content.ReadAsStringAsync();
    }

    protected virtual void AddHeaders(HttpRequestMessage requestMessage)
    {
        if (CurrentTenant.Id.HasValue)
        {
            requestMessage.Headers.Add(TenantResolverConsts.DefaultTenantKey, CurrentTenant.Id.Value.ToString());
        }

        var currentCulture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!currentCulture.IsNullOrEmpty())
        {
            requestMessage.Headers.AcceptLanguage.Add(new(currentCulture));
        }
    }
}