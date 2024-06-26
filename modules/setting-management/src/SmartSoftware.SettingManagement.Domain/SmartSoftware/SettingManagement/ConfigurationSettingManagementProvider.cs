﻿using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SmartSoftware.DependencyInjection;
using SmartSoftware.Settings;

namespace SmartSoftware.SettingManagement;

public class ConfigurationSettingManagementProvider : ISettingManagementProvider, ITransientDependency
{
    public string Name => ConfigurationSettingValueProvider.ProviderName;

    protected IConfiguration Configuration { get; }

    public ConfigurationSettingManagementProvider(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public virtual Task<string> GetOrNullAsync(SettingDefinition setting, string providerKey)
    {
        return Task.FromResult(Configuration[ConfigurationSettingValueProvider.ConfigurationNamePrefix + setting.Name]);
    }

    public virtual Task SetAsync(SettingDefinition setting, string value, string providerKey)
    {
        throw new SmartSoftwareException($"Can not set a setting value to the application configuration.");
    }

    public virtual Task ClearAsync(SettingDefinition setting, string providerKey)
    {
        throw new SmartSoftwareException($"Can not set a setting value to the application configuration.");
    }
}
