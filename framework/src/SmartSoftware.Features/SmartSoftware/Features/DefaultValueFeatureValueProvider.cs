﻿using System.Threading.Tasks;

namespace SmartSoftware.Features;

public class DefaultValueFeatureValueProvider : FeatureValueProvider //TODO: Directly implement IFeatureValueProvider
{
    public const string ProviderName = "D";

    public override string Name => ProviderName;

    public DefaultValueFeatureValueProvider(IFeatureStore settingStore)
        : base(settingStore)
    {

    }

    public override Task<string?> GetOrNullAsync(FeatureDefinition setting)
    {
        return Task.FromResult<string?>(setting.DefaultValue);
    }
}
