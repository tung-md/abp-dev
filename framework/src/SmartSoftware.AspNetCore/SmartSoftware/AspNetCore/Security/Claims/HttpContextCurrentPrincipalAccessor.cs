﻿using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SmartSoftware.Security.Claims;

namespace SmartSoftware.AspNetCore.Security.Claims;

public class HttpContextCurrentPrincipalAccessor : ThreadCurrentPrincipalAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentPrincipalAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override ClaimsPrincipal GetClaimsPrincipal()
    {
        return _httpContextAccessor.HttpContext?.User ?? base.GetClaimsPrincipal();
    }
}