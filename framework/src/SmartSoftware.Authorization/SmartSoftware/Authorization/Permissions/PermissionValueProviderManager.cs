﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartSoftware.DependencyInjection;

namespace SmartSoftware.Authorization.Permissions;

public class PermissionValueProviderManager : IPermissionValueProviderManager, ISingletonDependency
{
    public IReadOnlyList<IPermissionValueProvider> ValueProviders => _lazyProviders.Value;
    private readonly Lazy<List<IPermissionValueProvider>> _lazyProviders;

    protected SmartSoftwarePermissionOptions Options { get; }
    protected IServiceProvider ServiceProvider { get; }

    public PermissionValueProviderManager(
        IServiceProvider serviceProvider,
        IOptions<SmartSoftwarePermissionOptions> options)
    {
        Options = options.Value;
        ServiceProvider = serviceProvider;

        _lazyProviders = new Lazy<List<IPermissionValueProvider>>(GetProviders, true);
    }
    
    protected virtual List<IPermissionValueProvider> GetProviders()
    {
        var providers = Options
            .ValueProviders
            .Select(type => (ServiceProvider.GetRequiredService(type) as IPermissionValueProvider)!)
            .ToList();
        
        var multipleProviders = providers.GroupBy(p => p.Name).FirstOrDefault(x => x.Count() > 1);
        if(multipleProviders != null)
        {
            throw new SmartSoftwareException($"Duplicate permission value provider name detected: {multipleProviders.Key}. Providers:{Environment.NewLine}{multipleProviders.Select(p => p.GetType().FullName!).JoinAsString(Environment.NewLine)}");
        }

        return providers;
    }
}
