﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using SmartSoftware.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartSoftware.Account.Web.ProfileManagement;
using SmartSoftware.Validation;

namespace SmartSoftware.Account.Web.Pages.Account;

public class ManageModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    public ProfileManagementPageCreationContext ProfileManagementPageCreationContext { get; private set; }

    protected ProfileManagementPageOptions Options { get; }

    public ManageModel(IOptions<ProfileManagementPageOptions> options)
    {
        Options = options.Value;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        ProfileManagementPageCreationContext = new ProfileManagementPageCreationContext(ServiceProvider);

        foreach (var contributor in Options.Contributors)
        {
            await contributor.ConfigureAsync(ProfileManagementPageCreationContext);
        }

        if (ReturnUrl != null)
        {
            if (!Url.IsLocalUrl(ReturnUrl) &&
                !ReturnUrl.StartsWith(UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Request.PathBase).RemovePostFix("/")) &&
                !await AppUrlProvider.IsRedirectAllowedUrlAsync(ReturnUrl))
            {
                ReturnUrl = null;
            }
        }

        return Page();
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
