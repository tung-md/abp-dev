﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SmartSoftware.AspNetCore.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection;

public static class SmartSoftwareAspNetCoreServiceCollectionExtensions
{
    public static IWebHostEnvironment GetHostingEnvironment(this IServiceCollection services)
    {
        var hostingEnvironment = services.GetSingletonInstanceOrNull<IWebHostEnvironment>();

        if (hostingEnvironment == null)
        {
            return new EmptyHostingEnvironment()
            {
                EnvironmentName = Environments.Development
            };
        }

        return hostingEnvironment;
    }

    public static IServiceCollection TransformSmartSoftwareClaims(this IServiceCollection services)
    {
        return services.AddTransient<IClaimsTransformation, SmartSoftwareClaimsTransformation>();
    }
}