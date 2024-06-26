﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SmartSoftware.Domain.Entities;
using SmartSoftware.ObjectExtending;
using SmartSoftware.Validation;
using SmartSoftware.CmsKit.Admin.Blogs;
using SmartSoftware.CmsKit.Blogs;

namespace SmartSoftware.CmsKit.Admin.Web.Pages.CmsKit.Blogs;

public class UpdateModalModel : CmsKitAdminPageModel
{
    protected IBlogAdminAppService BlogAdminAppService { get; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public UpdateBlogViewModel ViewModel { get; set; }

    public UpdateModalModel(IBlogAdminAppService blogAdminAppService)
    {
        BlogAdminAppService = blogAdminAppService;
    }

    public async Task OnGetAsync()
    {
        var blog = await BlogAdminAppService.GetAsync(Id);

        ViewModel = ObjectMapper.Map<BlogDto, UpdateBlogViewModel>(blog);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var dto = ObjectMapper.Map<UpdateBlogViewModel, UpdateBlogDto>(ViewModel);

        await BlogAdminAppService.UpdateAsync(Id, dto);

        return NoContent();
    }
    
    public class UpdateBlogViewModel : ExtensibleObject, IHasConcurrencyStamp
    {
        [Required]
        [DynamicMaxLength(typeof(BlogConsts), nameof(BlogConsts.MaxNameLength))]
        public string Name { get; set; }

        [DynamicMaxLength(typeof(BlogConsts), nameof(BlogConsts.MaxSlugLength))]
        [Required]
        public string Slug { get; set; }

        [HiddenInput]
        public string ConcurrencyStamp { get; set; }
    }
}
