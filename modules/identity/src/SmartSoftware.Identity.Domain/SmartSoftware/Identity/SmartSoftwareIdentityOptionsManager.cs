﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SmartSoftware.Identity.Settings;
using SmartSoftware.Options;
using SmartSoftware.Settings;

namespace SmartSoftware.Identity;

public class SmartSoftwareIdentityOptionsManager : SmartSoftwareDynamicOptionsManager<IdentityOptions>
{
    protected ISettingProvider SettingProvider { get; }

    public SmartSoftwareIdentityOptionsManager(IOptionsFactory<IdentityOptions> factory,
        ISettingProvider settingProvider)
        : base(factory)
    {
        SettingProvider = settingProvider;
    }

    protected override async Task OverrideOptionsAsync(string name, IdentityOptions options)
    {
        options.Password.RequiredLength = await SettingProvider.GetAsync(IdentitySettingNames.Password.RequiredLength, options.Password.RequiredLength);
        options.Password.RequiredUniqueChars = await SettingProvider.GetAsync(IdentitySettingNames.Password.RequiredUniqueChars, options.Password.RequiredUniqueChars);
        options.Password.RequireNonAlphanumeric = await SettingProvider.GetAsync(IdentitySettingNames.Password.RequireNonAlphanumeric, options.Password.RequireNonAlphanumeric);
        options.Password.RequireLowercase = await SettingProvider.GetAsync(IdentitySettingNames.Password.RequireLowercase, options.Password.RequireLowercase);
        options.Password.RequireUppercase = await SettingProvider.GetAsync(IdentitySettingNames.Password.RequireUppercase, options.Password.RequireUppercase);
        options.Password.RequireDigit = await SettingProvider.GetAsync(IdentitySettingNames.Password.RequireDigit, options.Password.RequireDigit);

        options.Lockout.AllowedForNewUsers = await SettingProvider.GetAsync(IdentitySettingNames.Lockout.AllowedForNewUsers, options.Lockout.AllowedForNewUsers);
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(await SettingProvider.GetAsync(IdentitySettingNames.Lockout.LockoutDuration, options.Lockout.DefaultLockoutTimeSpan.TotalSeconds.To<int>()));
        options.Lockout.MaxFailedAccessAttempts = await SettingProvider.GetAsync(IdentitySettingNames.Lockout.MaxFailedAccessAttempts, options.Lockout.MaxFailedAccessAttempts);

        options.SignIn.RequireConfirmedEmail = await SettingProvider.GetAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail, options.SignIn.RequireConfirmedEmail);
        options.SignIn.RequireConfirmedPhoneNumber = await SettingProvider.GetAsync(IdentitySettingNames.SignIn.RequireConfirmedPhoneNumber, options.SignIn.RequireConfirmedPhoneNumber);
    }
}