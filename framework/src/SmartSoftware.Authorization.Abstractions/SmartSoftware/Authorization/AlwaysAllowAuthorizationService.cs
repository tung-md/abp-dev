﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SmartSoftware.Security.Claims;

namespace SmartSoftware.Authorization;

public class AlwaysAllowAuthorizationService : ISmartSoftwareAuthorizationService
{
    public IServiceProvider ServiceProvider { get; }

    public ClaimsPrincipal CurrentPrincipal => _currentPrincipalAccessor.Principal;

    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    public AlwaysAllowAuthorizationService(IServiceProvider serviceProvider, ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        ServiceProvider = serviceProvider;
        _currentPrincipalAccessor = currentPrincipalAccessor;
    }

    public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
    {
        return Task.FromResult(AuthorizationResult.Success());
    }

    public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
    {
        return Task.FromResult(AuthorizationResult.Success());
    }
}
